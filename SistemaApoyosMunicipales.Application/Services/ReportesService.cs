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
        private const string ColorFondoCabecera = "#FDFBF7"; // Fondo muy sutil y elegante para la tabla
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
                        col.Item().PaddingBottom(25).Row(row =>
                        {
                            row.AutoItem().Width(4).Background(ColorOro);
                            row.RelativeItem()
                               .Background(ColorGrisFondo)
                               .Padding(15) // Aumentamos padding para que el texto respire
                               .Text(texto =>
                               {
                                   texto.Span("SÍNTESIS EJECUTIVA: ").Bold().FontColor(ColorVino);
                                   texto.Span("Se documenta la gestión de recursos para ");
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

        // --- COMPONENTES ---

        private void GenerarCabecera(IContainer container, string titulo, DateTimeOffset desde, DateTimeOffset hasta)
        {
            // 1. Si por alguna razón el appsettings viene vacío, le ponemos el valor por defecto
            string rutaRelativa = string.IsNullOrWhiteSpace(_rutaLogo)
                ? "wwwroot/images/LogoPresidencia.png"
                : _rutaLogo;

            // 2. Intentamos armar la ruta usando el directorio de ejecución actual (Raíz de la API)
            string rutaAbsoluta = Path.Combine(Directory.GetCurrentDirectory(), rutaRelativa);

            // 3. Si no existe ahí (a veces pasa en modo Debug), intentamos en la carpeta Bin de compilación
            if (!File.Exists(rutaAbsoluta))
            {
                rutaAbsoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
            }

            // 4. NUEVO DEBUG REFORZADO: Si sigue sin existir, ahora sí te dirá las rutas reales donde buscó
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

            // 5. El diseño de tu cabecera se mantiene intacto y hermoso
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // IZQUIERDA: Logo (Ya garantizado que existe gracias al IF de arriba)
                    row.ConstantItem(65).Height(65).AlignLeft().Image(rutaAbsoluta);

                    // CENTRO: Títulos institucionales
                    row.RelativeItem().PaddingLeft(15).AlignMiddle().Column(c =>
                    {
                        c.Item().Text("Tula de Allende Hidalgo").FontSize(13).Bold().FontColor(ColorVino).LetterSpacing(0.3f);
                        c.Item().Text("Presidencia Municipal 2024-2027").FontSize(9.5f).FontColor(ColorGrisTexto);
                        c.Item().PaddingTop(2).Text("Sistema de Apoyos Municipales").FontSize(9.5f).Italic().FontColor(ColorGrisTexto);
                    });

                    // DERECHA: Periodo
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
                    c.RelativeColumn(3f);   // Más espacio para circunscripción
                    c.RelativeColumn(4f);   // Mucho más espacio para nombres de delegados largos
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
            // Se añade un fondo muy sutil y padding lateral para el "leve diseño"
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
            // Agregado un poco de PaddingHorizontal para que el texto no toque la línea imaginaria de la columna
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

        // REPORTE POR COMUNIDAD
        public async Task<byte[]> GenerarReportePorComunidadAsync(Guid comunidadId, FiltroReporteDto filtro)
        {
            var comunidad = await _reportesRepository.ObtenerComunidadAsync(comunidadId);
            if (comunidad is null) throw new NotFoundException("Comunidad no encontrada");

            var (desde, hasta) = CalcularRango(filtro);
            var apoyos = await _reportesRepository.ObtenerApoyosDeComunidadAsync(comunidadId, desde, hasta, filtro.ApoyoIds);

            return Document.Create(doc =>
            {
                doc.Page(p =>
                {
                    p.Size(PageSizes.A4); p.Margin(45);
                    p.Header().Element(c => GenerarCabecera(c, $"Informe: {comunidad.Value.Comunidad}", desde, hasta));
                    p.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().PaddingBottom(15).Text($"Relación de apoyos para {comunidad.Value.Comunidad}").Bold().FontColor(ColorVino);
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c => {
                                c.RelativeColumn(1.5f); c.RelativeColumn(3f); c.RelativeColumn(1.5f); c.RelativeColumn(2.5f);
                            });
                            t.Header(h => {
                                CeldaHeader(h, "Folio"); CeldaHeader(h, "Fondo"); CeldaHeader(h, "Fecha"); CeldaHeader(h, "Monto", true);
                            });
                            foreach (var a in apoyos)
                            {
                                t.Cell().Element(CeldaEstilo).Text(a.Folio).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).Text(a.Fondo).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).Text(a.FechaApoyo.ToString("dd/MM/yyyy")).FontSize(8.5f);
                                t.Cell().Element(CeldaEstilo).AlignRight().Text(FormatearMoneda(a.MontoOtorgado)).FontSize(8.5f).Bold().FontColor(ColorVino);
                            }
                        });
                    });
                    p.Footer().Element(GenerarPiePagina);
                });
            }).GeneratePdf();
        }
    }
}