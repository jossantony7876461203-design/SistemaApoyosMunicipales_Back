using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration; // <-- AGREGADO: Necesario para IConfiguration
using Microsoft.Extensions.DependencyInjection;
using SistemaApoyosMunicipales.Application.Interfaces;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Services;
using SistemaApoyosMunicipales.Application.Services.Auth;
using SistemaApoyosMunicipales.Application.Settings; // <-- AGREGADO: Necesario para ReportesSettings
using SistemaApoyosMunicipales.Application.Validators.Rol;
using SistemaApoyosMunicipales.Infrastructure.Auth.Services;

namespace SistemaApoyosMunicipales.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration) // <-- AGREGADO: Pasamos configuration como parámetro
        {
            // =========================
            // SETTINGS
            // =========================
            // <-- AGREGADO: Aquí mapeas el JSON a tu clase de C#
            services.Configure<ReportesSettings>(configuration.GetSection("ReportesSettings"));

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
            services.AddScoped<IEstadoSolicitudService, EstadoSolicitudService>();

            // Asegúrate de tener registrado también el ReportesService si no lo tenías:
            services.AddScoped<IReportesService, ReportesService>();

            // =========================
            // FLUENT VALIDATION
            // =========================
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AuthService>();

            return services;
        }
    }
}