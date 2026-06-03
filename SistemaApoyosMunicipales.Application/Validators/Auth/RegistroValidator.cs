using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Auth;

namespace SistemaApoyosMunicipales.Application.Validators.Auth;

public class RegistroValidator : AbstractValidator<RegistroDto>
{
    public RegistroValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(255).WithMessage("El correo no puede exceder 255 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula.")
            .Matches("[a-z]").WithMessage("Debe contener al menos una minúscula.")
            .Matches("[0-9]").WithMessage("Debe contener al menos un número.");
    }
}