using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.Application.Services;

public sealed class UsuariosService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuariosService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }




    // =========================================================
    // 1. OBTENER TODOS (ACTIVOS)
    // =========================================================

    public async Task<PaginatedResult<ObtenerUsuariosRolDto>> ObtenerActivosAsync(
    PaginationRequest pagination)
    {
        var resultado =
            await _usuarioRepository.ObtenerTodosActivosAsync(pagination, true);

        return new PaginatedResult<ObtenerUsuariosRolDto>
        {
            Items = resultado.Items.Select(u => new ObtenerUsuariosRolDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Activo = u.Activo,
                CorreoVerificado = u.CorreoVerificado,
                Rol = u.Rol?.Nombre,
                SubRol = u.SubRol?.Nombre,
                UltimoAcceso = u.UltimoAcceso
            }).ToList(),

            PageNumber = resultado.PageNumber,
            PageSize = resultado.PageSize,
            TotalRecords = resultado.TotalRecords,
            TotalPages = resultado.TotalPages,
            HasPreviousPage = resultado.HasPreviousPage,
            HasNextPage = resultado.HasNextPage
        };
    }




    //public async Task AsignarRolAsync(Guid usuarioId, Guid rolId, Guid? subRolId = null)
    //{
    //    1.Validar que el usuario exista
    //    var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

    //    if (usuario is null)
    //        throw new Exception("El usuario no existe.");

    //    2.Validar que el rol exista
    //    var rolExiste = await _context.Roles
    //        .AnyAsync(r => r.Id == rolId);

    //    if (!rolExiste)
    //        throw new Exception("El rol no existe.");

    //    3.Validación de dominio(REGLA IMPORTANTE)
    //    if (subRolId.HasValue)
    //    {
    //        var subRol = await _context.SubRoles
    //            .FirstOrDefaultAsync(sr => sr.Id == subRolId.Value);

    //        if (subRol is null)
    //            throw new Exception("El sub-rol no existe.");

    //        if (subRol.RolId != rolId)
    //            throw new Exception("El sub-rol no pertenece al rol seleccionado.");
    //    }

    //    4.Ejecutar actualización(repo)
    //    await _usuarioRepository.AsignarRolAsync(usuarioId, rolId, subRolId);
    //}











    //// =========================================================
    //// 2. OBTENER INACTIVOS
    //// =========================================================
    //public async Task<PaginatedResult<UsuarioDto>> ObtenerInactivosAsync(
    //    PaginationRequest pagination)
    //{
    //    var resultado =
    //        await _usuarioRepository.ObtenerTodosAsync(pagination, false);

    //    return new PaginatedResult<UsuarioDto>
    //    {
    //        Items = resultado.Items.Select(u => new UsuarioDto
    //        {
    //            Id = u.Id,
    //            Nombre = u.Nombre,
    //            Correo = u.Correo,
    //            Activo = u.Activo,
    //            CorreoVerificado = u.CorreoVerificado,
    //            Rol = u.Rol?.Nombre,
    //            SubRol = u.SubRol?.Nombre,
    //            UltimoAcceso = u.UltimoAcceso
    //        }).ToList(),

    //        PageNumber = resultado.PageNumber,
    //        PageSize = resultado.PageSize,
    //        TotalRecords = resultado.TotalRecords,
    //        TotalPages = resultado.TotalPages,
    //        HasPreviousPage = resultado.HasPreviousPage,
    //        HasNextPage = resultado.HasNextPage
    //    };
    //}

    //// =========================================================
    //// 3. OBTENER POR ID
    //// =========================================================
    //public async Task<UsuarioDto> ObtenerPorIdAsync(Guid id)
    //{
    //    var usuario = await _usuarioRepository.ObtenerPorIdConRolAsync(id);

    //    if (usuario is null)
    //        throw new NotFoundException("Usuario no encontrado.");

    //    return new UsuarioDto
    //    {
    //        Id = usuario.Id,
    //        Nombre = usuario.Nombre,
    //        Correo = usuario.Correo,
    //        Activo = usuario.Activo,
    //        CorreoVerificado = usuario.CorreoVerificado,
    //        Rol = usuario.Rol?.Nombre,
    //        SubRol = usuario.SubRol?.Nombre,
    //        UltimoAcceso = usuario.UltimoAcceso
    //    };
    //}

    //// =========================================================
    //// 4. PATCH UPDATE (UPDERT SOLO CAMPOS EDITABLES)
    //// =========================================================
    //public async Task ActualizarAsync(Guid id, ActualizarUsuarioDto dto)
    //{
    //    var usuario = await _usuarioRepository.ObtenerPorIdConRolAsync(id);

    //    if (usuario is null)
    //        throw new NotFoundException("Usuario no encontrado.");

    //    if (!string.IsNullOrWhiteSpace(dto.Nombre))
    //        usuario.Nombre = dto.Nombre.Trim();

    //    if (!string.IsNullOrWhiteSpace(dto.Correo))
    //        usuario.Correo = dto.Correo.Trim().ToLower();

    //    if (dto.RolId.HasValue)
    //        usuario.RolId = dto.RolId;

    //    if (dto.SubRolId.HasValue)
    //        usuario.SubRolId = dto.SubRolId;

    //    usuario.UpdatedAt = DateTimeOffset.UtcNow;

    //    _usuarioRepository.Actualizar(usuario);

    //    await _unitOfWork.SaveChangesAsync();
    //}

    //// =========================================================
    //// 5. CAMBIAR ESTATUS (UPSERT ACTIVO/INACTIVO)
    //// =========================================================
    //public async Task CambiarEstatusAsync(Guid id, CambiarEstatusUsuarioDto dto)
    //{
    //    var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);

    //    if (usuario is null)
    //        throw new NotFoundException("Usuario no encontrado.");

    //    usuario.Activo = dto.Activo;

    //    if (!dto.Activo)
    //        usuario.UltimoAcceso = usuario.UltimoAcceso; // opcional lógica extra

    //    usuario.UpdatedAt = DateTimeOffset.UtcNow;

    //    _usuarioRepository.Actualizar(usuario);

    //    await _unitOfWork.SaveChangesAsync();
    //}
}
