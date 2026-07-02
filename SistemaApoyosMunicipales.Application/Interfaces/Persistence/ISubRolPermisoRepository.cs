using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface ISubRolPermisoRepository
    {
        Task<bool> ExisteAsync(Guid subRolId, Guid permisoId);

        Task AsignarAsync(SubRolPermiso subRolPermiso);

        Task ActualizarAsync(SubRolPermiso subRolPermiso);

        Task QuitarAsync(Guid subRolId, Guid permisoId);

        Task QuitarTodosAsync(Guid subRolId);

        Task<SubRolPermiso?> ObtenerUnoAsync(Guid subRolId, Guid permisoId);

        Task<IEnumerable<SubRolPermiso>> ObtenerPorSubRolAsync(Guid subRolId);
    }
}
