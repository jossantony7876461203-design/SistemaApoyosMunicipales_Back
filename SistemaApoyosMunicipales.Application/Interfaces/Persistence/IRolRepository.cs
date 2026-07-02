using SistemaApoyosMunicipales.Application.Common.Models;
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

        Task<PaginatedResult<Rol>> ObtenerTodosAsync(PaginationRequest request, bool activos);

        Task<IEnumerable<Rol>> ObtenerActivosAsync();
        Task ActualizarAsync(Rol rol);
        Task CambiarEstatusAsync(Guid id, bool activo);
 
    }


}