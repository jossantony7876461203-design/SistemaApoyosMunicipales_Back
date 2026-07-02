using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Usuario
{
    public sealed class ActualizarUsuarioDto
    {
        public string? Nombre { get; set; }

        public string? Correo { get; set; }
    }
}
