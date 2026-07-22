using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    // RegistroApoyoDocumentoDto
    public class RegistroApoyoDocumentoDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
        public bool Facturado { get; set; }
        public string? MetodoPago { get; set; }

        public DateTimeOffset? FechaFacturado { get; set; }
    }
}
