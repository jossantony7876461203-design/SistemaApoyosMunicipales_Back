using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IComunidadService
    {
        Task<Guid?> CrearAsync(CrearComunidadDto dto);

        Task<ComunidadDto> ObtenerPorIdAsync(Guid id);

        Task<ComunidadDto> ObtenerPorClaveInternaAsync(string claveInterna);

        Task<PaginatedResult<ComunidadDto>> ObtenerTodasAsync(
            PaginationRequest pagination);

        Task<PaginatedResult<ComunidadDto>> ObtenerInactivasAsync(
    PaginationRequest pagination);

        Task ActualizarAsync(
            Guid id,
            ActualizarComunidadDto dto);

        Task CambiarEstatusAsync(
    Guid id,
    CambiarEstatusComunidadDto dto);
        Task ActualizarIneAsync(Guid id, ActualizarIneDto dto);
    }
}
