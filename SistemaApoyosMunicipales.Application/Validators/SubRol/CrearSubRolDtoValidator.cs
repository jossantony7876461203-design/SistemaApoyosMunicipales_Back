using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.SubRol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.SubRol
{
    public sealed class CrearSubRolDtoValidator
         : AbstractValidator<CrearSubRolDto>
    {
        public CrearSubRolDtoValidator()
        {
            RuleFor(x => x.RolId)
                .NotEmpty()
                .WithMessage("Debe seleccionar un rol.");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio.")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion))
                .WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }
}
