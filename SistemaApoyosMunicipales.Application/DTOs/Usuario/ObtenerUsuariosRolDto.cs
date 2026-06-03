using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Usuario
{
    public sealed class ObtenerUsuariosRolDto
    {

        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public bool CorreoVerificado { get; set; }

        public string? Rol { get; set; }

        public string? SubRol { get; set; }

        public DateTimeOffset? UltimoAcceso { get; set; }
    }
}
