using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class MontoPorComunidadDto
    {
        public Guid ComunidadId { get; set; }
        public string Comunidad { get; set; } = string.Empty;
        public string? Delegado { get; set; }
        public decimal MontoTotal { get; set; }
        public int TotalApoyos { get; set; }
    }
}
