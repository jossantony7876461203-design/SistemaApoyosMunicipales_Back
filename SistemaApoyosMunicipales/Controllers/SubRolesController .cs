using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.SubRol;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/subroles")]
    [Authorize]
    public sealed class SubRolesController : ControllerBase
    {
        private readonly ISubRolService _subRolService;

        public SubRolesController(
            ISubRolService subRolService)
        {
            _subRolService = subRolService;
        }

        // =========================================================
        // POST: api/subroles
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearSubRolDto dto)
        {
            var id =
                await _subRolService.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id },
                new
                {
                    Mensaje = "Subrol creado correctamente.",
                    Id = id
                });
        }

        // =========================================================
        // GET: api/subroles/{id}
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(
            Guid id)
        {
            var subRol =
                await _subRolService.ObtenerPorIdAsync(id);

            return Ok(subRol);
        }

        // =========================================================
        // GET: api/subroles/activos
        // =========================================================
        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerActivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado =
                await _subRolService.ObtenerActivosAsync(
                    new PaginationRequest
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    });

            return Ok(resultado);
        }

        // =========================================================
        // GET: api/subroles/inactivos
        // =========================================================
        [HttpGet("inactivos")]
        public async Task<IActionResult> ObtenerInactivos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var resultado =
                await _subRolService.ObtenerInactivosAsync(
                    new PaginationRequest
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    });

            return Ok(resultado);
        }

        // =========================================================
        // GET: api/subroles/rol/{rolId}
        // =========================================================
        [HttpGet("rol/{rolId:guid}")]
        public async Task<IActionResult> ObtenerPorRol(
            Guid rolId)
        {
            var resultado =
                await _subRolService.ObtenerPorRolAsync(rolId);

            return Ok(resultado);
        }

        // =========================================================
        // PATCH: api/subroles/{id}
        // =========================================================
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Actualizar(
            Guid id,
            [FromBody] ActualizarSubRolDto dto)
        {
            await _subRolService
                .ActualizarAsync(id, dto);

            return Ok(new
            {
                Mensaje = "Subrol actualizado correctamente."
            });
        }

        // =========================================================
        // PATCH: api/subroles/{id}/estatus
        // =========================================================
        [HttpPatch("{id:guid}/estatus")]
        public async Task<IActionResult> CambiarEstatus(
            Guid id,
            [FromBody] CambiarEstatusSubRolDto dto)
        {
            await _subRolService
                .CambiarEstatusAsync(id, dto);

            return Ok(new
            {
                Mensaje = dto.Activo
                    ? "Subrol activado correctamente."
                    : "Subrol desactivado correctamente."
            });
        }
    }
}
