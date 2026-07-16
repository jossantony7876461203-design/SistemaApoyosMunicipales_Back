using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class ReporteApoyoGlobalDto
    {
        public string Folio { get; set; } = default!;
        public string Comunidad { get; set; } = default!;
        public string Fondo { get; set; } = default!;
        public DateTimeOffset FechaApoyo { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string Estado { get; set; } = default!;
    }
}
