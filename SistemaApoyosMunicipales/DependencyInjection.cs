using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SistemaApoyosMunicipales.API.Binders;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Infrastructure.Security;
using System.Text;

namespace SistemaApoyosMunicipales.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(
            this IServiceCollection services,
            IConfiguration configuration   // <-- AGREGADO: sin esto, "configuration" no existe
        )
        {

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
            });

            // --- Swagger ---
            services.AddSwaggerGen(options =>
            {
                const string schemeId = "Bearer";

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sistema Apoyos Municipales API",
                    Version = "v1"
                });

                options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Pega solo el token, sin escribir 'Bearer'."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(schemeId, document),
                        new List<string>()
                    }
                });
            });

            // --- JWT ---
            services.AddScoped<IJwtService, JwtService>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}