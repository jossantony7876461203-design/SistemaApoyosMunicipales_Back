using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class RolPermiso
    {
        public Guid RolId { get; set; }
        public Guid PermisoId { get; set; }
        public DateTimeOffset AsignadoAt { get; set; }

        // Navegación
        public Rol Rol { get; set; } = null!;
        public Permiso Permiso { get; set; } = null!;
    }
}
