using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class EstadoSolicitudCatalogoDto
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
