using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Comunidad
{
    public sealed class ComunidadDto
    {
        public Guid Id { get; set; }

        public string ClaveInterna { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string CodigoPostal { get; set; } = string.Empty;

        public string? Delegado { get; set; }

        public string? TelefonoDelegado { get; set; }

        public bool Activo { get; set; } = true;
        public string? DelegadoIneUrl { get; set; }

    }
}
