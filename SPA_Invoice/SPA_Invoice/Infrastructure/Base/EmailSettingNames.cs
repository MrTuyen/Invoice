namespace SPA_Invoice.Infrastructure.Base
{
    public static class EmailSettingNames
    {
        public const string DefaultFromAddress = "MailDefaultFromAddress";

        public const string DefaultFromDisplayName = "MailDefaultFromDisplayName";

        public const string DefaultToAddress = "MailDefaultToAddress";

        public const string BodyHtml = "MailBodyHtml";

        public static class Smtp
        {
            public const string Host = "MailHost";

            public const string Port = "MailPort";

            public const string UserName = "MailUserName";

            public const string Password = "MailPassword";

            public const string Domain = "MailDomain";

            public const string EnableSsl = "MailEnableSsl";

            public const string UseDefaultCredentials = "MailUseDefaultCredentials";
        }
    }
}