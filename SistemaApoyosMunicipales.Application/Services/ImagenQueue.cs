using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ImagenQueue : IImagenQueue
    {
        // Cola FIFO thread-safe
        private readonly ConcurrentQueue<ImagenTarea> _cola = new();

        // Historial de tareas para consultar estado
        private readonly ConcurrentDictionary<Guid, ImagenTarea> _historial = new();

        public void Encolar(ImagenTarea tarea)
        {
            _historial[tarea.Id] = tarea;
            _cola.Enqueue(tarea);
        }

        public bool Desencolar(out ImagenTarea? tarea)
        {
            return _cola.TryDequeue(out tarea);
        }

        public ImagenTarea? ObtenerEstado(Guid tareaId)
        {
            _historial.TryGetValue(tareaId, out var tarea);
            return tarea;
        }

        public void ActualizarEstado(Guid tareaId, Action<ImagenTarea> actualizar)
        {
            if (_historial.TryGetValue(tareaId, out var tarea))
                actualizar(tarea);
        }
    }
}
