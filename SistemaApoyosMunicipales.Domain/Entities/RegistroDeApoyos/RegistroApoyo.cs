using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Domain.Estados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos
{
    public sealed class RegistroApoyo
    {
        public Guid Id { get; set; }

        // Folio de control interno, lo envía el front-end.
        public string Folio { get; set; } = string.Empty;

        public Guid ApoyoId { get; set; }
        public Guid ComunidadId { get; set; }
        public Guid EstadoSolicitudId { get; set; }

        public DateTimeOffset FechaApoyo { get; set; }

        public decimal MontoOtorgado { get; set; }

        public string? Observaciones { get; set; }

        public Guid RegistradoPor { get; set; }

        public bool Activo { get; set; } = true;
        public DateTimeOffset? DeletedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        // Navegación
        public Apoyo.Apoyo Apoyo { get; set; } = null!;
        public Comunidad.Comunidad Comunidad { get; set; } = null!;
        public EstadoSolicitud EstadoSolicitud { get; set; } = null!;
        public ICollection<RegistroApoyoDocumento> Documentos { get; set; } = [];
    }
}