using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Services;

namespace SistemaApoyosMunicipales.API.Controllers;

[ApiController]
[Route("api/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // GET: api/usuarios/activos?pageNumber=1&pageSize=10
    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _usuarioService.ObtenerActivosAsync(
            new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

        return Ok(result);
    }
    // =========================================================
    // GET: api/usuarios/inactivos
    // =========================================================
    [HttpGet("inactivos")]
    public async Task<IActionResult> ObtenerInactivos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result =
            await _usuarioService.ObtenerInactivosAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var usuario =
            await _usuarioService.ObtenerPorIdAsync(id);

        return Ok(usuario);
    }
    // =========================================================
    // PATCH: api/usuarios/{id}/estatus
    // =========================================================
    [HttpPatch("{id:guid}/estatus")]
    public async Task<IActionResult> CambiarEstatus(
        Guid id,
        [FromBody] CambiarEstatusUsuarioDto dto)
    {
        await _usuarioService
            .CambiarEstatusAsync(id, dto);

        return Ok(new
        {
            Mensaje = dto.Activo
                ? "Usuario activado correctamente."
                : "Usuario desactivado correctamente."
        });
    }

    // =========================================================
    // PATCH: api/usuarios/{id}
    // =========================================================
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarUsuarioDto dto)
    {
        await _usuarioService
            .ActualizarAsync(id, dto);

        return Ok(new
        {
            Mensaje = "Usuario actualizado correctamente."
        });
    }


    // =========================================================
    // PATCH: api/usuarios/{id}/rol
    // Asignar Rol/SubRol a un usuario
    // =========================================================
    [HttpPatch("{id:guid}/asignar-rol")]
    public async Task<IActionResult> AsignarRol(
        Guid id,
        [FromBody] AsignarRolUsuarioDto dto)
    {
        await _usuarioService
            .AsignarRolAsync(id, dto);

        return Ok(new
        {
            Mensaje = "Rol asignado correctamente."
        });
    }
}