using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class Rol
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navegación
        public ICollection<SubRol> SubRoles { get; set; } = new List<SubRol>();
        public ICollection<RolPermiso> RolesPermisos { get; set; } = new List<RolPermiso>();
    }
}
