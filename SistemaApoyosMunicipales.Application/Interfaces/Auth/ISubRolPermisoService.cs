using SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface ISubRolPermisoService
    {
        // Switch visual uno por uno
        Task UpsertAsync(Guid subRolId, UpsertSubRolPermisoDto dto);

        // Carga masiva (copiar plantilla, etc.)
        Task UpsertMasivoAsync(Guid subRolId, UpsertMasivoSubRolPermisoDto dto);

        // Todos los permisos del catálogo con su CRUD para este sub-rol
        Task<IEnumerable<SubRolPermisoDto>> ObtenerPorSubRolAsync(Guid subRolId);

        // Solo los asignados
        Task<IEnumerable<SubRolPermisoDto>> ObtenerAsignadosAsync(Guid subRolId);

        // Solo los no asignados
        Task<IEnumerable<SubRolPermisoDto>> ObtenerNoAsignadosAsync(Guid subRolId);
    }
}
