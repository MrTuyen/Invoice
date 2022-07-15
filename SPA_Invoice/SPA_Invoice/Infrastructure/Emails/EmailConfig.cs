namespace SPA_Invoice.Infrastructure.EmailConfig
{
    public class EmailConfig
    {
        public string MailDefaultFromAddress { get; set; }

        public string MailDefaultFromDisplayName { get; set; }

        public string MailDefaultToAddress { get; set; }

        public string MailHost { get; set; }

        public int MailPort { get; set; }

        public string MailUserName { get; set; }

        public string MailPassword { get; set; }

        public string MailDomain { get; set; }

        public bool MailEnableSsl { get; set; }

        public bool MailUseDefaultCredentials { get; set; }

        public bool MailBodyHtml { get; set; }
    }
}