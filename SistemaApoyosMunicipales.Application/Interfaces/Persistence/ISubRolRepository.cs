using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Repositories
{
    public interface ISubRolRepository
    {
        Task CrearAsync(SubRol subRol);

        Task<SubRol?> ObtenerPorIdAsync(Guid id);

        Task<bool> ExisteAsync(
            Guid rolId,
            string nombre);

        Task ActualizarAsync(SubRol subRol);

        Task CambiarEstatusAsync(
            Guid id,
            bool activo);

        Task<PaginatedResult<SubRol>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true);

        Task<IEnumerable<SubRol>> ObtenerPorRolAsync(
            Guid rolId);
    }
}
