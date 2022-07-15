using SPA_Invoice.Infrastructure.Abstractions;
using System;

namespace SPA_Invoice.Infrastructure.Base
{
    public abstract class EmailSenderConfiguration : IEmailSenderConfiguration
    {
        public string DefaultFromAddress => GetNotEmptySettingValue(EmailSettingNames.DefaultFromAddress);

        public string DefaultFromDisplayName => EmailSetting.GetSettingValue<string>(EmailSettingNames.DefaultFromDisplayName);

        public string DefaultToAddress => GetNotEmptySettingValue(EmailSettingNames.DefaultToAddress);

        public bool BodyHtml => EmailSetting.GetSettingValue<bool>(EmailSettingNames.BodyHtml);

        protected readonly IEmailSetting EmailSetting;

        protected EmailSenderConfiguration(IEmailSetting emailSetting)
        {
            EmailSetting = emailSetting;
        }

        protected virtual string GetNotEmptySettingValue(string name)
        {
            var value = EmailSetting.GetSettingValue<string>(name);
            if (string.IsNullOrEmpty(value))
            {
                throw new ApplicationException($"Setting value for '{name}' is null or empty!");
            }

            return value;
        }
    }
}