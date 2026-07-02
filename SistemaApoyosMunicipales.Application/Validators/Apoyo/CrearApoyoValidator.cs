using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Apoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Apoyo
{
    public class CrearApoyoValidator : AbstractValidator<CrearApoyoDto>
    {
        public CrearApoyoValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty()
                    .WithMessage("El código es requerido.")
                .MaximumLength(50)
                    .WithMessage("El código no puede exceder 50 caracteres.")
                .Matches("^[A-Za-z0-9\\-]+$")
                    .WithMessage("El código solo puede contener letras, números y guiones.");

            RuleFor(x => x.Nombre)
                .NotEmpty()
                    .WithMessage("El nombre es requerido.")
                .MaximumLength(200)
                    .WithMessage("El nombre no puede exceder 200 caracteres.");

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
