using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IPermisoRepository
    {
        Task CrearAsync(Permiso permiso);

        Task<Permiso?> ObtenerPorIdAsync(Guid id);

        Task<bool> ExisteAsync(string codigo);

        Task<PaginatedResult<Permiso>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true);

        Task<IEnumerable<Permiso>> ObtenerPorModuloAsync(string modulo);

        Task CambiarEstatusAsync(Guid id, bool activo);
    }
}
