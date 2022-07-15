using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Npgsql;
using SAB.Library.Data;
using SendEmailService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Linq;
using HtmlAgilityPack;

namespace AppCenterService
{
    public partial class Service : ServiceBase
    {

        private GmailService service;
        private NpgsqlConnection conn;
        private bool isRequest = false;
        private UserCredential credential;
        private System.Timers.Timer timer;
        private int timeRepeatSeconds = 100;
        private string LogPath = string.Empty;
        private string ConnectionString = string.Empty;

        private static string ApplicationName = "OnFinance";
        private static string[] Scopes = { GmailService.Scope.GmailReadonly };

        private MailMessage msg;
        private SmtpClient smtpClient;

        public Service()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LogPath = ConfigurationManager.AppSettings["LogPath"];
                ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                this.conn = new NpgsqlConnection(ConnectionString);
                this.GmailAPI_Initialize();

                //Thread newProcess = new Thread(ListenForNotifications)
                //{
                //    IsBackground = true
                //};
                //newProcess.Start();

                this.timer = new System.Timers.Timer
                {
                    Interval = timeRepeatSeconds,
                    AutoReset = false,
                    Enabled = true
                };
                timer.Elapsed += new ElapsedEventHandler(WorkerProcess);
                timer.Start();

                timeRepeatSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["TimeRepeatSeconds"]);
            }
            catch (Exception objMainEx)
            {
                WriteLog("OnStart", objMainEx.ToString());
            }
        }

        private void WorkerProcess(object sender, EventArgs e)
        {
            timer.Interval = timeRepeatSeconds * 1000;  //Thời gian delay giữa các tiến trình
            timer.Stop();   //Dừng timer lại cho đến khi xử lý hết công việc phía dưới rồi start lại
            if (!this.isRequest)
            {
                this.isRequest = true;
                try
                {
                    this.ListInbox(this.service);
                }
                catch (Exception exp)
                {
                    WriteLog("WorkerProcess", exp.ToString());
                }
            }
            else this.isRequest = false;
            timer.Start();
        }

        private void ListenForNotifications()
        {
            ExecuteListenNotify();
            conn.Notification += PostgresNotificationReceived;
            while (true)
            {
                try
                {
                    if (!(conn.State == ConnectionState.Open))
                    {
                        WriteLog("ListenForNotifications", "ReConnecting...");
                        ExecuteListenNotify();
                    }
                    conn.Wait();
                }
                catch (Exception ex)
                {
                    Thread.Sleep(10000);
                    FileLogger.LogAction($"exp: {ex}");
                }
            }
        }

        private void ExecuteListenNotify()
        {
            conn.Open();
            var listenCommand = conn.CreateCommand();
            listenCommand.CommandText = $"listen email_notify";
            listenCommand.ExecuteNonQuery();
        }

        private void GmailAPI_Initialize()
        {
            try
            {
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                        Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
                }

                this.service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch (Exception exp)
            {
                WriteLog("GmailAPI_Initialize", exp.Message);
            }
        }

        private void PostgresNotificationReceived(object sender, NpgsqlNotificationEventArgs e)
        {
            string actionName = e.AdditionalInformation;
            var ArrObj = e.AdditionalInformation.Split('|');
            string id = ArrObj[0];
            string emailTo = ArrObj[1];
            string subject = ArrObj[2];
            string content = ArrObj[3];
            string attachment = ArrObj[4];
            int status = int.Parse(ArrObj[5]);

            new Thread(() => MailInvoiceSender(new EmailDataBO
            {
                ID = Int64.Parse(id),
                STATUS = status,
                Username = ConfigurationManager.AppSettings["UsernameEmail"],
                Password = ConfigurationManager.AppSettings["PasswordEmail"],
                Host = "smtp.gmail.com",
                Port = 587,
                MailTo = emailTo,
                Subject = subject,
                CONTENT = content,
                ATTACHMENTLINK = attachment
            }))
            {
                IsBackground = true
            }.Start();

        }

        private void ListInbox(GmailService service)
        {
            try
            {
                var inboxlistRequest = service.Users.Messages.List("me");
                inboxlistRequest.LabelIds = "INBOX";
                inboxlistRequest.MaxResults = 100;
                inboxlistRequest.IncludeSpamTrash = false;
                var emailListResponse = inboxlistRequest.Execute();
                if (emailListResponse != null && emailListResponse.Messages != null)
                {
                    List<string> lstEmailTo = new List<string>();
                    foreach (var email in emailListResponse.Messages)
                    {
                        var emailInfoRequest = service.Users.Messages.Get("me", email.Id);
                        var emailInfoResponse = emailInfoRequest.Execute();
                        if (emailInfoResponse != null)
                        {
                            foreach (var mParts in emailInfoResponse.Payload.Headers)
                            {
                                new Thread(() =>
                                {
                                    if (mParts.Name == "X-Failed-Recipients")
                                    {
                                        lstEmailTo.Add(mParts.Value);
                                    }
                                }).Start();
                            }
                        }
                    }
                    if (lstEmailTo.Count == 0)
                        return;
                    string strEmailTo = string.Join(",", lstEmailTo);
                    HelperDAO helperDAO = new HelperDAO();
                    var list = helperDAO.GetEmail(strEmailTo);

                    foreach (var itme in list)
                    {
                        if (itme.STATUS == 1)
                            try
                            {
                                var result = helperDAO.UpdateStatusEmail(itme.ID, 3);
                                if (!result)
                                {
                                    WriteLog("UpdateStatusEmail", "Lỗi cập nhật trạng thái gửi email");
                                }
                            }
                            catch (Exception exp)
                            {
                                WriteLog("UpdateStatusEmail", "Lỗi cập nhật trạng thái gửi email");
                            }
                    }


                    string strIDs = string.Join(",", list.Select(x => x.ID).ToList());
                    Debug.WriteLine(DateTime.Now.Minute);
                }
            }
            catch (Exception exp)
            {
                WriteLog("ListInbox", exp.Message);
            }
        }

        public void MailInvoiceSender(EmailDataBO emailData)
        {
            string smtpUserName = emailData.Username;
            string smtpPassword = emailData.Password;
            string smtpHost = emailData.Host;
            int smtpPort = emailData.Port;

            string toEmail = emailData.MailTo;
            string subject = emailData.Subject;
            string body = emailData.CONTENT;

            try
            {
                if (emailData.STATUS != 1)
                    return;
                smtpClient = new SmtpClient();
                smtpClient.EnableSsl = true;
                smtpClient.Host = smtpHost;
                smtpClient.Port = smtpPort;
                smtpClient.UseDefaultCredentials = true;

                smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                msg = new MailMessage
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(smtpUserName),
                    Subject = subject,
                    Body = body,
                    Priority = MailPriority.Normal
                };

                if (emailData.ATTACHMENTLINK != null)
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(body);
                    var document = html.DocumentNode;

                    string formCode = document.QuerySelectorAll("li").Where(x => x.InnerText.Contains("Mẫu số")).First().QuerySelector("strong").InnerText.Trim();
                    string symbolCode = document.QuerySelectorAll("li").Where(x => x.InnerText.Contains("Ký hiệu")).First().QuerySelector("strong").InnerText.Trim();
                    string number = document.QuerySelectorAll("li").Where(x => x.InnerText.Contains("Số hóa đơn")).First().QuerySelector("strong").InnerText.Trim();

                    var link = emailData.ATTACHMENTLINK.Split(',');

                    foreach (var item in link)
                    {
                        string fileName = string.Empty;
                        if (item.Contains("pdf"))
                            fileName = string.Format("Hoa_Don_Dien_Tu_{0}_{1}_{2}.pdf", formCode, symbolCode, number);
                        else if (item.Contains("xml"))
                            fileName = string.Format("Hoa_Don_Dien_Tu_{0}_{1}_{2}.xml", formCode, symbolCode, number);

                        string physicalAttachmentLink = string.Format(@"D:/NOVAON/SPA_Invoice/SPA_Invoice/NOVAON_FOLDER{0}", item);


                        Stream fsSource = new FileStream(physicalAttachmentLink, FileMode.Open, FileAccess.Read);
                        msg.Attachments.Add(new Attachment(fsSource, fileName));
                    }

                }
                if (toEmail != null)
                {
                    List<string> lstEmail = new List<string>();
                    foreach (var item in toEmail.Split(','))
                    {
                        lstEmail.Add(item);
                        if (lstEmail.Count % 50 == 0)
                        {
                            msg.To.Add(string.Join(",", lstEmail.Select(x => x)));
                            smtpClient.Send(msg);
                            lstEmail = new List<string>();
                        }
                    }
                    if (lstEmail.Count > 0)
                    {
                        msg.To.Add(string.Join(",", lstEmail.Select(x => x)));
                        smtpClient.Send(msg);
                    }
                }
                //EmailBLL emailBLL = new EmailBLL();
                //emailBLL.UpdateEmail(new EmailBO() { ID = invoice.EMAILID, CONTENT = bodyCustom, STATUS = 1 });
            }
            catch (Exception objEx)
            {
                smtpClient.Dispose();
                msg.Dispose();
            }
            finally
            {
                smtpClient.Dispose();
                msg.Dispose();
            }
        }

        private void WriteLog(string functionName, string strMessage)
        {
            string fileLogName = LogPath + "log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"; // File ghi log lỗi
            File.AppendAllText(fileLogName, string.Format("{0}{1} {2}: {3}", Environment.NewLine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), functionName, strMessage));
        }
    }
}