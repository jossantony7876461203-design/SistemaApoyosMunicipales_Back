using SistemaApoyosMunicipales.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Storage
{
    public interface IImagenQueue
    {
        void Encolar(ImagenTarea tarea);
        bool Desencolar(out ImagenTarea? tarea);
        ImagenTarea? ObtenerEstado(Guid tareaId);
        void ActualizarEstado(Guid tareaId, Action<ImagenTarea> actualizar);
    }
}
