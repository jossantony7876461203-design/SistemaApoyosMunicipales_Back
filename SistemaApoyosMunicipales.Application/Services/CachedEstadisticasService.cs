using Microsoft.Extensions.Caching.Memory;
using SistemaApoyosMunicipales.Application.DTOs.Dashboard;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;


namespace SistemaApoyosMunicipales.Application.Services
{

    /// <summary>
    /// Decorador que cachea en memoria los resultados de IEstadisticasService.
    /// El dashboard es de solo lectura y tolera datos con unos minutos de
    /// retraso, así que cacheamos agresivamente para que cargue instantáneo.
    /// </summary>
    public sealed class CachedEstadisticasService : IEstadisticasService
    {
        private readonly IEstadisticasService _inner;
        private readonly IMemoryCache _cache;

        // Tiempo que el dato se considera "fresco" antes de recalcular.
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

        private const string KeyMontoPorComunidad = "estadisticas:monto-por-comunidad";
        private const string KeyResumen = "estadisticas:resumen";
        private const string KeyApoyosPorMes = "estadisticas:apoyos-por-mes:{0}";
        private const string KeyDistribucionPorFondo = "estadisticas:distribucion-por-fondo";
        private const string KeyRecientes = "estadisticas:recientes:{0}";
        private const string KeyTopComunidades = "estadisticas:top-comunidades:{0}:{1}";

        public CachedEstadisticasService(IEstadisticasService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public Task<List<MontoPorComunidadDto>> ObtenerMontoPorComunidadAsync()
        {
            return _cache.GetOrCreateAsync(KeyMontoPorComunidad, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Ttl;
                return await _inner.ObtenerMontoPorComunidadAsync();
            })!;
        }

        public Task<ResumenDashboardDto> ObtenerResumenAsync()
        {
            return _cache.GetOrCreateAsync(KeyResumen, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Ttl;
                return await _inner.ObtenerResumenAsync();
            })!;
        }

        public Task<List<ApoyosPorMesDto>> ObtenerApoyosPorMesAsync(int anio)
        {
            var key = string.Format(KeyApoyosPorMes, anio);

            return _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Ttl;
                return await _inner.ObtenerApoyosPorMesAsync(anio);
            })!;
        }

        public Task<List<DistribucionPorFondoDto>> ObtenerDistribucionPorFondoAsync()
        {
            return _cache.GetOrCreateAsync(KeyDistribucionPorFondo, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Ttl;
                return await _inner.ObtenerDistribucionPorFondoAsync();
            })!;
        }

        public Task<List<ApoyoRecienteDto>> ObtenerRecientesAsync(int top)
        {
            var key = string.Format(KeyRecientes, top);

            return _cache.GetOrCreateAsync(key, async entry =>
            {
                // Lista más volátil (cambia con cada apoyo nuevo): TTL más corto.
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await _inner.ObtenerRecientesAsync(top);
            })!;
        }

        public Task<TopComunidadesDto> ObtenerTopComunidadesAsync(int anio, int top)
        {
            var key = string.Format(KeyTopComunidades, anio, top);

            return _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Ttl;
                return await _inner.ObtenerTopComunidadesAsync(anio, top);
            })!;
        }
    }

}