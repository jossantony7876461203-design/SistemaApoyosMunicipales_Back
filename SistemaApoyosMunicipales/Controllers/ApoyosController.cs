using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Apoyos;
using SistemaApoyosMunicipales.Application.Interfaces;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/apoyos")]
    public sealed class ApoyosController : ControllerBase
    {
        private readonly IApoyoService _apoyoService;
        private readonly IValidator<CrearApoyoDto> _crearValidator;
        private readonly IValidator<ActualizarApoyoDto> _actualizarValidator;

        public ApoyosController(
            IApoyoService apoyoService,
            IValidator<CrearApoyoDto> crearValidator,
            IValidator<ActualizarApoyoDto> actualizarValidator)
        {
            _apoyoService = apoyoService;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        // POST: api/apoyos
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearApoyoDto dto)
        {
            var validacion = await _crearValidator.ValidateAsync(dto);

            if (!validacion.IsValid)
                return BadRequest(new
                {
                    statusCode = 400,
                    errores = validacion.Errors
                        .Select(e => e.ErrorMessage)
                });

            var id = await _apoyoService.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id },
                new { Mensaje = "Apoyo creado correctamente.", Id = id });
        }

        // GET: api/apoyos/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var apoyo = await _apoyoService.ObtenerPorIdAsync(id);
            return Ok(apoyo);
        }

        // GET: api/apoyos/activos
        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado = await _apoyoService.ObtenerActivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(resultado);
        }

        // GET: api/apoyos/inactivos
        [HttpGet("inactivos")]
        public async Task<IActionResult> ObtenerInactivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado = await _apoyoService.ObtenerInactivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(resultado);
        }

        // PATCH: api/apoyos/{id}
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Actualizar(
            Guid id,
            [FromBody] ActualizarApoyoDto dto)
        {
            var validacion = await _actualizarValidator.ValidateAsync(dto);

            if (!validacion.IsValid)
                return BadRequest(new
                {
                    statusCode = 400,
                    errores = validacion.Errors
                        .Select(e => e.ErrorMessage)
                });

            await _apoyoService.ActualizarAsync(id, dto);

            return Ok(new { Mensaje = "Apoyo actualizado correctamente." });
        }

        // PATCH: api/apoyos/{id}/estatus
        [HttpPatch("{id:guid}/estatus")]
        public async Task<IActionResult> CambiarEstatus(
            Guid id,
            [FromBody] CambiarEstatusApoyoDto dto)
        {
            await _apoyoService.CambiarEstatusAsync(id, dto);

            return Ok(new
            {
                Mensaje = dto.Activo
                    ? "Apoyo activado correctamente."
                    : "Apoyo desactivado correctamente."
            });
        }

        // DELETE: api/apoyos/{id}  (Soft Delete)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _apoyoService.EliminarAsync(id);
            return Ok(new { Mensaje = "Apoyo eliminado correctamente." });
        }
    }
}