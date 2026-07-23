using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.DTOs.Dashboard;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class EstadisticasRepository : IEstadisticasRepository
    {
        private readonly AppDbContext _context;

        public EstadisticasRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // MONTO POR COMUNIDAD
        // =========================
        public async Task<List<MontoPorComunidadDto>> ObtenerMontoPorComunidadAsync()
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Where(x => x.DeletedAt == null && x.Activo)
                .GroupBy(x => new
                {
                    x.ComunidadId,
                    x.Comunidad.Nombre,
                    x.Comunidad.Delegado
                })
                .Select(g => new MontoPorComunidadDto
                {
                    ComunidadId = g.Key.ComunidadId,
                    Comunidad = g.Key.Nombre,
                    Delegado = g.Key.Delegado,
                    MontoTotal = g.Sum(x => x.MontoOtorgado),
                    TotalApoyos = g.Count()
                })
                .OrderByDescending(x => x.MontoTotal)
                .ToListAsync();
        }

        // =========================
        // RESUMEN (KPIs)
        // =========================
        public async Task<ResumenDashboardDto> ObtenerResumenAsync()
        {
            var ahora = DateTimeOffset.UtcNow;

            var inicioDeMes = new DateTimeOffset(
                ahora.Year,
                ahora.Month,
                1,
                0,
                0,
                0,
                TimeSpan.Zero);

            var registrosActivos = _context.RegistroApoyos
                .AsNoTracking()
                .Where(x =>
                    x.DeletedAt == null &&
                    x.Activo);

            var totalApoyos = await registrosActivos.CountAsync();

            var apoyosEsteMes = await registrosActivos
                .CountAsync(x => x.CreatedAt >= inicioDeMes);

            var comunidadesAtendidas = await registrosActivos
                .Select(x => x.ComunidadId)
                .Distinct()
                .CountAsync();

            var comunidadesNuevasEsteMes = await registrosActivos
                .Where(x => x.CreatedAt >= inicioDeMes)
                .Select(x => x.ComunidadId)
                .Distinct()
                .CountAsync();

            var fondosActivos = await _context.Apoyos
                .AsNoTracking()
                .CountAsync(x =>
                    x.Activo &&
                    x.DeletedAt == null);

            var pendientesValidar = await registrosActivos
                .CountAsync(x =>
                    x.EstadoSolicitud != null &&
                    x.EstadoSolicitud.Clave != null &&
                    x.EstadoSolicitud.Clave.ToLower() == "Pendiente");

            return new ResumenDashboardDto
            {
                TotalApoyos = totalApoyos,
                ApoyosEsteMes = apoyosEsteMes,
                ComunidadesAtendidas = comunidadesAtendidas,
                ComunidadesNuevasEsteMes = comunidadesNuevasEsteMes,
                FondosActivos = fondosActivos,
                PendientesValidar = pendientesValidar
            };
        }
        // =========================
        // APOYOS POR MES (gráfica de barras)
        // =========================
        public async Task<List<ApoyosPorMesDto>> ObtenerApoyosPorMesAsync(int anio)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Where(x =>
                    x.DeletedAt == null &&
                    x.Activo &&
                    x.FechaApoyo.Year == anio)
                .GroupBy(x => new { Mes = x.FechaApoyo.Month, x.Apoyo.Nombre })
                .Select(g => new ApoyosPorMesDto
                {
                    Mes = g.Key.Mes,
                    Fondo = g.Key.Nombre,
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Mes)
                .ThenBy(x => x.Fondo)
                .ToListAsync();
        }

        // =========================
        // DISTRIBUCIÓN POR FONDO (dona)
        // =========================
        public async Task<List<DistribucionPorFondoDto>> ObtenerDistribucionPorFondoAsync()
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Where(x => x.DeletedAt == null && x.Activo)
                .GroupBy(x => x.Apoyo.Nombre)
                .Select(g => new DistribucionPorFondoDto
                {
                    Fondo = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();
        }

        // =========================
        // APOYOS RECIENTES (tabla)
        // =========================
        public async Task<List<ApoyoRecienteDto>> ObtenerRecientesAsync(int top)
        {
            return await _context.RegistroApoyos
                .AsNoTracking()
                .Where(x => x.DeletedAt == null && x.Activo)
                .OrderByDescending(x => x.CreatedAt)
                .Take(top)
                .Select(x => new ApoyoRecienteDto
                {
                    Id = x.Id,
                    Comunidad = x.Comunidad.Nombre,
                    TipoApoyo = x.Apoyo.Nombre,
                    Estado = x.EstadoSolicitud.Nombre,
                    Fecha = x.FechaApoyo
                })
                .ToListAsync();
        }

        // =========================
        // TOP COMUNIDADES + CONTEO POR ESTADO
        // =========================
        public async Task<TopComunidadesDto> ObtenerTopComunidadesAsync(int anio, int top)
        {
            var registrosDelAnio = _context.RegistroApoyos
                .AsNoTracking()
                .Where(x =>
                    x.DeletedAt == null &&
                    x.Activo &&
                    x.FechaApoyo.Year == anio);

            var topComunidades = await registrosDelAnio
                .GroupBy(x => new { x.ComunidadId, x.Comunidad.Nombre })
                .Select(g => new TopComunidadItemDto
                {
                    ComunidadId = g.Key.ComunidadId,
                    Comunidad = g.Key.Nombre,
                    TotalApoyos = g.Count()
                })
                .OrderByDescending(x => x.TotalApoyos)
                .Take(top)
                .ToListAsync();

            var pendientes = await registrosDelAnio
                .CountAsync(x => x.EstadoSolicitud.Clave == "Pendiente");

            var validados = await registrosDelAnio
                .CountAsync(x => x.EstadoSolicitud.Clave == "Validado");

            var aprobados = await registrosDelAnio
                .CountAsync(x => x.EstadoSolicitud.Clave == "Aprobado");

            return new TopComunidadesDto
            {
                TopComunidades = topComunidades,
                Pendientes = pendientes,
                Validados = validados,
                Aprobados = aprobados
            };
        }
    }
}
