using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.SubRol
{
    public sealed class ActualizarSubRolDto
    {
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }
    }
}
