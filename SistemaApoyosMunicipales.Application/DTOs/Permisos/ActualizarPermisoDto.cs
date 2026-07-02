using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Permisos
{
    public class ActualizarPermisoDto
    {
        public string? Nombre { get; set; }
        public string? Modulo { get; set; }
        public string? Descripcion { get; set; }
    }
}
