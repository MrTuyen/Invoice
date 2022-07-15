using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class GlobalBO
    {
        //thông tin chung của doanh nghiệp
        public string COMUSERNAME { get; set; }
        public string COMTAXCODE { get; set; }


        //danh mục sản phẩm
        public string CATEGORY { get; set; }


        //trạng thái hóa đơn
        public int INVOICESTATUSID { get; set; }
        public string INVOICESTATUSNAME { get; set; }


        //đơn vị tiền tệ
        public string CURRENCYUNIT { get; set; }


        //loại hóa đơn
        public int INVOICETYPEID { get; set; }
        public string INVOICETYPENAME { get; set; }


        //hình thức thanh toán
        public string PAYMENTMETHOD { get; set; }


        //trạng thái thanh toán
        public string PAYMENTSTATUS { get; set; }


        // đơn vị tính của sản phẩm 
        public string QUANTITYUNIT { get; set; }


        // mẫu số hóa đơn 
        public string FORMCODE { get; set; }

        // ảnh nền hóa đơn theo mẫu số
        public string BGIMAGE { get; set; }

        // ký hiệu hóa đơn 
        public string SYMBOLCODE { get; set; }

        public int ID { get; set; }
        public string NUMBERSTATUSNAME { get; set; }

        //Loại thuế suất
        public int TAXRATE { get; set; }
        //Trạng thái
        public bool ISACTIVE { get; set; }
        public bool ISACTIVED { get; set; }
    }
}
