using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Dashboard
{
    public sealed class TopComunidadesDto
    {
        public List<TopComunidadItemDto> TopComunidades { get; set; } = new();
        public int Pendientes { get; set; }
        public int Validados { get; set; }
        public int Aprobados { get; set; }
    }
}
