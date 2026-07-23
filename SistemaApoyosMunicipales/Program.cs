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

// Asignación limpia de puertos: usa el puerto dinámico de Render si existe, o el 8080 por defecto para local/Docker.
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
                "https://amtda-apoyos-municipales-tula-de-al.vercel.app/"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ==========================================
// APP
// ==========================================
var app = builder.Build();

// ==========================================
// MIDDLEWARES
// ==========================================
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Sistema Apoyos Municipales API v1");
});

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();