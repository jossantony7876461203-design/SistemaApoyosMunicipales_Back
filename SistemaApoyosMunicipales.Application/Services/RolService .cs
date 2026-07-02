using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Rol;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Validators.Rol;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RolService(IRolRepository rolRepository, IUnitOfWork unitOfWork)
        {
            _rolRepository = rolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CrearRolAsync(CrearRolDto dto)
        {
            if (await _rolRepository.ExistePorNombreAsync(dto.Nombre))
                throw new Exception("Ya existe un rol con ese nombre.");

            var rol = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                Activo = true,
                CreatedAt = DateTime.UtcNow
            };

            await _rolRepository.CrearAsync(rol);
            await _unitOfWork.SaveChangesAsync();

            return rol.Id;
        }

        public async Task<RolDto?> ObtenerPorIdAsync(Guid id)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(id);

            if (rol is null)
                return null;

            return new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Activo = rol.Activo,
              
            };
        }

        public async Task<PaginatedResult<RolDto>> ObtenerActivosAsync(PaginationRequest request)
        {
            var roles = await _rolRepository.ObtenerTodosAsync(request, true);

            return new PaginatedResult<RolDto>
            {
                Items = roles.Items.Select(r => new RolDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = r.Activo,
            
                }).ToList(),

                PageNumber = roles.PageNumber,
                PageSize = roles.PageSize,
                TotalRecords = roles.TotalRecords,
                TotalPages = roles.TotalPages,
                HasPreviousPage = roles.HasPreviousPage,
                HasNextPage = roles.HasNextPage
            };
        }
        public async Task ActualizarRolAsync(Guid id, ActualizarRolDto dto)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(id);

            if (rol is null)
                throw new Exception("El rol no existe.");

            var existe = await _rolRepository.ExistePorNombreAsync(dto.Nombre);

            if (existe && rol.Nombre != dto.Nombre)
                throw new Exception("Ya existe un rol con ese nombre.");

            rol.Nombre = dto.Nombre.Trim();
            rol.Descripcion = dto.Descripcion;

            await _rolRepository.ActualizarAsync(rol);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task CambiarEstatusAsync(Guid id, CambiarEstatusRolDto dto)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(id);

            if (rol is null)
                throw new Exception("El rol no existe.");

            // UPsert lógico (update de estado)
            await _rolRepository.CambiarEstatusAsync(id, dto.Activo);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaginatedResult<RolDto>> ObtenerInactivosAsync(PaginationRequest request)
        {
            var roles = await _rolRepository.ObtenerTodosAsync(request, false);

            return new PaginatedResult<RolDto>
            {
                Items = roles.Items
                    .Select(r => new RolDto
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        Descripcion = r.Descripcion,
                        Activo = r.Activo,
                       
                    })
                    .ToList(),

                PageNumber = roles.PageNumber,
                PageSize = roles.PageSize,
                TotalRecords = roles.TotalRecords,
                TotalPages = roles.TotalPages,
                HasPreviousPage = roles.HasPreviousPage,
                HasNextPage = roles.HasNextPage
            };
        }
    }
}
