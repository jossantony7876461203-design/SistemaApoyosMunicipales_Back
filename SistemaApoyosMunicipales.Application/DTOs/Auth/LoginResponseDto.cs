using SistemaApoyosMunicipales.Application.DTOs.Permisos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Auth
{


    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string SubRol { get; set; } = string.Empty;
        public List<UsuarioPermisoDto> Permisos { get; set; } = new();

    }



    public class UsuarioPermisoDto
    {
        public string Modulo { get; set; } = string.Empty;
        public string Permiso { get; set; } = string.Empty;
        public bool PuedeCrear { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
