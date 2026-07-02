using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Usuario
{
    public sealed class ActualizarUsuarioDtoValidator
        : AbstractValidator<ActualizarUsuarioDto>
    {
        public ActualizarUsuarioDtoValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.Nombre) ||
                    !string.IsNullOrWhiteSpace(x.Correo))
                .WithMessage(
                    "Debe enviar al menos nombre o correo.");

            When(x => !string.IsNullOrWhiteSpace(x.Nombre), () =>
            {
                RuleFor(x => x.Nombre)
                    .MaximumLength(200);
            });

            When(x => !string.IsNullOrWhiteSpace(x.Correo), () =>
            {
                RuleFor(x => x.Correo)
                    .EmailAddress()
                    .MaximumLength(255);
            });
        }
    }
}
