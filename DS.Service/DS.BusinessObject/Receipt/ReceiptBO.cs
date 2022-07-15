﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Receipt
{
    public class ReceiptBO : BaseEntity
    {
        public long ID { get; set; }
        public int INVOICEID { get; set; }
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
        public decimal DISCOUNTMONEY { get; set; }
        public decimal TOTALPAYMENT { get; set; }
        public int INVOICESTATUS { get; set; }
        public string INVOICESTATUSNAME { get; set; }
        public int PAYMENTSTATUS { get; set; }
        public string PAYMENTSTATUSNAME { get; set; }
        public int INVOICETYPE { get; set; }
        public string INVOICETYPENAME { get; set; }
        public DateTime INITTIME { get; set; }
        public DateTime UPDATEDTIME { get; set; }
        public DateTime SIGNTIME { get; set; }
        public DateTime SIGNEDTIME { get; set; }
        public bool ISSELECTED { get; set; }
        public bool ISEMAILED { get; set; }
        public string SIGNLINK { get; set; }
        public string SIGNEDLINK { get; set; }
        public string SIGNEDXML { get; set; }
        public string PREVIEWLINK { get; set; }
        public long REFERENCE { get; set; }
        public string CANCELEDLINK { get; set; }
        public string CANCELREASON { get; set; }
        public DateTime CANCELTIME { get; set; }
        public string STRCANCELTIME { get; set; }
        public long TOTALROW { get; set; }
        public string STRLISTPRODUCT { get; set; }
        public string REFERENCECODE { get; set; }
        public string CHECKSUMXML { get; set; }
        public bool ISINVOICEWAITING { get; set; }
        public string STRINVOICEWAITINGTIME { get; set; }
        public DateTime INVOICEWAITINGTIME { get; set; }
        public string CONVERTUSERNAME { get; set; }
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

        /// <summary>
        /// Tên trạng thái biên bản hóa đơn
        /// </summary>
        public string REPORTSTATUSNAME { get; set; }

        /// <summary>
        /// Kiểu định dạng file pdf
        /// A4 hoặc A5
        /// </summary>
        public string PdfPageSize { get; set; }

        public string CUSTOMERCODE { get; set; }
        public int USINGINVOICETYPE { get; set; }
    }
}
