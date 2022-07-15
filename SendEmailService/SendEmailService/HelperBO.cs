using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendEmailService
{
    public class HelperBO
    {
    }
    public class EmailDataBO
    {
        public long ID { get; set; }
        public int STATUS { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string CONTENT { get; set; }
        public string RECIEVERNAME { get; set; }
        public string ATTACHMENTLINK { get; set; }
        public List<Stream> StreamAttachment { get; set; }
        public List<string> FileName { get; set; }
    }

}
