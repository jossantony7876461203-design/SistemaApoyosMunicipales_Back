using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class PagoRegistroApoyoDto
    {
        public string MetodoPago { get; set; } = default!;
        public decimal Monto { get; set; }
        public string? Referencia { get; set; }
    }
}
