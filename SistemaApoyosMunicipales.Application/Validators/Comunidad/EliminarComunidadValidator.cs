using FluentValidation;

namespace SistemaApoyosMunicipales.Application.Validators.Comunidad;

public sealed class EliminarComunidadValidator
    : AbstractValidator<Guid>
{
    public EliminarComunidadValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("El identificador es obligatorio.");
    }
}