using SPA_Invoice.Infrastructure.Abstractions;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SPA_Invoice.Infrastructure.Base
{
    public abstract class EmailSenderBase : IEmailSender
    {
        private readonly IEmailSenderConfiguration _configuration;

        protected EmailSenderBase(IEmailSenderConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public virtual void Send(string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public virtual async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public virtual void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public virtual async Task SendAsync(MailMessage mail, bool normalize = true)
        {
            if (normalize)
            {
                NormalizeMail(mail);
            }

            await SendEmailAsync(mail);
        }

        public virtual void Send(MailMessage mail, bool normalize = true)
        {
            if (normalize)
            {
                NormalizeMail(mail);
            }

            SendEmail(mail);
        }

        protected abstract Task SendEmailAsync(MailMessage mail);

        protected abstract void SendEmail(MailMessage mail);

        protected virtual void NormalizeMail(MailMessage mail)
        {
            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            if (string.IsNullOrEmpty(mail.From?.Address))
            {
                mail.From = new MailAddress(
                                            _configuration.DefaultFromAddress,
                                            _configuration.DefaultFromDisplayName,
                                            Encoding.UTF8);
            }

            if (mail.To.Count == 0)
            {
                mail.To.Add(new MailAddress(_configuration.DefaultToAddress));
            }

            mail.IsBodyHtml = _configuration.BodyHtml;

            if (mail.HeadersEncoding == null)
            {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null)
            {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null)
            {
                mail.BodyEncoding = Encoding.UTF8;
            }
        }
    }
}