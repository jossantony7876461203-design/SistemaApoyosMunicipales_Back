using Dapper;
using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public sealed class ReportesRepository : IReportesRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReportesRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // =========================
        // RESUMEN POR COMUNIDAD (reporte anual/global)
        // =========================
        public async Task<List<ReporteComunidadResumenDto>> ObtenerResumenPorComunidadAsync(
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? comunidadIds,
            List<Guid>? apoyoIds)
        {
            var parametros = new DynamicParameters();
            parametros.Add("Desde", desde);
            parametros.Add("Hasta", hasta);

            var filtros = new StringBuilder(@"
                r.deleted_at IS NULL
                AND r.activo = true
                AND r.fecha_apoyo >= @Desde
                AND r.fecha_apoyo <= @Hasta
            ");

            if (comunidadIds is { Count: > 0 })
            {
                filtros.Append(" AND r.comunidad_id = ANY(@ComunidadIds)");
                parametros.Add("ComunidadIds", comunidadIds.ToArray());
            }

            if (apoyoIds is { Count: > 0 })
            {
                filtros.Append(" AND r.apoyo_id = ANY(@ApoyoIds)");
                parametros.Add("ApoyoIds", apoyoIds.ToArray());
            }

            var sql = $@"
                SELECT
                    c.id            AS ComunidadId,
                    c.nombre        AS Comunidad,
                    c.delegado      AS Delegado,
                    COUNT(r.id)     AS TotalApoyos,
                    COALESCE(SUM(r.monto_otorgado), 0) AS TotalDinero
                FROM registros_apoyo r
                JOIN comunidades c ON c.id = r.comunidad_id
                WHERE {filtros}
                GROUP BY c.id, c.nombre, c.delegado
                ORDER BY TotalDinero DESC;
            ";

            using var conexion = _connectionFactory.CrearConexion();

            var resultado = await conexion.QueryAsync<ReporteComunidadResumenDto>(sql, parametros);
            return resultado.ToList();
        }

        // =========================
        // DATOS DE UNA COMUNIDAD (para el reporte por comunidad)
        // =========================
        public async Task<(string Comunidad, string? Delegado)?> ObtenerComunidadAsync(Guid comunidadId)
        {
            const string sql = @"
                SELECT nombre AS Comunidad, delegado AS Delegado
                FROM comunidades
                WHERE id = @ComunidadId AND deleted_at IS NULL;
            ";

            using var conexion = _connectionFactory.CrearConexion();

            var resultado = await conexion.QuerySingleOrDefaultAsync<(string Comunidad, string? Delegado)?>(
                sql, new { ComunidadId = comunidadId });

            return resultado;
        }

        // =========================
        // APOYOS DE UNA COMUNIDAD (detalle del reporte por comunidad)
        // =========================
        public async Task<List<ReporteApoyoDetalleDto>> ObtenerApoyosDeComunidadAsync(
            Guid comunidadId,
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? apoyoIds)
        {
            var parametros = new DynamicParameters();
            parametros.Add("ComunidadId", comunidadId);
            parametros.Add("Desde", desde);
            parametros.Add("Hasta", hasta);

            var filtros = new StringBuilder(@"
                r.deleted_at IS NULL
                AND r.activo = true
                AND r.comunidad_id = @ComunidadId
                AND r.fecha_apoyo >= @Desde
                AND r.fecha_apoyo <= @Hasta
            ");

            if (apoyoIds is { Count: > 0 })
            {
                filtros.Append(" AND r.apoyo_id = ANY(@ApoyoIds)");
                parametros.Add("ApoyoIds", apoyoIds.ToArray());
            }

            var sql = $@"
                SELECT
                    r.folio                AS Folio,
                    a.nombre                AS Fondo,
                    r.fecha_apoyo            AS FechaApoyo,
                    r.monto_otorgado        AS MontoOtorgado,
                    es.nombre                AS Estado
                FROM registros_apoyo r
                JOIN apoyos a              ON a.id = r.apoyo_id
                JOIN estados_solicitud es  ON es.id = r.estado_solicitud_id
                WHERE {filtros}
                ORDER BY r.fecha_apoyo DESC;
            ";

            using var conexion = _connectionFactory.CrearConexion();

            var resultado = await conexion.QueryAsync<ReporteApoyoDetalleDto>(sql, parametros);
            return resultado.ToList();
        }


        // =========================
        // TODOS LOS APOYOS (global, con filtros de comunidad y/o fondo)
        // =========================
        public async Task<List<ReporteApoyoGlobalDto>> ObtenerTodosLosApoyosAsync(
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? comunidadIds,
            List<Guid>? apoyoIds)
        {
            var parametros = new DynamicParameters();
            parametros.Add("Desde", desde);
            parametros.Add("Hasta", hasta);

            var filtros = new StringBuilder(@"
        r.deleted_at IS NULL
        AND r.activo = true
        AND r.fecha_apoyo >= @Desde
        AND r.fecha_apoyo <= @Hasta
    ");

            if (comunidadIds is { Count: > 0 })
            {
                filtros.Append(" AND r.comunidad_id = ANY(@ComunidadIds)");
                parametros.Add("ComunidadIds", comunidadIds.ToArray());
            }

            if (apoyoIds is { Count: > 0 })
            {
                filtros.Append(" AND r.apoyo_id = ANY(@ApoyoIds)");
                parametros.Add("ApoyoIds", apoyoIds.ToArray());
            }

            var sql = $@"
        SELECT
            r.folio               AS Folio,
            c.nombre               AS Comunidad,
            a.nombre               AS Fondo,
            r.fecha_apoyo           AS FechaApoyo,
            r.monto_otorgado       AS MontoOtorgado,
            es.nombre               AS Estado
        FROM registros_apoyo r
        JOIN comunidades c          ON c.id = r.comunidad_id
        JOIN apoyos a               ON a.id = r.apoyo_id
        JOIN estados_solicitud es   ON es.id = r.estado_solicitud_id
        WHERE {filtros}
        ORDER BY r.fecha_apoyo DESC;
    ";

            using var conexion = _connectionFactory.CrearConexion();

            var resultado = await conexion.QueryAsync<ReporteApoyoGlobalDto>(sql, parametros);
            return resultado.ToList();
        }

        // =========================
        // RESUMEN POR FONDO (catálogo "apoyos" agrupado)
        // =========================
        public async Task<List<ReporteFondoDto>> ObtenerResumenPorFondoAsync(
            DateTimeOffset desde,
            DateTimeOffset hasta,
            List<Guid>? apoyoIds)
        {
            var parametros = new DynamicParameters();
            parametros.Add("Desde", desde);
            parametros.Add("Hasta", hasta);

            var filtroApoyoId = string.Empty;

            if (apoyoIds is { Count: > 0 })
            {
                filtroApoyoId = " AND r.apoyo_id = ANY(@ApoyoIds) ";
                parametros.Add("ApoyoIds", apoyoIds.ToArray());
            }

            var sql = $@"
        SELECT
            a.id                     AS FondoId,
            a.nombre                 AS Nombre,
            COUNT(r.id)               AS TotalApoyos,
            COALESCE(SUM(r.monto_otorgado), 0) AS TotalDinero
        FROM apoyos a
        LEFT JOIN registros_apoyo r
            ON r.apoyo_id = a.id
            AND r.deleted_at IS NULL
            AND r.activo = true
            AND r.fecha_apoyo >= @Desde
            AND r.fecha_apoyo <= @Hasta
            {filtroApoyoId}
        GROUP BY a.id, a.nombre
        ORDER BY TotalDinero DESC;
    ";

            using var conexion = _connectionFactory.CrearConexion();

            var resultado = await conexion.QueryAsync<ReporteFondoDto>(sql, parametros);
            return resultado.ToList();
        }
    }
}
