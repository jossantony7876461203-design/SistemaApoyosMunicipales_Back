using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class TopComunidadItemDto
    {
        public Guid ComunidadId { get; set; }
        public string Comunidad { get; set; } = string.Empty;
        public int TotalApoyos { get; set; }
    }
}
