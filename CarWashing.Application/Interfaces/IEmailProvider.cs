namespace CarWashing.Application.Interfaces;

public interface IEmailProvider
{
    Task SendEmailAsync(string email, string subject, string message);
}