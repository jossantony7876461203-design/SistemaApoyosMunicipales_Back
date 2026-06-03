using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool CorreoVerificado { get; set; }
        public DateTimeOffset? CorreoVerificadoAt { get; set; }
        public Guid? RolId { get; set; }
        public Guid? SubRolId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? UltimoAcceso { get; set; }

        // Navegación
        public Rol? Rol { get; set; }
        public SubRol? SubRol { get; set; }
    }
}
