using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Usuario
{
    public sealed class AsignarRolUsuarioDto
    {
        public Guid RolId { get; set; }

        public Guid? SubRolId { get; set; }
    }
}
