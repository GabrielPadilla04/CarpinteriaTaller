using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace TP5_Servicios_API_REST.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoOtpAsync(string destinatarioEmail, string codigoOtp)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string server = emailSettings["Server"]!;
            int port = int.Parse(emailSettings["Port"]!);
            string senderName = emailSettings["SenderName"]!;
            string senderEmail = emailSettings["SenderEmail"]!;
            string password = emailSettings["Password"]!;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", destinatarioEmail));
            message.Subject = "Código de recuperación de contraseña";

            message.Body = new TextPart("html")
            {
                Text = $"<div style='font-family: Arial, sans-serif; padding: 20px;'>" +
                       $"<h2>Recuperación de Contraseña</h2>" +
                       $"<p>Has solicitado restablecer tu contraseña. Tu código de verificación de 6 dígitos es:</p>" +
                       $"<h1 style='color: #181c24; letter-spacing: 4px;'>{codigoOtp}</h1>" +
                       $"<p>Este código expirará en 10 minutos.</p>" +
                       $"</div>"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(server, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}