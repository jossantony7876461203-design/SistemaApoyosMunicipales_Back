using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;

namespace SistemaApoyosMunicipales.Application.Validators.Comunidad;

public sealed class CrearComunidadValidator
    : AbstractValidator<CrearComunidadDto>
{
    public CrearComunidadValidator()
    {
        RuleFor(x => x.ClaveInterna)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CodigoPostal)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Delegado)
            .MaximumLength(200);

        RuleFor(x => x.TelefonoDelegado)
            .MaximumLength(20);
    }
}