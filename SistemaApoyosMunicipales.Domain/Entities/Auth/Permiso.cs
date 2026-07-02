using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class Permiso
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; } 

        // Navegación
        public ICollection<RolPermiso> RolesPermisos { get; set; } = [];
        public ICollection<SubRolPermiso> SubRolesPermisos { get; set; } = [];
    }
}
