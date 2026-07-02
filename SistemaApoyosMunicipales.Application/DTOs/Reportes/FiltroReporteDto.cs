using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class FiltroReporteDto
    {
        public int? AnioInicio { get; set; }
        public int? MesInicio { get; set; }   // 1-12

        public int? AnioFin { get; set; }
        public int? MesFin { get; set; }       // 1-12

        public List<Guid>? ComunidadIds { get; set; }
        public List<Guid>? ApoyoIds { get; set; }
    }
}
