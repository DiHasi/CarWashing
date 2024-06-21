using System.Net.Mail;
using CarWashing.Application.Interfaces;

namespace CarWashing.Infrastructure;

public class EmailProvider : IEmailProvider
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient("localhost")
        {
            Port = 1025,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = false
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@car.wash"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}