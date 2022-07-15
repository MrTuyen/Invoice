using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice
{
    public class FormSearchInvoice
    {
        public string COMTAXCODE { get; set; }
        public int? INVOICETYPE { get; set; }
        public int? INVOICESTATUS { get; set; }
        public int? PAYMENTSTATUS { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public long? NUMBER { get; set; }
        public string CUSTOMER { get; set; }
        public string PHONENUMBER { get; set; }
        public string TAXCODE { get; set; }
        public string STRFROMDATE { get; set; }
        public string STRTODATE { get; set; }
        public string TIME { get; set; }
        public DateTime? FROMDATE { get; set; }
        public DateTime? TODATE { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
        public int? REPORTYPE { get; set; }
        public string KEYWORD { get; set; }
        public int? STATUS { get; set; }
        public string CUSTOMERCODE { get; set; }
        public long FROMNUMBER { get; set; }
        public long TONUMBER { get; set; }
        public int USINGINVOICETYPE { get; set; } //Loại hóa đơn đang sử dụng của user
         //nhóm khách hàng dùng cho trường học
        public string CUSTAXCODE { get; set; }
        // Phân quyền cộng tác viên theo email.
        public string PARTNEREMAIL { get; set; }
    }
}
