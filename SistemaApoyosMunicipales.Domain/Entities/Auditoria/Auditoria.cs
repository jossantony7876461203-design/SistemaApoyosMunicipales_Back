using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.Auditoria
{
    public sealed class Auditoria
    {
        public Guid Id { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public Guid EntidadId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? Comentario { get; set; }
        public Guid? UsuarioId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
