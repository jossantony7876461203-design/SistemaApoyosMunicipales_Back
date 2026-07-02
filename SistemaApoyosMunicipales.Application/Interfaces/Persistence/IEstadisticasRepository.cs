using SistemaApoyosMunicipales.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IEstadisticasRepository
    {
        Task<List<MontoPorComunidadDto>> ObtenerMontoPorComunidadAsync();

        Task<ResumenDashboardDto> ObtenerResumenAsync();

        Task<List<ApoyosPorMesDto>> ObtenerApoyosPorMesAsync(int anio);

        Task<List<DistribucionPorFondoDto>> ObtenerDistribucionPorFondoAsync();

        Task<List<ApoyoRecienteDto>> ObtenerRecientesAsync(int top);

        Task<TopComunidadesDto> ObtenerTopComunidadesAsync(int anio, int top);
    }
}
