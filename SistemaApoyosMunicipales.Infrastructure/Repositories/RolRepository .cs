using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;


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

        public async Task<IEnumerable<Rol>> ObtenerTodosAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Rol>> ObtenerActivosAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Activo)
                .ToListAsync();
        }
    }
}
