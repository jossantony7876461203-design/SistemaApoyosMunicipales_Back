using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class ReporteApoyoDetalleDto
    {
        public string Folio { get; set; } = string.Empty;
        public string Fondo { get; set; } = string.Empty;
        public DateTimeOffset FechaApoyo { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Beneficiario { get; set; } = string.Empty;
    }
}
