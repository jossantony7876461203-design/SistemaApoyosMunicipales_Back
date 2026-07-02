using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IReportesRepository
    {
        Task<List<ReporteComunidadResumenDto>> ObtenerResumenPorComunidadAsync(
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? comunidadIds,
            List<Guid>? apoyoIds);

        Task<(string Comunidad, string? Delegado)?> ObtenerComunidadAsync(Guid comunidadId);

        Task<List<ReporteApoyoDetalleDto>> ObtenerApoyosDeComunidadAsync(
            Guid comunidadId,
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? apoyoIds);
    }
}
