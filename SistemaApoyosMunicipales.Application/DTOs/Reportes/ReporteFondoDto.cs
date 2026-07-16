using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Reportes
{
    public sealed class ReporteFondoDto
    {
        public Guid FondoId { get; set; }
        public string Nombre { get; set; } = default!;
        public int TotalApoyos { get; set; }
        public decimal TotalDinero { get; set; }
    }
}
