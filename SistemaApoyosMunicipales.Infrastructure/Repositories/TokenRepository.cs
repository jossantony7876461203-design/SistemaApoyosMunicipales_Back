using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Repositories
{
    public sealed class TokenRepository : ITokenRepository
    {

        private readonly AppDbContext _context;

        public TokenRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task CrearAsync(TokenVerificacion token)
        {
            await _context.TokensVerificacion.AddAsync(token);
        }

        public async Task<TokenVerificacion?> ObtenerPorHashAsync(string tokenHash)
        {
            return await _context.TokensVerificacion
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task InvalidarTokensAsync(Guid usuarioId, string tipo)
        {
            var tokensActivos = await _context.TokensVerificacion
                .Where(t => t.UsuarioId == usuarioId && t.Tipo == tipo && !t.Usado)
                .ToListAsync();

            foreach (var token in tokensActivos)
                token.Usado = true;
        }

        public async Task<TokenVerificacion?> ObtenerUltimoPorUsuarioYTipoAsync(Guid usuarioId, string tipo)
        {
            return await _context.TokensVerificacion
                .Where(t => t.UsuarioId == usuarioId && t.Tipo == tipo)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }


    }
}