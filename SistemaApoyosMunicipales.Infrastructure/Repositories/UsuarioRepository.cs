using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence.Extensions;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Correo == correo);
        }

        public async Task CrearAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .FindAsync(id);
        }

        public async Task<IEnumerable<UsuarioPermisoDto>> ObtenerPermisosAsync(Guid usuarioId)
        {
            return await _context.Database
                .SqlQueryRaw<UsuarioPermisoDto>(@"
            SELECT
                modulo         AS ""Modulo"",
                permiso        AS ""Permiso"",
                puede_crear    AS ""PuedeCrear"",
                puede_leer     AS ""PuedeLeer"",
                puede_editar   AS ""PuedeEditar"",
                puede_eliminar AS ""PuedeEliminar""
            FROM v_usuario_permisos
            WHERE usuario_id = @usuarioId",
                    new Npgsql.NpgsqlParameter("usuarioId", usuarioId))
                .ToListAsync();
        }

        public async Task ActualizarUltimoAccesoAsync(Guid usuarioId)
        {
            await _context.Usuarios
                .Where(u => u.Id == usuarioId)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(x => x.UltimoAcceso, DateTime.UtcNow)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
        }


        public async Task<Usuario?> ObtenerConRolAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.SubRol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PaginatedResult<Usuario>> ObtenerTodosActivosAsync(
    PaginationRequest pagination,
    bool activos = true)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Include(x => x.Rol)
                .Include(x => x.SubRol)
                .Where(x => x.Activo == activos)
                .OrderBy(x => x.Nombre)
                .PaginateAsync(
                    pagination.PageNumber,
                    pagination.PageSize);
        }



        public async Task AsignarRolAsync(Guid usuarioId, Guid rolId, Guid? subRolId = null)
        {
            await _context.Usuarios
                .Where(u => u.Id == usuarioId)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(x => x.RolId, rolId)
                    .SetProperty(x => x.SubRolId, subRolId)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
        }
    }
}