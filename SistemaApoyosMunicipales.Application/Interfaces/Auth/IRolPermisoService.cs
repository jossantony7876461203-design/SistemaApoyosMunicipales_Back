using SistemaApoyosMunicipales.Application.DTOs.RolPermiso;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IRolPermisoService
    {
        // Asigna o quita según dto.Asignado (para switches del front)
        Task UpsertAsync(Guid rolId, UpsertPermisoRolDto dto);

        // Lista todos los permisos del catálogo marcando cuáles tiene el rol
        Task<IEnumerable<RolPermisoDto>> ObtenerPorRolAsync(Guid rolId);

        // Solo los que tiene asignados
        Task<IEnumerable<RolPermisoDto>> ObtenerAsignadosAsync(Guid rolId);

        // Solo los que NO tiene asignados
        Task<IEnumerable<RolPermisoDto>> ObtenerNoAsignadosAsync(Guid rolId);
    }
}
