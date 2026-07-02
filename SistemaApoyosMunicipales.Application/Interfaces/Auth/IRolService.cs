using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Rol;
using SistemaApoyosMunicipales.Application.Validators.Rol;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IRolService
    {
        Task<Guid> CrearRolAsync(CrearRolDto dto);

        Task<PaginatedResult<RolDto>> ObtenerActivosAsync(PaginationRequest request);
        Task ActualizarRolAsync(Guid id, ActualizarRolDto dto);
        Task CambiarEstatusAsync(Guid id, CambiarEstatusRolDto dto);
        Task<PaginatedResult<RolDto>> ObtenerInactivosAsync(PaginationRequest request);
        Task<RolDto?> ObtenerPorIdAsync(Guid id);
    }
}
