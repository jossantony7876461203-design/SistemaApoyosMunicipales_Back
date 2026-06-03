using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IRolRepository
    {
        Task<bool> ExistePorNombreAsync(string nombre);

        Task CrearAsync(Rol rol);

        Task<Rol?> ObtenerPorIdAsync(Guid id);

        Task<IEnumerable<Rol>> ObtenerTodosAsync();

        Task<IEnumerable<Rol>> ObtenerActivosAsync();
    }


}