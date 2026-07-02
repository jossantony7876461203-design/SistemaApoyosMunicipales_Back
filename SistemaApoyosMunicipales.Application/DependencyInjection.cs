using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaApoyosMunicipales.Application.Interfaces;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Services;
using SistemaApoyosMunicipales.Application.Services.Auth;
using SistemaApoyosMunicipales.Application.Validators.Rol;
using SistemaApoyosMunicipales.Infrastructure.Auth.Services;

namespace SistemaApoyosMunicipales.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // =========================
            // SERVICES
            // =========================
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IComunidadService, ComunidadService>();
            services.AddScoped<IUsuarioService, UsuariosService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<ISubRolService, SubRolService>();
           services.AddScoped<IPermisoService, PermisoService>();
            services.AddScoped<IRolPermisoService, RolPermisoService>();
            services.AddScoped<ISubRolPermisoService, SubRolPermisoService>();
            services.AddScoped<IApoyoService, ApoyoService>();
            services.AddScoped<IRegistroApoyoService, RegistroApoyoService>();
  

            // =========================
            // FLUENT VALIDATION
            // =========================
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AuthService>();

            return services;
        }
    }
}