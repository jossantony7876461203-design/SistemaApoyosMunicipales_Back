using DocumentFormat.OpenXml.Office2010.Excel;
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

        public RegistroApoyoService(
            IRegistroApoyoRepository registroRepository,
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService)
        {
            _registroRepository = registroRepository;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        // =========================
        // CREAR - CORREGIDO
        // =========================
        public async Task<Guid> CrearAsync(CrearRegistroApoyoDto dto, Guid usuarioId)
        {
            var folio = dto.Folio.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(folio))
                throw new ValidationException("El folio es obligatorio.");

            if (await _registroRepository.ExisteFolioAsync(folio))
                throw new ValidationException("Ya existe un apoyo registrado con ese folio.");

            ValidarDatosRegistro(dto.ApoyoId, dto.ComunidadId, dto.EstadoSolicitudId, dto.MontoOtorgado);

            if (usuarioId == Guid.Empty)
                throw new UnauthorizedException("No se pudo identificar al usuario autenticado.");



            var registro = new RegistroApoyo
            {
                Id = Guid.NewGuid(),
                Folio = folio,
                ApoyoId = dto.ApoyoId,
                ComunidadId = dto.ComunidadId,
                EstadoSolicitudId = dto.EstadoSolicitudId,
                FechaApoyo = dto.FechaApoyo,
                MontoOtorgado = dto.MontoOtorgado,
                Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim(),
                RegistradoPor = usuarioId,
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

     

            // Subir documentos si existen
            if (dto.Archivos is not null && dto.Archivos.Count > 0)
            {
                var nuevosDocumentos = await SubirDocumentosAsync(
                    registro.Id,  // CORREGIDO: usar registro.Id
                    dto.Archivos,
                    dto.Montos,
                    dto.TiposDocumento,
                    dto.Descripciones,
                    dto.Facturados,
                    dto.MetodosPago,
                    dto.FechasFacturado);

                foreach (var documento in nuevosDocumentos)  // CORREGIDO: usar nuevosDocumentos
                    registro.Documentos.Add(documento);
            }

            await _registroRepository.AgregarAsync(registro);
            await _unitOfWork.SaveChangesAsync();

            return registro.Id;
        }

        // =========================
        // ACTUALIZAR - CORREGIDO
        // =========================
        public async Task ActualizarAsync(Guid id, ActualizarRegistroApoyoDto dto)
        {
            var registro = await _registroRepository.ObtenerPorIdParaEditarAsync(id);

            if (registro is null)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

            var folio = dto.Folio.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(folio))
            {
                throw new ValidationException("El folio es obligatorio.");
            }

            if (await _registroRepository.ExisteFolioEnOtroRegistroAsync(folio, id))
            {
                throw new ValidationException("Ya existe otro apoyo registrado con ese folio.");
            }

            ValidarDatosRegistro(
                dto.ApoyoId,
                dto.ComunidadId,
                dto.EstadoSolicitudId,
                dto.MontoOtorgado);

            // Actualizar datos del registro
            registro.Folio = folio;
            registro.ApoyoId = dto.ApoyoId;
            registro.ComunidadId = dto.ComunidadId;
            registro.EstadoSolicitudId = dto.EstadoSolicitudId;
            registro.FechaApoyo = dto.FechaApoyo;
            registro.MontoOtorgado = dto.MontoOtorgado;
            registro.Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones)
                ? null
                : dto.Observaciones.Trim();
            registro.UpdatedAt = DateTimeOffset.UtcNow;

            var publicIdsAEliminar = new List<string>();

            // Si se enviaron nuevos archivos, reemplazar documentos
            if (dto.Archivos is not null && dto.Archivos.Count > 0)
            {
                // Obtener documentos actuales para eliminar de Cloudinary
                var documentosActuales = await _registroRepository.ObtenerDocumentosAsync(id);

                publicIdsAEliminar = documentosActuales
                    .Select(documento => documento.PublicId)
                    .Where(publicId => !string.IsNullOrWhiteSpace(publicId))
                    .ToList();

                // Eliminar documentos de la base de datos
                await _registroRepository.EliminarDocumentosAsync(id);

                // Subir nuevos documentos
                var nuevosDocumentos = await SubirDocumentosAsync(
                    id,
                    dto.Archivos,
                    dto.Montos,
                    dto.TiposDocumento,
                    dto.Descripciones,
                    dto.Facturados,
                    dto.MetodosPago,
                    dto.FechasFacturado);

                await _registroRepository.AgregarDocumentosAsync(nuevosDocumentos);
            }

            // Guardar cambios en la base de datos
            await _unitOfWork.SaveChangesAsync();

            // Eliminar documentos viejos de Cloudinary (después de guardar en DB)
            foreach (var publicId in publicIdsAEliminar)
            {
                await _cloudinaryService.EliminarImagenAsync(publicId);
            }
        }


        
        // =========================
        // OBTENER POR ID
        // =========================
        public async Task<ObtenerRegistroApoyoDto> ObtenerPorIdAsync(Guid id)
        {
            var registro = await _registroRepository.ObtenerPorIdAsync(id);

            if (registro is null)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

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
                    .Select(documento => new RegistroApoyoDocumentoDto
                    {
                        Id = documento.Id,
                        NombreArchivo = documento.NombreArchivo,
                        Url = documento.Url,
                        TipoDocumento = documento.TipoDocumento,
                        Monto = documento.Monto,
                        Descripcion = documento.Descripcion,
                        Facturado = documento.Facturado,
                        MetodoPago = documento.MetodoPago,
                        FechaFacturado = documento.FechaFacturado
                    })
                    .ToList() ?? new List<RegistroApoyoDocumentoDto>()
            };
        }

        // =========================
        // OBTENER POR COMUNIDAD
        // =========================
        public async Task<PaginatedResult<ObtenerRegistroApoyoListadoDto>> ObtenerPorComunidadAsync(
            Guid comunidadId,
            PaginationRequest pagination)
        {
            var resultado = await _registroRepository.ObtenerPorComunidadAsync(comunidadId, pagination);

            return new PaginatedResult<ObtenerRegistroApoyoListadoDto>
            {
                Items = resultado.Items
                    .Select(registro => new ObtenerRegistroApoyoListadoDto
                    {
                        Id = registro.Id,
                        Apoyo = registro.Apoyo?.Nombre ?? string.Empty,
                        Comunidad = registro.Comunidad?.Nombre ?? string.Empty,
                        EstadoSolicitud = registro.EstadoSolicitud?.Nombre ?? string.Empty,
                        MontoOtorgado = registro.MontoOtorgado,
                        FechaApoyo = registro.FechaApoyo,
                        Activo = registro.Activo

                    })
                    .ToList(),
                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        // =========================
        // CAMBIAR ESTADO
        // =========================
        public async Task CambiarEstadoAsync(Guid id, Guid estadoSolicitudId)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("El identificador del registro no es válido.");
            }

            if (estadoSolicitudId == Guid.Empty)
            {
                throw new ValidationException("Debes seleccionar un estado de solicitud.");
            }

            var existe = await _registroRepository.ExisteAsync(id);

            if (!existe)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

            await _registroRepository.CambiarEstatusAsync(id, estadoSolicitudId);
            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // AGREGAR DOCUMENTOS
        // =========================
        public async Task<List<RegistroApoyoDocumentoDto>> AgregarDocumentosAsync(
            Guid id,
            AgregarDocumentosRegistroApoyoDto dto)
        {
            var existe = await _registroRepository.ExisteAsync(id);

            if (!existe)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

            if (dto.Archivos is null || dto.Archivos.Count == 0)
            {
                throw new BadRequestException("Debes adjuntar al menos un archivo.");
            }

            var nuevosDocumentos = await SubirDocumentosAsync(
                id,
                dto.Archivos,
                dto.Montos,
                dto.TiposDocumento,
                dto.Descripciones,
                dto.Facturados,
                dto.MetodosPago,
                dto.FechasFacturado);

            await _registroRepository.AgregarDocumentosAsync(nuevosDocumentos);
            await _unitOfWork.SaveChangesAsync();

            return nuevosDocumentos
                .Select(documento => new RegistroApoyoDocumentoDto
                {
                    Id = documento.Id,
                    NombreArchivo = documento.NombreArchivo,
                    Url = documento.Url,
                    TipoDocumento = documento.TipoDocumento,
                    Monto = documento.Monto,
                    Descripcion = documento.Descripcion,
                    Facturado = documento.Facturado,
                    MetodoPago = documento.MetodoPago,
                    FechaFacturado = documento.FechaFacturado
                })
                .ToList();
        }

        // =========================
        // DETALLE
        // =========================
        public async Task<ObtenerRegistroApoyoDetalleDto> ObtenerDetalleAsync(Guid id)
        {
            var registro = await _registroRepository.ObtenerPorIdAsync(id);

            if (registro is null)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

            return new ObtenerRegistroApoyoDetalleDto
            {
                Id = registro.Id,
                Descripcion = registro.Observaciones,
                Delegado = registro.Comunidad?.Delegado,
                Estatus = registro.EstadoSolicitud?.Nombre,
                Documentos = registro.Documentos?
                    .Select(documento => new RegistroApoyoDocumentoDetalleDto
                    {
                        Id = documento.Id,
                        NombreArchivo = documento.NombreArchivo,
                        Url = documento.Url,
                        TipoDocumento = documento.TipoDocumento,
                        Monto = documento.Monto,
                        Descripcion = documento.Descripcion,
                        Facturado = documento.Facturado,
                        MetodoPago = documento.MetodoPago,
                        FechaFacturado = documento.FechaFacturado

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
                Items = resultado.Items
                    .Select(registro => new ObtenerRegistroApoyoGlobalDto
                    {
                        Id = registro.Id,
                        Folio = registro.Folio,
                        Comunidad = registro.Comunidad?.Nombre,
                        Fondo = registro.Apoyo?.Nombre,
                        TipoApoyo = registro.Apoyo?.Nombre,
                        FechaRegistro = registro.CreatedAt,
                        Estado = registro.EstadoSolicitud?.Nombre,
                        Delegado = registro.Comunidad?.Delegado

                    })
                    .ToList(),
                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        // =========================
        // ELIMINAR
        // =========================
        public async Task EliminarAsync(Guid id)
        {
            var existe = await _registroRepository.ExisteAsync(id);

            if (!existe)
            {
                throw new NotFoundException("El registro de apoyo no existe.");
            }

            await _registroRepository.EliminarAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // CATÁLOGO DE ESTADOS
        // =========================
        public async Task<List<EstadoSolicitudCatalogoDto>> ObtenerEstadosSolicitudAsync()
        {
            var estados = await _registroRepository.ObtenerEstadosSolicitudAsync();

            return estados
                .Select(estado => new EstadoSolicitudCatalogoDto
                {
                    Id = estado.Id,
                    Nombre = estado.Nombre ?? string.Empty
                })
                .ToList();
        }

        // =========================
        // VALIDACIONES GENERALES
        // =========================
        private static void ValidarDatosRegistro(
            Guid apoyoId,
            Guid comunidadId,
            Guid estadoSolicitudId,
            decimal montoOtorgado)
        {
            if (apoyoId == Guid.Empty)
            {
                throw new ValidationException("Debes seleccionar un tipo de apoyo.");
            }

            if (comunidadId == Guid.Empty)
            {
                throw new ValidationException("Debes seleccionar una comunidad.");
            }

            if (estadoSolicitudId == Guid.Empty)
            {
                throw new ValidationException("Debes seleccionar un estado de solicitud.");
            }

            if (montoOtorgado <= 0)
            {
                throw new ValidationException("El monto otorgado debe ser mayor a cero.");
            }
        }

        // =========================
        // SUBIR DOCUMENTOS - ACTUALIZADO CON NUEVOS PARÁMETROS
        // =========================
        private static readonly HashSet<string> TiposDocumentoValidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "imagen",
                "factura",
                "otro"
            };

        private async Task<List<RegistroApoyoDocumento>> SubirDocumentosAsync(
            Guid registroId,
            List<IFormFile> archivos,
            List<decimal>? montos,
            List<string>? tiposDocumento,
            List<string>? descripciones,
            List<bool>? facturados,
            List<string>? metodosPago,
            List<DateTimeOffset?>? fechasFacturado)
        {
            if (archivos.Count == 0)
            {
                return new List<RegistroApoyoDocumento>();
            }

            if (tiposDocumento is not null)
            {
                foreach (var tipo in tiposDocumento)
                {
                    if (string.IsNullOrWhiteSpace(tipo))
                    {
                        continue;
                    }

                    if (!TiposDocumentoValidos.Contains(tipo))
                    {
                        throw new BadRequestException(
                            $"'{tipo}' no es un tipo de documento válido. " +
                            "Usa: imagen, factura u otro.");
                    }
                }
            }

            if (montos is not null && montos.Any(monto => monto < 0))
            {
                throw new ValidationException("Los montos de los documentos no pueden ser negativos.");
            }

            var documentos = new List<RegistroApoyoDocumento>();

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

                    var monto = montos is not null && index < montos.Count
                        ? montos[index]
                        : 0m;

                    var tipo = tiposDocumento is not null &&
                               index < tiposDocumento.Count &&
                               !string.IsNullOrWhiteSpace(tiposDocumento[index])
                        ? tiposDocumento[index].Trim().ToLowerInvariant()
                        : "factura";

                    var descripcion = descripciones is not null &&
                                      index < descripciones.Count &&
                                      !string.IsNullOrWhiteSpace(descripciones[index])
                        ? descripciones[index].Trim()
                        : null;

                    var facturado = facturados is not null &&
                                    index < facturados.Count
                        ? facturados[index]
                        : false;

                    var metodoPago = metodosPago is not null &&
                                     index < metodosPago.Count &&
                                     !string.IsNullOrWhiteSpace(metodosPago[index])
                        ? metodosPago[index].Trim()
                        : null;

                    var fechaFacturado = fechasFacturado is not null &&
                                         index < fechasFacturado.Count
                        ? fechasFacturado[index]
                        : null;

                    return new RegistroApoyoDocumento
                    {
                        Id = Guid.NewGuid(),
                        RegistroApoyoId = registroId,
                        NombreArchivo = archivo.FileName,
                        TipoDocumento = tipo,
                        Monto = monto,
                        Descripcion = descripcion,
                        Url = resultado.Url,
                        PublicId = resultado.PublicId,
                        Facturado = facturado,
                        MetodoPago = metodoPago,
                        FechaFacturado = fechaFacturado,
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