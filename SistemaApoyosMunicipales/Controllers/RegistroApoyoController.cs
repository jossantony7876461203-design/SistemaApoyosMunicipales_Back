using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegistroApoyoController : ControllerBase
    {
        private readonly IRegistroApoyoService _registroApoyoService;
        private readonly ICurrentUserService _currentUser;

        public RegistroApoyoController(
            IRegistroApoyoService registroApoyoService,
            ICurrentUserService currentUser)
        {
            _registroApoyoService = registroApoyoService;
            _currentUser = currentUser;
        }

        private Guid GetUsuarioId()
        {
            if (_currentUser.UserId.HasValue &&
                _currentUser.UserId.Value != Guid.Empty)
            {
                return _currentUser.UserId.Value;
            }

            throw new UnauthorizedException(
                "No se pudo identificar al usuario autenticado.");
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Crear(
            [FromForm] CrearRegistroApoyoDto dto)
        {
            var usuarioId = GetUsuarioId();

            var id = await _registroApoyoService.CrearAsync(
                dto,
                usuarioId);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id },
                id);
        }

        [HttpGet]
        public async Task<
            ActionResult<PaginatedResult<ObtenerRegistroApoyoGlobalDto>>>
            ObtenerTodos(
                [FromQuery] PaginationRequest pagination)
        {
            var resultado = await _registroApoyoService
                .ObtenerTodosAsync(pagination);

            return Ok(resultado);
        }

        [HttpGet("estados-solicitud")]
        public async Task<
            ActionResult<List<EstadoSolicitudCatalogoDto>>>
            ObtenerEstadosSolicitud()
        {
            var estados = await _registroApoyoService
                .ObtenerEstadosSolicitudAsync();

            return Ok(estados);
        }

        [HttpGet("comunidad/{comunidadId:guid}")]
        public async Task<
            ActionResult<
                PaginatedResult<ObtenerRegistroApoyoListadoDto>>>
            ObtenerPorComunidad(
                Guid comunidadId,
                [FromQuery] PaginationRequest pagination)
        {
            var resultado = await _registroApoyoService
                .ObtenerPorComunidadAsync(
                    comunidadId,
                    pagination);

            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ObtenerRegistroApoyoDto>>
            ObtenerPorId(Guid id)
        {
            var registro = await _registroApoyoService
                .ObtenerPorIdAsync(id);

            return Ok(registro);
        }

        [HttpGet("{id:guid}/detalle")]
        public async Task<ActionResult<ObtenerRegistroApoyoDetalleDto>>
            ObtenerDetalle(Guid id)
        {
            var detalle = await _registroApoyoService
                .ObtenerDetalleAsync(id);

            return Ok(detalle);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Actualizar(
            Guid id,
            [FromForm] ActualizarRegistroApoyoDto dto)
        {
            await _registroApoyoService.ActualizarAsync(
                id,
                dto);

            return NoContent();
        }

        [HttpPatch("{id:guid}/estado")]
        public async Task<IActionResult> CambiarEstado(
            Guid id,
            [FromBody] CambiarEstadoRegistroApoyoDto dto)
        {
            await _registroApoyoService.CambiarEstadoAsync(
                id,
                dto.EstadoSolicitudId);

            return NoContent();
        }

        [HttpPost("{id:guid}/documentos")]
        public async Task<
            ActionResult<List<RegistroApoyoDocumentoDto>>>
            AgregarDocumentos(
                Guid id,
                [FromForm] AgregarDocumentosRegistroApoyoDto dto)
        {
            var documentos = await _registroApoyoService
                .AgregarDocumentosAsync(id, dto);

            return Ok(documentos);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _registroApoyoService.EliminarAsync(id);

            return NoContent();
        }
    }
}