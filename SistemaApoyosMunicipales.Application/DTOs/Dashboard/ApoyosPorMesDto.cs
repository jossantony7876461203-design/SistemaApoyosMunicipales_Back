using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class ApoyosPorMesDto
    {
        public int Mes { get; set; }
        public string Fondo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
