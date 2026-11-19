using System.Net;
using System.Net.Mail;

namespace SmartDiscount.Notification.API.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var host = _config["Email:SmtpHost"];
        var port = int.Parse(_config["Email:SmtpPort"] ?? "587");
        var user = _config["Email:SmtpUser"];
        var pass = _config["Email:SmtpPass"];
        var from = _config["Email:From"];
        var fromName = _config["Email:FromName"] ?? "SmartDiscount";

        using var message = new MailMessage
        {
            From = new MailAddress(from, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email envoye a {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Echec envoi email a {Email}", toEmail);
            throw;
        }
    }
}