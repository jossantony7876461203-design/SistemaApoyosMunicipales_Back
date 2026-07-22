using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaApoyosMunicipales.Application.DTOs.Reportes;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Settings;
using SistemaApoyosMunicipales.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

// Alias explícitos
using Document = QuestPDF.Fluent.Document;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ReportesService : IReportesService
    {
        private readonly IReportesRepository _reportesRepository;
        private readonly string _rutaLogo;
        private static readonly CultureInfo CulturaMx = new("es-MX");

        // --- CONFIGURACIÓN DE DISEÑO ---
        private const string ColorVino = "#4B0016";
        private const string ColorOro = "#c9980b";
        private const string ColorGrisTexto = "#333333";
        private const string ColorGrisFondo = "#f8f8f8";
        private const string ColorFondoCabecera = "#FDFBF7";
        private const string ColorLinea = "#E0E0E0";

        public ReportesService(
            IReportesRepository reportesRepository,
            IOptions<ReportesSettings> settings)
        {
            _reportesRepository = reportesRepository;
            _rutaLogo = settings.Value.RutaLogo;
        }

        public async Task<byte[]> GenerarReporteAnualAsync(FiltroReporteDto filtro)
        {
            var (desde, hasta) = CalcularRango(filtro);
            var comunidades = await _reportesRepository.ObtenerResumenPorComunidadAsync(desde, hasta, filtro.ComunidadIds, filtro.ApoyoIds);

            var reporte = new ReporteAnualDto
            {
                Desde = desde,
                Hasta = hasta,
                TotalApoyos = comunidades.Sum(c => c.TotalApoyos),
                TotalDinero = comunidades.Sum(c => c.TotalDinero),
                TotalComunidades = comunidades.Count,
                Comunidades = comunidades
            };

            return GenerarPdfReporteAnual(reporte);
        }

        // --- REPORTE POR COMUNIDAD (CORREGIDO) ---
        public async Task<byte[]> GenerarReportePorComunidadAsync(Guid comunidadId, FiltroReporteDto filtro)
        {
            var comunidad = await _reportesRepository.ObtenerComunidadAsync(comunidadId);
            if (comunidad is null)
                throw new NotFoundException("Comunidad no encontrada");

            var (desde, hasta) = CalcularRango(filtro);
            var apoyos = await _reportesRepository.ObtenerApoyosDeComunidadAsync(
                comunidadId, desde, hasta, filtro.ApoyoIds);

            // Calcular totales para la síntesis ejecutiva
            var totalApoyos = apoyos.Count;
            var totalMonto = apoyos.Sum(a => a.MontoOtorgado);
            var fondosUtilizados = apoyos.Select(a => a.Fondo).Distinct().Count();

            return Document.Create(doc =>
            {
                doc.Page(p =>
                {
                    p.Size(PageSizes.A4);
                    p.Margin(45);
                    p.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial").FontColor(ColorGrisTexto));

                    p.Header().Element(c => GenerarCabecera(c, $"Informe: {comunidad.Value.Comunidad}", desde, hasta));

                    p.Content().PaddingTop(20).Column(col =>
                    {
                        // SÍNTESIS EJECUTIVA CON PERIODO
                        col.Item().PaddingBottom(15).Row(row =>
                        {
                            row.AutoItem().Width(4).Background(ColorOro);
                            row.RelativeItem()
                               .Background(ColorGrisFondo)
                               .Padding(15)
                               .Text(texto =>
                               {
                                   texto.Span("SÍNTESIS EJECUTIVA: ").Bold().FontColor(ColorVino);
                                   texto.Span($"Durante el periodo comprendido del ");
                                   texto.Span(desde.ToString("dd 'de' MMMM 'de' yyyy", CulturaMx)).Bold().FontColor(ColorVino);
                                   texto.Span($" al ");
                                   texto.Span(hasta.ToString("dd 'de' MMMM 'de' yyyy", CulturaMx)).Bold().FontColor(ColorVino);
                                   texto.Span($", la comunidad de ");
                                   texto.Span(comunidad.Value.Comunidad).Bold().FontColor(ColorVino);
                                   texto.Span($" ha recibido un total de ");
                                   texto.Span(totalApoyos.ToString("N0")).Bold().FontColor(ColorVino);
                                   texto.Span(" apoyos, con una inversión total de ");
                                   texto.Span(FormatearMoneda(totalMonto)).Bold().FontColor(ColorVino);
                                   texto.Span($" distribuidos en {fondosUtilizados} fondos diferentes.");
                               });
                        });

                        // INFORMACIÓN DEL DELEGADO POR SEPARADO (CORREGIDO)
                        col.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem();
                            row.ConstantItem(250).Background(ColorGrisFondo).Padding(10).Text(x =>
                            {
                                x.Span("DELEGADO: ").Bold().FontColor(ColorVino);
                                x.Span(comunidad.Value.Delegado ?? "SIN ASIGNAR").Bold();
                            });
                        });

                        // TÍTULO DE LA TABLA
                        col.Item().PaddingBottom(10).Text("Detalle de Apoyos").FontSize(11).Bold().FontColor(ColorVino);

                        // TABLA DE APOYOS
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c => {
                                c.RelativeColumn(1.5f);
                                c.RelativeColumn(2.5f);
                                c.RelativeColumn(1.5f);
                                c.RelativeColumn(2.5f);
                            });

                            t.Header(h =>
                            {
                                CeldaHeader(h, "Folio");
                                CeldaHeader(h, "Fondo");
                                CeldaHeader(h, "Fecha");
                                CeldaHeader(h, "Monto", true);
                            });

                            foreach (var a in apoyos)
                            {
                                t.Cell().Element(CeldaEstilo).Text(a.Folio).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).Text(a.Fondo).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).Text(a.FechaApoyo.ToString("dd/MM/yyyy")).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).AlignRight().Text(FormatearMoneda(a.MontoOtorgado)).FontSize(8.5f).Bold().FontColor(ColorVino);
                            }

                            // FILA DE TOTAL
                            t.Cell().ColumnSpan(3).Element(CeldaEstilo).AlignRight().Text("TOTAL:").Bold().FontColor(ColorVino);
                            t.Cell().Element(CeldaEstilo).AlignRight().Text(FormatearMoneda(totalMonto)).Bold().FontColor(ColorVino);
                        });
                    });

                    p.Footer().Element(GenerarPiePagina);
                });
            }).GeneratePdf();
        }

        // --- COMUNIDADES EXCEL ---
        public async Task<byte[]> ExportarComunidadesExcelAsync(FiltroReporteDto filtro)
        {
            var (desde, hasta) = CalcularRango(filtro);
            var comunidades = await _reportesRepository.ObtenerResumenPorComunidadAsync(
                desde, hasta, filtro.ComunidadIds, filtro.ApoyoIds);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Comunidades");

            EscribirEncabezados(hoja, "Comunidad", "Delegado", "Total Apoyos", "Monto Total");

            int fila = 2;
            foreach (var c in comunidades)
            {
                hoja.Cell(fila, 1).Value = c.Comunidad;
                hoja.Cell(fila, 2).Value = c.Delegado ?? "-";
                hoja.Cell(fila, 3).Value = c.TotalApoyos;
                hoja.Cell(fila, 4).Value = c.TotalDinero;
                hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";
                fila++;
            }

            return FinalizarLibro(hoja, libro);
        }

        // --- FONDOS EXCEL ---
        public async Task<byte[]> ExportarFondosExcelAsync(FiltroReporteDto filtro)
        {
            var (desde, hasta) = CalcularRango(filtro);
            var fondos = await _reportesRepository.ObtenerResumenPorFondoAsync(desde, hasta, filtro.ApoyoIds);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Fondos");

            EscribirEncabezados(hoja, "Fondo", "Total Apoyos", "Monto Total");

            int fila = 2;
            foreach (var f in fondos)
            {
                hoja.Cell(fila, 1).Value = f.Nombre;
                hoja.Cell(fila, 2).Value = f.TotalApoyos;
                hoja.Cell(fila, 3).Value = f.TotalDinero;
                hoja.Cell(fila, 3).Style.NumberFormat.Format = "$#,##0.00";
                fila++;
            }

            return FinalizarLibro(hoja, libro);
        }

        // --- APOYOS EXCEL ---
        public async Task<byte[]> ExportarApoyosExcelAsync(FiltroReporteDto filtro)
        {
            var (desde, hasta) = CalcularRango(filtro);
            var apoyos = await _reportesRepository.ObtenerTodosLosApoyosAsync(
                desde, hasta, filtro.ComunidadIds, filtro.ApoyoIds);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Apoyos");

            EscribirEncabezados(hoja, "Folio", "Comunidad", "Fondo", "Fecha", "Monto", "Estado");

            int fila = 2;
            foreach (var a in apoyos)
            {
                hoja.Cell(fila, 1).Value = a.Folio;
                hoja.Cell(fila, 2).Value = a.Comunidad;
                hoja.Cell(fila, 3).Value = a.Fondo;
                hoja.Cell(fila, 4).Value = a.FechaApoyo.DateTime;
                hoja.Cell(fila, 4).Style.DateFormat.Format = "dd/MM/yyyy";
                hoja.Cell(fila, 5).Value = a.MontoOtorgado;
                hoja.Cell(fila, 5).Style.NumberFormat.Format = "$#,##0.00";
                hoja.Cell(fila, 6).Value = a.Estado;
                fila++;
            }

            return FinalizarLibro(hoja, libro);
        }

        // --- APOYOS POR COMUNIDAD EXCEL ---
        public async Task<byte[]> ExportarApoyosPorComunidadExcelAsync(Guid comunidadId, FiltroReporteDto filtro)
        {
            var comunidad = await _reportesRepository.ObtenerComunidadAsync(comunidadId);
            if (comunidad is null)
                throw new NotFoundException("Comunidad no encontrada");

            var (desde, hasta) = CalcularRango(filtro);
            var apoyos = await _reportesRepository.ObtenerApoyosDeComunidadAsync(
                comunidadId, desde, hasta, filtro.ApoyoIds);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Detalle Comunidad");

            // Título con el nombre de la comunidad
            hoja.Cell("A1").Value = $"Apoyos recibidos - {comunidad.Value.Comunidad}";
            hoja.Range("A1:E1").Merge();
            hoja.Cell("A1").Style.Font.SetBold().Font.SetFontSize(13)
                .Font.SetFontColor(XLColor.White);
            hoja.Cell("A1").Style.Fill.SetBackgroundColor(XLColor.FromHtml(ColorVino));

            hoja.Cell("A2").Value = $"Delegado: {comunidad.Value.Delegado ?? "-"}  |  Periodo: {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}";
            hoja.Range("A2:E2").Merge();
            hoja.Cell("A2").Style.Font.SetItalic();

            // Encabezados
            int filaHeader = 4;
            string[] encabezados = { "Folio", "Fondo", "Fecha", "Monto", "Estado" };
            for (int i = 0; i < encabezados.Length; i++)
            {
                var celda = hoja.Cell(filaHeader, i + 1);
                celda.Value = encabezados[i];
                celda.Style.Font.SetBold().Font.SetFontColor(XLColor.White);
                celda.Style.Fill.SetBackgroundColor(XLColor.FromHtml(ColorVino));
            }

            int fila = filaHeader + 1;
            foreach (var a in apoyos)
            {
                hoja.Cell(fila, 1).Value = a.Folio;
                hoja.Cell(fila, 2).Value = a.Fondo;
                hoja.Cell(fila, 3).Value = a.FechaApoyo.DateTime;
                hoja.Cell(fila, 3).Style.DateFormat.Format = "dd/MM/yyyy";
                hoja.Cell(fila, 4).Value = a.MontoOtorgado;
                hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";
                hoja.Cell(fila, 5).Value = a.Estado;
                fila++;
            }

            // Total
            hoja.Cell(fila, 1).Value = "TOTAL";
            hoja.Cell(fila, 1).Style.Font.SetBold();
            hoja.Cell(fila, 4).FormulaA1 = $"=SUM(D{filaHeader + 1}:D{fila - 1})";
            hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";
            hoja.Range(fila, 1, fila, 5).Style.Font.SetBold();

            hoja.Columns().AdjustToContents();
            hoja.SheetView.FreezeRows(filaHeader);

            using var stream = new MemoryStream();
            libro.SaveAs(stream);
            return stream.ToArray();
        }

        // --- HELPERS PRIVADOS ---
        private static void EscribirEncabezados(IXLWorksheet hoja, params string[] encabezados)
        {
            for (int i = 0; i < encabezados.Length; i++)
            {
                var celda = hoja.Cell(1, i + 1);
                celda.Value = encabezados[i];
                celda.Style.Font.SetBold();
                celda.Style.Font.SetFontColor(XLColor.White);
                celda.Style.Fill.SetBackgroundColor(XLColor.FromHtml(ColorVino));
            }
        }

        private static byte[] FinalizarLibro(IXLWorksheet hoja, XLWorkbook libro)
        {
            hoja.Columns().AdjustToContents();
            hoja.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            libro.SaveAs(stream);
            return stream.ToArray();
        }

        private static (DateTimeOffset Desde, DateTimeOffset Hasta) CalcularRango(FiltroReporteDto filtro)
        {
            DateTimeOffset desde = filtro.AnioInicio.HasValue
                ? new DateTimeOffset(filtro.AnioInicio.Value, filtro.MesInicio ?? 1, 1, 0, 0, 0, TimeSpan.Zero)
                : new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

            DateTimeOffset hasta = filtro.AnioFin.HasValue
                ? new DateTimeOffset(filtro.AnioFin.Value, filtro.MesFin ?? 12, DateTime.DaysInMonth(filtro.AnioFin.Value, filtro.MesFin ?? 12), 23, 59, 59, TimeSpan.Zero)
                : DateTimeOffset.UtcNow;

            return (desde, hasta);
        }

        private byte[] GenerarPdfReporteAnual(ReporteAnualDto reporte)
        {
            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(45);
                    pagina.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial").FontColor(ColorGrisTexto));

                    pagina.Header().Element(c => GenerarCabecera(c, "Reporte Consolidado de Apoyos", reporte.Desde, reporte.Hasta));

                    pagina.Content().PaddingTop(20).Column(col =>
                    {
                        // SÍNTESIS EJECUTIVA CON PERIODO
                        col.Item().PaddingBottom(25).Row(row =>
                        {
                            row.AutoItem().Width(4).Background(ColorOro);
                            row.RelativeItem()
                               .Background(ColorGrisFondo)
                               .Padding(15)
                               .Text(texto =>
                               {
                                   texto.Span("SÍNTESIS EJECUTIVA: ").Bold().FontColor(ColorVino);
                                   texto.Span($"Durante el periodo comprendido del ");
                                   texto.Span(reporte.Desde.ToString("dd 'de' MMMM 'de' yyyy", CulturaMx)).Bold().FontColor(ColorVino);
                                   texto.Span($" al ");
                                   texto.Span(reporte.Hasta.ToString("dd 'de' MMMM 'de' yyyy", CulturaMx)).Bold().FontColor(ColorVino);
                                   texto.Span($", se documenta la gestión de recursos para ");
                                   texto.Span(reporte.TotalComunidades.ToString("N0")).Bold();
                                   texto.Span(" comunidades, con un total de ");
                                   texto.Span(reporte.TotalApoyos.ToString("N0")).Bold();
                                   texto.Span(" apoyos y una inversión de ");
                                   texto.Span(FormatearMoneda(reporte.TotalDinero)).Bold().FontColor(ColorVino);
                                   texto.Span(".");
                               });
                        });

                        col.Item().PaddingBottom(10).Text("Detalle por Circunscripción").FontSize(11).Bold().FontColor(ColorVino);
                        col.Item().Element(c => TablaComunidades(c, reporte.Comunidades));
                    });

                    pagina.Footer().Element(GenerarPiePagina);
                });
            }).GeneratePdf();
        }

        // --- COMPONENTES DE DISEÑO ---
        private void GenerarCabecera(IContainer container, string titulo, DateTimeOffset desde, DateTimeOffset hasta)
        {
            string rutaRelativa = string.IsNullOrWhiteSpace(_rutaLogo)
                ? "wwwroot/images/LogoPresidencia.png"
                : _rutaLogo;

            string rutaAbsoluta = Path.Combine(Directory.GetCurrentDirectory(), rutaRelativa);

            if (!File.Exists(rutaAbsoluta))
            {
                rutaAbsoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
            }

            if (!File.Exists(rutaAbsoluta))
            {
                string rutaIntento1 = Path.Combine(Directory.GetCurrentDirectory(), rutaRelativa);
                string rutaIntento2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);

                throw new Exception(
                    $"[ERROR DE LOGO]: El archivo no se encontró.\n" +
                    $"Valor en appsettings: '{_rutaLogo}'\n" +
                    $"Ruta intentada 1: '{rutaIntento1}'\n" +
                    $"Ruta intentada 2: '{rutaIntento2}'\n" +
                    $"Por favor, verifica en cuál de esas carpetas físicas te falta pegar la carpeta 'wwwroot'."
                );
            }

            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.ConstantItem(65).Height(65).AlignLeft().Image(rutaAbsoluta);
                    row.RelativeItem().PaddingLeft(15).AlignMiddle().Column(c =>
                    {
                        c.Item().Text("Tula de Allende Hidalgo").FontSize(13).Bold().FontColor(ColorVino).LetterSpacing(0.3f);
                        c.Item().Text("Presidencia Municipal 2024-2027").FontSize(9.5f).FontColor(ColorGrisTexto);
                        c.Item().PaddingTop(2).Text("Sistema de Apoyos Municipales").FontSize(9.5f).Italic().FontColor(ColorGrisTexto);
                    });
                    row.ConstantItem(130).AlignRight().AlignMiddle().Column(c =>
                    {
                        c.Item().AlignRight().Text("PERIODO").FontSize(7.5f).Bold().FontColor(ColorOro).LetterSpacing(0.5f);
                        c.Item().AlignRight().Text($"{desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}").FontSize(9.5f);
                    });
                });

                col.Item().PaddingTop(12).LineHorizontal(1.5f).LineColor(ColorVino);
                col.Item().PaddingTop(8).AlignCenter().Text(titulo.ToUpper()).FontSize(12).Bold().FontColor(ColorVino).LetterSpacing(0.5f);
            });
        }

        private static void TablaComunidades(IContainer contenedor, List<ReporteComunidadResumenDto> comunidades)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3f);
                    c.RelativeColumn(4f);
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(2.5f);
                });

                tabla.Header(header =>
                {
                    CeldaHeader(header, "Circunscripción");
                    CeldaHeader(header, "Gestión Local / Delegado");
                    CeldaHeader(header, "Movimientos", alinearDerecha: true);
                    CeldaHeader(header, "Total Otorgado", alinearDerecha: true);
                });

                foreach (var c in comunidades)
                {
                    tabla.Cell().Element(CeldaEstilo).Text(c.Comunidad).FontSize(9);
                    tabla.Cell().Element(CeldaEstilo).Text(c.Delegado ?? "-").FontSize(9);
                    tabla.Cell().Element(CeldaEstilo).AlignRight().Text(c.TotalApoyos.ToString("N0")).FontSize(9);
                    tabla.Cell().Element(CeldaEstilo).AlignRight().Text(FormatearMoneda(c.TotalDinero)).FontSize(9).Bold().FontColor(ColorVino);
                }
            });
        }

        private static void CeldaHeader(TableCellDescriptor header, string texto, bool alinearDerecha = false)
        {
            var contenedor = header.Cell()
                .Background(ColorFondoCabecera)
                .PaddingHorizontal(5)
                .PaddingVertical(10)
                .BorderBottom(1.5f)
                .BorderColor(ColorVino);

            if (alinearDerecha) contenedor = contenedor.AlignRight();
            else contenedor = contenedor.AlignLeft();

            contenedor.Text(texto).FontSize(9f).Bold().FontColor(ColorVino);
        }

        private static IContainer CeldaEstilo(IContainer container)
        {
            return container.BorderBottom(0.5f).BorderColor(ColorLinea).PaddingHorizontal(5).PaddingVertical(8).AlignMiddle();
        }

        private static void GenerarPiePagina(IContainer container)
        {
            container.PaddingTop(10).BorderTop(0.5f).BorderColor(ColorLinea).Row(row =>
            {
                row.RelativeItem().Text("Presidencia de Tula de Allende Hidalgo 2024-2027").FontSize(7.5f).Italic();
                row.ConstantItem(100).AlignRight().Text(x =>
                {
                    x.Span("Página ").FontSize(7.5f);
                    x.CurrentPageNumber().FontSize(7.5f);
                });
            });
        }

        private static string FormatearMoneda(decimal monto) => monto.ToString("C2", CulturaMx);
    }
}