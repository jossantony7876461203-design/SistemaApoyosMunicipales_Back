using Microsoft.AspNetCore.Http;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class RegistroApoyoService : IRegistroApoyoService
    {
        private readonly IRegistroApoyoRepository _registroRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICurrentUserService _currentUser;

        public RegistroApoyoService(
            IRegistroApoyoRepository registroRepository,
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            ICurrentUserService currentUser)
        {
            _registroRepository = registroRepository;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _currentUser = currentUser;
        }

        // =========================
        // CREATE (AUDITORÍA)
        // =========================
        public async Task<Guid> CrearAsync(CrearRegistroApoyoDto dto, Guid usuarioId)
        {
            var registro = new RegistroApoyo
            {
                Id = Guid.NewGuid(),
                Folio = dto.Folio,
                ApoyoId = dto.ApoyoId,
                ComunidadId = dto.ComunidadId,
                EstadoSolicitudId = dto.EstadoSolicitudId,
                FechaApoyo = dto.FechaApoyo,
                MontoOtorgado = dto.MontoOtorgado,
                Observaciones = dto.Observaciones,

                // AUDITORÍA
                RegistradoPor = usuarioId,

                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            // Subimos los documentos ANTES de tocar el contexto,
            // y los agregamos a la colección existente (NO se reasigna la propiedad).
            if (dto.Archivos != null && dto.Archivos.Count > 0)
            {
                var documentos = await SubirDocumentosAsync(
                    registro.Id,
                    dto.Archivos,
                    dto.Montos,
                    dto.TiposDocumento);

                foreach (var doc in documentos)
                    registro.Documentos.Add(doc);
            }

            await _registroRepository.AgregarAsync(registro);

            // Un único SaveChangesAsync: inserta el registro y sus documentos juntos.
            await _unitOfWork.SaveChangesAsync();

            return registro.Id;
        }

        // =========================
        // UPDATE (AUDITORÍA)
        // =========================
        public async Task ActualizarAsync(Guid id, ActualizarRegistroApoyoDto dto)
        {
            var registro = await _registroRepository.ObtenerPorIdParaEditarAsync(id);

            if (registro is null)
                throw new NotFoundException("El registro de apoyo no existe.");

            registro.Folio = dto.Folio;
            registro.ApoyoId = dto.ApoyoId;
            registro.ComunidadId = dto.ComunidadId;
            registro.EstadoSolicitudId = dto.EstadoSolicitudId;
            registro.FechaApoyo = dto.FechaApoyo;
            registro.MontoOtorgado = dto.MontoOtorgado;
            registro.Observaciones = dto.Observaciones;
            registro.UpdatedAt = DateTimeOffset.UtcNow;

            // 🔥 MODIFICADO: Manejar documentos sin conflicto de concurrencia
            var publicIdsAEliminar = new List<string>();

            if (dto.Archivos?.Count > 0)
            {
                // Obtener documentos actuales para eliminar de Cloudinary después
                var documentosActuales = await _registroRepository.ObtenerDocumentosAsync(id);
                publicIdsAEliminar = documentosActuales.Select(x => x.PublicId).ToList();

                // ELIMINAR documentos viejos de la BD (directamente, sin tocar registro)
                await _registroRepository.EliminarDocumentosAsync(id);

                // SUBIR y GUARDAR nuevos documentos
                var nuevosDocumentos = await SubirDocumentosAsync(
                    id,
                    dto.Archivos,
                    dto.Montos,
                    dto.TiposDocumento);

                await _registroRepository.AgregarDocumentosAsync(nuevosDocumentos);
            }

            // Solo guardar el registro actualizado
            await _unitOfWork.SaveChangesAsync();

            // Eliminar de Cloudinary después de confirmar la BD
            foreach (var publicId in publicIdsAEliminar)
                await _cloudinaryService.EliminarImagenAsync(publicId);
        }

        public async Task<ObtenerRegistroApoyoDto> ObtenerPorIdAsync(Guid id)
        {
            var registro = await _registroRepository.ObtenerPorIdAsync(id);

            if (registro is null)
                throw new NotFoundException("El registro de apoyo no existe.");

            return new ObtenerRegistroApoyoDto
            {
                Id = registro.Id,
                ApoyoId = registro.ApoyoId,
                Apoyo = registro.Apoyo?.Nombre,
                ComunidadId = registro.ComunidadId,
                Comunidad = registro.Comunidad?.Nombre,
                EstadoSolicitudId = registro.EstadoSolicitudId,
                EstadoSolicitud = registro.EstadoSolicitud?.Nombre,
                FechaApoyo = registro.FechaApoyo,
                MontoOtorgado = registro.MontoOtorgado,
                Observaciones = registro.Observaciones,
                Activo = registro.Activo,
                CreatedAt = registro.CreatedAt,

                Documentos = registro.Documentos?
                    .Select(d => new RegistroApoyoDocumentoDto
                    {
                        Id = d.Id,
                        NombreArchivo = d.NombreArchivo,
                        Url = d.Url,
                        TipoDocumento = d.TipoDocumento
                    })
                    .ToList() ?? new List<RegistroApoyoDocumentoDto>()
            };
        }

        public async Task<PaginatedResult<ObtenerRegistroApoyoListadoDto>>
            ObtenerPorComunidadAsync(Guid comunidadId, PaginationRequest pagination)
        {
            var resultado = await _registroRepository.ObtenerPorComunidadAsync(comunidadId, pagination);

            return new PaginatedResult<ObtenerRegistroApoyoListadoDto>
            {
                Items = resultado.Items.Select(x => new ObtenerRegistroApoyoListadoDto
                {
                    Id = x.Id,
                    Apoyo = x.Apoyo?.Nombre,
                    Comunidad = x.Comunidad?.Nombre,
                    EstadoSolicitud = x.EstadoSolicitud?.Nombre,
                    MontoOtorgado = x.MontoOtorgado,
                    FechaApoyo = x.FechaApoyo,
                    Activo = x.Activo
                }).ToList(),

                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        public async Task CambiarEstadoAsync(Guid id, Guid estadoSolicitudId)
        {
            await _registroRepository.CambiarEstatusAsync(id, estadoSolicitudId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RegistroApoyoDocumentoDto>> AgregarDocumentosAsync(
         Guid id,
         AgregarDocumentosRegistroApoyoDto dto)
        {
            // 1. Verificar que el registro existe (solo lectura ligera)
            var existe = await _registroRepository.ExisteAsync(id);
            if (!existe)
                throw new NotFoundException("El registro de apoyo no existe.");

            // 2. Validar que hay archivos
            if (dto.Archivos is null || dto.Archivos.Count == 0)
                throw new BadRequestException("Debes adjuntar al menos un archivo.");

            // 3. Subir documentos a Cloudinary
            var nuevosDocumentos = await SubirDocumentosAsync(
                id,
                dto.Archivos,
                dto.Montos,
                dto.TiposDocumento);

            // 4. Guardar documentos DIRECTAMENTE en su tabla (sin tocar el registro)
            await _registroRepository.AgregarDocumentosAsync(nuevosDocumentos);
            await _unitOfWork.SaveChangesAsync();

            // 5. Retornar respuesta
            return nuevosDocumentos.Select(d => new RegistroApoyoDocumentoDto
            {
                Id = d.Id,
                NombreArchivo = d.NombreArchivo,
                Url = d.Url,
                TipoDocumento = d.TipoDocumento,
                Monto = d.Monto
            }).ToList();
        }
        public async Task<ObtenerRegistroApoyoDetalleDto> ObtenerDetalleAsync(Guid id)
        {
            var registro = await _registroRepository.ObtenerPorIdAsync(id);

            if (registro is null)
                throw new NotFoundException("El registro de apoyo no existe.");

            return new ObtenerRegistroApoyoDetalleDto
            {
                Id = registro.Id,
                Descripcion = registro.Observaciones,
                Delegado = registro.Comunidad?.Delegado,
                Estatus = registro.EstadoSolicitud?.Nombre,

                Documentos = registro.Documentos?
                    .Select(d => new RegistroApoyoDocumentoDetalleDto
                    {
                        Id = d.Id,
                        NombreArchivo = d.NombreArchivo,
                        Url = d.Url,
                        TipoDocumento = d.TipoDocumento,
                        Monto = d.Monto
                    })
                    .ToList() ?? new List<RegistroApoyoDocumentoDetalleDto>()
            };
        }

        // =========================
        // LISTADO GLOBAL
        // =========================
        public async Task<PaginatedResult<ObtenerRegistroApoyoGlobalDto>> ObtenerTodosAsync(
            PaginationRequest pagination)
        {
            var resultado = await _registroRepository.ObtenerTodosAsync(pagination);

            return new PaginatedResult<ObtenerRegistroApoyoGlobalDto>
            {
                Items = resultado.Items.Select(x => new ObtenerRegistroApoyoGlobalDto
                {
                    Id = x.Id,
                    Folio = x.Folio,
                    Comunidad = x.Comunidad?.Nombre,
                    Fondo = x.Apoyo?.Nombre,
                    TipoApoyo = x.Apoyo?.Nombre,
                    FechaRegistro = x.CreatedAt,
                    Estado = x.EstadoSolicitud?.Nombre,
                    Delegado = x.Comunidad?.Delegado
                }).ToList(),

                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        public async Task EliminarAsync(Guid id)
        {
            await _registroRepository.EliminarAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // DOCUMENTOS
        // =========================
        private static readonly HashSet<string> TiposDocumentoValidos =
            new(StringComparer.OrdinalIgnoreCase) { "imagen", "factura", "otro" };

        private async Task<List<RegistroApoyoDocumento>> SubirDocumentosAsync(
            Guid registroId,
            List<IFormFile> archivos,
            List<decimal>? montos,
            List<string>? tiposDocumento)
        {
            // Validamos ANTES de subir nada a Cloudinary, para no
            // gastar uploads si el request va a fallar de todos modos.
            if (tiposDocumento != null)
            {
                foreach (var tipo in tiposDocumento)
                {
                    if (!TiposDocumentoValidos.Contains(tipo))
                        throw new BadRequestException(
                            $"'{tipo}' no es un tipo de documento válido. Usa: imagen, factura u otro.");
                }
            }

            var documentos = new List<RegistroApoyoDocumento>();

            // Convertimos a array para poder acceder por índice de forma segura,
            // ya que Chunk(3) hace los lotes pero necesitamos el índice ORIGINAL
            // de cada archivo para emparejarlo con su monto/tipo correspondiente.
            var archivosIndexados = archivos
                .Select((archivo, index) => (archivo, index))
                .ToList();

            foreach (var lote in archivosIndexados.Chunk(3))
            {
                var tareas = lote.Select(async item =>
                {
                    var (archivo, index) = item;

                    await using var stream = archivo.OpenReadStream();

                    var resultado = await _cloudinaryService.SubirImagenAsync(
                        stream,
                        archivo.FileName,
                        "registro-apoyos");

                    var monto = montos != null && index < montos.Count
                        ? montos[index]
                        : 0m;

                    var tipo = tiposDocumento != null && index < tiposDocumento.Count
                        ? tiposDocumento[index].ToLowerInvariant()
                        : "factura";

                    return new RegistroApoyoDocumento
                    {
                        Id = Guid.NewGuid(),
                        RegistroApoyoId = registroId,
                        NombreArchivo = archivo.FileName,
                        TipoDocumento = tipo,
                        Monto = monto,
                        Url = resultado.Url,
                        PublicId = resultado.PublicId,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                });

                var resultadoLote = await Task.WhenAll(tareas);
                documentos.AddRange(resultadoLote);
            }

            return documentos;
        }
    }
}