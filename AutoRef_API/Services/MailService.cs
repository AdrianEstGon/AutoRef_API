namespace AutoRef_API.Services;
using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

using System.Threading.Tasks;

public class MailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("AutoRef", _mailSettings.SmtpUsername));
        message.To.Add(new MailboxAddress("User", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, false);
            await client.AuthenticateAsync(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

