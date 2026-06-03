using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{

    public class Sesion
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public DateTimeOffset ExpiraAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
