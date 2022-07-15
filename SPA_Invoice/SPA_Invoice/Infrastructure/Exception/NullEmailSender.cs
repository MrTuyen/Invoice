using SPA_Invoice.Infrastructure.Abstractions;
using SPA_Invoice.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SPA_Invoice.Infrastructure.Exception
{
    public class NullEmailSender : EmailSenderBase
    {
        public NullEmailSender(IEmailSenderConfiguration configuration)
            : base(configuration)
        {
        }

        protected override Task SendEmailAsync(MailMessage mail)
        {
            Debug.WriteLine("USING NullEmailSender!");
            Debug.WriteLine("SendEmailAsync:");
            LogEmail(mail);
            return Task.FromResult(0);
        }

        protected override void SendEmail(MailMessage mail)
        {
            Debug.WriteLine("USING NullEmailSender!");
            Debug.WriteLine("SendEmail:");
            LogEmail(mail);
        }

        private void LogEmail(MailMessage mail)
        {
            Debug.WriteLine(mail.To.ToString());
            Debug.WriteLine(mail.CC.ToString());
            Debug.WriteLine(mail.Subject);
            Debug.WriteLine(mail.Body);
        }
    }
}