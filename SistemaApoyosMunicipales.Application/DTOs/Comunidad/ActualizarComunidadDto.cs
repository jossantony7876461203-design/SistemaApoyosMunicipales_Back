using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Comunidad
{
    public sealed class ActualizarComunidadDto
    {
        public string? ClaveInterna { get; set; }

        public string? Nombre { get; set; }

        public string? CodigoPostal { get; set; }

        public string? Delegado { get; set; }

        public string? TelefonoDelegado { get; set; }

    }
}
