using Microsoft.AspNetCore.Mvc;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
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
}