using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SistemaApoyosMunicipales.Infrastructure.Security;

public sealed class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerarToken(Usuario usuario, IEnumerable<UsuarioPermisoDto> permisos)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var credenciales = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        // Claims estándar + claims del negocio
        var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
    new(JwtRegisteredClaimNames.Email, usuario.Correo),
    new(JwtRegisteredClaimNames.Name,  usuario.Nombre),
    new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
    new(JwtRegisteredClaimNames.Iat,
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
        ClaimValueTypes.Integer64),
    new("rol",     usuario.Rol?.Nombre    ?? string.Empty),
    new("sub_rol", usuario.SubRol?.Nombre ?? string.Empty),
    new("permisos", JsonSerializer.Serialize(permisos))
};

        var expiracion = DateTime.UtcNow.AddHours(
            int.Parse(_config["Jwt:ExpiracionHoras"] ?? "8"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiracion,
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}