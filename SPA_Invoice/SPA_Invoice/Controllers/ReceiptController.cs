using DS.BusinessLogic.Customer;
using DS.BusinessLogic.EmailSender;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Receipt;
using DS.BusinessLogic.Report;
using DS.BusinessObject.Customer;
using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.BusinessObject.Receipt;
using DS.BusinessObject.Report;
using DS.Common.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;

namespace SPA_Invoice.Controllers
{
    public class ReceiptController : BaseController
    {
        // GET: Receipt
        public ActionResult Index()
        {
            return PartialView();
        }

        /// <summary>
        /// truongnv 20200307
        /// Lấy báo cáo thống kê
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDashboard()
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.Now.Date;

                InvoiceBLL objInvoiceBLL = new InvoiceBLL();

                List<InvoiceBO> lst = objInvoiceBLL.GetInvoiceByDate(objUser.COMTAXCODE, fromDate, toDate);
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }

                //Invoice money
                var TotalMoneyNotPay = lst.Where(x => x.PAYMENTSTATUS == 1).Sum(x => x.TOTALPAYMENT);   //Chưa thanh toán
                var TotalMoneyPaied = lst.Where(x => x.PAYMENTSTATUS == 2).Sum(x => x.TOTALPAYMENT);    //Đã thanh toán
                var TotalMoneyNotApproval = lst.Where(x => x.INVOICESTATUS == 1).Sum(x => x.TOTALPAYMENT); //Chưa duyệt

                var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                var lastDay = DateTime.Now;
                var totalInvoiceSigned = objInvoiceBLL.GetInvoiceCountSigned(objUser.COMTAXCODE, firstDay, lastDay,objUser.USINGINVOICETYPE);
                return Json(new { rs = true, TotalMoneyNotPay, TotalMoneyPaied, TotalMoneyNotApproval, totalInvoiceSigned });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được danh sách", objEx, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = "Lỗi khi lấy thông tin sản phẩm." });
            }
        }

        /// <summary>
        /// truongnv 20200307
        /// Lấy danh sách biên lai phân trang
        /// </summary>
        /// <param name="form"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetReceiptPaging(FormSearchInvoice form, int currentPage, int itemPerPage)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.Now.Date;
                if (form.TIME != null)
                {
                    string[] d = form.TIME.Split(';');
                    fromDate = DateTime.ParseExact(d[0], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                    toDate = DateTime.ParseExact(d[1], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                }
                form.FROMDATE = string.IsNullOrEmpty(fromDate.ToString("dd/MM/yyyy")) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(fromDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(toDate.ToString("dd/MM/yyyy")) ? DateTime.Now : DateTime.ParseExact(toDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.ITEMPERPAGE = itemPerPage;
                form.CURRENTPAGE = currentPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                ReceiptBLL oBLL = new ReceiptBLL();

                List<InvoiceBO> result = oBLL.GetReceiptPaging(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.NOTE))
                    {
                        item.NOTE = item.NOTE.Replace("<br />", "\n");
                    }
                    if (!string.IsNullOrEmpty(item.CANCELEDLINK))
                        item.SIGNLINK = item.CANCELEDLINK;
                    else if (!string.IsNullOrEmpty(item.SIGNEDLINK))
                        item.SIGNLINK = item.SIGNEDLINK;

                    // Kiểm tra hóa đơn điều chỉnh (invoiceBeModified), hóa đơn điều chỉnh (item) tham chiếu tới hóa đơn bị điều chỉnh qua item.REFERENCE = invoiceBeModified.ID của hóa đơn bị điều chỉnh
                    // => Check tồn tại biên bản cho hóa đơn bị điều chỉnh chưa invoiceBeModified
                    if (item.INVOICETYPE == 5)
                    {
                        var invoiceBeModified = result.Where(x => x.ID == item.REFERENCE).FirstOrDefault();
                        if (invoiceBeModified != null && invoiceBeModified.INVOICETYPE == 1)
                        {
                            invoiceBeModified.ISEXISTMODIFIEDREPORT = "1";
                        }
                    }
                }

                if (oBLL.ResultMessageBO.IsError)
                {
                    return Json(new { rs = false, msg = "Lỗi" });
                }

                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// lấy hóa đơn theo trạng thái
        /// </summary>
        /// <param name="form"></param>
        /// <param name="type"></param>
        /// <param name="intpage"></param>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public ActionResult GetReceiptByStatus(FormSearchInvoice form, int type, int intpage, int reportType)
        {
            try
            {
                form.ITEMPERPAGE = 10;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.CURRENTPAGE = intpage;
                form.INVOICETYPE = type;
                form.REPORTYPE = reportType;

                form.FROMDATE = string.IsNullOrEmpty(form.STRFROMDATE) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(form.STRFROMDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(form.STRTODATE) ? DateTime.Now : DateTime.ParseExact(form.STRTODATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<InvoiceBO> result = invoiceBLL.GetInvoiceByStatus(form);

                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;

                long TotalPages = 1;
                if (TotalRow % form.ITEMPERPAGE == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / form.ITEMPERPAGE.Value;
                else
                    TotalPages = TotalRow / form.ITEMPERPAGE.Value + 1;

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy được danh sách theo trang thái", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceByStatus");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateFilePdfToView(long invoiceId)
        {
            ServiceResult serviceResult = SaveTemplateInvoice(invoiceId);
            if (serviceResult.ErrorCode == 1000)
                return Json(new { rs = false, msg = serviceResult.Message });

            return Json(new { rs = true, msg = serviceResult.Message });
        }

        public ActionResult GetReportByInvoiceIdReportType(long invoiceId, string reportType)
        {
            try
            {
                ReportBLL reportBLL = new ReportBLL();
                var result = reportBLL.GetReportByInvoiceIdReportType(invoiceId, reportType);
                if (reportBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(reportBLL.ResultMessageBO.Message, reportBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetReportByInvoiceIdReportType");
                    return Json(new { rs = false, msg = reportBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm biên bản", ex, MethodBase.GetCurrentMethod().Name, "UpdateReport");
                return Json(new { rs = false, msg = $"Biên bản đã tồn tại cho hóa đơn này!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportExcell(FormSearchInvoice form, string fieldsCookies)
        {
            try
            {
                InvoiceBO invoice = new InvoiceBO();
                ExcelInvoiceBO excelInvoiceBO = JsonConvert.DeserializeObject<ExcelInvoiceBO>(fieldsCookies);
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.Now.Date;
                if (form.TIME != null)
                {
                    string[] d = form.TIME.Split(';');
                    fromDate = DateTime.ParseExact(d[0], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                    toDate = DateTime.ParseExact(d[1], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                }
                form.FROMDATE = string.IsNullOrEmpty(fromDate.ToString("dd/MM/yyyy")) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(fromDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(toDate.ToString("dd/MM/yyyy")) ? DateTime.Now : DateTime.ParseExact(toDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = null;
                form.ITEMPERPAGE = null;

                List<InvoiceBO> result = invoiceBLL.GetInvoice(form);

                string strFileName = "Danh_sach_hoa_don_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                return PushFileCustomizationForInvoice(result, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách hóa đơn.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Không thể xuất danh sách hóa đơn." });
            }
        }

        public ActionResult AddReport(ReportBO report)
        {
            try
            {
                string msg = SaveReport(report);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                else
                    return Json(new { rs = true, msg = "Lưu biên bản thành công." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm biên bản", ex, MethodBase.GetCurrentMethod().Name, "AddReport");
                return Json(new { rs = false, msg = $"Biên bản đã tồn tại cho hóa đơn này!" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// truongnv 20200410
        /// Xóa hóa đơn
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        public ActionResult RemoveInvoice(string idInvoices)
        {
            try
            {
                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<string> arr = new List<string>();
                for (int i = 0; i < lstInvoiceid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstInvoiceid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                string msg = invoiceBLL.DeleteInvoice(ids);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = $"Xóa thành công {lstInvoiceid.Length}/{lstInvoiceid.Length} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemoveInvoice");
            }

            return Json(new { rs = true, msg = "Xóa thành công" });
        }

        /// <summary>
        /// Lưu và ký biên bản hóa đơn
        /// truongnv 20200218
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public ActionResult SaveAndSignDocumentPDF(ReportBO report)
        {
            try
            {
                string msg = SaveReport(report, true);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                else
                    return Json(new { rs = true, msg = "Lưu biên bản thành công." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm biên bản", ex, MethodBase.GetCurrentMethod().Name, "SaveAndSignDocumentPDF");
                return Json(new { rs = false, msg = $"Biên bản đã tồn tại cho hóa đơn này!" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Lưu biên bản hủy hóa đơn
        /// truongnv 20200218
        /// </summary>
        /// <param name="report">dữ liệu báo cáo</param>
        /// <param name="isSigned">cờ kiểm tra xem hành xử người dùng là gì (false: Lưu thông thường, true: lưu và ký)</param>
        /// <returns></returns>
        private string SaveReport(ReportBO report, bool isSigned = false)
        {
            string msg = string.Empty;
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                ReportBLL reportBLL = new ReportBLL();
                var invoice = invoiceBLL.GetInvoiceById(report.INVOICEID);

                report.REPORTSTATUS = isSigned == true ? 1 : 0;
                if (report.REPORTTYPE == "1")
                {
                    var linkReport = invoiceBLL.GenerateReportToPdf(report, isSigned);
                    if (!string.IsNullOrEmpty(linkReport))
                    {
                        // Kiểm tra hàm lưu và ký có tồn tại biên bản chưa. Có => Ký. Chưa => Lưu và ký
                        report.LINK = linkReport;
                        if (report.ID == 0)
                        {
                            var result = reportBLL.AddReport(report);
                        }
                        else
                        {
                            var result = reportBLL.UpdateReport(report);
                        }
                        // update cancel reason to pm_invoice in the same time if create report cancel first
                        invoice.CANCELREASON = report.REASON;
                        invoiceBLL.UpdateCancelInvoice(invoice);
                        if (reportBLL.ResultMessageBO.IsError)
                        {
                            ConfigHelper.Instance.WriteLog(reportBLL.ResultMessageBO.Message, reportBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReport");
                            msg = reportBLL.ResultMessageBO.Message;
                        }
                    }
                    else
                    {
                        ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "CreateReport");
                        msg = "Không lưu được biên bản vào thư mục.";
                    }
                }
                else
                {
                    var linkReport = invoiceBLL.GenerateReportToPdf(report, isSigned);
                    if (!string.IsNullOrEmpty(linkReport))
                    {
                        // Kiểm tra hàm lưu và ký có tồn tại biên bản chưa. Có => Ký. Chưa => Lưu và ký
                        report.LINK = linkReport;
                        if (report.ID == 0)
                        {
                            var result = reportBLL.AddReport(report);
                        }
                        else
                        {
                            var result = reportBLL.UpdateReport(report);
                        }
                        // update change reason to pm_invoice in the same time if create report cancel first
                        invoice.CHANGEREASON = report.REASON;
                        invoiceBLL.UpdateModifiedInvoice(invoice);
                        if (reportBLL.ResultMessageBO.IsError)
                        {
                            ConfigHelper.Instance.WriteLog(reportBLL.ResultMessageBO.Message, reportBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReport");
                            msg = reportBLL.ResultMessageBO.Message;
                        }
                    }
                    else
                    {
                        ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "CreateReport");
                        msg = "Không lưu được biên bản vào thư mục.";
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm biên bản", ex, MethodBase.GetCurrentMethod().Name, "AddReport");
                msg = "Biên bản đã tồn tại cho hóa đơn này.";
            }
            return msg;
        }

        public ActionResult UploadFileReport(FormCollection collection)
        {
            var id = collection[0];
            var comTaxCode = collection[1];
            try
            {
                string root = ConfigurationManager.AppSettings["ImageFolder"];

                string dstFilePath = string.Format("/{0}/{1}/{2}/", root, comTaxCode, id);
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var phyDstFilePath = Server.MapPath("~/" + dstFilePath);
                        DirectoryInfo dir = new DirectoryInfo(phyDstFilePath);
                        if (!dir.Exists)
                        {
                            dir.Create();
                        }

                        var fileName = Path.GetFileName(fileContent.FileName);
                        string sav = Path.Combine(phyDstFilePath, fileName);

                        InvoiceBO invoice = new InvoiceBO
                        {
                            ID = int.Parse(id),
                            COMTAXCODE = objUser.COMTAXCODE,
                            ATTACHMENTFILELINK = dstFilePath + fileName
                        };

                        fileContent.SaveAs(sav);
                        InvoiceBLL invoiceBLL = new InvoiceBLL();
                        var result = invoiceBLL.UpdateAttachFileLink(invoice);
                        if (invoiceBLL.ResultMessageBO.IsError)
                        {
                            ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UploadFileReport");
                            return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                        }
                    }
                }
                return Json(new { rs = true, dstFilePath });
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải lên được file";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "UploadFileReport");
                return Json(new { rs = false, msg });
            }
        }

        public ActionResult DownloadFileReport(long invoiceId)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.GetInvoiceById(invoiceId);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "DownloadFileReport");
                }
                var file = result.ATTACHMENTFILELINK.Split('/');

                var phyDstFilePath = Server.MapPath("~/" + result.ATTACHMENTFILELINK);

                byte[] fileBytes = System.IO.File.ReadAllBytes(phyDstFilePath);
                string fileName = file.Last();
                return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "UploadFileReport");
                return Json(new { rs = false, msg });

            }
        }

        public ActionResult RemoveFileReport(InvoiceBO invoice)
        {
            try
            {
                string fullPath = Request.MapPath("~/" + invoice.ATTACHMENTFILELINK);
                invoice.ATTACHMENTFILELINK = null;
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.UpdateAttachFileLink(invoice);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "RemoveFileReport");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                if (result)
                {
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    return Json(new { rs = true, msg = "Xóa file thành công" });
                }
                else
                {
                    return Json(new { rs = false, msg = "Lỗi khi xóa file" });
                }
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi khi xóa file";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "RemoveFileReport");
                return Json(new { rs = false, msg });
            }
        }

        /// <summary>
        /// Lấy danh sách email gửi của 1 hóa đơn
        /// </summary>
        /// <param name="id">id hóa đơn trong DB</param>
        /// <returns></returns>
        public ActionResult GetEmailHistoryByInvoiceId(long id)
        {
            try
            {
                EmailBLL emailBLL = new EmailBLL();
                var result = emailBLL.GetEmailHistoryByInvoiceId(id);
                if (emailBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(emailBLL.ResultMessageBO.Message, emailBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetEmailHistoryByInvoiceId");
                    return Json(new { rs = false, msg = emailBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được danh sách lịch sử email.", ex, MethodBase.GetCurrentMethod().Name, "GetEmailHistoryByInvoiceId");
                return Json(new { rs = false, msg = $"Lỗi không lấy được danh sách lịch sử email." }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SendCancelEmail(InvoiceBO invoice)
        {
            try
            {
                EmailBO email = new EmailBO()
                {
                    INVOICEID = invoice.ID,
                    STATUS = 1,
                    MAILTO = invoice.CUSEMAIL,
                    RECIEVERNAME = "Thông báo phát hành Hóa đơn điện tử - " + invoice.COMNAME,
                    MAILTYPE = "Huy",
                    ATTACHEMENTLINK = invoice.SIGNLINK,
                    COMTAXCODE = invoice.COMTAXCODE,
                };
                EmailBLL emailBLL = new EmailBLL();
                var emailID = emailBLL.AddEmail(email);
                invoice.EMAILID = emailID;

                EmailData emailData = new EmailData()
                {
                    Username = ConfigurationManager.AppSettings["UsernameEmail"],
                    Password = ConfigurationManager.AppSettings["PasswordEmail"],
                    Host = "smtp.gmail.com",
                    Port = 587,
                    MailTo = invoice.CUSEMAIL,
                    Subject = invoice.COMNAME + " gửi thông báo xóa bỏ hóa đơn điện tử số " + invoice.NUMBER + " cho " + invoice.CUSNAME
                };
                EmailSender.MailInvoiceSender(emailData, invoice, 2);
                return Json(new { rs = true, msg = "Gửi mail thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi gửi email!", ex, MethodBase.GetCurrentMethod().Name, "SendEmail");
                return Json(new { rs = false, msg = "Gửi mail không thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddReceipt(ReceiptBO invoice, int type)
        {
            string msg = string.Empty;
            bool success = false;
            try
            {
                invoice.INVOICESTATUS = 1;
                invoice.COMNAME = objUser.COMNAME;
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                invoice.COMADDRESS = objUser.COMADDRESS;
                invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
                invoice.TOTALPAYMENT = invoice.TOTALMONEY;
                invoice.CUSNAME = invoice.CUSNAME == null ? objUser.COMNAME : invoice.CUSNAME;
                invoice.CUSTOMERCODE = invoice.CUSID;
                invoice.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                /*
                 * kiểm tra đã tồn tại học sinh này trong hệ thống chưa? nếu chưa thì thêm mới ngược lại cập nhật
                 * truongnv 20200219
                 */
                if (!string.IsNullOrEmpty(invoice.CUSTAXCODE) && !string.IsNullOrEmpty(invoice.CUSNAME))
                {
                    CustomerBO customerBO = new CustomerBO()
                    {
                        CUSID = invoice.CUSTAXCODE,
                        COMTAXCODE = invoice.COMTAXCODE,
                        CUSTAXCODE = invoice.CUSTAXCODE,
                        CUSBUYER = invoice.CUSBUYER == null ? invoice.CUSNAME : invoice.CUSBUYER,
                        CUSNAME = invoice.CUSNAME,
                        CUSADDRESS = invoice.CUSADDRESS,
                        CUSPHONENUMBER = invoice.CUSPHONENUMBER,
                        CUSEMAIL = invoice.CUSEMAIL,
                        CUSPAYMENTMETHOD = invoice.CUSPAYMENTMETHOD,
                        CUSBANKNAME = invoice.CUSBANKNAME,
                        CUSACCOUNTNUMBER = invoice.CUSACCOUNTNUMBER
                    };

                    CustomerBLL customerBLL = new CustomerBLL();
                    msg = customerBLL.CheckCustomerDuplicateTaxCode(customerBO);
                    if (msg.Length == 0)
                    {
                        success = customerBLL.AddCustomer(customerBO);
                        if (!success)
                            return Json(new { rs = false, msg = "Không thêm mới được khách hàng." });
                    }
                }

                invoice.INVOICEWAITINGTIME = invoice.STRINVOICEWAITINGTIME != null ? DateTime.ParseExact(invoice.STRINVOICEWAITINGTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;

                ReceiptBLL receiptBLL = new ReceiptBLL();
                long invoiceId = 0;
                msg = receiptBLL.AddReceipt(invoice, out invoiceId);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm hóa đơn thu học phí", ex, MethodBase.GetCurrentMethod().Name, "AddReceipt");
                return Json(new { rs = false, msg = $"Lỗi tạo hóa đơn" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateReceipt(ReceiptBO invoice, int type)
        {
            long invoiceId = 0;
            bool success = false;
            string msg = string.Empty;
            try
            {
                invoice.TOTALPAYMENT = invoice.TOTALMONEY;
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                invoice.DUEDATE = DateTime.ParseExact(invoice.STRDUEDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                invoice.INVOICEWAITINGTIME = invoice.STRINVOICEWAITINGTIME != null ? DateTime.ParseExact(invoice.STRINVOICEWAITINGTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                invoice.CUSNAME = invoice.CUSNAME == null? objUser.COMNAME : invoice.CUSNAME;
                invoice.CUSTOMERCODE = invoice.CUSID;
                /*
                 * kiểm tra đã tồn tại học sinh này trong hệ thống chưa? nếu chưa thì thêm mới ngược lại cập nhật
                 * truongnv 20200219
                 */
                if (!string.IsNullOrEmpty(invoice.CUSTAXCODE) && !string.IsNullOrEmpty(invoice.CUSNAME))
                {
                    CustomerBO customerBO = new CustomerBO()
                    {
                        CUSID = invoice.CUSTAXCODE,
                        COMTAXCODE = invoice.COMTAXCODE,
                        CUSTAXCODE = invoice.CUSTAXCODE,
                        CUSBUYER = invoice.CUSBUYER == null ? invoice.CUSNAME : invoice.CUSBUYER,
                        CUSNAME = invoice.CUSNAME,
                        CUSADDRESS = invoice.CUSADDRESS,
                        CUSPHONENUMBER = invoice.CUSPHONENUMBER,
                        CUSEMAIL = invoice.CUSEMAIL,
                        CUSPAYMENTMETHOD = invoice.CUSPAYMENTMETHOD,
                        CUSBANKNAME = invoice.CUSBANKNAME,
                        CUSACCOUNTNUMBER = invoice.CUSACCOUNTNUMBER
                    };

                    CustomerBLL customerBLL = new CustomerBLL();
                    msg = customerBLL.CheckCustomerDuplicateTaxCode(customerBO);
                    if (msg.Length == 0)
                    {
                        success = customerBLL.AddCustomer(customerBO);
                        if (!success)
                            return Json(new { rs = false, msg = "Không thêm mới được khách hàng." });
                    }
                }

                ReceiptBLL receiptBLL = new ReceiptBLL();
                msg = receiptBLL.UpdateReceipt(invoice, out invoiceId);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "UpdateReceipt");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Độc số tiền thành chữ
        /// truongnv 20200309
        /// </summary>
        /// <param name="number"></param>
        /// <param name="kindOfMoney"></param>
        /// <returns></returns>
        public JsonResult ReadNumberToWords(decimal? number, string kindOfMoney)
        {
            decimal money = 0;
            try
            {
                if (number == null)
                    return Json(new { rs = true, data = string.Empty });
                else
                    decimal.TryParse(number.ToString(), out money);

                var obj = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(money, kindOfMoney);
                return Json(new { rs = true, data = obj });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi chuyển đổi số thành chữ số", objEx, MethodBase.GetCurrentMethod().Name, "ReadNumberToWords");
                return Json(new { rs = false, reponseText = "Không thể chuyển số thành chữ số. Vui lòng thử lại!" });
            }
        }
    }
}