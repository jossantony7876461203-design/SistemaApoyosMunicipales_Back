using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Permisos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IPermisoService
    {
        Task<Guid> CrearAsync(CrearPermisoDto dto);

        Task<PermisoDto> ObtenerPorIdAsync(Guid id);

        Task<PaginatedResult<PermisoDto>> ObtenerActivosAsync(
            PaginationRequest pagination);

        Task<PaginatedResult<PermisoDto>> ObtenerInactivosAsync(
            PaginationRequest pagination);

        Task<IEnumerable<PermisoDto>> ObtenerPorModuloAsync(string modulo);

        Task ActualizarAsync(Guid id, ActualizarPermisoDto dto);

        Task CambiarEstatusAsync(Guid id, CambiarEstatusPermisoDto dto);
    }
}
