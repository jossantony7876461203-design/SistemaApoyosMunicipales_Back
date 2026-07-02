using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Comunidad
{
    public sealed class Comunidad
    {
        // 1. Identificador Único Global (UUID)
        public Guid Id { get; set; }

        // 2. Columnas Atómicas de Negocio
        public string ClaveInterna { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string? Delegado { get; set; }
        public string? TelefonoDelegado { get; set; }
        public string? DelegadoIneUrl { get; set; }
        public string? DelegadoInePubId { get; set; }

        // 3. Columnas de Control, Auditoría y Soft Delete
        public bool Activo { get; set; } = true;
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
