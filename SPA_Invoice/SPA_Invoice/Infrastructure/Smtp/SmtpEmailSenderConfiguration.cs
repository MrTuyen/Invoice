using SPA_Invoice.Infrastructure.Abstractions;
using SPA_Invoice.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA_Invoice.Infrastructure.Smtp
{
    public class SmtpEmailSenderConfiguration : EmailSenderConfiguration, ISmtpEmailSenderConfiguration
    {
        public string Host => GetNotEmptySettingValue(EmailSettingNames.Smtp.Host);

        public int Port => EmailSetting.GetSettingValue<int>(EmailSettingNames.Smtp.Port);

        public string UserName => GetNotEmptySettingValue(EmailSettingNames.Smtp.UserName);

        public string Password => GetNotEmptySettingValue(EmailSettingNames.Smtp.Password);

        public string Domain => EmailSetting.GetSettingValue<string>(EmailSettingNames.Smtp.Domain);

        public bool EnableSsl => EmailSetting.GetSettingValue<bool>(EmailSettingNames.Smtp.EnableSsl);

        public bool UseDefaultCredentials => EmailSetting.GetSettingValue<bool>(EmailSettingNames.Smtp.UseDefaultCredentials);

        public SmtpEmailSenderConfiguration(IEmailSetting emailSetting)
            : base(emailSetting)
        {
        }
    }
}