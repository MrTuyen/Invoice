using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.EmailSender
{
    public class EmailData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Content { get; set; }
        public List<Stream> StreamAttachment { get; set; }
        public List<string> FileName { get; set; }
    }

    public class EmailBO
    {
        public long ID { get; set; }
        public long INVOICEID { get; set; }
        public string MAILTO { get; set; }
        public string PAYMENTSTATUS { get; set; }
        public string RECIEVERNAME { get; set; }
        public string CONTENT { get; set; }
        public string ATTACHEMENTLINK { get; set; }
        public int STATUS { get; set; }
        public string MAILTYPE { get; set; }
        public string COMTAXCODE { get; set; }
        public DateTime INITTIME { get; set; }
        public long TOTALROW { get; set; }
        public long NUMBER { get; set; }
        public string CUSNAME { get; set; }
    }

    public class FormSearchEmail
    {
        public string COMTAXCODE { get; set; }
        public string MAILTYPE { get; set; }
        public string MAILTO { get; set; }
        public int? STATUS { get; set; }
        public long? NUMBER { get; set; }
        public string CUSNAME { get; set; }
        public string TIME { get; set; }
        public DateTime? FROMDATE { get; set; }
        public DateTime? TODATE { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
