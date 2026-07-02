using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Rol;

using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Validators.Rol;


namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public sealed class RolesController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        // =========================================================
        // POST: api/roles
        // Crear rol
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearRolDto dto)
        {
            var id = await _rolService.CrearRolAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id },
                new
                {
                    Mensaje = "Rol creado correctamente.",
                    Id = id
                }
            );
        }

        // =========================================================
        // GET: api/roles/{id}
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var rol = await _rolService.ObtenerPorIdAsync(id);

            if (rol is null)
                return NotFound();

            return Ok(rol);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var roles = await _rolService.ObtenerActivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(roles);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarRolDto dto)
        {
            await _rolService.ActualizarRolAsync(id, dto);

            return Ok(new
            {
                Mensaje = "Rol actualizado correctamente.",
                Id = id
            });
        }

        [HttpPatch("{id:guid}/estatus")]
        public async Task<IActionResult> CambiarEstatus(Guid id, [FromBody] CambiarEstatusRolDto dto)
        {
            await _rolService.CambiarEstatusAsync(id, dto);

            return Ok(new
            {
                Mensaje = dto.Activo
                    ? "Rol activado correctamente."
                    : "Rol desactivado correctamente.",
                Id = id
            });
        }

        [HttpGet("inactivos")]
        public async Task<IActionResult> ObtenerInactivos(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            var roles = await _rolService.ObtenerInactivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return Ok(roles);
        }
    }
}