using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class SubRolPermisoRepository : ISubRolPermisoRepository
    {
        private readonly AppDbContext _context;

        public SubRolPermisoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(Guid subRolId, Guid permisoId)
        {
            return await _context.SubRolesPermisos
                .AnyAsync(x =>
                    x.SubRolId == subRolId &&
                    x.PermisoId == permisoId);
        }

        public async Task AsignarAsync(SubRolPermiso subRolPermiso)
        {
            await _context.SubRolesPermisos.AddAsync(subRolPermiso);
        }

        public Task ActualizarAsync(SubRolPermiso subRolPermiso)
        {
            _context.SubRolesPermisos.Update(subRolPermiso);
            return Task.CompletedTask;
        }

        public async Task QuitarAsync(Guid subRolId, Guid permisoId)
        {
            await _context.SubRolesPermisos
                .Where(x =>
                    x.SubRolId == subRolId &&
                    x.PermisoId == permisoId)
                .ExecuteDeleteAsync();
        }

        public async Task QuitarTodosAsync(Guid subRolId)
        {
            await _context.SubRolesPermisos
                .Where(x => x.SubRolId == subRolId)
                .ExecuteDeleteAsync();
        }

        public async Task<SubRolPermiso?> ObtenerUnoAsync(
            Guid subRolId,
            Guid permisoId)
        {
            return await _context.SubRolesPermisos
                .FirstOrDefaultAsync(x =>
                    x.SubRolId == subRolId &&
                    x.PermisoId == permisoId);
        }

        public async Task<IEnumerable<SubRolPermiso>> ObtenerPorSubRolAsync(
            Guid subRolId)
        {
            return await _context.SubRolesPermisos
                .AsNoTracking()
                .Include(x => x.Permiso)
                .Where(x =>
                    x.SubRolId == subRolId &&
                    x.Permiso.DeletedAt == null)
                .OrderBy(x => x.Permiso.Modulo)
                .ThenBy(x => x.Permiso.Nombre)
                .ToListAsync();
        }
    }
}
