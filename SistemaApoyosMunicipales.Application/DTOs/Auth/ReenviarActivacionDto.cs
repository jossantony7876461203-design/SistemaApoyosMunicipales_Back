using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Auth
{
    public sealed class ReenviarActivacionDto
    {
        public string Correo { get; set; } = default!;
    }
}
