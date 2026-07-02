using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso
{
    public class UpsertSubRolPermisoDto
    {
        public Guid PermisoId { get; set; }
        public bool Asignado { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeLeer { get; set; } = true;
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
