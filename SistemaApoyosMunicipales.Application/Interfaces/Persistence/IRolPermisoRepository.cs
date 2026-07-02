using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IRolPermisoRepository
    {
        Task<bool> ExisteAsync(Guid rolId, Guid permisoId);

        Task AsignarAsync(RolPermiso rolPermiso);

        Task QuitarAsync(Guid rolId, Guid permisoId);

        Task<IEnumerable<RolPermiso>> ObtenerPorRolAsync(
            Guid rolId,
            bool soloAsignados = true);
    }
}
