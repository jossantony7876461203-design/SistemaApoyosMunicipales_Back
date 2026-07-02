using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Auth;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Exceptions;
using SistemaApoyosMunicipales.Infrastructure.Repositories;

namespace SistemaApoyosMunicipales.Application.Services;

public sealed class UsuariosService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRolRepository _rolRepository;
    private readonly ISubRolRepository _subRolRepository;  

    public UsuariosService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IRolRepository rolRepository,
        ISubRolRepository subRolRepository)            
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _rolRepository = rolRepository;
        _subRolRepository = subRolRepository;          
    }

    // =========================================================
    // 1. OBTENER ACTIVOS
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

    // =========================================================
    // 2. OBTENER INACTIVOS
    // =========================================================
    public async Task<PaginatedResult<ObtenerUsuariosRolDto>> ObtenerInactivosAsync(
        PaginationRequest pagination)
    {
        var resultado =
            await _usuarioRepository.ObtenerTodosActivosAsync(pagination, false);

        return new PaginatedResult<ObtenerUsuariosRolDto>
        {
            Items = resultado.Items.Select(x => new ObtenerUsuariosRolDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Correo = x.Correo,
                Rol = x.Rol?.Nombre,
                SubRol = x.SubRol?.Nombre,
                Activo = x.Activo
            }).ToList(),

            PageNumber = resultado.PageNumber,
            PageSize = resultado.PageSize,
            TotalRecords = resultado.TotalRecords,
            TotalPages = resultado.TotalPages,
            HasPreviousPage = resultado.HasPreviousPage,
            HasNextPage = resultado.HasNextPage
        };
    }

    // =========================================================
    // 3. OBTENER POR ID
    // =========================================================
    public async Task<UsuarioDetalleDto> ObtenerPorIdAsync(Guid id)
    {
        var usuario = await _usuarioRepository.ObtenerConRolAsync(id);

        if (usuario is null)
            throw new NotFoundException("El usuario no existe.");

        return new UsuarioDetalleDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            CorreoVerificado = usuario.CorreoVerificado,
            UltimoAcceso = usuario.UltimoAcceso,
            Rol = usuario.Rol?.Nombre,
            SubRol = usuario.SubRol?.Nombre
        };
    }

    // =========================================================
    // 4. CAMBIAR ESTATUS
    // =========================================================
    public async Task CambiarEstatusAsync(
        Guid usuarioId,
        CambiarEstatusUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

        if (usuario is null)
            throw new NotFoundException("El usuario no existe.");

        await _usuarioRepository.CambiarEstatusAsync(usuarioId, dto.Activo);
        await _unitOfWork.SaveChangesAsync();
    }

    // =========================================================
    // 5. ACTUALIZAR
    // =========================================================
    public async Task ActualizarAsync(
        Guid usuarioId,
        ActualizarUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

        if (usuario is null)
            throw new NotFoundException("El usuario no existe.");

        if (!string.IsNullOrWhiteSpace(dto.Correo))
        {
            var correoNormalizado = dto.Correo.Trim();

            if (!string.Equals(
                    usuario.Correo,
                    correoNormalizado,
                    StringComparison.OrdinalIgnoreCase))
            {
                var existe =
                    await _usuarioRepository.ExisteCorreoAsync(correoNormalizado);

                if (existe)
                    throw new ValidationException("El correo ya se encuentra registrado.");

                usuario.Correo = correoNormalizado;
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.Nombre))
            usuario.Nombre = dto.Nombre.Trim();

        await _unitOfWork.SaveChangesAsync();
    }

    // =========================================================
    // 6. ASIGNAR ROL
    // =========================================================
    public async Task AsignarRolAsync(
        Guid usuarioId,
        AsignarRolUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

        if (usuario is null)
            throw new NotFoundException("El usuario no existe.");

        var rol = await _rolRepository.ObtenerPorIdAsync(dto.RolId);

        if (rol is null)
            throw new NotFoundException("El rol no existe.");

        if (!rol.Activo)
            throw new ValidationException("El rol se encuentra inactivo.");

        if (dto.SubRolId.HasValue)
        {
            var subRol =
                await _subRolRepository.ObtenerPorIdAsync(dto.SubRolId.Value);

            if (subRol is null)
                throw new NotFoundException("El subrol no existe.");

            if (!subRol.Activo)
                throw new ValidationException("El subrol se encuentra inactivo.");

            if (subRol.RolId != dto.RolId)
                throw new ValidationException(
                    "El subrol no pertenece al rol seleccionado.");
        }

        await _usuarioRepository.AsignarRolAsync(
            usuarioId,
            dto.RolId,
            dto.SubRolId);

        await _unitOfWork.SaveChangesAsync();
    }



}