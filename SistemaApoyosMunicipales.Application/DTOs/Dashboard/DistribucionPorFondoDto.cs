using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class DistribucionPorFondoDto
    {
        public string Fondo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
