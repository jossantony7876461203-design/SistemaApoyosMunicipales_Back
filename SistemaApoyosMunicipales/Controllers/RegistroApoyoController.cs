using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo;

using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroApoyoController : ControllerBase
    {
        private readonly IRegistroApoyoService _registroApoyoService;
        private readonly ICurrentUserService _currentUser;

        // GUID default para cuando no hay usuario autenticado
        private static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public RegistroApoyoController(
            IRegistroApoyoService registroApoyoService,
            ICurrentUserService currentUser)
        {
            _registroApoyoService = registroApoyoService;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Obtiene el ID del usuario autenticado o el default si no existe
        /// </summary>
        private Guid GetUsuarioId()
        {
            // Si hay usuario autenticado, usar ese
            if (_currentUser.UserId.HasValue)
                return _currentUser.UserId.Value;

            // Si no, usar el default
            return DefaultUserId;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Crear([FromForm] CrearRegistroApoyoDto dto)
        {
            var usuarioId = GetUsuarioId();
            var id = await _registroApoyoService.CrearAsync(dto, usuarioId);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerRegistroApoyoDto>> ObtenerPorId(Guid id)
        {
            var registro = await _registroApoyoService.ObtenerPorIdAsync(id);
            return Ok(registro);
        }

        [HttpGet("comunidad/{comunidadId}")]
        public async Task<ActionResult<PaginatedResult<ObtenerRegistroApoyoListadoDto>>>
            ObtenerPorComunidad(Guid comunidadId, [FromQuery] PaginationRequest pagination)
        {
            var resultado = await _registroApoyoService.ObtenerPorComunidadAsync(comunidadId, pagination);
            return Ok(resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(Guid id, [FromForm] ActualizarRegistroApoyoDto dto)
        {
            await _registroApoyoService.ActualizarAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoRegistroApoyoDto dto)
        {
            await _registroApoyoService.CambiarEstadoAsync(id, dto.EstadoSolicitudId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _registroApoyoService.EliminarAsync(id);
            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ObtenerRegistroApoyoGlobalDto>>>
            ObtenerTodos([FromQuery] PaginationRequest pagination)
        {
            var resultado = await _registroApoyoService.ObtenerTodosAsync(pagination);
            return Ok(resultado);
        }

        [HttpGet("{id}/detalle")]
        public async Task<ActionResult<ObtenerRegistroApoyoDetalleDto>> ObtenerDetalle(Guid id)
        {
            var detalle = await _registroApoyoService.ObtenerDetalleAsync(id);
            return Ok(detalle);
        }

        [HttpPost("{id}/documentos")]
        public async Task<ActionResult<List<RegistroApoyoDocumentoDto>>> AgregarDocumentos(
    Guid id,
    [FromForm] AgregarDocumentosRegistroApoyoDto dto)
        {
            var documentos = await _registroApoyoService.AgregarDocumentosAsync(id, dto);
            return Ok(documentos);
        }
    }
}