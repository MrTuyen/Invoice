namespace SPA_Invoice.Infrastructure.Abstractions
{
    public interface IEmailSenderConfiguration
    {
        string DefaultFromAddress { get; }

        string DefaultFromDisplayName { get; }

        string DefaultToAddress { get; }

        bool BodyHtml { get; }
    }
}