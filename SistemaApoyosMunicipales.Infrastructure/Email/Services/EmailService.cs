//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using SistemaApoyosMunicipales.Application.Interfaces.Auth;
//using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
//using SistemaApoyosMunicipales.Infrastructure.Email.Settings;

//namespace SistemaApoyosMunicipales.Infrastructure.Email.Services;

//public sealed class EmailService : IEmailService
//{
//    private readonly SmtpSettings _smtpSettings;

//    public EmailService(IOptions<SmtpSettings> smtpSettings)
//    {
//        _smtpSettings = smtpSettings.Value;
//    }

//    public async Task EnviarActivacionAsync(string correo, string nombre, string token)
//    {
//        var mensaje = new MimeMessage();
//        mensaje.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
//        mensaje.To.Add(new MailboxAddress(nombre, correo));
//        mensaje.Subject = "Activa tu cuenta - Sistema de Apoyos Municipales";

//        mensaje.Body = new TextPart("html")
//        {
//            Text = $@"
//            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 8px;'>
//                <h2 style='color: #4B0016; text-align: center;'>¡Bienvenido al Sistema de Apoyos Municipales!</h2>
//                <hr style='border: 0; border-top: 1px solid #eee;' />
//                <p>Hola <strong>{nombre}</strong>,</p>
//                <p>Para activar tu cuenta utiliza el siguiente código:</p>
//                <div style='background-color: #f3f4f6; padding: 15px; text-align: center; font-size: 26px; font-weight: bold; letter-spacing: 3px; margin: 20px 0; border-radius: 6px;'>
//                    {token}
//                </div>
//                <p style='font-size: 12px; color: #6b7280; text-align: center;'>Este es un correo automático, por favor no lo respondas.</p>
//            </div>"
//        };

//        using var cliente = new SmtpClient();

//        await cliente.ConnectAsync(
//    _smtpSettings.Server,
//    _smtpSettings.Port,
//    SecureSocketOptions.StartTls);

//        await cliente.AuthenticateAsync(
//            _smtpSettings.Username,
//            _smtpSettings.Password);

//        await cliente.SendAsync(mensaje);
//        await cliente.DisconnectAsync(true);
//    }
//}




using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Application.Interfaces.Auth.SistemaApoyosMunicipales.Application.Interfaces.Auth;
using SistemaApoyosMunicipales.Infrastructure.Email.Settings;

namespace SistemaApoyosMunicipales.Infrastructure.Email.Services;

public sealed class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly bool _esModoDesarrollo;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
        var entorno = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        _esModoDesarrollo = entorno == "Development";
    }

    public async Task EnviarActivacionAsync(string correo, string nombre, string token)
    {
        if (_esModoDesarrollo)
        {
            await SimularEnvioAsync(correo, nombre, token);
            return;
        }

        // await EnviarCorreoAsync(correo, nombre, token);
    }

    private async Task SimularEnvioAsync(string correo, string nombre, string token)
    {
        var urlActivacion = $"http://localhost:5173/activar?token={token}";

        var carpeta = Path.Combine(
            Directory.GetCurrentDirectory(),
            "correos_desarrollo");

        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"activacion_{correo}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

        var contenido = $"""
            ============================================
            SIMULACIÓN DE CORREO - MODO DESARROLLO
            ============================================
            Para:      {correo}
            Nombre:    {nombre}
            Fecha:     {DateTime.Now:dd/MM/yyyy HH:mm:ss}
            --------------------------------------------
            LINK DE ACTIVACIÓN:
            {urlActivacion}
            --------------------------------------------
            Copia y pega el link en el navegador.
            ============================================
            """;

        await File.WriteAllTextAsync(rutaArchivo, contenido);
    }

    // Producción — descomenta cuando tengas red
    //private async Task EnviarCorreoAsync(string correo, string nombre, string token)
    //{
    //    var urlActivacion = $"{_smtpSettings.UrlBase}/activar?token={token}";
    //
    //    var mensaje = new MimeMessage();
    //    mensaje.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
    //    mensaje.To.Add(new MailboxAddress(nombre, correo));
    //    mensaje.Subject = "Activa tu cuenta - Sistema de Apoyos Municipales";
    //    mensaje.Body = new TextPart("html")
    //    {
    //        Text = $@"
    //        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 8px;'>
    //            <h2 style='color: #4B0016; text-align: center;'>¡Bienvenido al Sistema de Apoyos Municipales!</h2>
    //            <hr style='border: 0; border-top: 1px solid #eee;' />
    //            <p>Hola <strong>{nombre}</strong>,</p>
    //            <p>Para activar tu cuenta haz click en el siguiente botón:</p>
    //            <div style='text-align: center; margin: 30px 0;'>
    //                <a href='{urlActivacion}'
    //                   style='background-color: #4B0016; color: #ffffff; padding: 14px 32px;
    //                          text-decoration: none; border-radius: 6px; font-size: 16px;
    //                          font-weight: bold; display: inline-block;'>
    //                    Activar mi cuenta
    //                </a>
    //            </div>
    //            <p style='font-size: 12px; color: #6b7280; text-align: center;'>
    //                Si no puedes hacer click, copia este link en tu navegador:<br/>
    //                <a href='{urlActivacion}'>{urlActivacion}</a>
    //            </p>
    //            <p style='font-size: 12px; color: #6b7280; text-align: center;'>Este link expira en 24 horas.</p>
    //        </div>"
    //    };
    //
    //    using var cliente = new SmtpClient();
    //    await cliente.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
    //    await cliente.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
    //    await cliente.SendAsync(mensaje);
    //    await cliente.DisconnectAsync(true);
    //}


    public async Task EnviarRecuperacionPasswordAsync(string correo, string nombre, string codigo)
    {
        if (_esModoDesarrollo)
        {
            await SimularRecuperacionAsync(correo, nombre, codigo);
            return;
        }

        // await EnviarCorreoRecuperacionAsync(correo, nombre, codigo);
    }

    private async Task SimularRecuperacionAsync(string correo, string nombre, string codigo)
    {
        var carpeta = Path.Combine(
            Directory.GetCurrentDirectory(),
            "correos_desarrollo");

        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"recuperacion_{correo}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

        var contenido = $"""
        ============================================
        SIMULACIÓN DE CORREO - RECUPERACIÓN
        ============================================
        Para:      {correo}
        Nombre:    {nombre}
        Fecha:     {DateTime.Now:dd/MM/yyyy HH:mm:ss}
        --------------------------------------------
        CÓDIGO DE RECUPERACIÓN (expira en 15 min):

                    {codigo}

        --------------------------------------------
        ============================================
        """;

        await File.WriteAllTextAsync(rutaArchivo, contenido);
    }
}