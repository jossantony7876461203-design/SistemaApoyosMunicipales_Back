using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using SistemaApoyosMunicipales.Domain.Estados;
using SistemaApoyosMunicipales.Infrastructure.Persistence;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class RegistroApoyoRepository
        : IRegistroApoyoRepository
    {
        private readonly AppDbContext _context;

        public RegistroApoyoRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(
            RegistroApoyo registro)
        {
            await _context.RegistroApoyos
                .AddAsync(registro);
        }

        public void Actualizar(
            RegistroApoyo registro)
        {
            _context.RegistroApoyos
                .Update(registro);
        }

        public async Task<RegistroApoyo?> ObtenerPorIdAsync(
            Guid id)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Include(x => x.Apoyo)
                .Include(x => x.Comunidad)
                .Include(x => x.EstadoSolicitud)
                .Include(x => x.Documentos)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.DeletedAt == null);
        }

        public async Task<RegistroApoyo?> ObtenerPorIdParaEditarAsync(
            Guid id)
        {
            return await _context.RegistroApoyos
                .Include(x => x.Documentos)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.DeletedAt == null);
        }

        public async Task CambiarEstatusAsync(
            Guid id,
            Guid estadoSolicitudId)
        {
            await _context.RegistroApoyos
                .Where(x =>
                    x.Id == id &&
                    x.DeletedAt == null)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(
                        p => p.EstadoSolicitudId,
                        estadoSolicitudId)
                    .SetProperty(
                        p => p.UpdatedAt,
                        DateTimeOffset.UtcNow));
        }

        public async Task EliminarAsync(
            Guid id)
        {
            await _context.RegistroApoyos
                .Where(x =>
                    x.Id == id &&
                    x.DeletedAt == null)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(
                        p => p.DeletedAt,
                        DateTimeOffset.UtcNow)
                    .SetProperty(
                        p => p.Activo,
                        false)
                    .SetProperty(
                        p => p.UpdatedAt,
                        DateTimeOffset.UtcNow));
        }

        public async Task<PaginatedResult<RegistroApoyo>>
            ObtenerPorComunidadAsync(
                Guid comunidadId,
                PaginationRequest pagination)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Include(x => x.Apoyo)
                .Include(x => x.Comunidad)
                .Include(x => x.EstadoSolicitud)
                .Include(x => x.Documentos)
                .Where(x =>
                    x.ComunidadId == comunidadId &&
                    x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task<PaginatedResult<RegistroApoyo>>
            ObtenerTodosAsync(
                PaginationRequest pagination)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Include(x => x.Apoyo)
                .Include(x => x.Comunidad)
                .Include(x => x.EstadoSolicitud)
                .Where(x => x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }

        public async Task<bool> ExisteAsync(
            Guid id)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == id &&
                    x.Activo &&
                    x.DeletedAt == null);
        }

        public async Task<bool> ExisteFolioAsync(
            string folio)
        {
            var folioNormalizado = folio
                .Trim()
                .ToUpper();

            return await _context.RegistroApoyos
                .AsNoTracking()
                .AnyAsync(x =>
                    x.DeletedAt == null &&
                    x.Folio.ToUpper() == folioNormalizado);
        }

        public async Task<bool> ExisteFolioEnOtroRegistroAsync(
            string folio,
            Guid registroId)
        {
            var folioNormalizado = folio
                .Trim()
                .ToUpper();

            return await _context.RegistroApoyos
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id != registroId &&
                    x.DeletedAt == null &&
                    x.Folio.ToUpper() == folioNormalizado);
        }

        public async Task<List<RegistroApoyoDocumento>>
            ObtenerDocumentosAsync(
                Guid registroApoyoId)
        {
            return await _context.RegistroApoyoDocumentos
                .AsNoTracking()
                .Where(x =>
                    x.RegistroApoyoId == registroApoyoId)
                .ToListAsync();
        }

        public async Task EliminarDocumentosAsync(
            Guid registroApoyoId)
        {
            await _context.RegistroApoyoDocumentos
                .Where(x =>
                    x.RegistroApoyoId == registroApoyoId)
                .ExecuteDeleteAsync();
        }

        public async Task AgregarDocumentoAsync(
            RegistroApoyoDocumento documento)
        {
            await _context.RegistroApoyoDocumentos
                .AddAsync(documento);
        }

        public async Task AgregarDocumentosAsync(
            List<RegistroApoyoDocumento> documentos)
        {
            await _context.RegistroApoyoDocumentos
                .AddRangeAsync(documentos);
        }

        public async Task<List<EstadoSolicitud>>
            ObtenerEstadosSolicitudAsync()
        {
            return await _context.EstadosSolicitud
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }
    }
}