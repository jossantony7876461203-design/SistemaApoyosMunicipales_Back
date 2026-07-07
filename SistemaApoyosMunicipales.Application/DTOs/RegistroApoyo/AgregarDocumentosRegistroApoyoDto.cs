using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class AgregarDocumentosRegistroApoyoDto
    {
    
        public List<IFormFile> Archivos { get; set; } = new();
        public List<decimal>? Montos { get; set; }
        public List<string>? Descripciones { get; set; }
        public List<string>? TiposDocumento { get; set; }

    }
}
