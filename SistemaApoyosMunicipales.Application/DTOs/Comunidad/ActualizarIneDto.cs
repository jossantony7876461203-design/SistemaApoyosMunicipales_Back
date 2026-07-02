using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Comunidad
{
    public class ActualizarIneDto
    {
        public IFormFile Imagen { get; set; } = null!;
    }
}
