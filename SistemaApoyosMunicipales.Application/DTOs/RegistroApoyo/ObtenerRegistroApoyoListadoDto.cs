using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class ObtenerRegistroApoyoListadoDto
    {
        public Guid Id { get; set; }

        public string Apoyo { get; set; } = string.Empty;

        public string Comunidad { get; set; } = string.Empty;

        public string EstadoSolicitud { get; set; } = string.Empty;

        public decimal MontoOtorgado { get; set; }

        public DateTimeOffset FechaApoyo { get; set; }

        public bool Activo { get; set; }
        public bool Facturado { get; set; }
        public string? MetodoPago { get; set; }
    }
}
