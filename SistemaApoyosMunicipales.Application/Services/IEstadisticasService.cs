using SistemaApoyosMunicipales.Application.DTOs.Dashboard;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class EstadisticasService : IEstadisticasService
    {
        private readonly IEstadisticasRepository _estadisticasRepository;

        public EstadisticasService(IEstadisticasRepository estadisticasRepository)
        {
            _estadisticasRepository = estadisticasRepository;
        }

        public async Task<List<MontoPorComunidadDto>> ObtenerMontoPorComunidadAsync()
        {
            return await _estadisticasRepository.ObtenerMontoPorComunidadAsync();
        }

        public async Task<ResumenDashboardDto> ObtenerResumenAsync()
        {
            return await _estadisticasRepository.ObtenerResumenAsync();
        }

        public async Task<List<ApoyosPorMesDto>> ObtenerApoyosPorMesAsync(int anio)
        {
            return await _estadisticasRepository.ObtenerApoyosPorMesAsync(anio);
        }

        public async Task<List<DistribucionPorFondoDto>> ObtenerDistribucionPorFondoAsync()
        {
            return await _estadisticasRepository.ObtenerDistribucionPorFondoAsync();
        }

        public async Task<List<ApoyoRecienteDto>> ObtenerRecientesAsync(int top)
        {
            return await _estadisticasRepository.ObtenerRecientesAsync(top);
        }

        public async Task<TopComunidadesDto> ObtenerTopComunidadesAsync(int anio, int top)
        {
            return await _estadisticasRepository.ObtenerTopComunidadesAsync(anio, top);
        }
    }
}
