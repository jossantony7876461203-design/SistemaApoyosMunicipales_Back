using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;

namespace SistemaApoyosMunicipales.API.Controllers;

[ApiController]
[Route("api/comunidades")]
[Authorize]
public sealed class ComunidadesController : ControllerBase
{
    private readonly IComunidadService _comunidadService;
    private readonly IImagenQueue _imagenQueue;

    public ComunidadesController(
     IComunidadService comunidadService,
     IImagenQueue imagenQueue)
    {
        _comunidadService = comunidadService;
        _imagenQueue = imagenQueue;
    }


    // POST: api/comunidades
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Crear([FromForm] CrearComunidadDto dto)
    {
        var tareaId = await _comunidadService.CrearAsync(dto);

        return Ok(new
        {
            Mensaje = "Comunidad creada correctamente.",
            TareaId = tareaId,    // null si no había imagen
            Info = tareaId is not null
                ? "La imagen se está procesando en segundo plano."
                : null
        });
    }

    // GET: api/comunidades/tarea/{tareaId}
    // El front consulta esto cada X segundos hasta Completada o Fallida
    [HttpGet("tarea/{tareaId:guid}")]
    public IActionResult ObtenerEstadoTarea(Guid tareaId)
    {
        var tarea = _imagenQueue.ObtenerEstado(tareaId);

        if (tarea is null)
            return NotFound(new { Mensaje = "Tarea no encontrada." });

        return Ok(new
        {
            TareaId = tarea.Id,
            Estado = tarea.Estado.ToString(),
            Url = tarea.UrlResultado,
            Error = tarea.Error
        });
    }

    // PATCH: api/comunidades/{id}/ine
    [HttpPatch("{id:guid}/ine")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ActualizarIne(
        Guid id,
        [FromForm] ActualizarIneDto dto)
    {
        await _comunidadService.ActualizarIneAsync(id, dto);
        return Ok(new { Mensaje = "INE actualizado correctamente." });
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var resultado =
            await _comunidadService.ObtenerTodasAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(
        Guid id)
    {
        var comunidad =
            await _comunidadService.ObtenerPorIdAsync(id);

        return Ok(comunidad);
    }


    [HttpGet("inactivas")]
    public async Task<IActionResult> ObtenerInactivas(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10)
    {
        var resultado =
            await _comunidadService.ObtenerInactivasAsync(
                new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

        return Ok(resultado);
    }

    [HttpGet("clave/{claveInterna}")]
    public async Task<IActionResult> ObtenerPorClave(
        string claveInterna)
    {
        var comunidad =
            await _comunidadService
                .ObtenerPorClaveInternaAsync(claveInterna);

        return Ok(comunidad);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarComunidadDto dto)
    {
        await _comunidadService.ActualizarAsync(id, dto);

        return Ok(new
        {
            Mensaje = "Comunidad actualizada correctamente."
        });
    }

    [HttpPatch("{id:guid}/estatus")]
    public async Task<IActionResult> CambiarEstatus(
     Guid id,
     [FromBody] CambiarEstatusComunidadDto dto)
    {
        await _comunidadService
            .CambiarEstatusAsync(id, dto);

        return Ok(new
        {
            Mensaje = dto.Activo
                ? "Comunidad activada correctamente."
                : "Comunidad desactivada correctamente."
        });
    }
}
