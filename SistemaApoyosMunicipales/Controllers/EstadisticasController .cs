using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.Dashboard;

using SistemaApoyosMunicipales.Application.Interfaces.Auth;

using System;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/estadisticas")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IEstadisticasService _estadisticasService;

        public EstadisticasController(IEstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        /// <summary>
        /// Monto total otorgado a cada comunidad, de mayor a menor.
        /// </summary>
        [HttpGet("monto-por-comunidad")]
        public async Task<ActionResult<List<MontoPorComunidadDto>>> ObtenerMontoPorComunidad()
        {
            var resultado = await _estadisticasService.ObtenerMontoPorComunidadAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// KPIs principales del dashboard: total de apoyos, comunidades atendidas,
        /// fondos activos y apoyos pendientes por validar.
        /// </summary>
        [HttpGet("resumen")]
        public async Task<ActionResult<ResumenDashboardDto>> ObtenerResumen()
        {
            var resultado = await _estadisticasService.ObtenerResumenAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Apoyos otorgados por mes y por fondo, para la gráfica de barras.
        /// </summary>
        [HttpGet("apoyos-por-mes")]
        public async Task<ActionResult<List<ApoyosPorMesDto>>> ObtenerApoyosPorMes(
            [FromQuery] int? anio)
        {
            var anioConsultado = anio ?? DateTime.UtcNow.Year;
            var resultado = await _estadisticasService.ObtenerApoyosPorMesAsync(anioConsultado);
            return Ok(resultado);
        }

        /// <summary>
        /// Distribución de apoyos por fondo, para la gráfica de dona.
        /// </summary>
        [HttpGet("distribucion-por-fondo")]
        public async Task<ActionResult<List<DistribucionPorFondoDto>>> ObtenerDistribucionPorFondo()
        {
            var resultado = await _estadisticasService.ObtenerDistribucionPorFondoAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Últimos apoyos registrados, para la tabla de "Apoyos recientes".
        /// </summary>
        [HttpGet("recientes")]
        public async Task<ActionResult<List<ApoyoRecienteDto>>> ObtenerRecientes(
            [FromQuery] int top = 4)
        {
            var resultado = await _estadisticasService.ObtenerRecientesAsync(top);
            return Ok(resultado);
        }

        /// <summary>
        /// Ranking de comunidades con más apoyos en el año, más el conteo
        /// global de apoyos pendientes, validados y aprobados.
        /// </summary>
        [HttpGet("top-comunidades")]
        public async Task<ActionResult<TopComunidadesDto>> ObtenerTopComunidades(
            [FromQuery] int? anio,
            [FromQuery] int top = 5)
        {
            var anioConsultado = anio ?? DateTime.UtcNow.Year;
            var resultado = await _estadisticasService.ObtenerTopComunidadesAsync(anioConsultado, top);
            return Ok(resultado);
        }
    }
}