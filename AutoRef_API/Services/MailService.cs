namespace AutoRef_API.Services;
using MailKit.Net.Smtp;

using MimeKit;

using System.Threading.Tasks;

public class MailService
{
    private readonly string _smtpServer = "smtp.gmail.com"; // Cambia por tu servidor SMTP
    private readonly int _smtpPort = 587; // Usualmente 587 para SMTP
    private readonly string _smtpUsername = "autorefasturias@gmail.com"; // Tu correo de envío
    private readonly string _smtpPassword = "parmqbpwxhjykkww"; // Tu contraseña de correo

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Administrador", _smtpUsername));
        message.To.Add(new MailboxAddress("User", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_smtpServer, _smtpPort, false);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

