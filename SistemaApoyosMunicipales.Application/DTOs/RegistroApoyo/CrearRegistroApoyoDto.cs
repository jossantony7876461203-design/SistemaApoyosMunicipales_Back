using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo
{
    public sealed class CrearRegistroApoyoDto
    {
        public string Folio { get; set; } = string.Empty;
        public Guid ApoyoId { get; set; }
        public Guid ComunidadId { get; set; }
        public Guid EstadoSolicitudId { get; set; }
        public DateTimeOffset FechaApoyo { get; set; }
        public decimal MontoOtorgado { get; set; }
        public string? Observaciones { get; set; }

        public List<IFormFile>? Archivos { get; set; }
        public List<decimal>? Montos { get; set; }
        public List<string>? Descripciones { get; set; }
        public List<string>? TiposDocumento { get; set; }
        public List<bool>? Facturados { get; set; }

        public List<string>? MetodosPago { get; set; }            // NUEVO, paralela a Archivos
        public List<DateTimeOffset?>? FechasFacturado { get; set; } // NUEVO, paralela a Archivos
    }


}