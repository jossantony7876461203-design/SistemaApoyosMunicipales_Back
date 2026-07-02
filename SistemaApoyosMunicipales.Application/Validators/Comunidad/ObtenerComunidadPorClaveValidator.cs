using FluentValidation;

namespace SistemaApoyosMunicipales.Application.Validators.Comunidad;

public sealed class ObtenerComunidadPorClaveValidator
    : AbstractValidator<string>
{
    public ObtenerComunidadPorClaveValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(100);
    }
}
