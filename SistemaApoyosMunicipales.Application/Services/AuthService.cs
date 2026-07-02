using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Permisos;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.Application.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;
    private readonly IJwtService _jwtService;
    private readonly ILogAccesoRepository _logAccesoRepository;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ITokenRepository tokenRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IHashService hashService,
        IJwtService jwtService,
        ILogAccesoRepository logAccesoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _hashService = hashService;
        _jwtService = jwtService;
        _logAccesoRepository = logAccesoRepository;
    }

    public async Task RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLower();

        var existeCorreo = await _usuarioRepository.ExisteCorreoAsync(correo);

        if (existeCorreo)
            throw new ValidationException("El correo ya está registrado.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Activo = false,
            CorreoVerificado = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _usuarioRepository.CrearAsync(usuario);

        var tokenPlano = _tokenService.GenerarTokenSeguro();

        var token = new TokenVerificacion
        {
            UsuarioId = usuario.Id,
            TokenHash = _hashService.GenerarSHA256(tokenPlano),
            Tipo = "activacion",
            ExpiraAt = DateTimeOffset.UtcNow.AddHours(24),
            Usado = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _tokenRepository.CrearAsync(token);

        await _emailService.EnviarActivacionAsync(
            usuario.Correo,
            usuario.Nombre,
            tokenPlano);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ActivarCuentaAsync(string tokenPlano)
    {
        var tokenHash = _hashService.GenerarSHA256(tokenPlano);

        var tokenVerificacion = await _tokenRepository.ObtenerPorHashAsync(tokenHash);

        if (tokenVerificacion == null)
            throw new NotFoundException("El token de verificación no es válido.");

        if (tokenVerificacion.Usado)
            throw new ValidationException("Este token ya ha sido utilizado.");

        if (tokenVerificacion.Tipo != "activacion")
            throw new ValidationException("El tipo de token es incorrecto.");

        if (tokenVerificacion.ExpiraAt < DateTimeOffset.UtcNow)
            throw new ValidationException("El token ha expirado. Solicita uno nuevo.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(tokenVerificacion.UsuarioId);

        if (usuario == null)
            throw new NotFoundException("El usuario no existe.");

        usuario.Activo = true;
        usuario.CorreoVerificado = true;
        usuario.UpdatedAt = DateTimeOffset.UtcNow;
        usuario.CorreoVerificadoAt = DateTimeOffset.UtcNow;

        tokenVerificacion.Usado = true;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var correo = dto.Correo.Trim().ToLower();

        var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);

        if (usuario is null)
            throw new UnauthorizedException("Credenciales incorrectas.");

        if (!usuario.Activo)
            throw new UnauthorizedException("Tu cuenta no está activa.");

        if (!usuario.CorreoVerificado)
            throw new UnauthorizedException("Debes verificar tu correo antes de iniciar sesión.");

        if (!_passwordHasher.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedException("Credenciales incorrectas.");

        var permisos = await _usuarioRepository.ObtenerPermisosAsync(usuario.Id);

        var usuarioConRol = await _usuarioRepository.ObtenerConRolAsync(usuario.Id);

        var token = _jwtService.GenerarToken(usuarioConRol!, permisos);

        await _usuarioRepository.ActualizarUltimoAccesoAsync(usuario.Id);

        await _logAccesoRepository.RegistrarAsync(new LogAcceso
        {
            UsuarioId = usuario.Id,
            Accion = "login",
            Exitoso = true,
            CreatedAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuarioConRol?.Rol?.Nombre ?? string.Empty,
            SubRol = usuarioConRol?.SubRol?.Nombre ?? string.Empty,
            Permisos = permisos.ToList()
        };
    }

    public async Task RecuperarPasswordAsync(RecuperarPasswordDto dto)
    {
        var correo = dto.Correo.Trim().ToLower();

        // Buscar usuario — siempre responde igual exista o no
        // Evita enumerar correos válidos
        var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);

        if (usuario is not null && usuario.Activo)
        {
            // Generar código de 6 dígitos
            var codigo = _tokenService.GenerarCodigo6Digitos();

            // Guardar hash del código en BD
            var token = new TokenVerificacion
            {
                UsuarioId = usuario.Id,
                TokenHash = _hashService.GenerarSHA256(codigo),
                Tipo = "recuperacion_password",
                ExpiraAt = DateTimeOffset.UtcNow.AddMinutes(15),
                Usado = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _tokenRepository.CrearAsync(token);

            await _emailService.EnviarRecuperacionPasswordAsync(
                usuario.Correo,
                usuario.Nombre,
                codigo);

            await _unitOfWork.SaveChangesAsync();
        }

        // Siempre devuelve el mismo mensaje — seguridad
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var codigoHash = _hashService.GenerarSHA256(dto.Token);

        var tokenVerificacion = await _tokenRepository
            .ObtenerPorHashAsync(codigoHash);

        if (tokenVerificacion is null)
            throw new NotFoundException("El código no es válido.");

        if (tokenVerificacion.Usado)
            throw new ValidationException("Este código ya fue utilizado.");

        if (tokenVerificacion.Tipo != "recuperacion_password")
            throw new ValidationException("El tipo de código es incorrecto.");

        if (tokenVerificacion.ExpiraAt < DateTimeOffset.UtcNow)
            throw new ValidationException("El código ha expirado. Solicita uno nuevo.");

        var usuario = await _usuarioRepository
            .ObtenerPorIdAsync(tokenVerificacion.UsuarioId);

        if (usuario is null)
            throw new NotFoundException("Usuario no encontrado.");

        // Actualizar contraseña
        usuario.PasswordHash = _passwordHasher.Hash(dto.NuevoPassword);
        usuario.UpdatedAt = DateTimeOffset.UtcNow;

        tokenVerificacion.Usado = true;

        await _unitOfWork.SaveChangesAsync();
    }
}