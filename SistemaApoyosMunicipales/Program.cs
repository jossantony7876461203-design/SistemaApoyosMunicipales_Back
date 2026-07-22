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

// Configuración de puertos compatible con Docker local y Render
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    // Si Render (u otro entorno en la nube) provee un puerto dinámico, lo usamos
    builder.WebHost.UseUrls($"http://*:{port}");
}
else
{
    // Si estamos localmente, usamos tu configuración habitual de Kestrel
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8080);  // HTTP
        options.ListenAnyIP(8081, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS para desarrollo local
        });
    });
}

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
                "http://tu-dominio.com" // Agrega tu dominio de producción
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

// Habilitar Swagger siempre (no solo en desarrollo) para pruebas
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