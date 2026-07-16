using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegistroDto> _registroValidator;
    private readonly IValidator<RecuperarPasswordDto> _recuperarValidator;
    private readonly IValidator<ResetPasswordDto> _resetValidator;

    public AuthController(
        IAuthService authService,
        IValidator<LoginDto> loginValidator,
        IValidator<RegistroDto> registroValidator,
        IValidator<RecuperarPasswordDto> recuperarValidator,
        IValidator<ResetPasswordDto> resetValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
        _registroValidator = registroValidator;
        _recuperarValidator = recuperarValidator;
        _resetValidator = resetValidator;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        var validacion = await _registroValidator.ValidateAsync(dto);
        if (!validacion.IsValid)
            return BadRequest(new
            {
                statusCode = 400,
                errores = validacion.Errors.Select(e => e.ErrorMessage)
            });

        await _authService.RegistrarAsync(dto);
        return Ok(new { mensaje = "Registro exitoso. Revisa tu correo para activar tu cuenta." });
    }

    [HttpPost("activar-cuenta")]
    public async Task<IActionResult> ActivarCuenta([FromQuery] string token)
    {
        await _authService.ActivarCuentaAsync(token);

        var html = """
            <html>
            <body style='font-family: Arial; text-align: center; padding: 50px;'>
                <h1 style='color: #4B0016;'>✅ Cuenta activada</h1>
                <p>Tu cuenta ha sido activada con éxito.</p>
                <p>Ya puedes iniciar sesión en el sistema.</p>
            </body>
            </html>
            """;

        return Content(html, "text/html");
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var validacion = await _loginValidator.ValidateAsync(dto);
        if (!validacion.IsValid)
            return BadRequest(new
            {
                statusCode = 400,
                errores = validacion.Errors.Select(e => e.ErrorMessage)
            });

        var resultado = await _authService.LoginAsync(dto);
        return Ok(resultado);
    }

    [HttpPost("recuperar-password")]
    public async Task<IActionResult> RecuperarPassword([FromBody] RecuperarPasswordDto dto)
    {
        var validacion = await _recuperarValidator.ValidateAsync(dto);
        if (!validacion.IsValid)
            return BadRequest(new
            {
                statusCode = 400,
                errores = validacion.Errors.Select(e => e.ErrorMessage)
            });

        await _authService.RecuperarPasswordAsync(dto);

        // Siempre mismo mensaje — no revelar si el correo existe
        return Ok(new { mensaje = "Si el correo existe recibirás un código en breve." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var validacion = await _resetValidator.ValidateAsync(dto);
        if (!validacion.IsValid)
            return BadRequest(new
            {
                statusCode = 400,
                errores = validacion.Errors.Select(e => e.ErrorMessage)
            });

        await _authService.ResetPasswordAsync(dto);
        return Ok(new { mensaje = "Contraseña actualizada correctamente." });
    }


    [HttpPost("reenviar-activacion")]
    public async Task<IActionResult> ReenviarActivacion([FromBody] ReenviarActivacionDto dto)
    {
        await _authService.ReenviarActivacionAsync(dto);

        return Ok(new
        {
            Mensaje = "Si el correo existe y la cuenta no está activa, se ha reenviado un nuevo enlace de activación."
        });
    }
}