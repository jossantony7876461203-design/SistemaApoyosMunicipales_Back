using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class ObtenerRegistroApoyoGlobalDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string? Comunidad { get; set; }
        public string? Fondo { get; set; }
        public string? TipoApoyo { get; set; }
        public DateTimeOffset FechaRegistro { get; set; }
        public string? Estado { get; set; }
        public string? Delegado { get; set; }
    }
}
