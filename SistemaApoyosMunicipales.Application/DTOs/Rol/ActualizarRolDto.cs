using System;
using System.Collections.Generic;
using System.Text;
namespace SistemaApoyosMunicipales.Application.DTOs.Rol
{
    public sealed class ActualizarRolDto
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
