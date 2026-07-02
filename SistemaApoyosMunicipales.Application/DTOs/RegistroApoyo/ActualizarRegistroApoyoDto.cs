using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class ActualizarRegistroApoyoDto
    {
        public string Folio { get; set; } = string.Empty;
        public Guid ApoyoId { get; set; }
        public Guid ComunidadId { get; set; }
        public Guid EstadoSolicitudId { get; set; }
        public DateTimeOffset FechaApoyo { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string? Observaciones { get; set; }


        /// <summary>Archivos de las facturas/documentos (opcional; si no se manda, no se tocan los documentos actuales).</summary>
        public List<IFormFile>? Archivos { get; set; }

        /// <summary>Monto de cada documento (mismo índice que Archivos).</summary>
        public List<decimal>? Montos { get; set; }

        /// <summary>Tipo de cada documento (opcional, mismo índice que Archivos).</summary>
        public List<string>? TiposDocumento { get; set; }
    }
}