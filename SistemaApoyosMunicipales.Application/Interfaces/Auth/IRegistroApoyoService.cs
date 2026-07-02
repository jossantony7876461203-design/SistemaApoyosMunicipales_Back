using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IRegistroApoyoService
    {
        Task<Guid> CrearAsync(
            CrearRegistroApoyoDto dto,  Guid usuarioId);

        Task ActualizarAsync(
            Guid id,
            ActualizarRegistroApoyoDto dto);

        Task<ObtenerRegistroApoyoDto>
            ObtenerPorIdAsync(Guid id);

        Task<PaginatedResult<ObtenerRegistroApoyoListadoDto>>
            ObtenerPorComunidadAsync(
                Guid comunidadId,
                PaginationRequest pagination);

        Task CambiarEstadoAsync(
            Guid id,
            Guid estadoSolicitudId);

        Task EliminarAsync(Guid id);

        Task<PaginatedResult<ObtenerRegistroApoyoGlobalDto>> ObtenerTodosAsync(PaginationRequest pagination);

        Task<ObtenerRegistroApoyoDetalleDto> ObtenerDetalleAsync(Guid id);

        Task<List<RegistroApoyoDocumentoDto>> AgregarDocumentosAsync(Guid id, AgregarDocumentosRegistroApoyoDto dto);
    }
}
