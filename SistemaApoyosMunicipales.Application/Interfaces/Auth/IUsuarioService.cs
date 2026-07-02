using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IUsuarioService
    {
        Task<PaginatedResult<ObtenerUsuariosRolDto>> ObtenerActivosAsync(
            PaginationRequest pagination);

        Task<PaginatedResult<ObtenerUsuariosRolDto>> ObtenerInactivosAsync(
            PaginationRequest pagination);

        Task<UsuarioDetalleDto> ObtenerPorIdAsync(Guid id);

        Task CambiarEstatusAsync(
            Guid usuarioId,
            CambiarEstatusUsuarioDto dto);

        Task ActualizarAsync(
            Guid usuarioId,
            ActualizarUsuarioDto dto);

        Task AsignarRolAsync(          // ✅ descomentado
            Guid usuarioId,
            AsignarRolUsuarioDto dto);
    }
}