using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Application.DTOs.Apoyos;
using SistemaApoyosMunicipales.Application.Interfaces;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Domain.Entities.Apoyo;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.Application.Services
{
    public sealed class ApoyoService : IApoyoService
    {
        private readonly IApoyoRepository _apoyoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApoyoService(
            IApoyoRepository apoyoRepository,
            IUnitOfWork unitOfWork)
        {
            _apoyoRepository = apoyoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CrearAsync(CrearApoyoDto dto)
        {
            var existe = await _apoyoRepository
                .ObtenerPorCodigoAsync(dto.Codigo.Trim().ToUpper());

            if (existe is not null)
                throw new ValidationException(
                    "El código del apoyo ya se encuentra registrado.");

            var apoyo = new Apoyo
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim().ToUpper(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                MontoMaximo = dto.MontoMaximo,
                RequiereValidacion = dto.RequiereValidacion,
                Activo = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _apoyoRepository.AgregarAsync(apoyo);
            await _unitOfWork.SaveChangesAsync();

            return apoyo.Id;
        }

        public async Task<ApoyoDto> ObtenerPorIdAsync(Guid id)
        {
            var apoyo = await _apoyoRepository
                .ObtenerPorIdAsync(id);

            if (apoyo is null)
                throw new NotFoundException("El apoyo no existe.");

            return new ApoyoDto
            {
                Id = apoyo.Id,
                Codigo = apoyo.Codigo,
                Nombre = apoyo.Nombre,
                Descripcion = apoyo.Descripcion,
                MontoMaximo = apoyo.MontoMaximo,
                RequiereValidacion = apoyo.RequiereValidacion,
                Activo = apoyo.Activo,
                CreatedAt = apoyo.CreatedAt,
                UpdatedAt = apoyo.UpdatedAt
            };
        }

        public async Task<PaginatedResult<ApoyoDto>> ObtenerActivosAsync(
            PaginationRequest pagination)
        {
            var resultado = await _apoyoRepository
                .ObtenerTodosAsync(pagination, true);

            return new PaginatedResult<ApoyoDto>
            {
                Items = resultado.Items
                    .Select(x => new ApoyoDto
                    {
                        Id = x.Id,
                        Codigo = x.Codigo,
                        Nombre = x.Nombre,
                        Descripcion = x.Descripcion,
                        MontoMaximo = x.MontoMaximo,
                        RequiereValidacion = x.RequiereValidacion,
                        Activo = x.Activo,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToList(),

                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        public async Task<PaginatedResult<ApoyoDto>> ObtenerInactivosAsync(
            PaginationRequest pagination)
        {
            var resultado = await _apoyoRepository
                .ObtenerTodosAsync(pagination, false);

            return new PaginatedResult<ApoyoDto>
            {
                Items = resultado.Items
                    .Select(x => new ApoyoDto
                    {
                        Id = x.Id,
                        Codigo = x.Codigo,
                        Nombre = x.Nombre,
                        Descripcion = x.Descripcion,
                        MontoMaximo = x.MontoMaximo,
                        RequiereValidacion = x.RequiereValidacion,
                        Activo = x.Activo,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToList(),

                PageNumber = resultado.PageNumber,
                PageSize = resultado.PageSize,
                TotalRecords = resultado.TotalRecords,
                TotalPages = resultado.TotalPages,
                HasPreviousPage = resultado.HasPreviousPage,
                HasNextPage = resultado.HasNextPage
            };
        }

        public async Task ActualizarAsync(
            Guid id,
            ActualizarApoyoDto dto)
        {
            var apoyo = await _apoyoRepository
                .ObtenerPorIdParaEditarAsync(id);

            if (apoyo is null)
                throw new NotFoundException("El apoyo no existe.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                apoyo.Nombre = dto.Nombre.Trim();

            if (dto.Descripcion is not null)
                apoyo.Descripcion = dto.Descripcion.Trim();

            if (dto.MontoMaximo.HasValue)
                apoyo.MontoMaximo = dto.MontoMaximo.Value;

            if (dto.RequiereValidacion.HasValue)
                apoyo.RequiereValidacion = dto.RequiereValidacion.Value;

            apoyo.UpdatedAt = DateTimeOffset.UtcNow;

            _apoyoRepository.Actualizar(apoyo);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarEstatusAsync(
    Guid id,
    CambiarEstatusApoyoDto dto)
        {
            var apoyo = await _apoyoRepository
                .ObtenerPorIdParaEditarAsync(id);  // encuentra activos e inactivos

            if (apoyo is null)
                throw new NotFoundException("El apoyo no existe.");

            apoyo.Activo = dto.Activo;
            apoyo.UpdatedAt = DateTimeOffset.UtcNow;
            // ✅ NO tocar DeletedAt

            _apoyoRepository.Actualizar(apoyo);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var apoyo = await _apoyoRepository
                .ObtenerPorIdAsync(id);

            if (apoyo is null)
                throw new NotFoundException("El apoyo no existe.");

            await _apoyoRepository.EliminarAsync(id);
        }
    }
}