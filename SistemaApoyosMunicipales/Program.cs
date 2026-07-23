using SistemaApoyosMunicipales.API;
using SistemaApoyosMunicipales.API.Middlewares;
using SistemaApoyosMunicipales.Application;
using SistemaApoyosMunicipales.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// SERVICES
// ==========================================
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

// Asignación de puerto dinámico (Render / Local / Docker)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://192.168.0.100:5173",
                "http://localhost:3000",
                "https://amtda-apoyos-municipales-tula-de-al.vercel.app" // 👈 Corregido: Sin barra al final
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ==========================================
// APP & MIDDLEWARES
// ==========================================
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// El middleware de CORS debe ejecutarse antes de Swagger, Autenticación y Controladores
app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Sistema Apoyos Municipales API v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();