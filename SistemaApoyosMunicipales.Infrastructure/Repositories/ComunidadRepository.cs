using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Comunidad;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class ComunidadRepository : IComunidadRepository
    {
        private readonly AppDbContext _context;

        public ComunidadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Comunidad comunidad)
        {
            await _context.Comunidades.AddAsync(comunidad);
        }

        public void Actualizar(Comunidad comunidad)
        {
            _context.Comunidades.Update(comunidad);
        }


        public async Task<Comunidad?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Comunidades
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id);
        }

        public async Task<Comunidad?> ObtenerPorClaveInternaAsync(
            string claveInterna)
        {
            return await _context.Comunidades
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.ClaveInterna == claveInterna);
        }

        public async Task<Comunidad?> ObtenerPorIdParaEditarAsync(Guid id)
        {
            return await _context.Comunidades
                // Sin AsNoTracking → EF rastrea los cambios
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaginatedResult<Comunidad>> ObtenerTodasAsync(
     PaginationRequest pagination,
     bool activos = true)
        {
            return await _context.Comunidades
                .AsNoTracking()
                .Where(x => x.Activo == activos)
                .OrderBy(x => x.Nombre)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }
    }
}
