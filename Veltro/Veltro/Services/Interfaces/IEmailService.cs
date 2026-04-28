namespace Veltro.Services.Interfaces;

/// <summary>Email sending service contract using MailKit.</summary>
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
}
