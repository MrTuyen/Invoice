using SPA_Invoice.Infrastructure.Abstractions;
using SPA_Invoice.Infrastructure.Base;

namespace SPA_Invoice.Infrastructure.Exception
{
    public class NullEmailSenderConfiguration : EmailSenderConfiguration
    {
        public NullEmailSenderConfiguration(IEmailSetting emailSetting)
            : base(emailSetting)
        {
        }

        protected override string GetNotEmptySettingValue(string name)
        {
            return EmailSetting.GetSettingValue<string>(name);
        }
    }
}