using SistemaApoyosMunicipales.Application.DTOs.EstadosSolicitud;

using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IEstadoSolicitudService
    {
        Task<Guid> CrearAsync(CrearEstadoSolicitudDto dto);
        Task ActualizarAsync(Guid id, ActualizarEstadoSolicitudDto dto);
        Task<EstadoSolicitudDto> ObtenerPorIdAsync(Guid id);
        Task<List<EstadoSolicitudDto>> ObtenerTodosAsync();
        Task CambiarEstatusAsync(Guid id, CambiarEstatusEstadoSolicitudDto dto);
    }
}
