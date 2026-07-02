using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class RolPermisoRepository : IRolPermisoRepository
    {
        private readonly AppDbContext _context;

        public RolPermisoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(Guid rolId, Guid permisoId)
        {
            return await _context.RolesPermisos
                .AnyAsync(x =>
                    x.RolId == rolId &&
                    x.PermisoId == permisoId);
        }

        public async Task AsignarAsync(RolPermiso rolPermiso)
        {
            await _context.RolesPermisos.AddAsync(rolPermiso);
        }

        public async Task QuitarAsync(Guid rolId, Guid permisoId)
        {
            await _context.RolesPermisos
                .Where(x =>
                    x.RolId == rolId &&
                    x.PermisoId == permisoId)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<RolPermiso>> ObtenerPorRolAsync(
            Guid rolId,
            bool soloAsignados = true)
        {
            // soloAsignados = true  → solo los que tiene el rol
            // soloAsignados = false → todos los del catálogo con flag Asignado
            return await _context.RolesPermisos
                .AsNoTracking()
                .Include(x => x.Permiso)
                .Where(x =>
                    x.RolId == rolId &&
                    x.Permiso.DeletedAt == null)
                .OrderBy(x => x.Permiso.Modulo)
                .ThenBy(x => x.Permiso.Nombre)
                .ToListAsync();
        }
    }
}
