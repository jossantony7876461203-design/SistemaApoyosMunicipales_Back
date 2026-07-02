using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso
{
    public class SubRolPermisoDto
    {
        public Guid PermisoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Asignado { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
