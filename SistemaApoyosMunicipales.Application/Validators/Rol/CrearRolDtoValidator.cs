using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Rol
{
    public sealed class CrearRolDtoValidator : AbstractValidator<CrearRolDto>
    {
        public CrearRolDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                .NotNull()
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("El nombre solo puede contener letras y espacios.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));
        }
    }
}
