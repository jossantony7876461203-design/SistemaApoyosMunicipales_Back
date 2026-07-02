using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class PermisoRepository : IPermisoRepository
    {
        private readonly AppDbContext _context;

        public PermisoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(Permiso permiso)
        {
            await _context.Permisos.AddAsync(permiso);
        }

        public async Task<Permiso?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Permisos
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.DeletedAt == null);
        }

        public async Task<bool> ExisteAsync(string codigo)
        {
            return await _context.Permisos
                .AnyAsync(x =>
                    x.Codigo == codigo &&
                    x.DeletedAt == null);
        }

        public async Task<PaginatedResult<Permiso>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true)
        {
            return await _context.Permisos
                .AsNoTracking()
                .Where(x =>
                    x.Activo == activos &&
                    x.DeletedAt == null)
                .OrderBy(x => x.Modulo)
                .ThenBy(x => x.Nombre)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task<IEnumerable<Permiso>> ObtenerPorModuloAsync(
            string modulo)
        {
            return await _context.Permisos
                .AsNoTracking()
                .Where(x =>
                    x.Modulo == modulo &&
                    x.Activo &&
                    x.DeletedAt == null)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task CambiarEstatusAsync(Guid id, bool activo)
        {
            await _context.Permisos
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Activo, activo));
        }

        // Soft Delete — marca DeletedAt en lugar de borrar
        public async Task EliminarAsync(Guid id)
        {
            await _context.Permisos
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.DeletedAt, DateTimeOffset.UtcNow)
                    .SetProperty(p => p.Activo, false));
        }
    }
}
