using SistemaApoyosMunicipales.Application.DTOs.Rol;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IRolService
    {
        Task<Guid> CrearRolAsync(CrearRolDto dto);

        Task<IEnumerable<Rol>> ObtenerActivosAsync();
    }
}
