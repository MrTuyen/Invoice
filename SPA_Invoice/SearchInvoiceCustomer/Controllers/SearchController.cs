using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Meter;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.Common.Enums;
using DS.Common.Helpers;
using DS.DataObject.Invoice;
using SPA_Invoice.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;

namespace SearchInvoiceCustomer.Controllers
{
    public class SearchController : BaseController
    {
        public static long total=0;
        public string comtaxcode = "";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetSearchCustomerID(string cusPhoneNumber, string customerCode, int currentPage, int itemPerPage,string fromdate,string todate,string comtaxcode)
        {
            try
            {
                int offset = (currentPage - 1) * itemPerPage;
                InvoiceBLL customerBLL = new InvoiceBLL();
                List<InvoiceBO> result = new List<InvoiceBO>();
                DateTime fromDate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result = customerBLL.GetSearchCustomerID(cusPhoneNumber, customerCode, currentPage, offset, fromDate, toDate, comtaxcode);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                total = Convert.ToInt64(TotalRow/12);
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = $"ngày không đúng định dạng" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SearchCode(string id)
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                var objInvoice = objInvoiceBLL.GetInvoiceByReferenceCode(id.ToUpper().Trim());
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }

                if (objInvoice != null)
                {
                    CreateFilePath(objInvoice);
                    return Json(new { rs = true, msg = objInvoice });
                }

                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi tìm hóa đơn theo mã tra cứu", objEx, MethodBase.GetCurrentMethod().Name, "SearchCode");
                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." });
            }
        }
        void CreateFilePath(DS.BusinessObject.Invoice.InvoiceBO objInvoice)
        {
            comtaxcode = objInvoice.COMTAXCODE;
            //Giữ lại đường dẫn trên DB
            objInvoice.PREVIEWLINK = objInvoice.SIGNLINK;
            //Tạo đường dẫn file xml, nếu là hóa đơn hủy thì lấy cho tải file hóa đơn gốc
            if (objInvoice.INVOICETYPE == 3)
            {
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;
                //objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-huy", "hoa-don-goc").Replace("_3.xml", "_1.xml");
                objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-xoa-bo", "hoa-don-goc").Replace("_3.xml", "_1.xml");
            }
            else if (objInvoice.INVOICETYPE == 4)
            {
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;
                objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-chuyen-doi", "hoa-don-goc").Replace("_4.xml", "_1.xml");
            }
            else
                objInvoice.SIGNEDXML = objInvoice.PREVIEWLINK != null ? objInvoice.PREVIEWLINK.Substring(0, objInvoice.PREVIEWLINK.Length - 4) + ".xml" : null;

            //Nếu là hóa đơn chuyển đổi thì vẫn phải tải xml từ folder hóa đơn gốc
            objInvoice.SIGNEDXML = objInvoice.SIGNEDXML.Replace("hoa-don-xoa-bo", "hoa-don-goc").Replace("_3.xml", "_1.xml");

            //Tạo url file pdf để view lên web
            objInvoice.SIGNEDLINK = objInvoice.PREVIEWLINK != null ? ConfigHelper.UriInvoiceFolder + objInvoice.PREVIEWLINK : null;
        }
    }
}