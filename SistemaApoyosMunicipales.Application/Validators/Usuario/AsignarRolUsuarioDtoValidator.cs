using FluentValidation;
using SistemaApoyosMunicipales.Application.DTOs.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Validators.Usuario
{

    public sealed class AsignarRolUsuarioDtoValidator
        : AbstractValidator<AsignarRolUsuarioDto>
    {
        public AsignarRolUsuarioDtoValidator()
        {
            RuleFor(x => x.RolId)
                .NotEmpty()
                .WithMessage("Debe seleccionar un rol.");
        }
    }
}
