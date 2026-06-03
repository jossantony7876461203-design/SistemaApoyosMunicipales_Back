using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;
using Microsoft.AspNetCore.Authorization;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.API.Controllers;

[ApiController]
[Route("api/comunidades")]
public sealed class ComunidadesController : ControllerBase
{
    private readonly IComunidadService _comunidadService;

    public ComunidadesController(
        IComunidadService comunidadService)
    {
        _comunidadService = comunidadService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CrearComunidadDto dto)
    {
        await _comunidadService.CrearAsync(dto);

        return Ok(new
        {
            Mensaje = "Comunidad creada correctamente."
        });
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
