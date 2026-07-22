using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ImagenBackgroundService : BackgroundService
    {
        private readonly IImagenQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ImagenBackgroundService> _logger;

        public ImagenBackgroundService(
            IImagenQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ImagenBackgroundService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ImagenBackgroundService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try // <-- Aquí es donde te pide el try
                {
                    if (_queue.Desencolar(out var tarea) && tarea is not null)
                    {
                        await ProcesarTareaAsync(tarea, stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(500, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado en el ciclo principal del BackgroundService.");
                }
            }

            _logger.LogInformation("ImagenBackgroundService detenido correctamente.");
        }

        private async Task ProcesarTareaAsync(ImagenTarea tarea, CancellationToken stoppingToken)
        {
            // Marcar como procesando
            _queue.ActualizarEstado(tarea.Id, t =>
            {
                t.Estado = EstadoTarea.Procesando;
            });

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cloudinary = scope.ServiceProvider
                    .GetRequiredService<ICloudinaryService>();

                var comunidadRepo = scope.ServiceProvider
                    .GetRequiredService<IComunidadRepository>();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                // 1. Subir imagen a Cloudinary (Pásale el token si tu interfaz lo soporta)
                using var stream = new MemoryStream(tarea.Bytes);

                var resultado = await cloudinary.SubirImagenAsync(
                    stream,
                    tarea.NombreArchivo,
                    tarea.Carpeta);

                // 2. Actualizar comunidad en BD (Pásale el stoppingToken si tus repositorios lo aceptan)
                var comunidad = await comunidadRepo
                    .ObtenerPorIdParaEditarAsync(tarea.EntidadId);

                if (comunidad is not null)
                {
                    if (!string.IsNullOrEmpty(comunidad.DelegadoInePubId))
                        await cloudinary.EliminarImagenAsync(comunidad.DelegadoInePubId);

                    comunidad.DelegadoIneUrl = resultado.Url;
                    comunidad.DelegadoInePubId = resultado.PublicId;
                    comunidad.UpdatedAt = DateTimeOffset.UtcNow;

                    await unitOfWork.SaveChangesAsync(stoppingToken);
                }

                // 3. Marcar como completada
                _queue.ActualizarEstado(tarea.Id, t =>
                {
                    t.Estado = EstadoTarea.Completada;
                    t.UrlResultado = resultado.Url;
                    t.PublicIdResultado = resultado.PublicId;
                });

                _logger.LogInformation(
                    "Tarea {TareaId} completada. URL: {Url}",
                    tarea.Id, resultado.Url);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Si se canceló porque se apagó el servidor, re-lanzamos para que lo catchee el bucle principal
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error procesando tarea {TareaId}", tarea.Id);

                _queue.ActualizarEstado(tarea.Id, t =>
                {
                    t.Estado = EstadoTarea.Fallida;
                    t.Error = ex.Message;
                });
            }
        }
    }
}