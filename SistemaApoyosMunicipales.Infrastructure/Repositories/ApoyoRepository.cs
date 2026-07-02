using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Apoyo;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Infrastructure.Persistence;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class ApoyoRepository : IApoyoRepository
    {
        private readonly AppDbContext _context;

        public ApoyoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Apoyo apoyo)
        {
            await _context.Apoyos.AddAsync(apoyo);
        }

        public void Actualizar(Apoyo apoyo)
        {
            _context.Apoyos.Update(apoyo);
        }

        // Solo lectura — no importa si está activo o inactivo
        // Solo excluye los eliminados (soft delete)
        public async Task<Apoyo?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Apoyos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.DeletedAt == null);  // ✅ no filtra por Activo
        }

        // Para editar — no importa si está activo o inactivo
        // Solo excluye los eliminados (soft delete)
        public async Task<Apoyo?> ObtenerPorIdParaEditarAsync(Guid id)
        {
            return await _context.Apoyos
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.DeletedAt == null);  // ✅ no filtra por Activo
        }

        public async Task<Apoyo?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _context.Apoyos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Codigo == codigo &&
                    x.DeletedAt == null);
        }

        // Filtra por Activo — para listar activos o inactivos
        public async Task<PaginatedResult<Apoyo>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true)
        {
            return await _context.Apoyos
                .AsNoTracking()
                .Where(x =>
                    x.Activo == activos &&
                    x.DeletedAt == null)   // excluye eliminados en ambas listas
                .OrderBy(x => x.Codigo)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task CambiarEstatusAsync(Guid id, bool activo)
        {
            await _context.Apoyos
                .Where(x =>
                    x.Id == id &&
                    x.DeletedAt == null)   // ✅ no toca eliminados
                .ExecuteUpdateAsync(x => x
                    .SetProperty(a => a.Activo, activo)
                    .SetProperty(a => a.UpdatedAt, DateTimeOffset.UtcNow));
        }

        public async Task EliminarAsync(Guid id)
        {
            await _context.Apoyos
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(a => a.DeletedAt, DateTimeOffset.UtcNow)
                    .SetProperty(a => a.Activo, false)
                    .SetProperty(a => a.UpdatedAt, DateTimeOffset.UtcNow));
        }

        public async Task<List<RegistroApoyoDocumento>>
            ObtenerDocumentosAsync(Guid registroApoyoId)
        {
            return await _context
                .RegistroApoyoDocumentos
                .AsNoTracking()
                .Where(x =>
                    x.RegistroApoyoId == registroApoyoId)
                .ToListAsync();
        }
    }
}