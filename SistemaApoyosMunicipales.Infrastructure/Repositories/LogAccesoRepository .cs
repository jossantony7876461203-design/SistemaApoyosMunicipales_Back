using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Repositories
{
    public sealed class LogAccesoRepository : ILogAccesoRepository
    {
        private readonly AppDbContext _context;

        public LogAccesoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(LogAcceso log)
        {
            await _context.LogAccesos.AddAsync(log);
        }
    }
}