using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class ReporteAnualDto
    {
        public DateTimeOffset Desde { get; set; }
        public DateTimeOffset Hasta { get; set; }

        public int TotalApoyos { get; set; }
        public decimal TotalDinero { get; set; }
        public int TotalComunidades { get; set; }

        public List<ReporteComunidadResumenDto> Comunidades { get; set; } = new();
        public List<ReporteComunidadResumenDto> Top5MasBeneficiadas { get; set; } = new();
        public List<ReporteComunidadResumenDto> Top5MenosBeneficiadas { get; set; } = new();
    }
}
