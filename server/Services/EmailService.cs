using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Overtake.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient
        {
            Host = _configuration["Email:Smtp:Host"],
            Port = int.Parse(_configuration["Email:Smtp:Port"]),
            EnableSsl = bool.Parse(_configuration["Email:Smtp:EnableSsl"]),
            Credentials = new NetworkCredential(
                _configuration["Email:Smtp:Username"],
                _configuration["Email:Smtp:Password"]
            )
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:Smtp:FromAddress"], _configuration["Email:Smtp:FromName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
