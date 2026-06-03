using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Auth;

namespace SistemaApoyosMunicipales.Application.Validators.Auth;

public class RecuperarPasswordValidator : AbstractValidator<RecuperarPasswordDto>
{
    public RecuperarPasswordValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.");
    }
}