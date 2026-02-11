using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendLowStockAlertAsync(string productName, int currentStock, List<string> adminEmails)
    {
        var email = new MimeMessage();
        // El remitente (tú)
        email.From.Add(MailboxAddress.Parse(_config["SMTP_USER"]));
        // El destinatario (podrías ser tú mismo o el admin del sistema)
        foreach (var adminEmail in adminEmails)
        {
            email.To.Add(MailboxAddress.Parse(adminEmail));
        }
        
        email.Subject = "⚠️ ALERTA: Stock Bajo en Inka Real Estate";

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
                <h1>Notificación de Inventario</h1>
                <p>El sistema ha detectado que el producto <strong>{productName}</strong> tiene un nivel crítico de stock.</p>
                <p><strong>Stock actual:</strong> {currentStock} unidades.</p>
                <br>
                <p><em>Este es un mensaje automático del Sistema de Gestión de Inventarios.</em></p>"
        };

        using var smtp = new SmtpClient();
        try
        {
            // Conexión segura por STARTTLS
            await smtp.ConnectAsync(
                _config["SMTP_HOST"], 
                int.Parse(_config["SMTP_PORT"] ?? "587"), 
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(_config["SMTP_USER"], _config["SMTP_PASS"]);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Aquí es donde el Middleware atrapará errores de conexión si los hay
            throw new Exception("Error al enviar el correo de notificación.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}