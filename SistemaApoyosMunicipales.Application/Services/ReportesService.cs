using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// Alias explícitos: el proyecto tiene "usings globales implícitos" que
// traen System.Reflection.Metadata.Document y System.ComponentModel.IContainer,
// y ambos chocan con los tipos de QuestPDF del mismo nombre.
using Document = QuestPDF.Fluent.Document;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ReportesService : IReportesService
    {
        private readonly IReportesRepository _reportesRepository;

        private static readonly CultureInfo CulturaMx = new("es-MX");

        public ReportesService(IReportesRepository reportesRepository)
        {
            _reportesRepository = reportesRepository;
        }

        // =========================
        // REPORTE ANUAL / GLOBAL POR COMUNIDADES
        // =========================
        public async Task<byte[]> GenerarReporteAnualAsync(FiltroReporteDto filtro)
        {
            var (desde, hasta) = CalcularRango(filtro);

            var comunidades = await _reportesRepository.ObtenerResumenPorComunidadAsync(
                desde, hasta, filtro.ComunidadIds, filtro.ApoyoIds);

            var reporte = new ReporteAnualDto
            {
                Desde = desde,
                Hasta = hasta,
                TotalApoyos = comunidades.Sum(c => c.TotalApoyos),
                TotalDinero = comunidades.Sum(c => c.TotalDinero),
                TotalComunidades = comunidades.Count,
                Comunidades = comunidades,
                Top5MasBeneficiadas = comunidades
                    .OrderByDescending(c => c.TotalDinero)
                    .Take(5)
                    .ToList(),
                Top5MenosBeneficiadas = comunidades
                    .OrderBy(c => c.TotalDinero)
                    .Take(5)
                    .ToList()
            };

            return GenerarPdfReporteAnual(reporte);
        }

        // =========================
        // REPORTE POR COMUNIDAD
        // =========================
        public async Task<byte[]> GenerarReportePorComunidadAsync(Guid comunidadId, FiltroReporteDto filtro)
        {
            var comunidad = await _reportesRepository.ObtenerComunidadAsync(comunidadId);

            if (comunidad is null)
                throw new NotFoundException("La comunidad no existe.");

            var (desde, hasta) = CalcularRango(filtro);

            var apoyos = await _reportesRepository.ObtenerApoyosDeComunidadAsync(
                comunidadId, desde, hasta, filtro.ApoyoIds);

            var reporte = new ReportePorComunidadDto
            {
                Desde = desde,
                Hasta = hasta,
                ComunidadId = comunidadId,
                Comunidad = comunidad.Value.Comunidad,
                Delegado = comunidad.Value.Delegado,
                TotalApoyos = apoyos.Count,
                TotalDinero = apoyos.Sum(a => a.MontoOtorgado),
                Apoyos = apoyos
            };

            return GenerarPdfReportePorComunidad(reporte);
        }

        // =========================
        // CÁLCULO DE RANGO DE FECHAS DINÁMICO
        // =========================
        private static (DateTimeOffset Desde, DateTimeOffset Hasta) CalcularRango(FiltroReporteDto filtro)
        {
            DateTimeOffset desde;
            DateTimeOffset hasta;

            if (filtro.AnioInicio.HasValue)
            {
                var mes = filtro.MesInicio ?? 1;
                desde = new DateTimeOffset(filtro.AnioInicio.Value, mes, 1, 0, 0, 0, TimeSpan.Zero);
            }
            else
            {
                desde = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }

            if (filtro.AnioFin.HasValue)
            {
                var mes = filtro.MesFin ?? 12;
                var ultimoDiaDelMes = DateTime.DaysInMonth(filtro.AnioFin.Value, mes);
                hasta = new DateTimeOffset(
                    filtro.AnioFin.Value, mes, ultimoDiaDelMes, 23, 59, 59, TimeSpan.Zero);
            }
            else
            {
                hasta = DateTimeOffset.UtcNow;
            }

            return (desde, hasta);
        }

        // =========================
        // DISEÑO DEL PDF: REPORTE ANUAL - ESTILO GUBERNAMENTAL
        // =========================
        private byte[] GenerarPdfReporteAnual(ReporteAnualDto reporte)
        {
            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(40);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(10).FontFamily("Arial"));

                    // ==================== HEADER GUBERNAMENTAL ====================
                    pagina.Header().Column(col =>
                    {
                        // Barra superior decorativa
                        col.Item().Height(4).Background("#C62828");

                        // Logo y título
                        col.Item().PaddingVertical(8).Row(row =>
                        {
                            row.RelativeItem(1).Column(colLogo =>
                            {
                                colLogo.Item().Text("GOBIERNO MUNICIPAL")
                                    .FontSize(8).Bold().FontColor("#1A237E").LetterSpacing(2);
                                colLogo.Item().Text("Sistema de Apoyos")
                                    .FontSize(14).Bold().FontColor("#263238");
                                colLogo.Item().Text("Programa de Desarrollo Comunitario")
                                    .FontSize(9).FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(120).Column(colLogo =>
                            {
                                colLogo.Item().AlignRight().Text("REPORTE")
                                    .FontSize(11).Bold().FontColor("#D32F2F").LetterSpacing(3);
                                colLogo.Item().AlignRight().Text($"AÑO {reporte.Hasta.Year}")
                                    .FontSize(18).Bold().FontColor("#C62828");
                            });
                        });

                        // Línea decorativa
                        col.Item().PaddingVertical(5).LineHorizontal(2).LineColor("#D32F2F");

                        // Información del periodo
                        col.Item().PaddingVertical(3).Row(row =>
                        {
                            row.ConstantItem(150).Text("PERIODO:").Bold().FontSize(9);
                            row.RelativeItem().Text($"{reporte.Desde:dd 'de' MMMM 'de' yyyy} - {reporte.Hasta:dd 'de' MMMM 'de' yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Darken2);
                        });

                        col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // ==================== CONTENIDO ====================
                    pagina.Content().PaddingTop(10).Column(col =>
                    {
                        // MÉTRICAS PRINCIPALES - Tarjetas mejoradas
                        col.Item().PaddingBottom(15).Row(fila =>
                        {
                            fila.RelativeItem().Padding(3).Element(container =>
                                TarjetaResumenGubernamental(container, "Total Apoyos", reporte.TotalApoyos.ToString("N0")));

                            fila.RelativeItem().Padding(3).Element(container =>
                                TarjetaResumenGubernamental(container, "Comunidades", reporte.TotalComunidades.ToString("N0")));

                            fila.RelativeItem().Padding(3).Element(container =>
                                TarjetaResumenGubernamental(container, "Monto Total", FormatearMoneda(reporte.TotalDinero)));
                        });

                        // SECCIÓN TOP 5 MÁS BENEFICIADAS
                        col.Item().PaddingTop(10).PaddingBottom(3).Text("COMUNIDADES MÁS BENEFICIADAS")
                            .Bold().FontSize(11).FontColor("#1A237E").LetterSpacing(1);

                        col.Item().PaddingBottom(15).Element(c =>
                            TablaComunidadesMejorada(c, reporte.Top5MasBeneficiadas, true));

                        // SECCIÓN TOP 5 MENOS BENEFICIADAS
                        col.Item().PaddingTop(5).PaddingBottom(3).Text("COMUNIDADES EN DESARROLLO PRIORITARIO")
                            .Bold().FontSize(11).FontColor("#1A237E").LetterSpacing(1);

                        col.Item().PaddingBottom(15).Element(c =>
                            TablaComunidadesMejorada(c, reporte.Top5MenosBeneficiadas, false));

                        // SECCIÓN DETALLE COMPLETO
                        col.Item().PaddingTop(5).PaddingBottom(3).Text("DETALLE COMPLETO POR COMUNIDAD")
                            .Bold().FontSize(11).FontColor("#1A237E").LetterSpacing(1);

                        col.Item().Element(c =>
                            TablaComunidadesMejorada(c, reporte.Comunidades, null));
                    });

                    // ==================== FOOTER GUBERNAMENTAL ====================
                    pagina.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#D32F2F");
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text("Sistema de Apoyos Municipales • Desarrollo Comunitario")
                                .FontSize(8).FontColor(Colors.Grey.Darken1);
                            row.ConstantItem(200).AlignRight().Text(texto =>
                            {
                                // Configuramos el estilo base para todo este bloque de texto
                                texto.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));

                                texto.Span("Página ");
                                texto.CurrentPageNumber().FontColor("#D32F2F");
                                texto.Span(" de ");
                                texto.TotalPages().FontColor("#D32F2F");
                                texto.Span($" • Generado: {DateTimeOffset.Now:dd/MM/yyyy HH:mm}");
                            }); // Aquí termina limpiamente
                        });
                    });
                });
            }).GeneratePdf();
        }

        // =========================
        // DISEÑO DEL PDF: REPORTE POR COMUNIDAD - ESTILO GUBERNAMENTAL
        // =========================
        private byte[] GenerarPdfReportePorComunidad(ReportePorComunidadDto reporte)
        {
            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(40);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(10).FontFamily("Arial"));

                    // ==================== HEADER GUBERNAMENTAL ====================
                    pagina.Header().Column(col =>
                    {
                        // Barra superior decorativa
                        col.Item().Height(4).Background("#C62828");

                        // Título de la comunidad
                        col.Item().PaddingVertical(8).Row(row =>
                        {
                            row.RelativeItem(1).Column(colComunidad =>
                            {
                                colComunidad.Item().Text("GOBIERNO MUNICIPAL")
                                    .FontSize(8).Bold().FontColor("#1A237E").LetterSpacing(2);
                                colComunidad.Item().Text(reporte.Comunidad)
                                    .FontSize(16).Bold().FontColor("#263238");
                                if (!string.IsNullOrWhiteSpace(reporte.Delegado))
                                {
                                    colComunidad.Item().Text($"Delegado: {reporte.Delegado}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                }
                            });

                            row.ConstantItem(130).Column(colInfo =>
                            {
                                colInfo.Item().AlignRight().Text("REPORTE POR COMUNIDAD")
                                    .FontSize(9).Bold().FontColor("#D32F2F").LetterSpacing(1);
                                colInfo.Item().AlignRight().Text($"FOLIO: {reporte.ComunidadId.ToString()[..8]}")
                                    .FontSize(8).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        // Línea decorativa
                        col.Item().PaddingVertical(5).LineHorizontal(2).LineColor("#D32F2F");

                        // Información del periodo
                        col.Item().PaddingVertical(3).Row(row =>
                        {
                            row.ConstantItem(150).Text("PERIODO:").Bold().FontSize(9);
                            row.RelativeItem().Text($"{reporte.Desde:dd 'de' MMMM 'de' yyyy} - {reporte.Hasta:dd 'de' MMMM 'de' yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Darken2);
                        });

                        col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // ==================== CONTENIDO ====================
                    pagina.Content().PaddingTop(10).Column(col =>
                    {
                        // MÉTRICAS PRINCIPALES
                        col.Item().PaddingBottom(15).Row(fila =>
                        {
                            fila.RelativeItem().Padding(3).Element(container =>
                                TarjetaResumenGubernamental(container, "Total Apoyos", reporte.TotalApoyos.ToString("N0")));

                            fila.RelativeItem().Padding(3).Element(container =>
                                TarjetaResumenGubernamental(container, "Monto Otorgado", FormatearMoneda(reporte.TotalDinero)));
                        });

                        // TABLA DE APOYOS
                        col.Item().PaddingTop(10).PaddingBottom(3).Text("RELACIÓN DE APOYOS OTORGADOS")
                            .Bold().FontSize(11).FontColor("#1A237E").LetterSpacing(1);

                        col.Item().Element(c => TablaApoyosMejorada(c, reporte.Apoyos));

                        // RESULTADOS Y OBSERVACIONES
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem(1).Column(colResumen =>
                            {
                                colResumen.Item().Background("#F5F5F5")
                                    .Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10).Column(colObs =>
                                    {
                                        colObs.Item().Text("RESUMEN EJECUTIVO").Bold()
                                            .FontSize(9).FontColor("#1A237E");
                                        colObs.Item().PaddingTop(3).Text(
                                            $"Se otorgaron {reporte.TotalApoyos} apoyos por un monto total de {FormatearMoneda(reporte.TotalDinero)}, " +
                                            $"beneficiando a la comunidad de {reporte.Comunidad}.")
                                            .FontSize(8).FontColor(Colors.Grey.Darken2);
                                    });
                            });

                            row.ConstantItem(200).Column(colFirma =>
                            {
                                colFirma.Item().AlignRight().Text("Vo. Bo. ____________________")
                                    .FontSize(8).FontColor(Colors.Grey.Darken2);
                                colFirma.Item().AlignRight().PaddingTop(2).Text(reporte.Delegado ?? "Delegado Municipal")
                                    .FontSize(9).Bold().FontColor("#263238");
                                colFirma.Item().AlignRight().Text("Comunidad: " + reporte.Comunidad)
                                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                            });
                        });
                    });

                    // ==================== FOOTER GUBERNAMENTAL ====================
                    pagina.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#D32F2F");
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text("Sistema de Apoyos Municipales • Desarrollo Comunitario")
                                .FontSize(8).FontColor(Colors.Grey.Darken1);

                            row.ConstantItem(200).AlignRight().Text(texto =>
                            {
                                // Configuramos el estilo base para todo este bloque de texto
                                texto.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));

                                texto.Span("Página ");
                                texto.CurrentPageNumber().FontColor("#D32F2F");
                                texto.Span(" de ");
                                texto.TotalPages().FontColor("#D32F2F");
                                texto.Span($" • Generado: {DateTimeOffset.Now:dd/MM/yyyy HH:mm}");
                            }); 
                        });
                    });
                });
            }).GeneratePdf();
        }

        // =========================
        // HELPERS DE DISEÑO REUTILIZABLES - VERSIÓN GUBERNAMENTAL
        // =========================

        private static void TarjetaResumenGubernamental(IContainer container, string etiqueta, string valor)
        {
            container.Background(Colors.White).Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(12).Column(col =>
                {
                    col.Item().Text(etiqueta).FontSize(8).FontColor(Colors.Grey.Darken1)
                        .LetterSpacing(1).Bold();
                    col.Item().PaddingTop(2).Text(valor).FontSize(18).Bold()
                        .FontColor("#263238");
                    col.Item().PaddingTop(3).LineHorizontal(1).LineColor("#D32F2F");
                });
        }

        private static void TablaComunidadesMejorada(
            IContainer contenedor,
            List<ReporteComunidadResumenDto> comunidades,
            bool? resaltarPosicion)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    if (resaltarPosicion.HasValue)
                        c.ConstantColumn(25);
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(1);
                    c.RelativeColumn(2);
                });

                tabla.Header(header =>
                {
                    if (resaltarPosicion.HasValue)
                        CeldaHeaderMejorada(header, "#", true);
                    CeldaHeaderMejorada(header, "Comunidad", true);
                    CeldaHeaderMejorada(header, "Delegado", true);
                    CeldaHeaderMejorada(header, "Apoyos", true);
                    CeldaHeaderMejorada(header, "Monto", true);
                });

                int posicion = 1;
                foreach (var c in comunidades)
                {
                    string fondoFila = posicion % 2 == 0 ? "#F5F5F5" : Colors.White;

                    if (resaltarPosicion.HasValue)
                    {
                        if (resaltarPosicion.Value)
                        {
                            // Top 5 - colores dorados para primeros lugares
                            string colorPos = posicion <= 3 ? "#F9A825" : "#FFA500";
                            tabla.Cell().Background(fondoFila).Padding(4)
                                .Text(posicion.ToString()).FontColor(colorPos).Bold().FontSize(8);
                        }
                        else
                        {
                            tabla.Cell().Background(fondoFila).Padding(4)
                                .Text(posicion.ToString()).FontColor(Colors.Grey.Darken1).FontSize(8);
                        }
                        posicion++;
                    }

                    tabla.Cell().Background(fondoFila).Padding(4).Text(c.Comunidad).FontSize(9);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(c.Delegado ?? "-").FontSize(9);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(c.TotalApoyos.ToString("N0"))
                        .FontColor("#D32F2F").Bold().FontSize(9);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(FormatearMoneda(c.TotalDinero))
                        .FontColor("#1A237E").FontSize(9);
                }
            });
        }

        private static void TablaApoyosMejorada(
            IContainer contenedor,
            List<ReporteApoyoDetalleDto> apoyos)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(2.5f);
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(1.5f);
                });

                tabla.Header(header =>
                {
                    CeldaHeaderMejorada(header, "Folio", true);
                    CeldaHeaderMejorada(header, "Fondo", true);
                    CeldaHeaderMejorada(header, "Fecha", true);
                    CeldaHeaderMejorada(header, "Monto", true);
                    CeldaHeaderMejorada(header, "Estado", true);
                    CeldaHeaderMejorada(header, "Beneficiario", true);
                });

                int fila = 0;
                foreach (var apoyo in apoyos)
                {
                    string fondoFila = fila % 2 == 0 ? "#F5F5F5" : Colors.White;

                    tabla.Cell().Background(fondoFila).Padding(4).Text(apoyo.Folio).FontSize(8);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(apoyo.Fondo).FontSize(8);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(apoyo.FechaApoyo.ToString("dd/MM/yyyy")).FontSize(8);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(FormatearMoneda(apoyo.MontoOtorgado))
                        .FontColor("#1A237E").FontSize(8);
                    tabla.Cell().Background(fondoFila).Padding(4).Text(apoyo.Estado)
                        .FontColor(apoyo.Estado == "Aprobado" ? "#1B5E20" : "#E65100")
                        .FontSize(8).Bold();
                    

                    fila++;
                }
            });
        }

        private static void CeldaHeaderMejorada(TableCellDescriptor header, string texto, bool principal)
        {
            header.Cell().Background("#C62828").Padding(6)
                .Text(texto).FontColor(Colors.White).Bold().FontSize(8)
                .LetterSpacing(principal ? 1 : 0);
        }

        private static string FormatearMoneda(decimal monto)
        {
            return monto.ToString("C2", CulturaMx);
        }
    }
}