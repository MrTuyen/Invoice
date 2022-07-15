using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class FormSearchNumber
    {
        public string COMTAXCODE { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }

    public class FormSearchUsingInvoice
    {
        public string COMTAXCODE { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public string MONTHQUARTER { get; set; }
        public int MONTH { get; set; }
        public int QUARTER { get; set; }
        public int YEAR { get; set; }
        public DateTime? FROMTIME { get; set; }
        public DateTime? TOTIME { get; set; }
        public string CREATOR { get; set; }
        public string LEGALNAME { get; set; }
    }

    public class InvoiceNumberBO
    {
        public long ID { get; set; }
        public string COMTAXCODE { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public long FROMNUMBER { get; set; }
        public long TONUMBER { get; set; }
        public long OLDFROMNUMBER { get; set; }
        public long OLDTONUMBER { get; set; }
        public DateTime? INITTIME { get; set; }
        public long CURRENTNUMBER { get; set; }
        public string AVAILABLENUMBER { get; set; }
        public string USEDNUMBER { get; set; }
        public DateTime? LASTINVOICETIME { get; set; }
        public int STATUS { get; set; }
        public string STATUSNAME { get; set; }
        public string NUMBERSTATUSNAME { get; set; }
        public DateTime? FROMTIME { get; set; }
        public DateTime? TOTIME { get; set; }
        public string STRFROMTIME { get; set; }
        public string STRTOTIME { get; set; }
        public string TEMPLATEPATH { get; set; }
        public string TEMPLATEHTML { get; set; }
        public long TOTALROW { get; set; }
        /// <summary>
        /// Style CSS định dạng template
        /// </summary>
        public string CSS { get; set; }
        public int TAXRATE { get; set; }

        public int USINGINVOICETYPE { get; set; }
        public bool ISSIGNING { get; set; }

        public int CHARONROW { get; set; }
        public int HEADERTEMPLATE { get; set; }
        public int FOTTERTEMPLATE { get; set; }
        public int RECORDPERPAGE { get; set; }
        public string NOTE { get; set; }
    }

    public class OutputInvoice
    {
        public decimal SUMREVENUENOTAX { get; set; }
        public decimal SUMREVENUEZEROPERCENTTAX { get; set; }
        public decimal SUMREVENUEONEPERCENTTAX { get; set; }
        public decimal SUMREVENUETWOPERCENTTAX { get; set; }
        public decimal SUMREVENUETHREEPERCENTTAX { get; set; }
        public decimal SUMREVENUEFIVEPERCENTTAX { get; set; }
        public decimal SUMREVENUETENPERCENTTAX { get; set; }
    }

    public class NextNumber
    {
        public long ID { get; set; }
    }
}

