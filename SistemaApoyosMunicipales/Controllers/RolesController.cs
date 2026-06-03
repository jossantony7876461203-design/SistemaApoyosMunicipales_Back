using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.Rol;

using SistemaApoyosMunicipales.Application.Interfaces.Auth;


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
            var roles = await _rolService.ObtenerActivosAsync();

            var rol = roles.FirstOrDefault(r => r.Id == id);

            if (rol is null)
                return NotFound();

            return Ok(rol);
        }

        // =========================================================
        // GET: api/roles/activos
        // =========================================================
        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos()
        {
            var roles = await _rolService.ObtenerActivosAsync();
            return Ok(roles);
        }
    }
}