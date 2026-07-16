using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

using System;
using System.Threading.Tasks;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    //[Authorize]
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


        [HttpGet("comunidades/excel")]
        public async Task<IActionResult> ExportarComunidadesExcel([FromQuery] FiltroReporteDto filtro)
        {
            var archivo = await _reportesService.ExportarComunidadesExcelAsync(filtro);
            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Comunidades_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("comunidades/{comunidadId:guid}/apoyos/excel")]
        public async Task<IActionResult> ExportarApoyosPorComunidadExcel(
    Guid comunidadId,
    [FromQuery] FiltroReporteDto filtro)
        {
            var archivo = await _reportesService.ExportarApoyosPorComunidadExcelAsync(comunidadId, filtro);
            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Apoyos_Comunidad_{DateTime.Now:yyyyMMdd}.xlsx");
        }


        [HttpGet("fondos/excel")]
        public async Task<IActionResult> ExportarFondosExcel([FromQuery] FiltroReporteDto filtro)
        {
            var archivo = await _reportesService.ExportarFondosExcelAsync(filtro);
            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Fondos_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("apoyos/excel")]
        public async Task<IActionResult> ExportarApoyosExcel([FromQuery] FiltroReporteDto filtro)
        {
            var archivo = await _reportesService.ExportarApoyosExcelAsync(filtro);
            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Apoyos_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}