using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class ApoyoRecienteDto
    {
        public Guid Id { get; set; }
        public string? Comunidad { get; set; }
        public string? TipoApoyo { get; set; }
        public string? Estado { get; set; }
        public DateTimeOffset Fecha { get; set; }
    }
}
