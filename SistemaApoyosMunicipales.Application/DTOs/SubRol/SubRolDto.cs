using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.SubRol
{
    public sealed class SubRolDto
    {
        public Guid Id { get; set; }

        public Guid RolId { get; set; }

        public string RolNombre { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
