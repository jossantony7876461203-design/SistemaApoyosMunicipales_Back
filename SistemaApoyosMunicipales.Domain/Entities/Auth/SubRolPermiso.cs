using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class SubRolPermiso
    {
        public Guid SubRolId { get; set; }
        public Guid PermisoId { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public DateTimeOffset AsignadoAt { get; set; }

        // Navegación
        public SubRol SubRol { get; set; } = null!;
        public Permiso Permiso { get; set; } = null!;
    }
}
