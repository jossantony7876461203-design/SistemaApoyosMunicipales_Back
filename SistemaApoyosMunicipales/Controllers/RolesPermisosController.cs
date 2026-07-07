using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.RolPermiso;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/roles/{rolId:guid}/permisos")]
    [Authorize]
    public sealed class RolesPermisosController : ControllerBase
    {
        private readonly IRolPermisoService _rolPermisoService;

        public RolesPermisosController(IRolPermisoService rolPermisoService)
        {
            _rolPermisoService = rolPermisoService;
        }

        // =========================================================
        // GET: api/roles/{rolId}/permisos
        // Todos los permisos del catálogo con flag Asignado
        // (para renderizar switches en el front)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos(Guid rolId)
        {
            var permisos = await _rolPermisoService
                .ObtenerPorRolAsync(rolId);

            return Ok(permisos);
        }

        // =========================================================
        // GET: api/roles/{rolId}/permisos/asignados
        // =========================================================
        [HttpGet("asignados")]
        public async Task<IActionResult> ObtenerAsignados(Guid rolId)
        {
            var permisos = await _rolPermisoService
                .ObtenerAsignadosAsync(rolId);

            return Ok(permisos);
        }

        // =========================================================
        // GET: api/roles/{rolId}/permisos/no-asignados
        // =========================================================
        [HttpGet("no-asignados")]
        public async Task<IActionResult> ObtenerNoAsignados(Guid rolId)
        {
            var permisos = await _rolPermisoService
                .ObtenerNoAsignadosAsync(rolId);

            return Ok(permisos);
        }

        // =========================================================
        // PATCH: api/roles/{rolId}/permisos
        // Switch visual — manda { permisoId, asignado: true/false }
        // =========================================================
        [HttpPatch]
        public async Task<IActionResult> Upsert(
            Guid rolId,
            [FromBody] UpsertPermisoRolDto dto)
        {
            await _rolPermisoService.UpsertAsync(rolId, dto);

            return Ok(new
            {
                Mensaje = dto.Asignado
                    ? "Permiso asignado correctamente."
                    : "Permiso removido correctamente."
            });
        }
    }
}
