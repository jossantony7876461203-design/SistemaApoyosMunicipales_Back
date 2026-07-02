using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Documentos
{
    public sealed class RegistroApoyoDocumento
    {
        public Guid Id { get; set; }
        public Guid RegistroApoyoId { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public decimal Monto { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        // Navegación
        public RegistroApoyo RegistroApoyo { get; set; } = null!;
    }
}
