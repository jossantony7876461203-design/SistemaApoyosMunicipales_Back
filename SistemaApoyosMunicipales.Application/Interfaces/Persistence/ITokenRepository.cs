using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface ITokenRepository
    {
      

        Task CrearAsync(TokenVerificacion token);

        Task<TokenVerificacion?> ObtenerPorHashAsync(string tokenHash);
    }
}