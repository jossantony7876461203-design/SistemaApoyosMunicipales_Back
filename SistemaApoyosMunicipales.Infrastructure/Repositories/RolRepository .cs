using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;


namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class RolRepository : IRolRepository
    {
        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistePorNombreAsync(string nombre)
        {
            return await _context.Roles
                .AnyAsync(r => r.Nombre == nombre);
        }

        public async Task CrearAsync(Rol rol)
        {
            await _context.Roles.AddAsync(rol);
        }

        public async Task<Rol?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.SubRoles)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PaginatedResult<Rol>> ObtenerTodosAsync(
        PaginationRequest pagination,
        bool activos = true)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(x => x.Activo == activos)
                .OrderBy(x => x.Nombre)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task<IEnumerable<Rol>> ObtenerActivosAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Activo)
                .ToListAsync();
        }

        public Task ActualizarAsync(Rol rol)
        {
            _context.Roles.Update(rol);
            return Task.CompletedTask;
        }

        public async Task CambiarEstatusAsync(Guid id, bool activo)
        {
            await _context.Roles
                .Where(r => r.Id == id)
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.Activo, activo));
        }


    }
}
