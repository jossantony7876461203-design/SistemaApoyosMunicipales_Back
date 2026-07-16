using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface ITokenRepository
    {
      

        Task CrearAsync(TokenVerificacion token);

        Task<TokenVerificacion?> ObtenerPorHashAsync(string tokenHash);
        Task InvalidarTokensAsync(Guid usuarioId, string tipo);

        Task<TokenVerificacion?> ObtenerUltimoPorUsuarioYTipoAsync(Guid usuarioId, string tipo);
    }
}