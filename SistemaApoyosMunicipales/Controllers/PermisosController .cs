using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Permisos;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/permisos")]
    [Authorize]
    public sealed class PermisosController : ControllerBase
    {
        private readonly IPermisoService _permisoService;

        public PermisosController(IPermisoService permisoService)
        {
            _permisoService = permisoService;
        }

        // POST: api/permisos
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearPermisoDto dto)
        {
            var id = await _permisoService.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id },
                new { Mensaje = "Permiso creado correctamente.", Id = id });
        }

        // GET: api/permisos/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var permiso = await _permisoService.ObtenerPorIdAsync(id);
            return Ok(permiso);
        }

        // GET: api/permisos/activos
        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado = await _permisoService.ObtenerActivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(resultado);
        }

        // GET: api/permisos/inactivos
        [HttpGet("inactivos")]
        public async Task<IActionResult> ObtenerInactivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado = await _permisoService.ObtenerInactivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(resultado);
        }

        // GET: api/permisos/modulo/{modulo}
        [HttpGet("modulo/{modulo}")]
        public async Task<IActionResult> ObtenerPorModulo(string modulo)
        {
            var permisos = await _permisoService
                .ObtenerPorModuloAsync(modulo);

            return Ok(permisos);
        }

        // PATCH: api/permisos/{id}
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Actualizar(
            Guid id,
            [FromBody] ActualizarPermisoDto dto)
        {
            await _permisoService.ActualizarAsync(id, dto);

            return Ok(new { Mensaje = "Permiso actualizado correctamente." });
        }

        // PATCH: api/permisos/{id}/estatus
        [HttpPatch("{id:guid}/estatus")]
        public async Task<IActionResult> CambiarEstatus(
            Guid id,
            [FromBody] CambiarEstatusPermisoDto dto)
        {
            await _permisoService.CambiarEstatusAsync(id, dto);

            return Ok(new
            {
                Mensaje = dto.Activo
                    ? "Permiso activado correctamente."
                    : "Permiso desactivado correctamente."
            });
        }

   
    }
}
