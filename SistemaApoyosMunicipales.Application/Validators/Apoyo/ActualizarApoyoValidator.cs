using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Apoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Apoyo
{
    public class ActualizarApoyoValidator : AbstractValidator<ActualizarApoyoDto>
    {
        public ActualizarApoyoValidator()
        {
            RuleFor(x => x.Nombre)
                .MaximumLength(200)
                    .WithMessage("El nombre no puede exceder 200 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Nombre));

            RuleFor(x => x.Descripcion)
                .MaximumLength(500)
                    .WithMessage("La descripción no puede exceder 500 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));

            RuleFor(x => x.MontoMaximo)
                .GreaterThan(0)
                    .WithMessage("El monto máximo debe ser mayor a 0.")
                .When(x => x.MontoMaximo.HasValue);
        }
    }
}
