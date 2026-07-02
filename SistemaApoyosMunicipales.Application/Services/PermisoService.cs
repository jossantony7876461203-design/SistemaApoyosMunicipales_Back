using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Permisos;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _permisoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermisoService(
            IPermisoRepository permisoRepository,
            IUnitOfWork unitOfWork)
        {
            _permisoRepository = permisoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CrearAsync(CrearPermisoDto dto)
        {
            var existe = await _permisoRepository
                .ExisteAsync(dto.Codigo.Trim().ToLower());

            if (existe)
                throw new ValidationException(
                    "Ya existe un permiso con ese código.");

            var permiso = new Permiso
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim().ToLower(),
                Nombre = dto.Nombre.Trim(),
                Modulo = dto.Modulo.Trim().ToLower(),
                Descripcion = dto.Descripcion?.Trim(),
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _permisoRepository.CrearAsync(permiso);
            await _unitOfWork.SaveChangesAsync();

            return permiso.Id;
        }

        public async Task<PermisoDto> ObtenerPorIdAsync(Guid id)
        {
            var permiso = await _permisoRepository
                .ObtenerPorIdAsync(id);

            if (permiso is null)
                throw new NotFoundException("El permiso no existe.");

            return MapearDto(permiso);
        }

        public async Task<PaginatedResult<PermisoDto>> ObtenerActivosAsync(
            PaginationRequest pagination)
        {
            var resultado = await _permisoRepository
                .ObtenerTodosAsync(pagination, true);

            return MapearPaginado(resultado);
        }

        public async Task<PaginatedResult<PermisoDto>> ObtenerInactivosAsync(
            PaginationRequest pagination)
        {
            var resultado = await _permisoRepository
                .ObtenerTodosAsync(pagination, false);

            return MapearPaginado(resultado);
        }

        public async Task<IEnumerable<PermisoDto>> ObtenerPorModuloAsync(
            string modulo)
        {
            if (string.IsNullOrWhiteSpace(modulo))
                throw new ValidationException("El módulo es requerido.");

            var permisos = await _permisoRepository
                .ObtenerPorModuloAsync(modulo.Trim().ToLower());

            return permisos.Select(MapearDto);
        }

        public async Task ActualizarAsync(Guid id, ActualizarPermisoDto dto)
        {
            var permiso = await _permisoRepository
                .ObtenerPorIdAsync(id);

            if (permiso is null)
                throw new NotFoundException("El permiso no existe.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                permiso.Nombre = dto.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Modulo))
                permiso.Modulo = dto.Modulo.Trim().ToLower();

            if (dto.Descripcion is not null)
                permiso.Descripcion = dto.Descripcion.Trim();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarEstatusAsync(
            Guid id,
            CambiarEstatusPermisoDto dto)
        {
            var permiso = await _permisoRepository
                .ObtenerPorIdAsync(id);

            if (permiso is null)
                throw new NotFoundException("El permiso no existe.");

            await _permisoRepository.CambiarEstatusAsync(id, dto.Activo);
            await _unitOfWork.SaveChangesAsync();
        }



        // ── Helpers privados ──────────────────────────────────────
        private static PermisoDto MapearDto(Permiso p) => new()
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Modulo = p.Modulo,
            Descripcion = p.Descripcion,
            Activo = p.Activo
        };

        private static PaginatedResult<PermisoDto> MapearPaginado(
            PaginatedResult<Permiso> resultado) => new()
            {
                Items = resultado.Items.Select(MapearDto).ToList(),
                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
    }
}
