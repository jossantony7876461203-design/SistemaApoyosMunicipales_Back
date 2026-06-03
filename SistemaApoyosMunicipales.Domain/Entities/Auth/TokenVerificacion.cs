using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auth
{
    public class TokenVerificacion
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; 
        public DateTimeOffset ExpiraAt { get; set; }
        public bool Usado { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
