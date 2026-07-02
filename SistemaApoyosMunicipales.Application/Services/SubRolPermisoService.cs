using SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;
using SistemaApoyosMunicipales.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class SubRolPermisoService : ISubRolPermisoService
    {
        private readonly ISubRolPermisoRepository _subRolPermisoRepository;
        private readonly ISubRolRepository _subRolRepository;
        private readonly IPermisoRepository _permisoRepository;
        private readonly IRolPermisoRepository _rolPermisoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubRolPermisoService(
            ISubRolPermisoRepository subRolPermisoRepository,
            ISubRolRepository subRolRepository,
            IPermisoRepository permisoRepository,
            IRolPermisoRepository rolPermisoRepository,
            IUnitOfWork unitOfWork)
        {
            _subRolPermisoRepository = subRolPermisoRepository;
            _subRolRepository = subRolRepository;
            _permisoRepository = permisoRepository;
            _rolPermisoRepository = rolPermisoRepository;
            _unitOfWork = unitOfWork;
        }

        // =========================================================
        // UPSERT — switch del front
        // =========================================================
        public async Task UpsertAsync(
            Guid subRolId,
            UpsertSubRolPermisoDto dto)
        {
            var subRol = await _subRolRepository
                .ObtenerPorIdAsync(subRolId);

            if (subRol is null)
                throw new NotFoundException("El sub-rol no existe.");

            if (!subRol.Activo)
                throw new ValidationException("El sub-rol se encuentra inactivo.");

            var permiso = await _permisoRepository
                .ObtenerPorIdAsync(dto.PermisoId);

            if (permiso is null)
                throw new NotFoundException("El permiso no existe.");

            // Validar que el ROL PADRE tenga ese permiso
            // El sub-rol no puede tener más de lo que su rol permite
            var rolTienePermiso = await _rolPermisoRepository
                .ExisteAsync(subRol.RolId, dto.PermisoId);

            if (!rolTienePermiso)
                throw new ValidationException(
                    "El rol padre no tiene asignado ese permiso. " +
                    "Asígnalo primero al rol.");

            var existe = await _subRolPermisoRepository
                .ExisteAsync(subRolId, dto.PermisoId);

            if (!dto.Asignado && existe)
            {
                // Switch OFF → quitar
                await _subRolPermisoRepository
                    .QuitarAsync(subRolId, dto.PermisoId);
                return;
            }

            if (dto.Asignado && !existe)
            {
                // Switch ON → crear nuevo
                await _subRolPermisoRepository.AsignarAsync(new SubRolPermiso
                {
                    SubRolId = subRolId,
                    PermisoId = dto.PermisoId,
                    PuedeCrear = dto.PuedeCrear,
                    PuedeLeer = dto.PuedeLeer,
                    PuedeEditar = dto.PuedeEditar,
                    PuedeEliminar = dto.PuedeEliminar,
                    AsignadoAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();
                return;
            }

            if (dto.Asignado && existe)
            {
                // Ya existe → actualizar CRUD
                var actual = await _subRolPermisoRepository
                    .ObtenerUnoAsync(subRolId, dto.PermisoId);

                actual!.PuedeCrear = dto.PuedeCrear;
                actual!.PuedeLeer = dto.PuedeLeer;
                actual!.PuedeEditar = dto.PuedeEditar;
                actual!.PuedeEliminar = dto.PuedeEliminar;

                await _unitOfWork.SaveChangesAsync();
            }
        }

        // =========================================================
        // UPSERT MASIVO — reemplaza toda la configuración
        // =========================================================
        public async Task UpsertMasivoAsync(
            Guid subRolId,
            UpsertMasivoSubRolPermisoDto dto)
        {
            var subRol = await _subRolRepository
                .ObtenerPorIdAsync(subRolId);

            if (subRol is null)
                throw new NotFoundException("El sub-rol no existe.");

            // Procesa cada permiso individualmente
            // reutiliza las validaciones del Upsert simple
            foreach (var item in dto.Permisos)
                await UpsertAsync(subRolId, item);
        }

        // =========================================================
        // TODOS los permisos del catálogo con su CRUD
        // =========================================================
        public async Task<IEnumerable<SubRolPermisoDto>> ObtenerPorSubRolAsync(
            Guid subRolId)
        {
            var subRol = await _subRolRepository
                .ObtenerPorIdAsync(subRolId);

            if (subRol is null)
                throw new NotFoundException("El sub-rol no existe.");

            // Permisos que el ROL PADRE tiene — techo máximo
            var permisosPadre = await _rolPermisoRepository
                .ObtenerPorRolAsync(subRol.RolId, soloAsignados: true);

            var idsPadre = permisosPadre
                .Select(x => x.PermisoId)
                .ToHashSet();

            // CRUD que ya tiene configurado el sub-rol
            var asignados = await _subRolPermisoRepository
                .ObtenerPorSubRolAsync(subRolId);

            var mapaAsignados = asignados
                .ToDictionary(x => x.PermisoId);

            // Solo muestra permisos que el rol padre tiene
            return permisosPadre.Select(rp => new SubRolPermisoDto
            {
                PermisoId = rp.PermisoId,
                Codigo = rp.Permiso.Codigo,
                Nombre = rp.Permiso.Nombre,
                Modulo = rp.Permiso.Modulo,
                Descripcion = rp.Permiso.Descripcion,
                Asignado = mapaAsignados.ContainsKey(rp.PermisoId),
                PuedeCrear = mapaAsignados.GetValueOrDefault(rp.PermisoId)?.PuedeCrear ?? false,
                PuedeLeer = mapaAsignados.GetValueOrDefault(rp.PermisoId)?.PuedeLeer ?? false,
                PuedeEditar = mapaAsignados.GetValueOrDefault(rp.PermisoId)?.PuedeEditar ?? false,
                PuedeEliminar = mapaAsignados.GetValueOrDefault(rp.PermisoId)?.PuedeEliminar ?? false
            });
        }

        public async Task<IEnumerable<SubRolPermisoDto>> ObtenerAsignadosAsync(
            Guid subRolId)
        {
            var todos = await ObtenerPorSubRolAsync(subRolId);
            return todos.Where(x => x.Asignado);
        }

        public async Task<IEnumerable<SubRolPermisoDto>> ObtenerNoAsignadosAsync(
            Guid subRolId)
        {
            var todos = await ObtenerPorSubRolAsync(subRolId);
            return todos.Where(x => !x.Asignado);
        }
    }
}
