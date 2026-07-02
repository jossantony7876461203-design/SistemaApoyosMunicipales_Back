using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class RegistroApoyoDocumentoRepository : IRegistroApoyoDocumentoRepository
    {
        private readonly AppDbContext _context;

        public RegistroApoyoDocumentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RegistroApoyoDocumento> ObtenerPorIdAsync(Guid id)
        {
            return await _context.RegistroApoyoDocumentos
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RegistroApoyoDocumento>> ObtenerPorRegistroApoyoIdAsync(Guid registroApoyoId)
        {
            return await _context.RegistroApoyoDocumentos
                .Where(x => x.RegistroApoyoId == registroApoyoId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<RegistroApoyoDocumento>> ObtenerTodosAsync(
            PaginationRequest pagination,
            Guid? registroApoyoId = null)
        {
            var query = _context.RegistroApoyoDocumentos
                .AsNoTracking()
                .AsQueryable();

            if (registroApoyoId.HasValue)
            {
                query = query.Where(x => x.RegistroApoyoId == registroApoyoId.Value);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

            return new PaginatedResult<RegistroApoyoDocumento>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = pagination.PageNumber > 1,
                HasNextPage = pagination.PageNumber < totalPages
            };
        }

        public async Task AgregarAsync(RegistroApoyoDocumento documento)
        {
            await _context.RegistroApoyoDocumentos.AddAsync(documento);
        }

        public async Task AgregarRangeAsync(IEnumerable<RegistroApoyoDocumento> documentos)
        {
            await _context.RegistroApoyoDocumentos.AddRangeAsync(documentos);
        }

        public void Actualizar(RegistroApoyoDocumento documento)
        {
            _context.RegistroApoyoDocumentos.Update(documento);
        }

        public async Task EliminarAsync(Guid id)
        {
            var documento = await ObtenerPorIdAsync(id);
            if (documento is not null)
            {
                _context.RegistroApoyoDocumentos.Remove(documento);
            }
        }

        public async Task EliminarPorRegistroApoyoIdAsync(Guid registroApoyoId)
        {
            var documentos = await _context.RegistroApoyoDocumentos
                .Where(x => x.RegistroApoyoId == registroApoyoId)
                .ToListAsync();

            if (documentos.Any())
            {
                _context.RegistroApoyoDocumentos.RemoveRange(documentos);
            }
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _context.RegistroApoyoDocumentos
                .AnyAsync(x => x.Id == id);
        }

        public async Task<int> ContarPorRegistroApoyoIdAsync(Guid registroApoyoId)
        {
            return await _context.RegistroApoyoDocumentos
                .CountAsync(x => x.RegistroApoyoId == registroApoyoId);
        }
    }

}
