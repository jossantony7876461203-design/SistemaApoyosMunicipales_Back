using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers
{
    [ApiController]
    [Route("api/subroles/{subRolId:guid}/permisos")]
    public sealed class SubRolesPermisosController : ControllerBase
    {
        private readonly ISubRolPermisoService _subRolPermisoService;

        public SubRolesPermisosController(
            ISubRolPermisoService subRolPermisoService)
        {
            _subRolPermisoService = subRolPermisoService;
        }

        // GET: api/subroles/{subRolId}/permisos
        // Todos con flag asignado y CRUD — para pintar la matriz en el front
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos(Guid subRolId)
        {
            var permisos = await _subRolPermisoService
                .ObtenerPorSubRolAsync(subRolId);

            return Ok(permisos);
        }

        // GET: api/subroles/{subRolId}/permisos/asignados
        [HttpGet("asignados")]
        public async Task<IActionResult> ObtenerAsignados(Guid subRolId)
        {
            var permisos = await _subRolPermisoService
                .ObtenerAsignadosAsync(subRolId);

            return Ok(permisos);
        }

        // GET: api/subroles/{subRolId}/permisos/no-asignados
        [HttpGet("no-asignados")]
        public async Task<IActionResult> ObtenerNoAsignados(Guid subRolId)
        {
            var permisos = await _subRolPermisoService
                .ObtenerNoAsignadosAsync(subRolId);

            return Ok(permisos);
        }

        // PATCH: api/subroles/{subRolId}/permisos
        // Switch visual uno por uno
        [HttpPatch]
        public async Task<IActionResult> Upsert(
            Guid subRolId,
            [FromBody] UpsertSubRolPermisoDto dto)
        {
            await _subRolPermisoService.UpsertAsync(subRolId, dto);

            return Ok(new
            {
                Mensaje = dto.Asignado
                    ? "Permiso asignado correctamente."
                    : "Permiso removido correctamente."
            });
        }

        // PATCH: api/subroles/{subRolId}/permisos/masivo
        // Carga masiva — copiar plantilla o configuración inicial
        [HttpPatch("masivo")]
        public async Task<IActionResult> UpsertMasivo(
            Guid subRolId,
            [FromBody] UpsertMasivoSubRolPermisoDto dto)
        {
            await _subRolPermisoService
                .UpsertMasivoAsync(subRolId, dto);

            return Ok(new
            {
                Mensaje = "Permisos actualizados correctamente."
            });
        }
    }
}
