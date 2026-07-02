using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class ResumenDashboardDto
    {
        public int TotalApoyos { get; set; }
        public int ApoyosEsteMes { get; set; }

        public int ComunidadesAtendidas { get; set; }
        public int ComunidadesNuevasEsteMes { get; set; }

        public int FondosActivos { get; set; }

        public int PendientesValidar { get; set; }
    }
}
