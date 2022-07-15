using SynctaskInvoicePMSmat.Common;
using System;
using System.Collections.Generic;

namespace SynctaskInvoicePMSmat.DTO
{
    public class InvoiceBO
    {
        public string PMSMAT_INVOICEID { get; set; }
        public string COMNAME { get; set; } = ConfigHelper.OrgComName;
        public string COMTAXCODE { get; set; } = ConfigHelper.OrgComtaxcode;
        public string COMADDRESS { get; set; } = ConfigHelper.OrgComAddress;
        public string FORMCODE { get; set; } = ConfigHelper.OrgFormCode;
        public string SYMBOLCODE { get; set; } = ConfigHelper.OrgSymbolCode;

        public string CUSNAME { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSBUYER { get; set; }
        public string CUSTOMERCODE { get; set; }
        public string CUSPAYMENTMETHOD { get; set; }
        public string CUSACCOUNTNUMBER { get; set; }

        public List<InvoiceDetailBO> LISTPRODUCT { get; set; }
        public DateTime INITTIME { get; set; } // ngày khởi tạo
        public DateTime SIGNEDTIME { get; set; } // ngày ký, ngày phát hành
        public string NOTE { get; set; } // Ghi chú khách hàng

        public int USINGINVOICETYPE { get; set; } = ConfigHelper.OrgUsingInvoiceType;

        public string PRODUCTNAME { get; set; }
        public double TAXRATE { get; set; }
        public double QUANTITY { get; set; }
        public string QUANTITYUNIT { get; set; }
        public double RETAILPRICE { get; set; }
        public double TOTALMONEY { get; set; }
        public double TOTALTAX { get; set; }
        public double TOTALPAYMENT { get; set; }

        public double STT { get; set; } // số thứ tự
        public string INVOICECODE { get; set; }
    }

    public class InvoiceDetailBO
    {
        public string PRODUCTNAME { get; set; }
        public double TAXRATE { get; set; }
        public double QUANTITY { get; set; }
        public string QUANTITYUNIT { get; set; }
        public double RETAILPRICE { get; set; }
        public double TOTALMONEY { get; set; }
        public double TOTALTAX { get; set; }
        public double TOTALPAYMENT { get; set; }
    }

    public class TokenModel
    {
        public string taxCode { get; set; }
        public string appId { get; set; }
    }

    public class data
    {
        public token token { get; set; }
    }

    public class token
    {
        public string RawData { get; set; }
    }
}
