using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Comunidad;

namespace SistemaApoyosMunicipales.Application.Validators.Comunidad;

public sealed class ActualizarComunidadValidator
    : AbstractValidator<ActualizarComunidadDto>
{
    public ActualizarComunidadValidator()
    {
        RuleFor(x => x.ClaveInterna)
            .MaximumLength(50)
            .When(x => x.ClaveInterna != null);

        RuleFor(x => x.Nombre)
            .MaximumLength(200)
            .When(x => x.Nombre != null);

        RuleFor(x => x.CodigoPostal)
            .MaximumLength(10)
            .When(x => x.CodigoPostal != null);

        RuleFor(x => x.Delegado)
            .MaximumLength(200)
            .When(x => x.Delegado != null);

        RuleFor(x => x.TelefonoDelegado)
            .MaximumLength(20)
            .When(x => x.TelefonoDelegado != null);
    }
}