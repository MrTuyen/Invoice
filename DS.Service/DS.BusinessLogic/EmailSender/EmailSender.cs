using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace DS.BusinessLogic.EmailSender
{
    public class EmailSender
    {
        public static void MailSender(EmailData emailData)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = body,
                        Priority = MailPriority.Normal,
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }
                    
                    msg.To.Add(toEmail);
                    
                    smtpClient.Send(msg);
                    smtpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "MailSender");
            }
        }

        /// <summary>
        /// Kiểm tra thiết lập email doanh nghiệp. Gửi thành công return true
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns></returns>
        public static bool CheckSendMail(EmailData emailData)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = body,
                        Priority = MailPriority.Normal,
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }

                    msg.To.Add(toEmail);

                    smtpClient.Send(msg);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void MailInvoiceSender(EmailData emailData, InvoiceBO invoice, int typeMail, string bodyCustom = null)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;
            if (typeMail == 1)
            {
                bodyCustom = string.IsNullOrWhiteSpace(bodyCustom) ? CreateEmailBody(invoice) : bodyCustom;
            }
            else if (typeMail == 2)
            {
                bodyCustom = CreateEmailCancelBody(invoice);
            }
            else
            {
                bodyCustom = "";
            }

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = bodyCustom,
                        Priority = MailPriority.Normal
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }
                    if (toEmail != null)
                    {
                        var lstEmail = toEmail.Split(',');
                        foreach (var item in lstEmail)
                        {
                            msg.To.Clear();
                            msg.To.Add(item.Trim());
                            smtpClient.Send(msg);
                        }
                        msg.Dispose();
                    }
                    smtpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                EmailBLL emailBLL = new EmailBLL();
                emailBLL.UpdateEmail(new EmailBO() { ID = invoice.EMAILID, CONTENT =  bodyCustom, STATUS = 3});
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "MailInvoiceSender");
            }
        }

        public static async void MailInvoiceSenderAsync(EmailData emailData, InvoiceBO invoice, int typeMail, string bodyCustom = null)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;
            if (typeMail == 1)
            {
                bodyCustom = string.IsNullOrWhiteSpace(bodyCustom) ? CreateEmailBody(invoice) : bodyCustom;
            }
            else if (typeMail == 2)
            {
                bodyCustom = CreateEmailCancelBody(invoice);
            }
            else
            {
                bodyCustom = "";
            }

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = bodyCustom,
                        Priority = MailPriority.Normal
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }
                    if (toEmail != null)
                    {
                        var lstEmail = toEmail.Split(',');
                        foreach (var item in lstEmail)
                        {
                            msg.To.Clear();
                            msg.To.Add(item.Trim());
                            await smtpClient.SendMailAsync(msg);
                        }
                        msg.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                EmailBLL emailBLL = new EmailBLL();
                emailBLL.UpdateEmail(new EmailBO() { ID = invoice.EMAILID, CONTENT = bodyCustom, STATUS = 3 });
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "MailInvoiceSender");
            }
        }

        public static void MailInvoiceSender(EmailData emailData, InvoiceBO invoice, int typeMail, System.Web.HttpContext context, string bodyCustom = null)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;
            if (typeMail == 1)
            {
                bodyCustom = string.IsNullOrWhiteSpace(bodyCustom) ? CreateEmailBody(invoice, context) : bodyCustom;
            }
            else if (typeMail == 2)
            {
                bodyCustom = CreateEmailCancelBody(invoice);
            }
            else
            {
                bodyCustom = "";
            }

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = bodyCustom,
                        Priority = MailPriority.Normal
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }
                    if (toEmail != null)
                    {
                        var lstEmail = toEmail.Split(',');
                        foreach (var item in lstEmail)
                        {
                            msg.To.Clear();
                            msg.To.Add(item.Trim());
                            smtpClient.Send(msg);
                        }
                        msg.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                EmailBLL emailBLL = new EmailBLL();
                emailBLL.UpdateEmail(new EmailBO() { ID = invoice.EMAILID, CONTENT = bodyCustom, STATUS = 3 });
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "MailInvoiceSender");
            }
        }

        public static void MailInvoiceSenderAsync(EmailData emailData, InvoiceBO invoice, int typeMail, System.Web.HttpContext context, string bodyCustom = null)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.Content;
            if (typeMail == 1)
            {
                bodyCustom = string.IsNullOrWhiteSpace(bodyCustom) ? CreateEmailBody(invoice, context) : bodyCustom;
            }
            else if (typeMail == 2)
            {
                bodyCustom = CreateEmailCancelBody(invoice);
            }
            else
            {
                bodyCustom = "";
            }

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;

                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = bodyCustom,
                        Priority = MailPriority.Normal
                    };

                    if (emailData.FileName != null)
                    {
                        for (int i = 0; i < emailData.FileName.Count; i++)
                        {
                            Attachment attachment = new Attachment(emailData.StreamAttachment[i], emailData.FileName[i].ToString());
                            msg.Attachments.Add(attachment);
                        }
                    }
                    if (toEmail != null)
                    {
                        var lstEmail = toEmail.Split(',');
                        foreach (var item in lstEmail)
                        {
                            msg.To.Clear();
                            msg.To.Add(item.Trim());
                            smtpClient.SendMailAsync(msg);
                        }
                        msg.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                EmailBLL emailBLL = new EmailBLL();
                emailBLL.UpdateEmail(new EmailBO() { ID = invoice.EMAILID, CONTENT = bodyCustom, STATUS = 3 });
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "MailInvoiceSender");
            }
        }

        public static string CreateEmailBody(InvoiceBO invoice)
        {
            try
            {
                string body = string.Empty;
                //using streamreader for reading my htmltemplate   
                try
                {
                    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath($"~/NOVAON_FOLDER/{invoice.COMTAXCODE}/EmailTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }

                // Using default email template
                if (string.IsNullOrEmpty(body))
                {
                    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/TemplateFiles/EmailTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }

                body = body.Replace("{COMNAME}", invoice.COMNAME); //replacing the required things  
                body = body.Replace("{INVOICENUMBER}", invoice.NUMBER.ToString("D7"));
                body = body.Replace("{INVOICEFORMCODE}", invoice.FORMCODE);
                body = body.Replace("{INVOICESYMBOLCODE}", invoice.SYMBOLCODE);
                body = body.Replace("{REFERENCECODE}", invoice.REFERENCECODE);
                body = body.Replace("{BUYERNAME}", invoice.CUSBUYER);
                body = body.Replace("{EMAILID}", invoice.EMAILID.ToString()); 
                return body;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "CreateEmailBody");
                return "";
            }
        }

        public static string CreateEmailBody(InvoiceBO invoice, System.Web.HttpContext context)
        {
            try
            {
                string body = string.Empty;
                // Read email template file of own company
                try
                {
                    using (StreamReader reader = new StreamReader(context.Server.MapPath($"~/NOVAON_FOLDER/{invoice.COMTAXCODE}/EmailTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }

                // Using default email template
                if (string.IsNullOrEmpty(body))
                {
                    using (StreamReader reader = new StreamReader(context.Server.MapPath("~/TemplateFiles/EmailTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }

                body = body.Replace("{COMNAME}", invoice.COMNAME); //replacing the required things  
                body = body.Replace("{INVOICENUMBER}", invoice.NUMBER.ToString("D7"));
                body = body.Replace("{INVOICEFORMCODE}", invoice.FORMCODE);
                body = body.Replace("{INVOICESYMBOLCODE}", invoice.SYMBOLCODE);
                body = body.Replace("{REFERENCECODE}", invoice.REFERENCECODE);
                body = body.Replace("{BUYERNAME}", invoice.CUSBUYER);
                body = body.Replace("{EMAILID}", invoice.EMAILID.ToString());
                return body;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "CreateEmailBody");
                return "";
            }
        }

        public static string CreateEmailCancelBody(InvoiceBO invoice)
        {
            try
            {
                string body = string.Empty;
                // Read email template file of own company
                try
                {
                    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath($"~/NOVAON_FOLDER/{invoice.COMTAXCODE}/EmailCancelTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }

                if (string.IsNullOrEmpty(body))
                {
                    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/TemplateFiles/EmailCancelTemplate.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                body = body.Replace("{COMNAME}", invoice.COMNAME); //replacing the required things  
                body = body.Replace("{INVOICENUMBER}", invoice.NUMBER.ToString("D7"));
                body = body.Replace("{INVOICEFORMCODE}", invoice.FORMCODE);
                body = body.Replace("{INVOICESYMBOLCODE}", invoice.SYMBOLCODE);
                body = body.Replace("{REFERENCECODE}", invoice.REFERENCECODE);
                body = body.Replace("{BUYERNAME}", invoice.CUSBUYER);
                body = body.Replace("{EMAILID}", invoice.EMAILID.ToString());
                return body;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "CreateEmailCancelBody");
                return "";
            }
        }
    }
}
