using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class SubRolRepository : ISubRolRepository
    {
        private readonly AppDbContext _context;

        public SubRolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(SubRol subRol)
        {
            await _context.SubRoles.AddAsync(subRol);
        }

        public async Task<SubRol?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.SubRoles
                .Include(x => x.Rol)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExisteAsync(
            Guid rolId,
            string nombre)
        {
            return await _context.SubRoles
                .AnyAsync(x =>
                    x.RolId == rolId &&
                    x.Nombre == nombre);
        }

        public Task ActualizarAsync(SubRol subRol)
        {
            _context.SubRoles.Update(subRol);

            return Task.CompletedTask;
        }

        public async Task CambiarEstatusAsync(
            Guid id,
            bool activo)
        {
            await _context.SubRoles
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(s => s.Activo, activo));
        }

        public async Task<PaginatedResult<SubRol>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true)
        {
            return await _context.SubRoles
                .AsNoTracking()
                .Include(x => x.Rol)
                .Where(x => x.Activo == activos)
                .OrderBy(x => x.Nombre)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task<IEnumerable<SubRol>> ObtenerPorRolAsync(
            Guid rolId)
        {
            return await _context.SubRoles
                .AsNoTracking()
                .Include(x => x.Rol)
                .Where(x =>
                    x.RolId == rolId &&
                    x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }
    }
}
