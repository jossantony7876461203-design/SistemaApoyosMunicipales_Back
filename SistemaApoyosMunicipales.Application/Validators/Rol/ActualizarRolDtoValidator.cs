using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Rol;

namespace SistemaApoyosMunicipales.Application.Validators.Rol
{
    public sealed class ActualizarRolDtoValidator : AbstractValidator<ActualizarRolDto>
    {
        public ActualizarRolDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotNull()
                    .WithMessage("El nombre del rol es obligatorio.")
                .NotEmpty()
                    .WithMessage("El nombre del rol no puede estar vacío.")
                .MinimumLength(3)
                    .WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(100)
                    .WithMessage("El nombre no puede exceder 100 caracteres.")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
                    .WithMessage("El nombre solo puede contener letras y espacios.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(250)
                    .WithMessage("La descripción no puede exceder 250 caracteres.")
                .Matches(@"^[a-zA-ZÀ-ÿ0-9\s.,;:()\-]+$")
                    .WithMessage("La descripción contiene caracteres no permitidos.")
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));
        }
    }
}