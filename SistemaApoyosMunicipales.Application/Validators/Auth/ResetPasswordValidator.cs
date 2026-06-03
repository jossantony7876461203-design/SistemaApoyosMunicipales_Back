using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Auth;

namespace SistemaApoyosMunicipales.Application.Validators.Auth;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El código es requerido.")
            .Length(6).WithMessage("El código debe tener 6 dígitos.")
            .Matches("^[0-9]{6}$").WithMessage("El código solo debe contener números.");

        RuleFor(x => x.NuevoPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula.")
            .Matches("[a-z]").WithMessage("Debe contener al menos una minúscula.")
            .Matches("[0-9]").WithMessage("Debe contener al menos un número.");
    }
}