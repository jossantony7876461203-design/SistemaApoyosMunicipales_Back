using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using SistemaApoyosMunicipales.Application.Services;
using SistemaApoyosMunicipales.Infrastructure.Auth.Services;
using SistemaApoyosMunicipales.Infrastructure.Email.Services;
using SistemaApoyosMunicipales.Infrastructure.Email.Settings;
using SistemaApoyosMunicipales.Infrastructure.Persistence;
using SistemaApoyosMunicipales.Infrastructure.Persistence.Repositories;
using SistemaApoyosMunicipales.Infrastructure.Persistence.UnitOfWork;
using SistemaApoyosMunicipales.Infrastructure.Repositories;
using SistemaApoyosMunicipales.Infrastructure.Security;
using SistemaApoyosMunicipales.Infrastructure.Storage.Services;

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
            // LICENCIA DE QUESTPDF (obligatorio declararla una vez)
            // =========================
            // Community = gratis para empresas/proyectos pequeños.
            // Revisa los términos de QuestPDF si tu organización es grande:
            // https://www.questpdf.com/license/
            QuestPDF.Settings.License = LicenseType.Community;

            // =========================
            // HTTP CONTEXT ACCESSOR (NECESARIO PARA CurrentUserService)
            // =========================
            services.AddHttpContextAccessor();

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
            services.AddScoped<IComunidadRepository, ComunidadRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<ISubRolRepository, SubRolRepository>();
            services.AddScoped<IPermisoRepository, PermisoRepository>();
            services.AddScoped<IRolPermisoRepository, RolPermisoRepository>();
            services.AddScoped<ISubRolPermisoRepository, SubRolPermisoRepository>();
            services.AddScoped<IApoyoRepository, ApoyoRepository>();
            services.AddScoped<IRegistroApoyoRepository, RegistroApoyoRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // =========================
            // ESTADÍSTICAS / DASHBOARD (con caché en memoria)
            // =========================
            services.AddMemoryCache();

            services.AddScoped<IEstadisticasRepository, EstadisticasRepository>();
            services.AddScoped<EstadisticasService>();
            services.AddScoped<IEstadisticasService>(sp =>
                new CachedEstadisticasService(
                    sp.GetRequiredService<EstadisticasService>(),
                    sp.GetRequiredService<IMemoryCache>()));

            // =========================
            // REPORTES (Dapper + QuestPDF)
            // =========================
            services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
            services.AddScoped<IReportesRepository, ReportesRepository>();
            services.AddScoped<IReportesService, ReportesService>();

            // ¡Descomentados y listos!
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Cola — Singleton porque vive toda la vida de la app
            services.AddSingleton<IImagenQueue, ImagenQueue>();

            // Worker — BackgroundService
            services.AddHostedService<ImagenBackgroundService>();

            // =========================
            // SECURITY
            // =========================
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<ITokenService, TokenService>();

            // =========================
            // EMAIL
            // =========================
            services.Configure<SmtpSettings>(options =>
                 configuration.GetSection("SmtpSettings").Bind(options)
                );

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}