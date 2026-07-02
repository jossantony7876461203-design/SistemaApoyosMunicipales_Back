using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

using System;
using System.Threading.Tasks;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        /// <summary>
        /// Reporte global: total de apoyos, dinero otorgado y desglose
        /// por comunidad (incluye top 5 más y menos beneficiadas).
        /// Filtros opcionales: rango de año/mes, comunidades, fondos.
        /// </summary>
        [HttpPost("anual-comunidades")]
        public async Task<IActionResult> ReporteAnualComunidades(
            [FromBody] FiltroReporteDto filtro)
        {
            var pdfBytes = await _reportesService.GenerarReporteAnualAsync(filtro);

            var nombreArchivo = $"reporte-comunidades-{DateTime.UtcNow:yyyyMMddHHmm}.pdf";

            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        /// <summary>
        /// Reporte de una comunidad específica: todos sus apoyos,
        /// monto de cada uno y total otorgado.
        /// Filtros opcionales: rango de año/mes, fondos.
        /// </summary>
        [HttpPost("por-comunidad/{comunidadId}")]
        public async Task<IActionResult> ReportePorComunidad(
            Guid comunidadId,
            [FromBody] FiltroReporteDto filtro)
        {
            var pdfBytes = await _reportesService.GenerarReportePorComunidadAsync(comunidadId, filtro);

            var nombreArchivo = $"reporte-comunidad-{DateTime.UtcNow:yyyyMMddHHmm}.pdf";

            return File(pdfBytes, "application/pdf", nombreArchivo);
        }
    }
}