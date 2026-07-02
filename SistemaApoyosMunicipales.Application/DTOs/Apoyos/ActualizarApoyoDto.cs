using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.Apoyos
{
    public class ActualizarApoyoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? MontoMaximo { get; set; }
        public bool? RequiereValidacion { get; set; }
    }
}
