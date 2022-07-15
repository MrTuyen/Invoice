namespace SPA_Invoice.Infrastructure.Abstractions
{
    public interface ISmtpEmailSenderConfiguration : IEmailSenderConfiguration
    {
        string Host { get; }

        int Port { get; }

        string UserName { get; }

        string Password { get; }

        string Domain { get; }

        bool EnableSsl { get; }

        bool UseDefaultCredentials { get; }
    }
}