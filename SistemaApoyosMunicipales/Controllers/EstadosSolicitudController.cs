using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaApoyosMunicipales.Application.DTOs.EstadosSolicitud;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;


namespace SistemaApoyosMunicipales.API.Controllers;

[ApiController]
[Route("api/estados-solicitud")]
[Authorize]
public sealed class EstadosSolicitudController : ControllerBase
{
    private readonly IEstadoSolicitudService _estadoSolicitudService;

    public EstadosSolicitudController(IEstadoSolicitudService estadoSolicitudService)
    {
        _estadoSolicitudService = estadoSolicitudService;
    }

    // POST: api/estados-solicitud
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEstadoSolicitudDto dto)
    {
        var id = await _estadoSolicitudService.CrearAsync(dto);

        return Ok(new
        {
            Mensaje = "Estado de solicitud creado correctamente.",
            Id = id
        });
    }

    // GET: api/estados-solicitud
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var estados = await _estadoSolicitudService.ObtenerTodosAsync();
        return Ok(estados);
    }

    // GET: api/estados-solicitud/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var estado = await _estadoSolicitudService.ObtenerPorIdAsync(id);
        return Ok(estado);
    }

    // PATCH: api/estados-solicitud/{id}
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarEstadoSolicitudDto dto)
    {
        await _estadoSolicitudService.ActualizarAsync(id, dto);

        return Ok(new
        {
            Mensaje = "Estado de solicitud actualizado correctamente."
        });
    }

    // PATCH: api/estados-solicitud/{id}/estatus
    [HttpPatch("{id:guid}/estatus")]
    public async Task<IActionResult> CambiarEstatus(Guid id, [FromBody] CambiarEstatusEstadoSolicitudDto dto)
    {
        await _estadoSolicitudService.CambiarEstatusAsync(id, dto);

        return Ok(new
        {
            Mensaje = dto.Activo
                ? "Estado de solicitud activado correctamente."
                : "Estado de solicitud desactivado correctamente."
        });
    }
}