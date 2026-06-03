using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario, IEnumerable<UsuarioPermisoDto> permisos);

    }
}
