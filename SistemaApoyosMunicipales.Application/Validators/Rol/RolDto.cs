using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Rol
{
    public sealed class RolDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
