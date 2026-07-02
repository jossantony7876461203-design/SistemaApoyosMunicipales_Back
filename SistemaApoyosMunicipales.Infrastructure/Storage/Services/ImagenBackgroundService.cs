using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using System;
using System.Collections.Generic;
using System.Text;

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
                if (_queue.Desencolar(out var tarea) && tarea is not null)
                {
                    await ProcesarTareaAsync(tarea);
                }
                else
                {
                    // Cola vacía — esperar 500ms antes de revisar de nuevo
                    await Task.Delay(500, stoppingToken);
                }
            }
        }

        private async Task ProcesarTareaAsync(ImagenTarea tarea)
        {
            // Marcar como procesando
            _queue.ActualizarEstado(tarea.Id, t =>
            {
                t.Estado = EstadoTarea.Procesando;
            });

            try
            {
                // Crear scope porque ICloudinaryService y repositorios
                // son Scoped y el BackgroundService es Singleton
                using var scope = _scopeFactory.CreateScope();

                var cloudinary = scope.ServiceProvider
                    .GetRequiredService<ICloudinaryService>();

                var comunidadRepo = scope.ServiceProvider
                    .GetRequiredService<IComunidadRepository>();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                // 1. Subir imagen a Cloudinary
                using var stream = new MemoryStream(tarea.Bytes);

                var resultado = await cloudinary.SubirImagenAsync(
                    stream,
                    tarea.NombreArchivo,
                    tarea.Carpeta);

                // 2. Actualizar comunidad en BD
                var comunidad = await comunidadRepo
                    .ObtenerPorIdParaEditarAsync(tarea.EntidadId);

                if (comunidad is not null)
                {
                    // Si tenía imagen anterior, eliminarla de Cloudinary
                    if (!string.IsNullOrEmpty(comunidad.DelegadoInePubId))
                        await cloudinary.EliminarImagenAsync(
                            comunidad.DelegadoInePubId);

                    comunidad.DelegadoIneUrl = resultado.Url;
                    comunidad.DelegadoInePubId = resultado.PublicId;
                    comunidad.UpdatedAt = DateTimeOffset.UtcNow;

                    await unitOfWork.SaveChangesAsync();
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
