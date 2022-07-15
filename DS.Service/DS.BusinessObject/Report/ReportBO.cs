using System;

namespace DS.BusinessObject.Report
{
    public class ReportBO
    {
        public long ID { get; set; }
        public long INVOICEID { get; set; }
        public string REPORTTYPE { get; set; }
        public string COMNAME { get; set; }
        public string COMLEGALNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public string COMADDRESS { get; set; }
        public string COMPHONENUMBER { get; set; }
        public string COMROLE { get; set; }

        public string CUSNAME { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSDELEGATE { get; set; }
        public string CUSPHONENUMBER { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSROLE { get; set; }
        public string REASON { get; set; }
        public string LINK { get; set; }
        /// <summary>
        /// Trạng thái biên bản hóa đơn: 0: chưa ký, 1: đã ký
        /// </summary>
        public int REPORTSTATUS{get;set;}
        public string STRREPORTTIME { get; set; }
        public DateTime REPORTTIME { get; set; }
    }
}
