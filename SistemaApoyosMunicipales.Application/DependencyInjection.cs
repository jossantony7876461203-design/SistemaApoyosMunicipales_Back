using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Services;
using SistemaApoyosMunicipales.Application.Services.Auth;

namespace SistemaApoyosMunicipales.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
        )
        {
            // =========================
            // SERVICES
            // =========================
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IComunidadService, ComunidadService>();
            services.AddScoped<IUsuarioService, UsuariosService>();
            services.AddScoped<IRolService, RolService>();
            // =========================
            // FLUENT VALIDATION
            // =========================
            services.AddValidatorsFromAssemblyContaining<AuthService>();

            return services;
        }
    }
}