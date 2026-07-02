using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public class ObtenerRegistroApoyoDto
    {
        public Guid Id { get; set; }
        public Guid ApoyoId { get; set; }
        public string? Apoyo { get; set; }
        public Guid ComunidadId { get; set; }
        public string? Comunidad { get; set; }
        public Guid EstadoSolicitudId { get; set; }
        public string? EstadoSolicitud { get; set; }
        public DateTimeOffset FechaApoyo { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string? Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Total de todas las facturas/documentos (opcional)
        public decimal TotalDocumentos => Documentos?.Sum(d => d.Monto) ?? 0;

        public List<RegistroApoyoDocumentoDto> Documentos { get; set; } = new();
    }
}
