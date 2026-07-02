using SistemaApoyosMunicipales.Application.DTOs.RolPermiso;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class RolPermisoService : IRolPermisoService
    {
        private readonly IRolPermisoRepository _rolPermisoRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IPermisoRepository _permisoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RolPermisoService(
            IRolPermisoRepository rolPermisoRepository,
            IRolRepository rolRepository,
            IPermisoRepository permisoRepository,
            IUnitOfWork unitOfWork)
        {
            _rolPermisoRepository = rolPermisoRepository;
            _rolRepository = rolRepository;
            _permisoRepository = permisoRepository;
            _unitOfWork = unitOfWork;
        }

        // =========================================================
        // UPSERT — switch del front manda true/false
        // =========================================================
        public async Task UpsertAsync(Guid rolId, UpsertPermisoRolDto dto)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(rolId);

            if (rol is null)
                throw new NotFoundException("El rol no existe.");

            if (!rol.Activo)
                throw new ValidationException("El rol se encuentra inactivo.");

            var permiso = await _permisoRepository
                .ObtenerPorIdAsync(dto.PermisoId);

            if (permiso is null)
                throw new NotFoundException("El permiso no existe.");

            if (!permiso.Activo)
                throw new ValidationException("El permiso se encuentra inactivo.");

            var existe = await _rolPermisoRepository
                .ExisteAsync(rolId, dto.PermisoId);

            if (dto.Asignado && !existe)
            {
                // Switch ON → asignar
                await _rolPermisoRepository.AsignarAsync(new RolPermiso
                {
                    RolId = rolId,
                    PermisoId = dto.PermisoId,
                    AsignadoAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();
                return;
            }

            if (!dto.Asignado && existe)
            {
                // Switch OFF → quitar
                await _rolPermisoRepository.QuitarAsync(rolId, dto.PermisoId);
                // ExecuteDeleteAsync ya guarda, no necesita SaveChanges
                return;
            }

            // Ya estaba en el estado deseado — no hacer nada
        }

        // =========================================================
        // TODOS los permisos del catálogo marcados con Asignado
        // (útil para renderizar todos los switches en el front)
        // =========================================================
        public async Task<IEnumerable<RolPermisoDto>> ObtenerPorRolAsync(
            Guid rolId)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(rolId);

            if (rol is null)
                throw new NotFoundException("El rol no existe.");

            // Permisos que YA tiene el rol
            var asignados = await _rolPermisoRepository
                .ObtenerPorRolAsync(rolId, soloAsignados: true);

            var asignadosIds = asignados
                .Select(x => x.PermisoId)
                .ToHashSet();

            // Todos los permisos activos del catálogo
            var todos = await _permisoRepository
                .ObtenerTodosAsync(
                    new Application.Common.Models.PaginationRequest
                    {
                        PageNumber = 1,
                        PageSize = int.MaxValue
                    },
                    activos: true);

            return todos.Items.Select(p => new RolPermisoDto
            {
                PermisoId = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Modulo = p.Modulo,
                Descripcion = p.Descripcion,
                Asignado = asignadosIds.Contains(p.Id)
            });
        }

        // =========================================================
        // SOLO los asignados
        // =========================================================
        public async Task<IEnumerable<RolPermisoDto>> ObtenerAsignadosAsync(
            Guid rolId)
        {
            var rol = await _rolRepository.ObtenerPorIdAsync(rolId);

            if (rol is null)
                throw new NotFoundException("El rol no existe.");

            var asignados = await _rolPermisoRepository
                .ObtenerPorRolAsync(rolId, soloAsignados: true);

            return asignados.Select(x => new RolPermisoDto
            {
                PermisoId = x.PermisoId,
                Codigo = x.Permiso.Codigo,
                Nombre = x.Permiso.Nombre,
                Modulo = x.Permiso.Modulo,
                Descripcion = x.Permiso.Descripcion,
                Asignado = true
            });
        }

        // =========================================================
        // SOLO los NO asignados
        // =========================================================
        public async Task<IEnumerable<RolPermisoDto>> ObtenerNoAsignadosAsync(
            Guid rolId)
        {
            var todos = await ObtenerPorRolAsync(rolId);
            var noAsignados = todos.Where(x => !x.Asignado);
            return noAsignados;
        }
    }
}
