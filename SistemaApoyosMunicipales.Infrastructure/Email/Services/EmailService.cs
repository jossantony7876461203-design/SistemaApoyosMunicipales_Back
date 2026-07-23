

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Infrastructure.Email.Settings;
using System.Reflection;

namespace SistemaApoyosMunicipales.Infrastructure.Email.Services;

public sealed class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    // ─────────────────────────────────────────────
    // ACTIVACIÓN
    // ─────────────────────────────────────────────

    public async Task EnviarActivacionAsync(string correo, string nombre, string token)
    {
        var urlActivacion = $"https://amtda-apoyos-municipales-tula-de-al.vercel.app/activar?token={token}";

        // 1. Cargar el HTML embebido desde los recursos de la DLL
        // NOTA: Ajusta "SistemaApoyosMunicipales.Infrastructure" si tu Namespace por defecto es diferente
        string resourceName = "SistemaApoyosMunicipales.Infrastructure.Email.Templates.ActivationTemplate.html";
        string htmlBody = await LoadTemplateAsync(resourceName);

        // 2. Reemplazar las etiquetas dinámicas del diseño
        htmlBody = htmlBody.Replace("{{NAME}}", nombre);
        htmlBody = htmlBody.Replace("{{ACTIVATION_URL}}", urlActivacion);

        // 3. Crear el correo con MimeKit
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        mensaje.To.Add(new MailboxAddress(nombre, correo));
        mensaje.Subject = "Activa tu cuenta - Sistema de Apoyos Municipales";

        mensaje.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        // 4. Envío seguro a través de MailKit usando tu configuración de Gmail
        using var cliente = new SmtpClient();
        var opcionesSeguridad = _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await cliente.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, opcionesSeguridad);
        await cliente.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
        await cliente.SendAsync(mensaje);
        await cliente.DisconnectAsync(true);
    }


    // ─────────────────────────────────────────────
    // RECUPERACIÓN DE CONTRASEÑA
    // ─────────────────────────────────────────────

    public async Task EnviarRecuperacionPasswordAsync(string correo, string nombre, string codigo)
    {
        // 1. Cargar el HTML embebido desde los recursos de la DLL
        // NOTA: Ajusta "SistemaApoyosMunicipales.Infrastructure" si tu Namespace por defecto es diferente
        string resourceName = "SistemaApoyosMunicipales.Infrastructure.Email.Templates.RecoveryTemplate.html";
        string htmlBody = await LoadTemplateAsync(resourceName);

        // 2. Reemplazar las etiquetas dinámicas de la plantilla
        htmlBody = htmlBody.Replace("{{NAME}}", nombre);
        htmlBody = htmlBody.Replace("{{CODE}}", codigo);

        // 3. Crear el correo con MimeKit
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        mensaje.To.Add(new MailboxAddress(nombre, correo));
        mensaje.Subject = "Recuperación de contraseña - Sistema de Apoyos Municipales";

        mensaje.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        // 4. Envío a Gmail utilizando MailKit y tu configuración intacta de SmtpSettings
        using var cliente = new SmtpClient();
        var opcionesSeguridad = _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await cliente.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, opcionesSeguridad);
        await cliente.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
        await cliente.SendAsync(mensaje);
        await cliente.DisconnectAsync(true);
    }


    // ─────────────────────────────────────────────────────────────
    // HELPER PRIVADO PARA CARGAR EL TEMPLATE EMBEBIDO
    // ─────────────────────────────────────────────────────────────
    private async Task<string> LoadTemplateAsync(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"No se encontró el template embebido: {resourceName}. Asegúrate de que la ruta sea correcta y que esté marcado como Embedded Resource.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}