using System.Net.Mail;

namespace SPA_Invoice.Infrastructure.Abstractions
{
    public interface ISmtpEmailSender : IEmailSender
    {
        SmtpClient BuildClient();
    }
}