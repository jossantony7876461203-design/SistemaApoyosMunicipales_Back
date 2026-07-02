using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.SubRol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface ISubRolService
    {
        Task<Guid> CrearAsync(
            CrearSubRolDto dto);

        Task ActualizarAsync(
            Guid id,
            ActualizarSubRolDto dto);

        Task CambiarEstatusAsync(
            Guid id,
            CambiarEstatusSubRolDto dto);

        Task<SubRolDto> ObtenerPorIdAsync(
            Guid id);

        Task<PaginatedResult<SubRolDto>> ObtenerActivosAsync(
            PaginationRequest pagination);

        Task<PaginatedResult<SubRolDto>> ObtenerInactivosAsync(
            PaginationRequest pagination);

        Task<IEnumerable<SubRolDto>> ObtenerPorRolAsync(
            Guid rolId);
    }
}
