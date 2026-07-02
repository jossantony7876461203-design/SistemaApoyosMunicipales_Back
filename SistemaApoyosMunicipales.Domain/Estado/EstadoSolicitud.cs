using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Estados
{
    public sealed class EstadoSolicitud
    {
        public Guid Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }

        // Navegación inversa
        public ICollection<RegistroApoyo> RegistrosApoyo { get; set; } = [];
    }
}
