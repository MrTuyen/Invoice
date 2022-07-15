using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Meter;
using DS.BusinessObject.Invoice;
using DS.Common.Enums;
using DS.Common.Helpers;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;

namespace SPA_Invoice.Controllers
{
    /// <summary>
    /// Controller phục vụ việc gọi từ trang tra cứu để thực hiện renderview sau khi thực hiện chuyển đổi sang hóa đơn giấy
    /// </summary>
    public class InvoiceConvertController : BaseController
    {
        // GET: InvoiceConvert
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tra cứu mã hóa đơn
        /// </summary>
        /// <param name="id">Mã tra cứu</param>
        /// <param name="fullName">Họ tên người chuyển đổi</param>
        /// CreatedBy: truongnv
        /// CreatedDate: 20200211
        /// <returns></returns>
        public ActionResult ConvertInvoice(long id, string fullName, InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();
                //var result = objInvoiceBLL.UpdateConvertInvoice(id, fullName, 4, "");

                //Lấy hóa đơn theo mã hóa đơn
                var objInvoice = objInvoiceBLL.GetInvoiceById(id, invoice.USINGINVOICETYPE);
                objInvoice.LISTPRODUCT = objInvoiceBLL.GetInvoiceDetail(id);

                //Lấy loại hóa đơn
                var invoiceType = objGlobalBLL.GetInvoiceTypeByID(4);

                //Gán biến là hóa đơn chuyển đổi để render ra mẫu hóa đơn chuyển đổi
                objInvoice.ISCONVERTED = true;
                objInvoice.CONVERTTIME = DateTime.Now;
                objInvoice.CONVERTUSERNAME = fullName;
                objInvoice.INVOICETYPENAME = invoiceType.INVOICETYPENAME;
                //Gán QRCode cho trường QRCODEBASE64
                objInvoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + objInvoice.REFERENCECODE);            

                if (objInvoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENNUOC)
                {
                    string fileConvertedPath = "";
                    string htmlContent = GetContentViewString(objInvoice, objInvoice.NUMBER);
                    long invoiceid;
                    RenderHtmlToPdf(objInvoice, htmlContent, out fileConvertedPath, out invoiceid);

                    return Json(new { rs = true, msg = fileConvertedPath, data = fileConvertedPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Convert View to string
                    var objGenerateInvoice = GetInvoiceContent(objInvoice, objInvoice.NUMBER);
                    if (objGenerateInvoice != null)
                    {
                        string fileName = objInvoice.COMTAXCODE + "_" + invoice.ID + "_" + objInvoice.INVOICETYPE + ".pdf";
                        //string fileConvertedPath = objInvoiceBLL.SaveInvoiceFile(objGenerateInvoice, objInvoice, fileName);
                        string fileConvertedPath = SaveInvoiceFile(objGenerateInvoice, objInvoice, fileName);
                        if (!objInvoiceBLL.ResultMessageBO.IsError)
                        {
                            return Json(new { rs = true, msg = fileConvertedPath, data = fileConvertedPath }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ConfigHelper.Instance.WriteLog("Lỗi chuyển đổi string sang pdf", "", MethodBase.GetCurrentMethod().Name, "InvoiceConvert");
                        return Json(new { rs = false, msg = $"Không thể chuyển đổi hóa đơn, vui lòng thử lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi tìm hóa đơn theo mã tra cứu", objEx, MethodBase.GetCurrentMethod().Name, "SearchCode");
                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreatePdfInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                var objInvoice = objInvoiceBLL.GetInvoiceById(invoice.ID, invoice.USINGINVOICETYPE);

                if (invoice == null || invoice.ID == 0)
                {
                    return Json(new { rs = false, msg = "Không tạo được file Pdf." }, JsonRequestBehavior.AllowGet);
                }
                bool pageSizeA5 = true;
                switch (objInvoice.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        //case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        pageSizeA5 = false;
                        break;
                }

                string fileConvertedPath = "";
                if (pageSizeA5)
                {
                    var temp = GetInvoiceContent(objInvoice, objInvoice.NUMBER); // GetInvoiceImportContent(invoice, invoice.NUMBER);
                    if (temp == null)
                    {
                        return Json(new { rs = false, msg = "Lỗi lấy nội dung file và đơn và tạo file Pdf." }, JsonRequestBehavior.AllowGet);
                    }

                    //Lưu file vào thư mục
                    string fileName = objInvoice.COMTAXCODE + "_" + objInvoice.ID + "_" + objInvoice.INVOICETYPE + ".pdf";
                    fileConvertedPath = SaveInvoiceFile(temp, objInvoice, fileName);
                    if (string.IsNullOrEmpty(fileConvertedPath))
                    {
                        return Json(new { rs = false, msg = "Lỗi lưu file hóa đơn vào thư mục." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string htmlContent = GetContentViewString(objInvoice, objInvoice.NUMBER);
                    RenderHtmlToPdf(objInvoice, htmlContent, out fileConvertedPath, out long invoiceid);
                    if (string.IsNullOrWhiteSpace(fileConvertedPath))
                    {
                        return Json(new { rs = false, msg = "Lỗi lấy nội dung file và đơn và tạo file Pdf." }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { rs = true, msg = fileConvertedPath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi tìm hóa đơn theo mã tra cứu", objEx, MethodBase.GetCurrentMethod().Name, "SearchCode");
                return Json(new { rs = false, msg = "Không tìm thấy hóa đơn theo mã tra cứu." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}