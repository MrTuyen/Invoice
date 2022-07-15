using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice
{
    public class ImportInvoiceTitleBO
    {
        public string MYFIELD { get; set; }
        public string YOURFIELD { get; set; }
        public string EXPLAINT { get; set; }
        public bool ISMANDATORY { get; set; }

        public List<string> ConfigFields { get; set; }

        public bool Required { get; set; }
    }

    public class ImportInvoiceColumnMaping
    {
        public int Index { get; set; }
        public string ColName { get; set; }
        public string ColReference { get; set; }
    }


    public class ImportInvoiceBO
    {
        public int INVOICEID { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public string CUSNAME { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSBUYER { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSPHONENUMBER { get; set; }
        public string CUSEMAIL { get; set; }
        public string CUSPAYMENTMETHOD { get; set; }
        public string CUSACCOUNTNUMBER { get; set; }
        public string CUSBANKNAME { get; set; }
        
        public decimal TOTALMONEY { get; set; }

        //public int TERMID { get; set; }
        //public DateTime DUEDATE { get; set; }
        public List<ImportInvoiceDetailBO> LISTPRODUCT { get; set; }

        public List<int> ErrorField { get; set; }

        public int USINGINVOICETYPE { get; set; }

        public string FROMDATESTR { get; set; }
        public string TODATESTR { get; set; }
        public int APARTMENTNO { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CUSTOMERCODE { get; set; }
    }

    public class ImportInvoiceDetailBO
    {
        public Int64 INVOICEID { get; set; }
        public string PRODUCTNAME { get; set; }
        public string QUANTITYUNIT { get; set; }
        public decimal RETAILPRICE { get; set; }
        public int QUANTITY { get; set; }
        public int TAXRATE { get; set; }
        public decimal TOTALMONEY { get; set; }
        public int APARTMENTNO { get; set; }

        public string SKU { get; set; }

        /// <summary>
        /// Chỉ số cũ
        /// </summary>
        public long OLDNO { get; set; }

        /// <summary>
        /// Chỉ số mới
        /// </summary>
        public long NEWNO { get; set; }

        /// <summary>
        /// HS nhân
        /// </summary>
        public int FACTOR { get; set; }

        /// <summary>
        /// Mã số công tơ điện (Dùng cho kh sử dụng hóa đơn tiền điện)
        /// </summary>
        public string METERCODE { get; set; }

        public string METERNAME { get; set; }

        public int TAXRATEWATER { get; set; }
    }

}
