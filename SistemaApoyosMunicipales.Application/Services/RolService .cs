using SistemaApoyosMunicipales.Application.DTOs.Rol;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
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

        public async Task<IEnumerable<Rol>> ObtenerActivosAsync()
        {
            return await _rolRepository.ObtenerActivosAsync();
        }
    }
}
