using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using SistemaApoyosMunicipales.Application.Validators.Comunidad;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteCorreoAsync(
            string correo);

        Task CrearAsync(
            Usuario usuario);

        Task<Usuario?> ObtenerPorCorreoAsync(
            string correo);

        Task<Usuario?> ObtenerPorIdAsync(Guid id);

        Task<IEnumerable<UsuarioPermisoDto>> ObtenerPermisosAsync(
            Guid usuarioId);

        Task ActualizarUltimoAccesoAsync(Guid usuarioId);

        Task<Usuario?> ObtenerConRolAsync(Guid id);

        Task<PaginatedResult<Usuario>> ObtenerTodosActivosAsync(
            PaginationRequest pagination,
            bool activos = true);

        Task AsignarRolAsync(Guid usuarioId, Guid rolId, Guid? subRolId = null);
        Task CambiarEstatusAsync(
    Guid usuarioId,
    bool activo);


    }
}