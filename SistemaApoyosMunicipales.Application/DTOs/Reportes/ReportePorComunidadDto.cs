using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class ReportePorComunidadDto
    {
        public DateTimeOffset Desde { get; set; }
        public DateTimeOffset Hasta { get; set; }

        public Guid ComunidadId { get; set; }
        public string Comunidad { get; set; } = string.Empty;
        public string? Delegado { get; set; }

        public int TotalApoyos { get; set; }
        public decimal TotalDinero { get; set; }

        public List<ReporteApoyoDetalleDto> Apoyos { get; set; } = new();
    }
}
