using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public  interface IUsuarioService
    {
        Task<PaginatedResult<ObtenerUsuariosRolDto>> ObtenerActivosAsync(PaginationRequest pagination);
        //Task AsignarRolAsync(Guid usuarioId, Guid rolId, Guid? subRolId = null);
    }
}
