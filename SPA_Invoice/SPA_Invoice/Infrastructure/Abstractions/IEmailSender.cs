using System.Net.Mail;
using System.Threading.Tasks;

namespace SPA_Invoice.Infrastructure.Abstractions
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, bool isBodyHtml = true);

        void Send(string to, string subject, string body, bool isBodyHtml = true);

        Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true);

        void Send(string from, string to, string subject, string body, bool isBodyHtml = true);

        void Send(MailMessage mail, bool normalize = true);

        Task SendAsync(MailMessage mail, bool normalize = true);
    }
}