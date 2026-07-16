using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IReportesService
    {
        Task<byte[]> GenerarReporteAnualAsync(FiltroReporteDto filtro);

        Task<byte[]> GenerarReportePorComunidadAsync(Guid comunidadId, FiltroReporteDto filtro);


        Task<byte[]> ExportarComunidadesExcelAsync(FiltroReporteDto filtro);
        Task<byte[]> ExportarApoyosPorComunidadExcelAsync(Guid comunidadId, FiltroReporteDto filtro);
        Task<byte[]> ExportarFondosExcelAsync(FiltroReporteDto filtro);
        Task<byte[]> ExportarApoyosExcelAsync(FiltroReporteDto filtro);

    }
}
