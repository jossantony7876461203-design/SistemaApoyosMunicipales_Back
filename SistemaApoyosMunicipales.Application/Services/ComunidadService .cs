using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ComunidadService : IComunidadService
    {
        private readonly IComunidadRepository _comunidadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImagenQueue _imagenQueue;

        public ComunidadService(
            IComunidadRepository comunidadRepository,
            IUnitOfWork unitOfWork,
              ICloudinaryService cloudinaryService,
              IImagenQueue imagenQueue)
        {
            _comunidadRepository = comunidadRepository;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _imagenQueue = imagenQueue;
        }


        public async Task<Guid?> CrearAsync(CrearComunidadDto dto)
        {
            var existe = await _comunidadRepository
                .ObtenerPorClaveInternaAsync(dto.ClaveInterna);

            if (existe is not null)
                throw new ValidationException(
                    "La clave interna ya se encuentra registrada.");

            // 1. Crear comunidad SIN imagen primero
            var comunidad = new Domain.Entities.Comunidad.Comunidad
            {
                Id = Guid.NewGuid(),
                ClaveInterna = dto.ClaveInterna.Trim(),
                Nombre = dto.Nombre.Trim(),
                CodigoPostal = dto.CodigoPostal.Trim(),
                Delegado = dto.Delegado?.Trim(),
                TelefonoDelegado = dto.TelefonoDelegado?.Trim(),
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _comunidadRepository.AgregarAsync(comunidad);
            await _unitOfWork.SaveChangesAsync();

            // 2. Si hay imagen, encolar en background
            Guid? tareaId = null;

            if (dto.DelegadoIne is not null)
            {
                // Leer bytes aquí porque IFormFile no es thread-safe
                await using var stream = dto.DelegadoIne.OpenReadStream();
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes);

                var tarea = new ImagenTarea
                {
                    NombreArchivo = dto.DelegadoIne.FileName,
                    Bytes = bytes,
                    Carpeta = "comunidades/ine",
                    EntidadId = comunidad.Id
                };

                _imagenQueue.Encolar(tarea);
                tareaId = tarea.Id;
            }

            return tareaId; // null si no había imagen
        }

        // =========================================================
        // Actualizar solo la imagen del INE
        // =========================================================
        public async Task ActualizarIneAsync(Guid id, ActualizarIneDto dto)
        {
            var comunidad = await _comunidadRepository
                .ObtenerPorIdParaEditarAsync(id);

            if (comunidad is null)
                throw new NotFoundException("La comunidad no existe.");

            string? pubIdAnterior = comunidad.DelegadoInePubId;
            string? pubIdSubido = null;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Subir imagen nueva
                await using var stream = dto.Imagen.OpenReadStream();

                var resultado = await _cloudinaryService.SubirImagenAsync(
                    stream,
                    dto.Imagen.FileName,
                    "comunidades/ine");

                pubIdSubido = resultado.PublicId;

                // 2. Actualizar entidad
                comunidad.DelegadoIneUrl = resultado.Url;
                comunidad.DelegadoInePubId = resultado.PublicId;
                comunidad.UpdatedAt = DateTimeOffset.UtcNow;

                // 3. Guardar en BD
                await _unitOfWork.SaveChangesAsync();

                // 4. Confirmar transacción
                await _unitOfWork.CommitAsync();

                // 5. Eliminar imagen anterior de Cloudinary
                // Se hace DESPUÉS del commit para no bloquear si falla
                if (pubIdAnterior is not null)
                    await _cloudinaryService.EliminarImagenAsync(pubIdAnterior);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();

                // Si la imagen nueva ya subió pero la BD falló, eliminarla
                if (pubIdSubido is not null)
                    await _cloudinaryService.EliminarImagenAsync(pubIdSubido);

                throw;
            }
        }

        public async Task<ComunidadDto> ObtenerPorIdAsync(Guid id)
        {
            var comunidad = await _comunidadRepository
                .ObtenerPorIdAsync(id);

            if (comunidad is null)
                throw new NotFoundException(
                    "La comunidad no existe.");

            return new ComunidadDto
            {
                Id = comunidad.Id,
                ClaveInterna = comunidad.ClaveInterna,
                Nombre = comunidad.Nombre,
                CodigoPostal = comunidad.CodigoPostal,
                Delegado = comunidad.Delegado,
                TelefonoDelegado = comunidad.TelefonoDelegado,
                Activo=comunidad.Activo,
                DelegadoIneUrl = comunidad.DelegadoIneUrl

            };
        }


        public async Task CambiarEstatusAsync(
      Guid id,
      CambiarEstatusComunidadDto dto)
        {
            var comunidad = await _comunidadRepository
                .ObtenerPorIdParaEditarAsync(id); // ← este

            if (comunidad is null)
                throw new NotFoundException(
                    "La comunidad no existe.");

            comunidad.Activo = dto.Activo;
            comunidad.DeletedAt = dto.Activo ? null : DateTimeOffset.UtcNow;
            comunidad.UpdatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
    Guid id,
    ActualizarComunidadDto dto)
        {
            var comunidad =
                await _comunidadRepository.ObtenerPorIdParaEditarAsync(id);

            if (comunidad is null)
                throw new NotFoundException(
                    "La comunidad no existe.");

            if (!string.IsNullOrWhiteSpace(dto.ClaveInterna))
            {
                var claveNormalizada = dto.ClaveInterna.Trim();

                if (!string.Equals(comunidad.ClaveInterna, claveNormalizada,
                        StringComparison.OrdinalIgnoreCase))
                {
                    var existente =
                        await _comunidadRepository
                            .ObtenerPorClaveInternaAsync(claveNormalizada);

                    if (existente is not null &&
                        existente.Id != comunidad.Id)
                    {
                        throw new ValidationException(
                            "La clave interna ya está registrada.");
                    }
                }

                comunidad.ClaveInterna = claveNormalizada;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                comunidad.Nombre =
                    dto.Nombre.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.CodigoPostal))
            {
                comunidad.CodigoPostal =
                    dto.CodigoPostal.Trim();
            }

            if (dto.Delegado is not null)
            {
                comunidad.Delegado =
                    dto.Delegado.Trim();
            }

            if (dto.TelefonoDelegado is not null)
            {
                comunidad.TelefonoDelegado =
                    dto.TelefonoDelegado.Trim();
            }

            comunidad.UpdatedAt =
                DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<PaginatedResult<ComunidadDto>>
      ObtenerTodasAsync(
          PaginationRequest pagination)
        {
            var resultado =
                await _comunidadRepository.ObtenerTodasAsync(
                    pagination,
                    true);

            return new PaginatedResult<ComunidadDto>
            {
                Items = resultado.Items
                    .Select(x => new ComunidadDto
                    {
                        Id = x.Id,
                        ClaveInterna = x.ClaveInterna,
                        Nombre = x.Nombre,
                        CodigoPostal = x.CodigoPostal,
                        Delegado = x.Delegado,
                        TelefonoDelegado = x.TelefonoDelegado,
                        Activo=x.Activo,
                        DelegadoIneUrl = x.DelegadoIneUrl
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

        public async Task<PaginatedResult<ComunidadDto>>
    ObtenerInactivasAsync(
        PaginationRequest pagination)
        {
            var resultado =
                await _comunidadRepository.ObtenerTodasAsync(
                    pagination,
                    false);

            return new PaginatedResult<ComunidadDto>
            {
                Items = resultado.Items
                    .Select(x => new ComunidadDto
                    {
                        Id = x.Id,
                        ClaveInterna = x.ClaveInterna,
                        Nombre = x.Nombre,
                        CodigoPostal = x.CodigoPostal,
                        Delegado = x.Delegado,
                        TelefonoDelegado = x.TelefonoDelegado
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





        public async Task<ComunidadDto>
    ObtenerPorClaveInternaAsync(
        string claveInterna)
        {
            var comunidad =
                await _comunidadRepository
                    .ObtenerPorClaveInternaAsync(claveInterna);

            if (comunidad is null)
                throw new NotFoundException(
                    "La comunidad no existe.");

            return new ComunidadDto
            {
                Id = comunidad.Id,
                ClaveInterna = comunidad.ClaveInterna,
                Nombre = comunidad.Nombre,
                CodigoPostal = comunidad.CodigoPostal,
                Delegado = comunidad.Delegado,
                TelefonoDelegado = comunidad.TelefonoDelegado,
                Activo=comunidad.Activo,
            };
        }
    }
}