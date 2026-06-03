using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class SubRol
    {
        public Guid Id { get; set; }
        public Guid RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navegación
        public Rol Rol { get; set; } = null!;
        public ICollection<SubRolPermiso> SubRolesPermisos { get; set; } = new List<SubRolPermiso>();
    }
}
