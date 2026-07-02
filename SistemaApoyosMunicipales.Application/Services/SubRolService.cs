using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.SubRol;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Exceptions;
using SistemaApoyosMunicipales.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{

  
    public class SubRolService: ISubRolService
    {

        private readonly ISubRolRepository _subRolRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubRolService(
    ISubRolRepository subRolRepository,
    IRolRepository rolRepository,
    IUnitOfWork unitOfWork)
        {
            _subRolRepository = subRolRepository;
            _rolRepository = rolRepository;
            _unitOfWork = unitOfWork;
        }



        public async Task<Guid> CrearAsync(
    CrearSubRolDto dto)
        {
            var rol =
                await _rolRepository.ObtenerPorIdAsync(dto.RolId);

            if (rol is null)
                throw new NotFoundException(
                    "El rol no existe.");

            if (!rol.Activo)
                throw new ValidationException(
                    "El rol se encuentra inactivo.");

            var existe =
                await _subRolRepository.ExisteAsync(
                    dto.RolId,
                    dto.Nombre.Trim());

            if (existe)
                throw new ValidationException(
                    "Ya existe un subrol con ese nombre para este rol.");

            var subRol = new Domain.Entities.Auth.SubRol
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                RolId = dto.RolId,
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _subRolRepository.CrearAsync(subRol);

            await _unitOfWork.SaveChangesAsync();

            return subRol.Id;
        }


        public async Task<SubRolDto> ObtenerPorIdAsync(
    Guid id)
        {
            var subRol =
                await _subRolRepository.ObtenerPorIdAsync(id);

            if (subRol is null)
                throw new NotFoundException(
                    "El subrol no existe.");

            return new SubRolDto
            {
                Id = subRol.Id,
                RolId = subRol.RolId,
                RolNombre = subRol.Rol.Nombre,
                Nombre = subRol.Nombre,
                Descripcion = subRol.Descripcion,
                Activo = subRol.Activo
            };
        }


        public async Task<PaginatedResult<SubRolDto>>
    ObtenerActivosAsync(
        PaginationRequest pagination)
        {
            var resultado =
                await _subRolRepository.ObtenerTodosAsync(
                    pagination,
                    true);

            return new PaginatedResult<SubRolDto>
            {
                Items = resultado.Items
                    .Select(x => new SubRolDto
                    {
                        Id = x.Id,
                        RolId = x.RolId,
                        RolNombre = x.Rol.Nombre,
                        Nombre = x.Nombre,
                        Descripcion = x.Descripcion,
                        Activo = x.Activo
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

        public async Task<PaginatedResult<SubRolDto>>
    ObtenerInactivosAsync(
        PaginationRequest pagination)
        {
            var resultado =
                await _subRolRepository.ObtenerTodosAsync(
                    pagination,
                    false);

            return new PaginatedResult<SubRolDto>
            {
                Items = resultado.Items
                    .Select(x => new SubRolDto
                    {
                        Id = x.Id,
                        RolId = x.RolId,
                        RolNombre = x.Rol.Nombre,
                        Nombre = x.Nombre,
                        Descripcion = x.Descripcion,
                        Activo = x.Activo
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

        public async Task<IEnumerable<SubRolDto>>
    ObtenerPorRolAsync(
        Guid rolId)
        {
            var subRoles =
                await _subRolRepository.ObtenerPorRolAsync(
                    rolId);

            return subRoles
                .Select(x => new SubRolDto
                {
                    Id = x.Id,
                    RolId = x.RolId,
                    RolNombre = x.Rol.Nombre,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Activo = x.Activo
                });
        }


        public async Task ActualizarAsync(
    Guid id,
    ActualizarSubRolDto dto)
        {
            var subRol =
                await _subRolRepository.ObtenerPorIdAsync(id);

            if (subRol is null)
                throw new NotFoundException(
                    "El subrol no existe.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                var existe =
                    await _subRolRepository.ExisteAsync(
                        subRol.RolId,
                        dto.Nombre.Trim());

                if (existe &&
                    !string.Equals(
                        subRol.Nombre,
                        dto.Nombre.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationException(
                        "Ya existe un subrol con ese nombre para este rol.");
                }

                subRol.Nombre = dto.Nombre.Trim();
            }

            subRol.Descripcion =
                dto.Descripcion?.Trim();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarEstatusAsync(
    Guid id,
    CambiarEstatusSubRolDto dto)
        {
            var subRol =
                await _subRolRepository.ObtenerPorIdAsync(id);

            if (subRol is null)
                throw new NotFoundException(
                    "El subrol no existe.");

            await _subRolRepository.CambiarEstatusAsync(
                id,
                dto.Activo);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
