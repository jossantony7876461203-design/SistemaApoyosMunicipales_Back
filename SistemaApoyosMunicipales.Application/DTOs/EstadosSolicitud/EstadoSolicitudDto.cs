using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.EstadosSolicitud
{
    public sealed class EstadoSolicitudDto
    {
        public Guid Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
