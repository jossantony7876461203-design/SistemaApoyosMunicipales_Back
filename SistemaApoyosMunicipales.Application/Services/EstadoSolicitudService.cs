using SistemaApoyosMunicipales.Application.DTOs.EstadosSolicitud;

using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Estados;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class EstadoSolicitudService : IEstadoSolicitudService
    {
        private readonly IEstadoSolicitudRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EstadoSolicitudService(
            IEstadoSolicitudRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // =========================
        // CREAR
        // =========================
        public async Task<Guid> CrearAsync(CrearEstadoSolicitudDto dto)
        {
            var clave = dto.Clave.Trim().ToLowerInvariant();
            var nombre = dto.Nombre.Trim();

            if (string.IsNullOrWhiteSpace(clave))
                throw new ValidationException("La clave es obligatoria.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidationException("El nombre es obligatorio.");

            if (await _repository.ExisteClaveAsync(clave))
                throw new ValidationException("Ya existe un estado de solicitud con esa clave.");

            var estado = new EstadoSolicitud
            {
                Id = Guid.NewGuid(),
                Clave = clave,
                Nombre = nombre,
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _repository.AgregarAsync(estado);
            await _unitOfWork.SaveChangesAsync();

            return estado.Id;
        }

        // =========================
        // ACTUALIZAR
        // =========================
        public async Task ActualizarAsync(Guid id, ActualizarEstadoSolicitudDto dto)
        {
            var estado = await _repository.ObtenerPorIdAsync(id);

            if (estado is null)
                throw new NotFoundException("El estado de solicitud no existe.");

            var clave = dto.Clave.Trim().ToLowerInvariant();
            var nombre = dto.Nombre.Trim();

            if (string.IsNullOrWhiteSpace(clave))
                throw new ValidationException("La clave es obligatoria.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidationException("El nombre es obligatorio.");

            if (await _repository.ExisteClaveEnOtroRegistroAsync(clave, id))
                throw new ValidationException("Ya existe otro estado de solicitud con esa clave.");

            estado.Clave = clave;
            estado.Nombre = nombre;
            estado.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();

            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // OBTENER POR ID
        // =========================
        public async Task<EstadoSolicitudDto> ObtenerPorIdAsync(Guid id)
        {
            var estado = await _repository.ObtenerPorIdAsync(id);

            if (estado is null)
                throw new NotFoundException("El estado de solicitud no existe.");

            return MapearDto(estado);
        }

        // =========================
        // OBTENER TODOS
        // =========================
        public async Task<List<EstadoSolicitudDto>> ObtenerTodosAsync()
        {
            var estados = await _repository.ObtenerTodosAsync();
            return estados.Select(MapearDto).ToList();
        }

        // =========================
        // CAMBIAR ESTATUS (activar/desactivar)
        // =========================
        public async Task CambiarEstatusAsync(Guid id, CambiarEstatusEstadoSolicitudDto dto)
        {
            var existe = await _repository.ExisteAsync(id);

            if (!existe)
                throw new NotFoundException("El estado de solicitud no existe.");

            // Regla de negocio: no permitir desactivar un estado que ya
            // tiene registros de apoyo asociados, para no dejar huérfanos
            // en pantallas de filtro/reportes.
            if (!dto.Activo && await _repository.TieneRegistrosAsociadosAsync(id))
            {
                throw new ValidationException(
                    "No se puede desactivar este estado porque tiene registros de apoyo asociados.");
            }

            await _repository.CambiarEstatusAsync(id, dto.Activo);
            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // HELPER DE MAPEO
        // =========================
        private static EstadoSolicitudDto MapearDto(EstadoSolicitud estado) => new()
        {
            Id = estado.Id,
            Clave = estado.Clave,
            Nombre = estado.Nombre,
            Descripcion = estado.Descripcion,
            Activo = estado.Activo,
            CreatedAt = estado.CreatedAt
        };
    }
}
