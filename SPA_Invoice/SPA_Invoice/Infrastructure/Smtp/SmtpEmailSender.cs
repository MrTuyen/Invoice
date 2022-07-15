using DS.Common.Helpers;
using SPA_Invoice.Infrastructure.Abstractions;
using SPA_Invoice.Infrastructure.Base;
using SPA_Invoice.Infrastructure.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace SPA_Invoice.Infrastructure.Smtp
{
    public class SmtpEmailSender /*: EmailSenderBase, ISmtpEmailSender*/
    {
        //private readonly ISmtpEmailSenderConfiguration _configuration;

        //public SmtpEmailSender(ISmtpEmailSenderConfiguration configuration)
        //    : base(configuration)
        //{
        //    _configuration = configuration;
        //}

        public SmtpClient BuildClient()
        {
            var host = System.Configuration.ConfigurationManager.AppSettings["Mail.Smtp.Domain"];
            var port = 587;

            var smtpClient = new SmtpClient(host, port);
            try
            {

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = true;


                var userName = System.Configuration.ConfigurationManager.AppSettings["Mail.Smtp.UserName"];
                if (!string.IsNullOrEmpty(userName))
                {
                    var password = System.Configuration.ConfigurationManager.AppSettings["Mail.Smtp.Password"];
                    smtpClient.Credentials = new NetworkCredential(userName, password);
                }
                return smtpClient;
            }
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        public async Task SendEmailAsync(MailMessage mail)
        {
            var smtpClient = BuildClient();
            mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Mail.Smtp.UserName"]);
            try
            {
                await smtpClient.SendMailAsync(mail);
            }
            catch (SmtpException smtpEx)
            {
                ConfigHelper.Instance.WriteLog(
                    MethodHelper.Instance.GetErrorMessage(smtpEx, "Lỗi gửi mail hệ thống!"),
                    smtpEx,
                    MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()),
                    string.Empty
                    );
            }
            catch (RankException exception)
            {
                ConfigHelper.Instance.WriteLog(
                    MethodHelper.Instance.GetErrorMessage(exception, "Lỗi gửi mail hệ thống!"),
                    exception,
                    MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()),
                    string.Empty
                    );
            }
            finally
            {
                smtpClient.Dispose();
            }
        }

        public void SendEmail(MailMessage mail)
        {
            var smtpClient = BuildClient();
            mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Mail.Smtp.UserName"]);

            try
            {

                smtpClient.Send(mail);
            }
            catch (SmtpException smtpEx)
            {
                ConfigHelper.Instance.WriteLog(
                    MethodHelper.Instance.GetErrorMessage(smtpEx, "Lỗi gửi mail hệ thống!"),
                    smtpEx,
                    MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()),
                    string.Empty
                    );
            }
            catch (RankException exception)
            {
                ConfigHelper.Instance.WriteLog(
                    MethodHelper.Instance.GetErrorMessage(exception, "Lỗi gửi mail hệ thống!"),
                    exception,
                    MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()),
                    string.Empty
                    );
            }
            finally
            {
                smtpClient.Dispose();
            }
        }
    }
}