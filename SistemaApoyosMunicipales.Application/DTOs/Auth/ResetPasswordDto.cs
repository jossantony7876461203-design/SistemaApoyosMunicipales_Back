using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string NuevoPassword { get; set; } = string.Empty;
    }
}
