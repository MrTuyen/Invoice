using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice
{
    public class InvoiceBO : BaseEntity
    {
        public long ID { get; set; }
        public int INVOICEID { get; set; }
        public int SIGNINDEX { get; set; }// thứ tự ký
        public string COMUSERNAME { get; set; }
        public string COMNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public string COMADDRESS { get; set; }
        public string COMPHONENUMBER { get; set; }
        public string COMACCOUNTNUMBER { get; set; }
        public string COMBANKNAME { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public long NUMBER { get; set; }
        public string CUSNAME { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSBUYER { get; set; }
        public string CUSEMAIL { get; set; }
        public string CUSPHONENUMBER { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSPAYMENTMETHOD { get; set; }
        public string CUSACCOUNTNUMBER { get; set; }
        public string CUSBANKNAME { get; set; }
        public int TERMID { get; set; }
        public DateTime DUEDATE { get; set; }
        public string STRDUEDATE { get; set; }
        public string DISCOUNTTYPE { get; set; }
        public decimal TOTALMONEY { get; set; }
        public decimal TAXMONEY { get; set; }
        public decimal OTHERTAXFEE { get; set; }
        public decimal REFUNDFEE { get; set; }
        public decimal SERVICEFEE { get; set; }
        public int SERVICEFEETAXRATE { get; set; }
        public decimal SERVICEFEETAX { get; set; }
        public decimal TOTALSERVICEFEE { get; set; }
        public decimal DISCOUNTMONEY { get; set; }
        public decimal TOTALPAYMENT { get; set; }
        public int INVOICESTATUS { get; set; }
        public int INVOICEWATING { get; set; }
        public string INVOICESTATUSNAME { get; set; }
        public int PAYMENTSTATUS { get; set; }
        public string PAYMENTSTATUSNAME { get; set; }
        public int INVOICETYPE { get; set; }
        public string INVOICETYPENAME { get; set; }
        public DateTime INITTIME { get; set; }
        public DateTime CREATEDTIME { get { return INITTIME; } }
        public DateTime UPDATEDTIME { get; set; }
        public DateTime SIGNTIME { get; set; }
        public DateTime SIGNEDTIME { get; set; }
        public bool ISSELECTED { get; set; }
        public bool ISEMAILED { get; set; }
        public string SIGNLINK { get; set; }
        public string LINKVIEW { get { return SIGNLINK; } }
        public string SIGNEDLINK { get; set; }
        public string SIGNEDXML { get; set; }
        public string PREVIEWLINK { get; set; }
        public long REFERENCE { get; set; }
        public string CANCELEDLINK { get; set; }
        public string CANCELREASON { get; set; }
        public DateTime CANCELTIME { get; set; }
        public string STRCANCELTIME { get; set; }
        public long TOTALROW { get; set; }

        public List<string> TOTALSIGNED { get; set; }

        public List<string> TOTALWAIT { get; set; }
        public List<string> TOTALRECORD { get; set; }
        public string STRLISTPRODUCT { get; set; }
        public string REFERENCECODE { get; set; }
        public string CHECKSUMXML { get; set; }
        public bool ISINVOICEWAITING { get; set; }
        public string STRINVOICEWAITINGTIME { get; set; }
        public DateTime INVOICEWAITINGTIME { get; set; }

        public string CONVERTUSERNAME { get; set; }
        public bool ISCONVERTED { get; set; }
        public DateTime CONVERTTIME { get; set; }
        public DateTime CHANGETYPE { get; set; }
        public string ISEXISTCANCELREPORT { get; set; }
        public string ISEXISTMODIFIEDREPORT { get; set; }
        public string MODIFIEDLINK { get; set; }
        public string CANCELLINK { get; set; }
        public string CHANGEREASON { get; set; }
        public string COMLEGALNAME { get; set; }
        public string COMLOGO { get; set; }
        public string BGIMAGE { get; set; }
        public string TEMPLATEPATH { get; set; }
        public string ATTACHMENTFILELINK { get; set; }
        public long IDTEMP { get; set; }
        public int INVOICEIDTEMP { get; set; }
        public string COMUSERNAMETEMP { get; set; }
        public string COMNAMETEMP { get; set; }
        public string COMTAXCODETEMP { get; set; }
        public string COMADDRESSTEMP { get; set; }
        public string COMPHONENUMBERTEMP { get; set; }
        public string FORMCODETEMP { get; set; }
        public string SYMBOLCODETEMP { get; set; }
        public long NUMBERTEMP { get; set; }
        public string CUSNAMETEMP { get; set; }
        public string CUSADDRESSTEMP { get; set; }
        public string CUSBUYERTEMP { get; set; }
        public string CUSEMAILTEMP { get; set; }
        public string CUSPHONENUMBERTEMP { get; set; }
        public string CUSTAXCODETEMP { get; set; }
        public string CUSPAYMENTMETHODTEMP { get; set; }
        public string CUSACCOUNTNUMBERTEMP { get; set; }
        public string CUSBANKNAMETEMP { get; set; }
        public int TERMIDTEMP { get; set; }
        public DateTime DUEDATETEMP { get; set; }
        public string STRDUEDATETEMP { get; set; }
        public string DISCOUNTTYPETEMP { get; set; }
        public decimal TOTALMONEYTEMP { get; set; }
        public decimal TAXMONEYTEMP { get; set; }
        public decimal DISCOUNTMONEYTEMP { get; set; }
        public decimal TOTALPAYMENTTEMP { get; set; }
        public int INVOICESTATUSTEMP { get; set; }
        public string INVOICESTATUSNAMETEMP { get; set; }
        public int PAYMENTSTATUSTEMP { get; set; }
        public string PAYMENTSTATUSNAMETEMP { get; set; }
        public int INVOICETYPETEMP { get; set; }
        public string INVOICETYPENAMETEMP { get; set; }
        public DateTime INITTIMETEMP { get; set; }
        public DateTime UPDATEDTIMETEMP { get; set; }
        public DateTime SIGNTIMETEMP { get; set; }
        public DateTime SIGNEDTIMETEMP { get; set; }
        
        public bool ISSELECTEDTEMP { get; set; }
        public bool ISEMAILEDTEMP { get; set; }
        public string SIGNLINKTEMP { get; set; }
        public string SIGNEDLINKTEMP { get; set; }
        public string SIGNEDXMLTEMP { get; set; }
        public string PREVIEWLINKTEMP { get; set; }
        public long REFERENCETEMP { get; set; }
        public string CANCELEDLINKTEMP { get; set; }
        public string CANCELREASONTEMP { get; set; }
        public DateTime CANCELTIMETEMP { get; set; }
        public string STRCANCELTIMETEMP { get; set; }
        public long TOTALROWTEMP { get; set; }
        public string STRLISTPRODUCTTEMP { get; set; }
        public string REFERENCECODETEMP { get; set; }
        public string CHECKSUMXMLTEMP { get; set; }
        public bool ISINVOICEWAITINGTEMP { get; set; }
        public string STRINVOICEWAITINGTIMETEMP { get; set; }
        public DateTime INVOICEWAITINGTIMETEMP { get; set; }
        public DateTime CONVERTTIMETEMP { get; set; }
        public DateTime CHANGETYPETEMP { get; set; }
        public DateTime MODIFIEDDATE { get; set; }
        public string ISEXISTCANCELREPORTTEMP { get; set; }
        public string ISEXISTMODIFIEDREPORTTEMP { get; set; }
        public string MODIFIEDLINKTEMP { get; set; }
        public string CANCELLINKTEMP { get; set; }
        public string CHANGEREASONTEMP { get; set; }
        public string COMLEGALNAMETEMP { get; set; }
        public string COMLOGOTEMP { get; set; }
        public string BGIMAGETEMP { get; set; }
        public string ATTACHMENTFILELINKTEMP { get; set; }
        public string NOTE { get; set; }
        public string CURRENCY { get; set; }
        public decimal EXCHANGERATE { get; set; }
        public long EMAILID { get; set; }
        public string CUSID { get; set; }
        public string QRCODEBASE64 { get; set; }

        public string NAMECONVERT { get; set; }

        public bool ISFREETRIAL { get; set; }
        /// <summary>
        /// Tên trạng thái biên bản hóa đơn
        /// </summary>
        public string REPORTSTATUSNAME { get; set; }

        /// <summary>
        /// Kiểu định dạng file pdf
        /// A4 hoặc A5
        /// </summary>
        public string PdfPageSize { get; set; }

        /// <summary>
        /// Khách hàng sử dụng loại hóa đơn là gì
        /// </summary>
        public int USINGINVOICETYPE { get; set; }

        public string USINGINVOICETYPETMP { get; set; }

        //public string[] USINGTYPE { get; set; }
        public string TotalAmountInWords { get; set; }

        // Các trường gán cho hóa đơn nhiều trang
        public int M_ISTAXRATE { get; set; }
        public bool M_ISMULTIPLEPAGE { get; set; }
        public decimal M_TOTALPAGES { get; set; }
        public decimal M_TOTALMONEYWITHOUTTAX { get; set; }
        public decimal M_TOTALDISCOUNT { get; set; }
        public decimal M_TOTALVATAMOUNT { get; set; }
        public decimal M_TOTALMONEY { get; set; }
        public decimal M_LSTKCT { get; set; }
        public decimal M_LSTPRODUCTSCOUNT { get; set; }
        public int M_INDEX { get; set; }

        // End

        /// <summary>
        /// Loại hóa đơn điều chỉnh tăng hay giảm
        /// 1: Tăng, 2: Giảm, 3: Điều chỉnh thông tin
        /// </summary>
        public int INVOICEMETHOD { get; set; }

        /// <summary>
        /// Kỳ thanh toán từ ngày
        /// </summary>
        public DateTime FROMDATE { get; set; }

        /// <summary>
        /// Kỳ thanh toán đến ngày
        /// </summary>
        public DateTime TODATE { get; set; }

        public string FROMDATESTR { get; set; }
        public string TODATESTR { get; set; }


        /// <summary>
        /// Số hộ
        /// </summary>
        public int APARTMENTNO { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CUSTOMERCODE { get; set; }

        public long SORTORDER { get; set; }

        public string CUSEMAILSEND { get; set; }

        public decimal CUSTOMFIELDEXCHANGERATE { get; set; }

        public decimal CUSTOMFIELDEXCHANGE { get; set; }

        //Đường dẫn file chứng chỉ
        public string CERTIFICATELINK { get; set; }

        #region Hỗ trợ làm tròn
        //Số lượng làm tròn
        public int QUANTITYPLACE { get; set; }

        public int PRICEPLACE { get; set; }

        public int MONEYPLACE { get; set; }
        #endregion

        #region Phiếu xuất kho
        public string DELIVERYORDERNUMBER { get; set; } // xuất kho tại
        public DateTime DELIVERYORDERDATE { get; set; } // ngày điều động
        public string DELIVERYORDERDATESTR { get; set; }
        public string FROMWAREHOUSENAME { get; set; } // xuất kho tại
        public string TOWAREHOUSENAME { get; set; } // nhập kho tại
        public string TRANSPORTATIONMETHOD { get; set; } // phương tiện vận chuyển
        public string CONTRACTNUMBER { get; set; } // hợp đồng số
        public string DELIVERYORDERCONTENT { get; set; } // về việc
        #endregion

        public bool ISIMPORTSUCCESS { get; set; } = true; // Kiểm tra import excel cho từng hóa đơn. Đủ điều kiện => true

        public string PMSMAT_INVOICEID { get; set; } // PMSMAT API

        public decimal TOTALQUANTITY { get; set; } // Tổng điện năng tiêu thụ

        /// <summary>
        /// Thông tin lưu mã code từ đối tác gọi API, tối đa 255 ký tự unicode
        /// </summary>
        public string INVOICECODE { get; set; }
        public DateTime AME_DATETIME0 { get; set; }
        public DateTime AME_DATETIME { get; set; }

        /// <summary>
        /// Truyền hình cab email cộng tác viên. Phân quyền hóa đơn cho cộng tác viên cụ thể theo email tài khoản của CTV trong doanh nghiệp
        /// </summary>
        public string PARTNEREMAIL { get; set; }
        /// <summary>
        /// Chiều cao header template, lấy từ hàm get_invoice phục vụ chia trang pdf
        /// </summary>
        public int HEADERTEMPLATE { get; set; }
        /// <summary>
        /// Chiều cao fotter template, lấy từ hàm get_invoice phục vụ chia trang pdf
        /// </summary>
        public int FOTTERTEMPLATE { get; set; }
        /// <summary>
        /// Số ký tự trên mỗi dòng sản phẩm
        /// </summary>
        public int CHARONROW { get; set; }

        public int RECORDPERPAGE { get; set; }

        public List<InvoiceDetailBO> LISTPRODUCT { get; set; }
        public List<InvoiceDetailBO> LISTPRODUCTTEMP { get; set; }
    }

    public class InvoiceDetailBO
    {
        public Int64 INVOICEID { get; set; }
        public string SKU { get; set; }
        public string PRODUCTNAME { get; set; }
        /* Định mức tiêu thụ */
        public decimal QUANTITY { get; set; } // Phiếu xuất kho: thực xuất

        /// <summary>
        /// Số lượng 
        /// </summary>
        public string QUANTITYUNIT { get; set; }
        public string QUANTITYUNITNAME { get; set; }
        public decimal RETAILPRICE { get; set; }
        public decimal SALEPRICE { get; set; }
        public bool ISPROMOTION { get; set; }
        public int TAXRATE { get; set; }
        public decimal DISCOUNTRATE { get; set; }
        public decimal TOTALMONEY { get; set; }
        public decimal TOTALDISCOUNT { get; set; }
        public decimal TOTALTAX { get; set; }
        public decimal OTHERTAXFEE { get; set; }
        public decimal REFUNDFEE { get; set; }
        public decimal TOTALPAYMENT { get; set; }
        public DateTime INITTIME { get; set; }
        //public string NOTE { get; set; }
        public bool ISDELETED { get; set; }

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

        /// <summary>
        /// Tên công tơ
        /// </summary>
        public string METERNAME { get; set; }

        /// <summary>
        /// Số hộ
        /// </summary>
        public int APARTMENTNO { get; set; }

        /// <summary>
        /// Phí bảo vệ môi trường đối với hóa đơn nước
        /// </summary>
        public int TAXRATEWATER { get; set; }

        public string DESCRIPTION { get; set; }

        /// <summary>
        /// Mã số xuất hàng
        /// </summary>
        public string CONSIGNMENTID { get; set; }

        /// <summary>
        /// dòng tiêu đề làm nhóm
        /// </summary>
        public int GROUPID { get; set; }

        #region Phiếu xuất kho
        public decimal INQUANTITY { get; set; } // thực nhập
        #endregion

    }

    public class InvoiceSearchFormBO
    {
        public string COMTAXCODE { get; set; }
        public string COMPANYNAME { get; set; }
        public string KEYWORD { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
        public int? STATUS { get; set; }
        public DateTime FROMTIME { get; set; }
        public DateTime TOTIME { get; set; }
    }

    public class InvoiceSummary
    {
        public int ID { get; set; }
        public string COMNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public bool ISFREETRIAL { get; set; }
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public Int64 FROMNUMBER { get; set; }
        public Int64 TONUMBER { get; set; }
        public Int64 TOTALNUMBER { get; set; }
        public string NUMBERSTATUSNAME { get; set; }
        public int NUMBERSTATUSID { get; set; }
        public Int64 USEDNUMBER { get; set; }
        public int TOTALROW { get; set; }
        public List<InvoiceSummaryDetail> ListSummaryDetail { get; set; }
        public long TOTALUSEDNUMBER { get; set; }

    }

    public class InvoiceSummaryDetail
    {
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public Int64 FROMNUMBER { get; set; }
        public Int64 TONUMBER { get; set; }
        public Int64 TOTALNUMBER { get; set; }
        public string NUMBERSTATUSNAME { get; set; }
        public int NUMBERSTATUSID { get; set; }
        public Int64 USEDNUMBER { get; set; }
    }

    public class CompanySummary
    {
        public int TOTAL { get; set; }
        public int FREETRIAL { get; set; }
        public int COUNTUSED { get; set; }
        public int WAITINGPUBLIC { get; set; }
        

    }

    public class ConvertResult1
    {
        public bool rs { get; set; }
        public string msg { get; set; }
    }
}
