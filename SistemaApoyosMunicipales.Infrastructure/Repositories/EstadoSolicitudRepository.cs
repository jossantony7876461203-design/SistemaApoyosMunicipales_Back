using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Estados;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class EstadoSolicitudRepository : IEstadoSolicitudRepository
    {
        private readonly AppDbContext _context; // ajusta al nombre real de tu DbContext

        public EstadoSolicitudRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteClaveAsync(string clave)
        {
            return await _context.EstadosSolicitud
                .AnyAsync(e => e.Clave == clave);
        }

        public async Task<bool> ExisteClaveEnOtroRegistroAsync(string clave, Guid id)
        {
            return await _context.EstadosSolicitud
                .AnyAsync(e => e.Clave == clave && e.Id != id);
        }

        public async Task AgregarAsync(EstadoSolicitud estado)
        {
            await _context.EstadosSolicitud.AddAsync(estado);
        }

        public async Task<EstadoSolicitud?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.EstadosSolicitud
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<EstadoSolicitud>> ObtenerTodosAsync()
        {
            return await _context.EstadosSolicitud
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _context.EstadosSolicitud
                .AnyAsync(e => e.Id == id);
        }

        public async Task<bool> TieneRegistrosAsociadosAsync(Guid id)
        {
            return await _context.RegistroApoyos
                .AnyAsync(r => r.EstadoSolicitudId == id);
        }

        public async Task CambiarEstatusAsync(Guid id, bool activo)
        {
            var estado = await _context.EstadosSolicitud
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estado is not null)
                estado.Activo = activo;
        }
    }
}
