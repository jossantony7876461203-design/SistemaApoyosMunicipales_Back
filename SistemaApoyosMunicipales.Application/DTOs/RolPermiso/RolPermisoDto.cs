using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RolPermiso
{
    public class RolPermisoDto
    {
        public Guid PermisoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Asignado { get; set; }
    }
}
