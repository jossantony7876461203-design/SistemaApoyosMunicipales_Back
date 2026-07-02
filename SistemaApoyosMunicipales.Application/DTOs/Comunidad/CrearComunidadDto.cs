using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Comunidad
{
    public sealed class CrearComunidadDto
    {
        public string ClaveInterna { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string CodigoPostal { get; set; } = string.Empty;

        public string? Delegado { get; set; }

        public string? TelefonoDelegado { get; set; }

        public IFormFile? DelegadoIne { get; set; }
    }
}
