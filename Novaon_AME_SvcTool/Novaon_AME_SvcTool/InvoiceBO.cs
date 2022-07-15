using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaon_AME_SvcTool
{
    public class InvoiceBO
    {
        public string AME_INVOICEID { get; set; }
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
        public string CUSBANKNAME { get; set; }
        public string CUSEMAIL { get; set; }

        public List<InvoiceDetailBO> LISTPRODUCT { get; set; }
        public DateTime INITTIME { get; set; } // ngày khởi tạo
        public DateTime SIGNEDTIME { get; set; } // ngày ký, ngày phát hành
        public DateTime AME_DATETIME0 { get; set; } // AME: ngày tạo chứng thư.
        public DateTime AME_DATETIME { get; set; } // AME: ngày chỉnh sửa chứng thư gần nhất. So sánh với AME_DATETIME0 để kiểm tra cập nhật thay vì thêm mới nếu hóa đơn chưa phát hành.
        public string NOTE { get; set; } // Ghi chú khách hàng

        public int USINGINVOICETYPE { get; set; } = ConfigHelper.OrgUsingInvoiceType;

        public string PRODUCTNAME { get; set; }
        public string MATHUE { get; set; }
        public decimal TAXRATE1 { get; set; }
        public int TAXRATE { get; set; }
        public decimal QUANTITY { get; set; }
        public string QUANTITYUNIT { get; set; }
        public decimal RETAILPRICE { get; set; }
        public decimal TOTALMONEY { get; set; }
        public decimal TOTALTAX { get; set; }
        public decimal TOTALPAYMENT { get; set; }

        public string STT { get; set; } // số thứ tự
        public string INVOICECODE { get; set; }
    }

    public class InvoiceDetailBO
    {
        public string PRODUCTNAME { get; set; }
        public decimal TAXRATE1 { get; set; }
        public int TAXRATE { get; set; }
        public decimal QUANTITY { get; set; }
        public string QUANTITYUNIT { get; set; }
        public decimal RETAILPRICE { get; set; }
        public decimal TOTALMONEY { get; set; }
        public decimal TOTALTAX { get; set; }
        public decimal TOTALPAYMENT { get; set; }
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
