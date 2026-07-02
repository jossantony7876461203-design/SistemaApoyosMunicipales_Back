using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Apoyos
{
    public class CrearApoyoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal? MontoMaximo { get; set; }
        public bool RequiereValidacion { get; set; }
    }
}
