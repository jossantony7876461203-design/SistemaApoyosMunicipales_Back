using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Apoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces
{
    public interface IApoyoService
    {
        Task<Guid> CrearAsync(CrearApoyoDto dto);

        Task<ApoyoDto> ObtenerPorIdAsync(Guid id);

        Task<PaginatedResult<ApoyoDto>> ObtenerActivosAsync(
            PaginationRequest pagination);

        Task<PaginatedResult<ApoyoDto>> ObtenerInactivosAsync(
            PaginationRequest pagination);

        Task ActualizarAsync(Guid id, ActualizarApoyoDto dto);

        Task CambiarEstatusAsync(Guid id, CambiarEstatusApoyoDto dto);

        Task EliminarAsync(Guid id);
    }
}
