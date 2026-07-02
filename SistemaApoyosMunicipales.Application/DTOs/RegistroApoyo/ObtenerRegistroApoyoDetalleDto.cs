using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class ObtenerRegistroApoyoDetalleDto
    {
        public Guid Id { get; set; }
        public string? Descripcion { get; set; }
        public string? Delegado { get; set; }
        public string? Estatus { get; set; }

        public List<RegistroApoyoDocumentoDetalleDto> Documentos { get; set; } = new();
    }

    public sealed class RegistroApoyoDocumentoDetalleDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}
