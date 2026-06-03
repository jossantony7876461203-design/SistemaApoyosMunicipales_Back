using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Infrastructure.Email.Services; 
using SistemaApoyosMunicipales.Infrastructure.Email.Settings;  
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using SistemaApoyosMunicipales.Infrastructure.Persistence.Repositories;
using SistemaApoyosMunicipales.Infrastructure.Persistence.UnitOfWork;
using SistemaApoyosMunicipales.Infrastructure.Repositories;
using SistemaApoyosMunicipales.Infrastructure.Security;
namespace SistemaApoyosMunicipales.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // =========================
            // DATABASE
            // =========================
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // =========================
            // REPOSITORIES & PERSISTENCE
            // =========================
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ILogAccesoRepository, LogAccesoRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ILogAccesoRepository, LogAccesoRepository>();
            services.AddScoped<IComunidadRepository, ComunidadRepository>();
            services.AddScoped<IRolRepository, RolRepository>();

            // ¡Descomentados y listos!
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // =========================
            // SECURITY
            // =========================
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<ITokenService, TokenService>();

            // =========================
            // EMAIL
            // =========================
            // 1. Vinculamos tu appsettings.json con la clase SmtpSettings usando IOptions
            services.Configure<SmtpSettings>(options =>
                 configuration.GetSection("SmtpSettings").Bind(options)
                );

            // 2. Registramos el servicio real SMTP que inyecta la interfaz de Application
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}