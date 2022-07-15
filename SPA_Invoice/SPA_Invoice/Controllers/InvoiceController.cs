using DS.BusinessLogic.Customer;
using DS.BusinessLogic.EmailSender;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Number;
using DS.BusinessLogic.Product;
using DS.BusinessLogic.Report;
using DS.BusinessObject;
using DS.BusinessObject.Customer;
using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.BusinessObject.Product;
using DS.BusinessObject.Report;
using DS.Common.Enums;
using DS.Common.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SPA_Invoice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static DS.Common.Enums.EnumHelper;
using DataTable = System.Data.DataTable;
using Document = iTextSharp.text.Document;
using SelectPdf;
using DS.BusinessLogic.Account;
using DS.BusinessObject.Invoice.ReleaseDocument;
using DS.BusinessObject.Invoice.EntepriseModel;
using ZaloDotNetSDK;
using DS.BusinessLogic.Company;
using SPA_Invoice.Common;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;
using DS.BusinessObject.Account;
using System.Web.Security;

namespace SPA_Invoice.Controllers
{
    public class InvoiceController : BaseController
    {
        private static readonly object _locker = new object(); // lock object for multithreading
        public PartialViewResult Index()
        {
            return PartialView();
        }

        public PartialViewResult InvoiceWait()
        {
            return PartialView();
        }

        public JsonResult LoadDashboard()
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                var lastDay = DateTime.Now;

                List<InvoiceBO> lst = objInvoiceBLL.GetInvoiceAll(objUser.COMTAXCODE, firstDay, lastDay, objUser.USINGINVOICETYPE);
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "LoadDashboard");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }

                //Invoice money
                var TotalMoneyPaied = lst.Where(x => x.INVOICESTATUS == 2).Sum(x => x.TOTALPAYMENT);    //Đã thanh toán
                var TotalMoneyNotApproval = lst.Where(x => x.INVOICESTATUS == 1).Sum(x => x.TOTALPAYMENT); //Chưa duyệt

                var totalInvoiceSigned = objInvoiceBLL.GetInvoiceCountSigned(objUser.COMTAXCODE, firstDay, lastDay, objUser.USINGINVOICETYPE);

                return Json(new { rs = true, TotalMoneyPaied, TotalMoneyNotApproval, totalInvoiceSigned });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không tải được dashboard quản lý hóa đơn.", objEx, MethodBase.GetCurrentMethod().Name, "LoadDashboard");
                return Json(new { rs = false, msg = "Không tải được dashboard quản lý hóa đơn." });
            }
        }

        public ActionResult GetInvoice(FormSearchInvoice form, int currentPage, int itemPerPage)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.MaxValue;
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
                form.PARTNEREMAIL = objUser.ISADMIN ? null : objUser.EMAIL;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<InvoiceBO> result = invoiceBLL.GetInvoice(form);
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

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                //result = result.FindAll(x => x.USINGINVOICETYPE == objUser.USINGINVOICETYPE);
                ResultDataInvoice data = new ResultDataInvoice()
                {
                    rs = true,
                    result = result.Distinct(),
                    TotalPages = TotalPages,
                    TotalRow = TotalRow
                };

                return new JsonResult()
                {
                    Data = data,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                //return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = $"Lỗi Ngày không hợp lệ bạn vui lòng thử lại" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceByStatus(FormSearchInvoice form, int type, int intpage, int reportType)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                form.ITEMPERPAGE = 10;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.CURRENTPAGE = intpage;
                form.INVOICETYPE = type;
                form.REPORTYPE = reportType;
                form.FROMDATE = string.IsNullOrEmpty(form.STRFROMDATE) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(form.STRFROMDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(form.STRTODATE) ? DateTime.Now : DateTime.ParseExact(form.STRTODATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.PARTNEREMAIL = objUser.ISADMIN ? null : objUser.EMAIL;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                List<InvoiceBO> result = invoiceBLL.GetInvoiceByStatus(form);
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
                //result = result.FindAll(x => x.USINGINVOICETYPE == objUser.USINGINVOICETYPE);
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy được danh sách theo trang thái", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceByStatus");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// truongnv 20200316
        /// Lấy danh sách hóa đơn phát hành lỗi
        /// </summary>
        /// <param name="form"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetInvoiceReleaseError(FormSearchInvoice form, int currentPage, int itemPerPage)
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
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<InvoiceBO> result = invoiceBLL.GetInvoiceReleaseError(form);

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

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceDetail(long invoiceid)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.GetInvoiceDetail(invoiceid);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceDetail");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được danh sách sản phẩm", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceDetail");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Tab hóa đơn xóa bỏ
        /// </summary>
        /// <param name="form"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetInvoiceDelete(FormSearchInvoice form, int currentPage, int itemPerPage)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.Now;
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
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                form.PARTNEREMAIL = objUser.ISADMIN ? null : objUser.EMAIL;

                List<InvoiceBO> result = invoiceBLL.GetInvoiceDelete(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                //result = result.FindAll(x => x.USINGINVOICETYPE == objUser.USINGINVOICETYPE);
                return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Tab hóa đơn chờ
        /// </summary>
        /// <param name="form"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemPerPage"></param>
        /// <returns></returns>
        public ActionResult GetInvoiceWating(FormSearchInvoice form, int currentPage, int itemPerPage)
        {
            try
            {
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
                form.ITEMPERPAGE = itemPerPage;
                form.CURRENTPAGE = currentPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                form.PARTNEREMAIL = objUser.ISADMIN ? null : objUser.EMAIL;

                List<InvoiceBO> result = invoiceBLL.GetInvoiceWating(form);

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

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                ResultDataInvoice data = new ResultDataInvoice()
                {
                    rs = true,
                    result = result.Distinct(),
                    TotalPages = TotalPages,
                    TotalRow = TotalRow
                };

                return new JsonResult()
                {
                    Data = data,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
                //return Json(new { rs = true, result, TotalPages, TotalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceById(long id)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.GetInvoiceById(id);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceById");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy hóa đơn theo ID", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceById");
                return Json(new { rs = false, msg = $"Lỗi lấy thông tin hóa đơn theo ID" }, JsonRequestBehavior.AllowGet);
            }
        }

        //type == 1 ? add new : add new and send email
        //[AuthorizeUser(Role = (int)UserRole.THEM_MOI_HOA_DON)]
        public ActionResult AddInvoice(InvoiceBO invoice, int type)
        {
            try
            {
                var usingInvoiceType = objUser.USINGINVOICETYPE;
                invoice.USINGINVOICETYPE = usingInvoiceType;
                invoice.SIGNLINK = null;
                invoice.INVOICESTATUS = 1;
                invoice.COMNAME = objUser.COMNAME;
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                invoice.COMADDRESS = objUser.COMADDRESS;
                invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
                invoice.QUANTITYPLACE = objUser.QUANTITYPLACE;
                invoice.PRICEPLACE = objUser.PRICEPLACE;
                invoice.MONEYPLACE = objUser.MONEYPLACE;
                invoice.PARTNEREMAIL = objUser.ISADMIN ? null : objUser.EMAIL;

                switch (usingInvoiceType)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        invoice.FROMDATE = DateTime.ParseExact(invoice.FROMDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        invoice.TODATE = DateTime.ParseExact(invoice.TODATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                        invoice.FROMDATE = DateTime.ParseExact(invoice.FROMDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (invoice.TODATESTR == null)
                        {
                            invoice.TODATE = DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            invoice.TODATE = DateTime.ParseExact(invoice.TODATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        invoice.DELIVERYORDERDATE = DateTime.ParseExact(invoice.DELIVERYORDERDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    default:
                        break;
                }

                /*
                 * kiểm tra nếu là khách hàng doanh nghiệp check trùng MST
                 * truongnv 20200219
                 */
                if (!string.IsNullOrEmpty(invoice.CUSTAXCODE) && !string.IsNullOrEmpty(invoice.CUSADDRESS) && !string.IsNullOrEmpty(invoice.CUSNAME))
                {
                    CustomerBO customerBO = new CustomerBO()
                    {
                        COMTAXCODE = invoice.COMTAXCODE,
                        CUSTAXCODE = invoice.CUSTAXCODE,
                        CUSBUYER = invoice.CUSBUYER,
                        CUSNAME = invoice.CUSNAME,
                        CUSADDRESS = invoice.CUSADDRESS,
                        CUSPHONENUMBER = invoice.CUSPHONENUMBER,
                        CUSEMAIL = invoice.CUSEMAIL,
                        CUSPAYMENTMETHOD = invoice.CUSPAYMENTMETHOD,
                        CUSBANKNAME = invoice.CUSBANKNAME,
                        CUSACCOUNTNUMBER = invoice.CUSACCOUNTNUMBER,
                        CUSID = invoice.CUSID
                    };

                    CustomerBLL customerBLL = new CustomerBLL();
                    string msg = customerBLL.CheckCustomerDuplicateTaxCode(customerBO);
                    if (msg.Length == 0)
                    {
                        customerBLL.AddCustomer(customerBO);
                    }
                }

                invoice.INVOICEWAITINGTIME = invoice.STRINVOICEWAITINGTIME != null ? DateTime.ParseExact(invoice.STRINVOICEWAITINGTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                if (!string.IsNullOrEmpty(invoice.NOTE))
                {
                    invoice.NOTE.Replace("\n", "<br />");
                }
                invoice.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                long result = 0;
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        result = invoiceBLL.CreateInvoice(invoice);
                        break;
                    default:
                        result = invoiceBLL.AddInvoice(invoice);
                        break;
                }

                if (result <= 0)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Invoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                if (result > 0)
                {
                    if (invoice.INVOICETYPE == (int)INVOICE_TYPE.ALTERNATIVE || invoice.INVOICETYPE == (int)INVOICE_TYPE.MODIFIED)
                    {
                        var reportBLL = new ReportBLL();
                        var report = reportBLL.GetReportByInvoiceIdReportType(invoice.REFERENCE, "2");
                        if (report != null)
                        {
                            report.INVOICEID = result;
                            reportBLL.UpdateModifiedReport(report);
                        }
                        if (reportBLL.ResultMessageBO.IsError)
                        {
                            ConfigHelper.Instance.WriteLog(reportBLL.ResultMessageBO.Message, reportBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetReportByInvoiceIdReportType && UpdateModifiedReport");
                            return Json(new { rs = false, msg = reportBLL.ResultMessageBO.Message });
                        }
                    }

                    return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { rs = false, msg = "Thất bại, vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "AddInvoice");
                return Json(new { rs = false, msg = $"Lỗi tạo hóa đơn" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[AuthorizeUser(Role = (int)UserRole.CAP_NHAT_HOA_DON)]
        public ActionResult UpdateInvoice(InvoiceBO invoice, int type)
        {
            try
            {
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                invoice.DUEDATE = DateTime.ParseExact(invoice.STRDUEDATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                invoice.INVOICEWAITINGTIME = invoice.STRINVOICEWAITINGTIME != null ? DateTime.ParseExact(invoice.STRINVOICEWAITINGTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                invoice.QUANTITYPLACE = objUser.QUANTITYPLACE;
                invoice.PRICEPLACE = objUser.PRICEPLACE;
                invoice.MONEYPLACE = objUser.MONEYPLACE;

                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        invoice.FROMDATE = DateTime.ParseExact(invoice.FROMDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        invoice.TODATE = DateTime.ParseExact(invoice.TODATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                        invoice.FROMDATE = DateTime.ParseExact(invoice.FROMDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (invoice.TODATESTR == null)
                        {
                            invoice.TODATE = DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            invoice.TODATE = DateTime.ParseExact(invoice.TODATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        invoice.DELIVERYORDERDATE = DateTime.ParseExact(invoice.DELIVERYORDERDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(invoice.NOTE))
                {
                    invoice.NOTE.Replace("\n", "<br />");
                }
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                long result = invoiceBLL.UpdateInvoice(invoice);

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Invoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "UpdateInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateCancelInvoice(InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                invoice.INVOICETYPE = 3;
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                invoice.CANCELTIME = DateTime.ParseExact(invoice.STRCANCELTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var invoiceSigedTime = invoiceBLL.GetInvoiceById(invoice.ID).SIGNEDTIME;
                if (invoice.CANCELTIME < invoiceSigedTime)
                {
                    return Json(new { rs = false, msg = $"Ngày hủy hóa đơn: <b class='text-success'>{invoice.CANCELTIME.ToString("dd/MM/yyyy")}</b> không nhỏ hơn ngày ký hóa đơn: <b class='text-danger'>{invoiceSigedTime.ToString("dd/MM/yyyy")}</b>." }, JsonRequestBehavior.AllowGet);
                }
                var result = invoiceBLL.UpdateCancelInvoice(invoice);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Invoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                if (result)
                {
                    SendCancelEmail(invoice);
                    // get invoice's information
                    //var updatedInvoice = invoiceBLL.GetInvoiceById(invoice.ID);

                    //var temp = GetInvoiceContent(updatedInvoice, invoice.NUMBER);
                    //string isGenerateCancelInvoiceSuccess = invoiceBLL.GenerateInvoiceByTemplate(temp, updatedInvoice);
                }

                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật hóa đơn xóa bỏ", ex, MethodBase.GetCurrentMethod().Name, "UpdateCancelInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Hóa đơn chuyển đổi
        //public ActionResult UpdateConvertInvoice(InvoiceBO invoice)
        //{
        //    try
        //    {
        //        invoice.INVOICETYPE = 4;
        //        invoice.COMTAXCODE = objUser.COMTAXCODE;
        //        InvoiceBLL invoiceBLL = new InvoiceBLL();
        //        var result = invoiceBLL.UpdateConvertInvoice(invoice);
        //        if (invoiceBLL.ResultMessageBO.IsError)
        //        {
        //            ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Invoice");
        //            return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
        //        }
        //        if (result > 0)
        //        {
        //            string isGenerateConvertInvoiceSuccess = invoiceBLL.GenerateInvoice(invoice.ID.ToString(), invoice.NUMBER);
        //            if (!string.IsNullOrEmpty(isGenerateConvertInvoiceSuccess))
        //            {
        //                invoiceBLL.UpdateInvoiceStatusSignedLink(invoice.ID, isGenerateConvertInvoiceSuccess);
        //            }
        //        }
        //        return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog("Lỗi cập nhật hóa đơn chuyển đổi", ex, MethodBase.GetCurrentMethod().Name, "UpdateConvertInvoice");
        //        return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //Hóa đơn thay thế
        public ActionResult UpdateReplaceInvoice(InvoiceBO invoice)
        {
            try
            {
                invoice.INVOICETYPE = 6;
                invoice.COMTAXCODE = objUser.COMTAXCODE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.UpdateReplaceInvoice(invoice);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Invoice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật hóa đơn chuyển đổi", ex, MethodBase.GetCurrentMethod().Name, "UpdateConvertInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// truongnv 20200406
        /// Đọc dữ liệu file excel lấy ra thông tin các sheet trong file
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult ReadFileExcel(FormCollection collection)
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Uploads/");
                        string extension = Path.GetExtension(path + fileContent.FileName);
                        if (extension != ".xls" && extension != ".xlsx") return Json(new { rs = false, msg = "File không đúng định dạng, Hỗ trợ định dạng .xls và .xlsx." });

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        foreach (string key in Request.Files)
                        {
                            HttpPostedFileBase postedFile = Request.Files[key];
                            postedFile.SaveAs(path + fileContent.FileName);
                        }
                        string fullPath = path + fileContent.FileName;
                        CacheHelper.Set("FullPath", fullPath);
                        // Session["FullPath"] = fullPath;
                        List<ExWorksheet> objSheets;
                        string msg = ExcelHelper.GetAllWorksheets(fullPath, out objSheets);
                        if (msg.Length > 0) return Json(new { rs = false, msg = msg });
                        return Json(new { rs = true, fileName = fileContent.FileName, listSheet = JsonConvert.SerializeObject(objSheets), msg = "Tệp dữ liệu đã tải lên thành công." }, JsonRequestBehavior.AllowGet);
                    }
                }
                ConfigHelper.Instance.WriteLog($"File không đúng định dạng theo quy định mẫu cung cấp.", string.Empty, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "File không đúng định dạng theo quy định mẫu cung cấp." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file excel", objEx, MethodBase.GetCurrentMethod().Name, "ReadFileExcel");
                return Json(new { rs = false, msg = "Lỗi đọc file excel." });
            }
        }

        /// <summary>
        /// truongnv 20200406
        /// Thực hiện ghép cột dữ liệu của kh vs onfinance
        /// </summary>
        /// <param name="headerRow">dòng để lấy tiêu đề các cột và dữ liệu</param>
        /// <param name="selectedSheet">Sheet để lấy dữ liệu</param>
        /// <returns></returns>
        public ActionResult MappingColumnExcel(int headerRow, int selectedSheet, string fromCode, string symbolCode)
        {
            string msg = string.Empty;
            try
            {
                var fullPath = CacheHelper.Get("FullPath");
                if (fullPath == null)
                    return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

                DataTable dt;

                //Test import hóa đơn sản phẩm dàn ngàng
                if (headerRow == 6)
                {
                    msg = ExcelHelper.GetHorizontalTableFromXls(fullPath.ToString(), selectedSheet, headerRow, fromCode, symbolCode, out dt);
                    if (!string.IsNullOrEmpty(msg))
                        return Json(new { rs = false, msg });
                }
                else
                {
                    msg = ExcelHelper.GetDataTableFromExcelFile(fullPath.ToString(), selectedSheet, headerRow, out dt);
                    if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu. Vui lòng kiểm tra xóa cột trắng sau cột cuối cùng." });
                }

                string[] arrCol;
                msg = ExcelHelper.GetColumnNamesFromDatable(dt, out arrCol);

                if (msg.Length > 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu. Vui lòng kiểm tra xóa cột trắng sau cột cuối cùng." });
                if (dt == null || dt.Rows.Count == 0) return Json(new { rs = false, msg = "Dòng tiêu đề bạn chọn không có dữ liệu. Vui lòng kiểm tra xóa cột trắng sau cột cuối cùng." });
                CacheHelper.Set("dtInvoice", dt);

                List<string> lstNameColumn = new List<string>();
                List<ImportInvoiceTitleBO> lstColumn = null;
                if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONGTGT || objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONBANHANG) // TH không phải là trường học
                {
                    lstColumn = new List<ImportInvoiceTitleBO>
                    {
                        new ImportInvoiceTitleBO { MYFIELD = "ID Hóa đơn", EXPLAINT = "Nhóm hóa đơn", ISMANDATORY = true, ConfigFields = new List<string>(lstNameColumn){ "id hóa đơn","nhóm hóa đơn" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tên doanh nghiệp", EXPLAINT = "Tên doanh nghiệp, kiểu ký tự" ,ConfigFields = new List<string>(lstNameColumn) },
                        new ImportInvoiceTitleBO { MYFIELD = "MST", EXPLAINT = "Mã số thuế, kiểu ký tự số" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Người mua" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "SĐT" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Email" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Hình thức thanh toán" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "STK" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Ngân hàng" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Hàng hóa, dịch vụ (*)" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn vị tính" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn giá" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Số lượng" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Thuế suất" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Thành tiền" ,ConfigFields = new List<string>(lstNameColumn)},
                        new ImportInvoiceTitleBO { MYFIELD = "Mã số xuất hàng", ConfigFields= new List<string>(lstNameColumn){ "mã số xuất hàng", "MS xuất hàng","msxh" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng (*)" ,Required = false, ConfigFields= new List<string>(lstNameColumn){ "Mã KH","Mã khách hàng" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Loại tiền", Required = false, ConfigFields= new List<string>(lstNameColumn){ "loại tiền","Loại tiền","LT" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tỷ giá", Required = false, ConfigFields= new List<string>(lstNameColumn){ "tỷ giá","Tỷ giá","tỷ Giá","TG"  }},
                        new ImportInvoiceTitleBO { MYFIELD = "% Chiết khấu", Required = false, ConfigFields= new List<string>(lstNameColumn){ "% Chiết khấu", "% chiết khấu","% CK" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tiền chiết khấu", Required = false, ConfigFields= new List<string>(lstNameColumn){ "tiền chiết khấu","Tiền chiết khấu","TIỀN CHIẾT KHẤU" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Kỳ từ ngày" , ConfigFields = new List<string>(){ "từ ngày", "Từ ngày" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Kỳ đến ngày" , ConfigFields = new List<string>(){ "đến ngày", "Đến ngày" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Emai CTV" , ConfigFields = new List<string>(){ "email ctv", "Email CTV", "EMAIL CTV" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Thuế, phí khác" , ConfigFields = new List<string>(){ "Thuế, phí khác", "thuế, phí khác", "THUẾ, PHÍ KHÁC" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Ghi chú" , ConfigFields = new List<string>(){ "Ghi chú", "ghi chú", "GHI CHÚ", "Ghi Chú" } }
                    };
                }
                else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC)
                {
                    lstColumn = new List<ImportInvoiceTitleBO>
                    {
                        new ImportInvoiceTitleBO { MYFIELD = "ID Hóa đơn" },
                        new ImportInvoiceTitleBO { MYFIELD = "Tên doanh nghiệp/Tên phụ huynh" },
                        new ImportInvoiceTitleBO { MYFIELD = "MST/ Mã HS" },
                        new ImportInvoiceTitleBO { MYFIELD = "Người mua/Tên học sinh" },
                        new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ/Lớp" },
                        new ImportInvoiceTitleBO { MYFIELD = "SĐT" },
                        new ImportInvoiceTitleBO { MYFIELD = "Email" },
                        new ImportInvoiceTitleBO { MYFIELD = "Hình thức thanh toán" },
                        new ImportInvoiceTitleBO { MYFIELD = "STK" },
                        new ImportInvoiceTitleBO { MYFIELD = "Ngân hàng" },
                        new ImportInvoiceTitleBO { MYFIELD = "Hàng hóa, dịch vụ/Lý do nộp (*)" },
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn vị tính" },
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn giá" },
                        new ImportInvoiceTitleBO { MYFIELD = "Số lượng" },
                        new ImportInvoiceTitleBO { MYFIELD = "Thuế suất" },
                        new ImportInvoiceTitleBO { MYFIELD = "Thành tiền" },
                        new ImportInvoiceTitleBO { MYFIELD = "Mã số xuất hàng", ConfigFields= new List<string>(){ "mã số xuất hàng", "MS xuất hàng","msxh" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng (*)" ,Required = false, ConfigFields= new List<string>(){ "Mã KH","Mã khách hàng" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Loại tiền" ,Required = false, ConfigFields= new List<string>(){ "loại tiền","Loại tiền","LT" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tỷ giá" ,Required = false, ConfigFields= new List<string>(){ "tỷ giá","Tỷ giá","tỷ Giá","TG" }}
                    };
                }
                else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                {
                    // kiểm tra xem sử dụng điện là sinh hoạt hay ngoài sinh hoạt để add vào tiêu đề cho đúng
                    if (dt.Columns.Contains("Số hộ") || dt.Columns.Contains("số hộ")) // Đây là TH sử dụng điện cho sinh hoạt (Cá nhân)
                    {
                        lstColumn = new List<ImportInvoiceTitleBO>
                        {
                            new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng", ConfigFields = new List<string>(){ "mã khách hàng", "MKH", "Mã KH" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Tên doanh nghiệp/Tên hộ gia đình" },
                            new ImportInvoiceTitleBO { MYFIELD = "MST", ConfigFields = new List<string>(){ "mã số thuế", "Mã số thuế" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Người mua hàng/Người nộp tiền" },
                            new ImportInvoiceTitleBO { MYFIELD = "Điện thoại" },
                            new ImportInvoiceTitleBO { MYFIELD = "Email" },
                            new ImportInvoiceTitleBO { MYFIELD = "Kỳ từ ngày" , ConfigFields = new List<string>(){ "từ ngày", "Từ ngày" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Kỳ đến ngày" , ConfigFields = new List<string>(){ "đến ngày", "Đến ngày" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Mã công tơ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Tên công tơ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Hệ số" },
                            new ImportInvoiceTitleBO { MYFIELD = "Số hộ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Chỉ số cũ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Chỉ số mới" },
                            new ImportInvoiceTitleBO { MYFIELD = "ĐNTT", ConfigFields= new List<string>(){ "Điện năng tiêu thụ", "điện năng tiêu thụ", "Số lượng", "SL" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Thành tiền trước thuế" },
                            new ImportInvoiceTitleBO { MYFIELD = "Thuế suất % (*)", Required = true, ConfigFields = new List<string>(){ "Thuế suất % (*)", "thuế suất", "Thuế suất","thusuat","thuế Suất" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Thuế", ConfigFields = new List<string>(){ "Tiền thuế", "tiền thuế" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Tổng thanh toán", Required = true },
                            new ImportInvoiceTitleBO { MYFIELD = "ID Hóa đơn", Required = true }
                        };
                    }
                    else
                    {
                        lstColumn = new List<ImportInvoiceTitleBO>
                        {
                            new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng", ConfigFields = new List<string>(){ "mã khách hàng", "MKH", "Mã KH" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Tên doanh nghiệp/Tên hộ gia đình" },
                            new ImportInvoiceTitleBO { MYFIELD = "MST", ConfigFields = new List<string>(){ "mã số thuế", "Mã số thuế" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Người mua hàng/Người nộp tiền" },
                            new ImportInvoiceTitleBO { MYFIELD = "Điện thoại" },
                            new ImportInvoiceTitleBO { MYFIELD = "Email" },
                            new ImportInvoiceTitleBO { MYFIELD = "Kỳ từ ngày" , ConfigFields = new List<string>(){ "từ ngày", "Từ ngày" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Kỳ đến ngày" , ConfigFields = new List<string>(){ "đến ngày", "Đến ngày" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Mã công tơ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Tên công tơ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Hệ số" },
                            new ImportInvoiceTitleBO { MYFIELD = "Mã giờ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Tên giờ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Chỉ số cũ" },
                            new ImportInvoiceTitleBO { MYFIELD = "Chỉ số mới" },
                            new ImportInvoiceTitleBO { MYFIELD = "ĐNTT/Số lượng", ConfigFields = new List<string>(){ "Điện năng tiêu thụ", "điện năng tiêu thụ", "Số lượng", "SL" }},
                            new ImportInvoiceTitleBO { MYFIELD = "Giá" },
                            new ImportInvoiceTitleBO { MYFIELD = "Thành tiền trước thuế" },
                            new ImportInvoiceTitleBO { MYFIELD = "Thuế suất % (*)", Required = true, ConfigFields = new List<string>(){ "Thuế suất % (*)", "thuế suất", "Thuế suất","thusuat","thuế Suất" } },
                            new ImportInvoiceTitleBO { MYFIELD = "Thuế" },
                            new ImportInvoiceTitleBO { MYFIELD = "Tổng thanh toán", Required = true },
                            new ImportInvoiceTitleBO { MYFIELD = "ID Hóa đơn", Required = true }
                        };
                    }
                }
                else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                {
                    lstColumn = new List<ImportInvoiceTitleBO>
                    {
                        new ImportInvoiceTitleBO { MYFIELD = "Nhóm hóa đơn/ ID hóa đơn (*)" ,Required = true, ConfigFields= new List<string>(){ "Số HĐ", "Số HD", "số hóa đơn", "Số hóa đơn", "số hóa đơn(*)", "Số hóa đơn(*)", "số hóa đơn (*)", "Số hóa đơn (*)", "id hóa đơn","nhóm hóa đơn","ID hóa đơn","ID Hóa đơn","Nhóm hóa đơn (*)" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Mã khách hàng (*)" ,Required = true, ConfigFields= new List<string>(){ "Mã KH","Mã khách hàng" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tên khách hàng" , ConfigFields= new List<string>(){ "tên KH","Họ tên"} },
                        new ImportInvoiceTitleBO { MYFIELD = "Mã số thuế" ,Required = true, ConfigFields= new List<string>(){ "MST","Mã ST"} },
                        new ImportInvoiceTitleBO { MYFIELD = "Địa chỉ" , ConfigFields= new List<string>(){ "địa chỉ","Địa Chỉ","ĐC"} },
                        new ImportInvoiceTitleBO { MYFIELD = "Người mua hàng/Người nộp tiền", ConfigFields= new List<string>(){ "người mua hàng","người nộp tiền", "Họ tên", "họ tên", "Họ tên(*)", "Họ tên (*)", "ho ten","HoTen","hoten" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Số điện thoại", ConfigFields= new List<string>(){ "SĐT","Số ĐT", "Số dt","số điện thoại" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Email", ConfigFields= new List<string>(){ "email","địa chỉ email", "Địa chỉ email" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Hình thức thanh toán",Required = true, ConfigFields= new List<string>(){ "Hình thức thanh toán (*)", "HTTT", "Hình thức TT" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Số hộ" , ConfigFields= new List<string>(){ "số hộ", "SH" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Số công tơ/Mã khách hàng" ,Required = true, ConfigFields= new List<string>(){ "số công tơ", "Mã số khách hàng" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Kỳ từ ngày" , ConfigFields= new List<string>(){ "từ ngày", "Từ ngày" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Kỳ đến ngày" , ConfigFields= new List<string>(){ "đến ngày", "Đến ngày" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Mã giá" , ConfigFields= new List<string>(){ "mã giá", "MG", "Mã hàng hóa/dịch vụ","Mã hàng hóa","mã dịch vụ","mã hàng hóa/dịch vụ" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Chỉ số cũ" ,Required = true},
                        new ImportInvoiceTitleBO { MYFIELD = "Chỉ số mới" ,Required = true},
                        new ImportInvoiceTitleBO { MYFIELD = "HS nhân" ,Required = true},
                        new ImportInvoiceTitleBO { MYFIELD = "Khối lượng tiêu thụ" ,Required = true, ConfigFields= new List<string>(){ "số lượng", "Số lượng", "số lượng(*)", "Số lượng (*)", "số lượng (*)", "Số lượng(*)" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn giá (*)" ,Required = true, ConfigFields= new List<string>(){ "đơn giá", "Đơn giá","ĐG" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Thành tiền (*)" ,Required = true, ConfigFields= new List<string>(){ "TT", "thành tiền","Thành tiền", "thành tiền (*)" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Tổng tiền trước thuế" , ConfigFields= new List<string>(){ "Tổng tiền trước thuế", "TTTT" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Thuế suất % (*)" ,Required = true, ConfigFields= new List<string>(){ "Thuế suất % (*)", "thuế suất", "Thuế suất","thusuat","thuế Suất" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Tổng tiền thuế" ,Required = true, ConfigFields= new List<string>(){ "tổng tiền thuế", "Tổng tiền thuế" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Phí BVMT (%)" ,Required = true, ConfigFields= new List<string>(){ "Phí BVMT", "PBVMT", "Phí BVMT (%)", "Phí BVMT(%)" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Tổng tiền phí BVMT", ConfigFields= new List<string>(){ "Tổng tiền phí BVMT", "Tổng tiền TBVMT" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Tổng tiền thanh toán",Required = true, ConfigFields= new List<string>(){ "Tổng tiền sau thuế", "tổng tiền thanh toán" } }
                    };
                }
                else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO)
                {
                    lstColumn = new List<ImportInvoiceTitleBO>
                    {
                        new ImportInvoiceTitleBO { MYFIELD = "Căn cứ điều lệnh số" ,Required = false, ConfigFields= new List<string>(lstNameColumn){ "Căn cứ điều lệnh số" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Của", Required = true, ConfigFields= new List<string>(lstNameColumn){ "Của" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Về việc", ConfigFields= new List<string>(lstNameColumn){ "Về việc" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Họ tên người vận chuyển" , ConfigFields= new List<string>(lstNameColumn){ "Họ tên người vận chuyển"}},
                        new ImportInvoiceTitleBO { MYFIELD = "Phương tiện vận chuyển", ConfigFields= new List<string>(lstNameColumn){ "Phương tiện vận chuyển" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Xuất kho tại", ConfigFields= new List<string>(lstNameColumn){ "Xuất kho tại" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Nhập kho tại", ConfigFields= new List<string>(lstNameColumn){ "Nhập kho tại" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Ngày", Required = true, ConfigFields= new List<string>(lstNameColumn){ "Ngày"}},
                        new ImportInvoiceTitleBO { MYFIELD = "Hợp đồng số" , ConfigFields= new List<string>(lstNameColumn){ "Hợp đồng số" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Ngày xuất kho", Required = true, ConfigFields= new List<string>(lstNameColumn){ "Ngày xuất kho" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Ngày nhập kho", Required = false, ConfigFields= new List<string>(lstNameColumn){ "Ngày nhập kho" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Mã số" , ConfigFields= new List<string>(lstNameColumn){ "Mã số" }},
                        new ImportInvoiceTitleBO { MYFIELD = "Tên nhãn hiệu, quy cách, phẩm chất vật tư(sản phẩm, hàng hóa)", Required = true, ConfigFields= new List<string>(){ "Hàng hóa, dịch vụ", "Tên nhãn hiệu, quy cách, phẩm chất vật tư(sản phẩm, hàng hóa)" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn vị tính", Required = false, ConfigFields= new List<string>(lstNameColumn){ "Đơn vị tính" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Thực xuất", Required = false, ConfigFields= new List<string>(lstNameColumn){ "Thực xuất" }  },
                        new ImportInvoiceTitleBO { MYFIELD = "Thực nhập", Required = false, ConfigFields= new List<string>(lstNameColumn){ "Thực nhập" }  },
                        new ImportInvoiceTitleBO { MYFIELD = "Đơn giá" ,Required = false, ConfigFields= new List<string>(lstNameColumn){ "Đơn giá" }  },
                        new ImportInvoiceTitleBO { MYFIELD = "Thành tiền" ,Required = false, ConfigFields= new List<string>(lstNameColumn){ "Thành tiền" } },
                        new ImportInvoiceTitleBO { MYFIELD = "Nhóm hóa đơn/ ID hóa đơn (*)", Required = true, ConfigFields= new List<string>(lstNameColumn){ "Số HĐ", "Số HD", "số hóa đơn", "Số hóa đơn", "số hóa đơn(*)", "Số hóa đơn(*)", "số hóa đơn (*)", "Số hóa đơn (*)", "id hóa đơn","nhóm hóa đơn","ID hóa đơn","ID Hóa đơn","Nhóm hóa đơn (*)" } }
                    };
                }

                CacheHelper.Set("ColumnInvoiceFinance", lstColumn);
                //Session["ColumnInvoiceFinance"] = lstColumn;

                List<ImportInvoiceColumnMaping> objColMaps = new List<ImportInvoiceColumnMaping>();

                for (var j = 0; j < arrCol.Length; j++)
                {
                    ImportInvoiceColumnMaping objMap = new ImportInvoiceColumnMaping();
                    objMap.Index = j;
                    objMap.ColName = arrCol[j];
                    objColMaps.Add(objMap);
                }

                for (int i = 0; i < lstColumn.Count; i++)
                {
                    for (var j = 0; j < arrCol.Length; j++)
                    {
                        if (lstColumn[i].MYFIELD.Equals(arrCol[j], StringComparison.OrdinalIgnoreCase))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                        else if (lstColumn[i].ConfigFields != null && lstColumn[i].ConfigFields.Contains(arrCol[j].ToString()))
                        {
                            CommonFunction.SetPropertyValue(lstColumn[i], "YOURFIELD", arrCol[j]);
                        }
                    }
                }

                var keyCookie = string.Format("cookieMapingExcel{0}_{1}", objUser.USINGINVOICETYPE, ElaHelper.FilterVietkey(Path.GetFileName(CacheHelper.Get("FullPath").ToString())).Replace(" ", "_"));
                var cookieMaping = HttpContext.Request.Cookies.Get(keyCookie);
                if (cookieMaping != null)
                {
                    var EncryptMaping = SAB.Library.Core.Crypt.Cryptography.Decrypt(cookieMaping.Value, "ABC123");
                    var dymListmaping = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(EncryptMaping);
                    foreach (var c in lstColumn)
                    {
                        //if (dymListmaping[c.MYFIELD].Value != null)
                        //    c.YOURFIELD = dymListmaping[c.MYFIELD].Value;

                        if (dymListmaping[c.MYFIELD] != null)
                            c.YOURFIELD = dymListmaping[c.MYFIELD].Value;
                    }
                }

                return Json(new
                {
                    rs = true,
                    data = lstColumn,
                    lstYourField = objColMaps.Distinct(),
                    clientData = JsonConvert.SerializeObject(arrCol)
                });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi ghép cột dữ liệu.", objEx, MethodBase.GetCurrentMethod().Name, "MappingColumnExcel");
                return Json(new { rs = false, msg = "Lỗi ghép cột dữ liệu." });
            }
        }

        /// <summary>
        /// truongnv 20200406
        /// Xem trước dữ liệu sau khi mapping xong
        /// </summary>
        /// <param name="listMap"></param>
        /// <param name="formCode"></param>
        /// <param name="symbolCode"></param>
        /// <returns></returns>
        public ActionResult PreviewInvoiceData(string listMap, string formCode, string symbolCode)
        {
            List<ProductBO> objProducts = null;
            try
            {
                var dt = CacheHelper.Get("dtInvoice");
                if (dt == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

                var keyCookie = string.Format("cookieMapingExcel{0}_{1}", objUser.USINGINVOICETYPE, ElaHelper.FilterVietkey(Path.GetFileName(CacheHelper.Get("FullPath").ToString())).Replace(" ", "_"));
                var cookieCheckExist = HttpContext.Request.Cookies.Get(keyCookie);
                if (cookieCheckExist != null)
                {
                    HttpContext.Request.Cookies.Remove(keyCookie);
                }

                //Lưu vết maping field
                var EncryptMaping = SAB.Library.Core.Crypt.Cryptography.Encrypt(listMap, "ABC123");
                var response = HttpContext.Response;
                HttpCookie cookieMaping = new HttpCookie(keyCookie, EncryptMaping);
                cookieMaping.Expires = DateTime.Now.AddYears(1);
                response.Cookies.Add(cookieMaping);

                var dtInvoice = (DataTable)dt;
                if (dtInvoice != null && dtInvoice.Rows.Count > 0)
                {
                    var lstPriceApartOne = new List<ProductBO>();
                    var lstPriceApartTwo = new List<ProductBO>();

                    var columnInvoiceFinance = CacheHelper.Get("ColumnInvoiceFinance");
                    if (columnInvoiceFinance == null) return Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
                    List<ImportInvoiceTitleBO> lstColumn = (List<ImportInvoiceTitleBO>)columnInvoiceFinance;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(listMap);
                    List<InvoiceBO> importInvoices = new List<InvoiceBO>();
                    // rowNum: start row to get data from excel file.
                    InvoiceBO importInvoice = null;
                    InvoiceDetailBO product = null;
                    int sortOrder = 0;
                    decimal totalInvoiceMoney = 0;
                    try
                    {
                        foreach (DataRow row in dtInvoice.Rows)
                        {
                            #region Hóa đơn tiền điện
                            if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                            {
                                if (objProducts == null)
                                {
                                    ProductBLL oBL = new ProductBLL();
                                    objProducts = oBL.GetListProductPriceByComtaxCode(objUser.COMTAXCODE);

                                    if (objProducts != null && objProducts.Count == 0)
                                    {
                                        ConfigHelper.Instance.WriteLog("Không lấy được thông tin mức giá lũy kế.", string.Empty, MethodBase.GetCurrentMethod().Name, "GetListProductPriceByComtaxCode");
                                        return Json(new { rs = false, msg = "Không lấy được thông tin sản phẩm cho hóa đơn." });
                                    }

                                    lstPriceApartOne = objProducts.FindAll(x => x.GROUPID == 1); // số hộ 1
                                    lstPriceApartTwo = objProducts.FindAll(x => x.GROUPID == 2); // số hộ 2
                                }

                                // Đây là TH sử dụng điện cho sinh hoạt (Cá nhân)
                                if (dtInvoice.Columns.Contains("Số hộ") || dtInvoice.Columns.Contains("số hộ"))
                                {
                                    int invoiceId = dic[lstColumn[20].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[20].MYFIELD]].ToString(), 0) : 0;
                                    string custaxcode = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty;
                                    string cusid = dic[lstColumn[0].MYFIELD] != null ? row[dic[lstColumn[0].MYFIELD]].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(custaxcode))
                                    {
                                        custaxcode = cusid;
                                    }
                                    if (string.IsNullOrEmpty(cusid))
                                    {
                                        cusid = custaxcode;
                                    }

                                    int apartNo = dic[lstColumn[12].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[12].MYFIELD]].ToString(), 0) : 0;
                                    int factor = dic[lstColumn[11].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[11].MYFIELD]].ToString(), 0) : 0;
                                    int oldNo = dic[lstColumn[13].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[13].MYFIELD]].ToString(), 0) : 0;
                                    int newNo = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[14].MYFIELD]].ToString(), 0) : 0;
                                    int quantity = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0;// điện năng tiều thụ
                                    // Insert pm_invoice_detail
                                    if (apartNo == 1)
                                    {
                                        objProducts = lstPriceApartOne;
                                    }
                                    else if (apartNo == 2)
                                    {
                                        objProducts = lstPriceApartTwo;
                                    }
                                    else
                                        return Json(new { rs = false, msg = "Số hộ sử dụng điện không được để trống." });

                                    //add detail invoce

                                    foreach (var item in objProducts)
                                    {
                                        importInvoice = new InvoiceBO
                                        {
                                            FORMCODE = formCode,
                                            SYMBOLCODE = symbolCode,
                                            CUSTAXCODE = custaxcode,
                                            CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                            CUSADDRESS = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                            CUSBUYER = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                            CUSPHONENUMBER = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                            CUSEMAIL = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                            CUSPAYMENTMETHOD = "TM/CK",
                                            CUSACCOUNTNUMBER = string.Empty,
                                            CUSBANKNAME = string.Empty,
                                            INVOICEID = invoiceId,
                                            TOTALMONEY = dic[lstColumn[16].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[16].MYFIELD]].ToString(), 0) : 0,
                                            TAXMONEY = dic[lstColumn[18].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[18].MYFIELD]].ToString(), 0) : 0,
                                            TOTALPAYMENT = dic[lstColumn[19].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[19].MYFIELD]].ToString(), 0) : 0,
                                            CUSTOMERCODE = cusid,
                                            CUSID = cusid,
                                            FROMDATESTR = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                            TODATESTR = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                        };

                                        int qTy = quantity;

                                        if (qTy > 0)
                                        {
                                            var checkQty = CommonFunction.IsWithin(quantity, item.FROMLEVEL, item.TOLEVEL);
                                            if (!checkQty && (quantity > item.FROMLEVEL))
                                            {
                                                //qTy = item.FROMLEVEL == 0 ? (item.TOLEVEL - item.FROMLEVEL) : (item.TOLEVEL - item.FROMLEVEL) + 1;
                                                qTy = item.FROMLEVEL == 0 ? (item.TOLEVEL - item.FROMLEVEL) : item.TOLEVEL == 0 ? (quantity - item.FROMLEVEL) + 1 : (item.TOLEVEL - item.FROMLEVEL) + 1;
                                            }
                                            else if (!checkQty && (quantity < item.FROMLEVEL))
                                            {
                                                qTy = 0;
                                            }
                                            else if (item.TOLEVEL > 0)
                                            {
                                                if (item.FROMLEVEL == 0)
                                                    qTy = (quantity - item.FROMLEVEL);
                                                else
                                                    qTy = (quantity - item.FROMLEVEL) + 1;
                                            }
                                        }

                                        decimal money = 0;
                                        //Tính tiền sản phẩm
                                        //string msg = CaculatorProductPrice(lstPriceApartOne, lstPriceApartTwo, qTy, apartNo, out money);
                                        //if (msg.Length > 0)
                                        //    return Json(new { rs = false, msg = msg });
                                        money = qTy * item.PRICE;
                                        decimal totalTax = money * (decimal)((dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[17].MYFIELD]].ToString(), -1) : -1) / 100.00);
                                        decimal totalPayment = money + totalTax;
                                        product = new InvoiceDetailBO
                                        {
                                            APARTMENTNO = apartNo,
                                            FACTOR = factor,
                                            SKU = item.SKU,
                                            PRODUCTNAME = item.PRODUCTNAME,
                                            OLDNO = oldNo,
                                            NEWNO = newNo,
                                            QUANTITY = qTy < 0 ? 0 : qTy,
                                            RETAILPRICE = item.PRICE,
                                            QUANTITYUNIT = "Khác",
                                            TAXRATE = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[17].MYFIELD]].ToString(), -1) : -1,
                                            TOTALMONEY = money,
                                            TOTALTAX = totalTax,
                                            TOTALPAYMENT = totalPayment,
                                            METERCODE = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                                            METERNAME = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty,
                                        };

                                        //add detail invoce
                                        List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                        importInvoice.LISTPRODUCT = list;

                                        importInvoices.Add(importInvoice);
                                    }
                                }
                                else // Hóa đơn ngoài sinh hoạt
                                {
                                    int invoiceId = dic[lstColumn[22].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[22].MYFIELD]].ToString(), 0) : 0;
                                    string custaxcode = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty;
                                    string cusid = dic[lstColumn[0].MYFIELD] != null ? row[dic[lstColumn[0].MYFIELD]].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(custaxcode))
                                    {
                                        custaxcode = cusid;
                                    }
                                    if (string.IsNullOrEmpty(cusid))
                                    {
                                        cusid = custaxcode;
                                    }

                                    importInvoice = new InvoiceBO
                                    {
                                        FORMCODE = formCode,
                                        SYMBOLCODE = symbolCode,
                                        CUSTAXCODE = custaxcode,
                                        CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                        CUSADDRESS = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                        CUSBUYER = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                        CUSPHONENUMBER = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                        CUSEMAIL = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                        CUSPAYMENTMETHOD = "TM/CK",
                                        CUSACCOUNTNUMBER = string.Empty,
                                        CUSBANKNAME = string.Empty,
                                        INVOICEID = invoiceId,
                                        TOTALMONEY = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) : 0,
                                        TOTALPAYMENT = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) : 0,
                                        CUSTOMERCODE = cusid,
                                        CUSID = cusid,
                                        FROMDATESTR = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                        TODATESTR = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                    };

                                    product = new InvoiceDetailBO
                                    {
                                        APARTMENTNO = 0,
                                        FACTOR = dic[lstColumn[11].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[11].MYFIELD]].ToString(), 0) : 0,
                                        SKU = dic[lstColumn[12].MYFIELD] != null ? row[dic[lstColumn[12].MYFIELD]].ToString() : string.Empty,
                                        PRODUCTNAME = dic[lstColumn[13].MYFIELD] != null ? row[dic[lstColumn[13].MYFIELD]].ToString() : string.Empty,
                                        OLDNO = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[14].MYFIELD]].ToString(), 0) : 0,
                                        NEWNO = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                        QUANTITY = dic[lstColumn[16].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[16].MYFIELD]].ToString(), 0) : 0,
                                        RETAILPRICE = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[17].MYFIELD]].ToString(), 0) : 0,
                                        QUANTITYUNIT = "Khác",
                                        TAXRATE = dic[lstColumn[19].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[19].MYFIELD]].ToString(), -1) : -1,
                                        TOTALMONEY = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) : 0,
                                        TOTALPAYMENT = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) : 0,
                                        METERCODE = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                                        METERNAME = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty
                                    };

                                    //add detail invoce
                                    List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                    importInvoice.LISTPRODUCT = list;

                                    importInvoices.Add(importInvoice);
                                }
                            }
                            #endregion
                            else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                            {
                                int invoiceid = dic[lstColumn[0].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[0].MYFIELD]].ToString(), 0) : 0;
                                if (dtInvoice.Columns.Contains("STT"))
                                    sortOrder = CommonFunction.NullSafeInteger(row["STT"].ToString(), 0);
                                else
                                    sortOrder++;
                                string custaxcode = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty;
                                string cusid = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty;
                                if (string.IsNullOrEmpty(custaxcode))
                                {
                                    custaxcode = cusid;
                                }
                                if (string.IsNullOrEmpty(cusid))
                                {
                                    cusid = custaxcode;
                                }
                                string formcode = formCode;
                                string cusname = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty;
                                string cusbuyer = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty;
                                int apartNo = dic[lstColumn[9].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[9].MYFIELD]].ToString(), 0) : 0;
                                if (importInvoice == null || importInvoice.INVOICEID != invoiceid)
                                {
                                    importInvoice = new InvoiceBO
                                    {
                                        FORMCODE = formcode,
                                        SYMBOLCODE = symbolCode,
                                        CUSNAME = cusname,
                                        CUSTAXCODE = custaxcode,
                                        CUSBUYER = cusbuyer,
                                        CUSADDRESS = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                        CUSPHONENUMBER = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                        CUSEMAIL = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                        CUSPAYMENTMETHOD = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                        TOTALMONEY = CommonFunction.NullSafeDecimal(row[dic[lstColumn[20].MYFIELD]].ToString(), 0),
                                        TOTALPAYMENT = CommonFunction.NullSafeDecimal(row[dic[lstColumn[25].MYFIELD]].ToString(), 0),
                                        INVOICEID = invoiceid,
                                        USINGINVOICETYPE = objUser.USINGINVOICETYPE,
                                        FROMDATESTR = dic[lstColumn[11].MYFIELD] != null ? row[dic[lstColumn[11].MYFIELD]].ToString() : string.Empty,
                                        TODATESTR = dic[lstColumn[12].MYFIELD] != null ? row[dic[lstColumn[12].MYFIELD]].ToString() : string.Empty,
                                        APARTMENTNO = apartNo,
                                        CUSTOMERCODE = cusid,
                                        CUSID = cusid,
                                        SORTORDER = sortOrder
                                    };
                                }
                                else
                                {
                                    importInvoice = new InvoiceBO
                                    {
                                        FORMCODE = importInvoice.FORMCODE,
                                        SYMBOLCODE = importInvoice.SYMBOLCODE,
                                        CUSNAME = importInvoice.CUSNAME,
                                        CUSTAXCODE = importInvoice.CUSTAXCODE,
                                        CUSBUYER = importInvoice.CUSBUYER,
                                        CUSADDRESS = importInvoice.CUSADDRESS,
                                        CUSPHONENUMBER = importInvoice.CUSPHONENUMBER,
                                        CUSEMAIL = importInvoice.CUSEMAIL,
                                        CUSPAYMENTMETHOD = importInvoice.CUSPAYMENTMETHOD,
                                        TOTALMONEY = importInvoice.TOTALMONEY,
                                        TOTALPAYMENT = importInvoice.TOTALPAYMENT,
                                        INVOICEID = invoiceid,
                                        USINGINVOICETYPE = objUser.USINGINVOICETYPE,
                                        FROMDATESTR = importInvoice.FROMDATESTR,
                                        TODATESTR = importInvoice.TODATESTR,
                                        APARTMENTNO = importInvoice.APARTMENTNO,
                                        CUSTOMERCODE = importInvoice.CUSTOMERCODE,
                                        CUSID = importInvoice.CUSID,
                                        SORTORDER = importInvoice.SORTORDER
                                    };
                                }


                                int factor = dic[lstColumn[16].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[16].MYFIELD]].ToString(), 0) : 0;
                                int oldNo = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[14].MYFIELD]].ToString(), -1) : 0;
                                int newNo = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[15].MYFIELD]].ToString(), -1) : 0;
                                int quantity = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[17].MYFIELD]].ToString(), 0) : 0;
                                string sku = dic[lstColumn[13].MYFIELD] != null ? row[dic[lstColumn[13].MYFIELD]].ToString() : string.Empty;
                                string metercode = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty;
                                int taxratewater = dic[lstColumn[23].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[23].MYFIELD]].ToString(), 0) : 0;
                                if (product == null || product.INVOICEID != invoiceid)
                                {
                                    product = new InvoiceDetailBO
                                    {
                                        INVOICEID = invoiceid,
                                        APARTMENTNO = apartNo,
                                        FACTOR = factor,
                                        SKU = sku,
                                        PRODUCTNAME = "Số nước",
                                        OLDNO = oldNo,
                                        NEWNO = newNo,
                                        QUANTITY = quantity,
                                        RETAILPRICE = dic[lstColumn[18].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[18].MYFIELD]].ToString(), 0) : 0,
                                        QUANTITYUNIT = "Khác",
                                        TAXRATE = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[21].MYFIELD]].ToString(), -1) : -1,
                                        TOTALMONEY = CommonFunction.NullSafeDecimal(row[dic[lstColumn[25].MYFIELD]].ToString(), 0),
                                        METERCODE = metercode,
                                        METERNAME = metercode,
                                        TAXRATEWATER = taxratewater
                                    };
                                }
                                else
                                {
                                    product = new InvoiceDetailBO
                                    {
                                        INVOICEID = product.INVOICEID,
                                        APARTMENTNO = product.APARTMENTNO,
                                        FACTOR = factor > 0 ? factor : product.FACTOR,
                                        SKU = sku,
                                        PRODUCTNAME = "Số nước",
                                        OLDNO = oldNo < 0 ? product.OLDNO : oldNo,
                                        NEWNO = newNo < 0 ? product.NEWNO : newNo,
                                        QUANTITY = quantity,
                                        RETAILPRICE = dic[lstColumn[18].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[18].MYFIELD]].ToString(), 0) : 0,
                                        QUANTITYUNIT = "Khác",
                                        TAXRATE = product.TAXRATE, //dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[21].MYFIELD]].ToString(), -1) : -1,
                                        TOTALMONEY = dic[lstColumn[20].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[20].MYFIELD]].ToString(), 0) : 0,
                                        METERCODE = product.METERCODE,
                                        METERNAME = product.METERNAME,
                                        TAXRATEWATER = product.TAXRATEWATER
                                    };
                                }

                                //add detail invoce
                                List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                importInvoice.LISTPRODUCT = list;

                                importInvoices.Add(importInvoice);
                            }
                            else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO)
                            {
                                importInvoice = new InvoiceBO
                                {
                                    FORMCODE = formCode,
                                    SYMBOLCODE = symbolCode,
                                    INVOICEID = dic[lstColumn[18].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[18].MYFIELD]].ToString(), -1) : 0,
                                    DELIVERYORDERNUMBER = dic[lstColumn[0].MYFIELD] != null ? row[dic[lstColumn[0].MYFIELD]].ToString() : string.Empty,
                                    CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                    DELIVERYORDERCONTENT = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty,
                                    CUSBUYER = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                    TRANSPORTATIONMETHOD = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                    FROMWAREHOUSENAME = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                    TOWAREHOUSENAME = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                    DELIVERYORDERDATESTR = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                    CONTRACTNUMBER = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                    FROMDATESTR = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                                    TODATESTR = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty,
                                    TOTALPAYMENT = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[17].MYFIELD]].ToString(), 0) : 0
                                };

                                product = new InvoiceDetailBO
                                {
                                    SKU = dic[lstColumn[11].MYFIELD] != null ? row[dic[lstColumn[11].MYFIELD]].ToString() : string.Empty,
                                    PRODUCTNAME = dic[lstColumn[12].MYFIELD] != null ? row[dic[lstColumn[12].MYFIELD]].ToString() : string.Empty,
                                    QUANTITYUNIT = dic[lstColumn[13].MYFIELD] != null ? row[dic[lstColumn[13].MYFIELD]].ToString() : string.Empty,
                                    RETAILPRICE = dic[lstColumn[16].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[16].MYFIELD]].ToString(), 0) : 0,
                                    QUANTITY = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[14].MYFIELD]].ToString(), 0) : 0,
                                    INQUANTITY = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                    TOTALMONEY = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[17].MYFIELD]].ToString(), 0) : 0,
                                    TOTALPAYMENT = dic[lstColumn[17].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[17].MYFIELD]].ToString(), 0) : 0
                                };

                                //add detail invoce
                                List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                importInvoice.LISTPRODUCT = list;

                                importInvoices.Add(importInvoice);
                            }
                            else if (objUser.USINGINVOICETYPE == (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC)
                            {
                                importInvoice = new InvoiceBO
                                {
                                    FORMCODE = formCode,
                                    SYMBOLCODE = symbolCode,
                                    CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                    CUSTAXCODE = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty,
                                    CUSBUYER = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                    CUSADDRESS = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                    CUSPHONENUMBER = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                    CUSEMAIL = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                    CUSPAYMENTMETHOD = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                    CUSACCOUNTNUMBER = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                    CUSBANKNAME = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                                    INVOICEID = dic[lstColumn[0].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[0].MYFIELD]].ToString(), -1) : 0,
                                    TOTALMONEY = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                    CUSTOMERCODE = dic[lstColumn[17].MYFIELD] != null ? row[dic[lstColumn[17].MYFIELD]].ToString() : string.Empty,
                                    CUSID = dic[lstColumn[17].MYFIELD] != null ? row[dic[lstColumn[17].MYFIELD]].ToString() : string.Empty,
                                    NOTE = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty,
                                    CURRENCY = dic[lstColumn[18].MYFIELD] != null ? row[dic[lstColumn[18].MYFIELD]].ToString() : "VND",
                                    EXCHANGERATE = dic[lstColumn[19].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[19].MYFIELD]].ToString(), 1) : 1
                                };

                                product = new InvoiceDetailBO
                                {
                                    PRODUCTNAME = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty,
                                    QUANTITYUNIT = dic[lstColumn[11].MYFIELD] != null ? row[dic[lstColumn[11].MYFIELD]].ToString() : string.Empty,
                                    RETAILPRICE = dic[lstColumn[12].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[12].MYFIELD]].ToString(), 0) : 0,
                                    QUANTITY = dic[lstColumn[13].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[13].MYFIELD]].ToString(), 0) : 0,
                                    TAXRATE = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[14].MYFIELD]].ToString(), 0) : -1,
                                    TOTALMONEY = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                    CONSIGNMENTID = dic[lstColumn[16].MYFIELD] != null ? row[dic[lstColumn[16].MYFIELD]].ToString() : string.Empty,
                                    DISCOUNTRATE = dic[lstColumn[20].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[20].MYFIELD]].ToString(), 0) : 0
                                };

                                //add detail invoce
                                List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                importInvoice.LISTPRODUCT = list;

                                importInvoices.Add(importInvoice);
                            }
                            else // Hóa đơn giá trị gia tăng
                            {
                                // Kiểm tra có chiết khấu không?
                                var isDiscount = "KHONG_CO_CO_CHIET_KHAU";
                                if (dic[lstColumn[20].MYFIELD] != null && CommonFunction.NullSafeDecimal(row[dic[lstColumn[20].MYFIELD]].ToString(), 0) > 0
                                    || dic[lstColumn[21].MYFIELD] != null && CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) > 0)
                                {
                                    isDiscount = "CHIET_KHAU_THEO_HANG_HOA";
                                }

                                importInvoice = new InvoiceBO
                                {
                                    FORMCODE = formCode,
                                    SYMBOLCODE = symbolCode,
                                    CUSNAME = dic[lstColumn[1].MYFIELD] != null ? row[dic[lstColumn[1].MYFIELD]].ToString() : string.Empty,
                                    CUSTAXCODE = dic[lstColumn[2].MYFIELD] != null ? row[dic[lstColumn[2].MYFIELD]].ToString() : string.Empty,
                                    CUSBUYER = dic[lstColumn[3].MYFIELD] != null ? row[dic[lstColumn[3].MYFIELD]].ToString() : string.Empty,
                                    CUSADDRESS = dic[lstColumn[4].MYFIELD] != null ? row[dic[lstColumn[4].MYFIELD]].ToString() : string.Empty,
                                    CUSPHONENUMBER = dic[lstColumn[5].MYFIELD] != null ? row[dic[lstColumn[5].MYFIELD]].ToString() : string.Empty,
                                    CUSEMAIL = dic[lstColumn[6].MYFIELD] != null ? row[dic[lstColumn[6].MYFIELD]].ToString() : string.Empty,
                                    CUSPAYMENTMETHOD = dic[lstColumn[7].MYFIELD] != null ? row[dic[lstColumn[7].MYFIELD]].ToString() : string.Empty,
                                    CUSACCOUNTNUMBER = dic[lstColumn[8].MYFIELD] != null ? row[dic[lstColumn[8].MYFIELD]].ToString() : string.Empty,
                                    CUSBANKNAME = dic[lstColumn[9].MYFIELD] != null ? row[dic[lstColumn[9].MYFIELD]].ToString() : string.Empty,
                                    INVOICEID = dic[lstColumn[0].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[0].MYFIELD]].ToString(), -1) : 0,
                                    TOTALMONEY = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                    CUSTOMERCODE = dic[lstColumn[17].MYFIELD] != null ? row[dic[lstColumn[17].MYFIELD]].ToString() : string.Empty,
                                    CUSID = dic[lstColumn[17].MYFIELD] != null ? row[dic[lstColumn[17].MYFIELD]].ToString() : string.Empty,
                                    NOTE = dic[lstColumn[26].MYFIELD] != null ? row[dic[lstColumn[26].MYFIELD]].ToString() : string.Empty,
                                    CURRENCY = dic[lstColumn[18].MYFIELD] != null ? row[dic[lstColumn[18].MYFIELD]].ToString() : "VND",
                                    EXCHANGERATE = dic[lstColumn[19].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[19].MYFIELD]].ToString(), 1) : 1,
                                    DISCOUNTTYPE = isDiscount,
                                    FROMDATESTR = dic[lstColumn[22].MYFIELD] != null ? row[dic[lstColumn[22].MYFIELD]].ToString() : string.Empty,
                                    TODATESTR = dic[lstColumn[23].MYFIELD] != null ? row[dic[lstColumn[23].MYFIELD]].ToString() : string.Empty,
                                    PARTNEREMAIL = dic[lstColumn[24].MYFIELD] != null ? row[dic[lstColumn[24].MYFIELD]].ToString() : (objUser.ISADMIN ? null : objUser.EMAIL)
                                };

                                product = new InvoiceDetailBO
                                {
                                    PRODUCTNAME = dic[lstColumn[10].MYFIELD] != null ? row[dic[lstColumn[10].MYFIELD]].ToString() : string.Empty,
                                    QUANTITYUNIT = dic[lstColumn[11].MYFIELD] != null ? row[dic[lstColumn[11].MYFIELD]].ToString() : string.Empty,
                                    RETAILPRICE = dic[lstColumn[12].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[12].MYFIELD]].ToString(), 0) : 0,
                                    QUANTITY = dic[lstColumn[13].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[13].MYFIELD]].ToString(), 0) : 0,
                                    TAXRATE = dic[lstColumn[14].MYFIELD] != null ? CommonFunction.NullSafeInteger(row[dic[lstColumn[14].MYFIELD]].ToString(), 0) : -1,
                                    TOTALMONEY = dic[lstColumn[15].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[15].MYFIELD]].ToString(), 0) : 0,
                                    CONSIGNMENTID = dic[lstColumn[16].MYFIELD] != null ? row[dic[lstColumn[16].MYFIELD]].ToString() : string.Empty,
                                    DISCOUNTRATE = dic[lstColumn[20].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[20].MYFIELD]].ToString(), 0) : 0,
                                    TOTALDISCOUNT = dic[lstColumn[21].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[21].MYFIELD]].ToString(), 0) : 0,
                                    OTHERTAXFEE = dic[lstColumn[25].MYFIELD] != null ? CommonFunction.NullSafeDecimal(row[dic[lstColumn[25].MYFIELD]].ToString(), 0) : 0
                                };

                                //add detail invoce
                                List<InvoiceDetailBO> list = new List<InvoiceDetailBO> { product };

                                importInvoice.LISTPRODUCT = list;

                                importInvoices.Add(importInvoice);
                            }
                        }
                    }
                    catch (Exception objEx)
                    {
                        ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreviewInoiceData");
                        return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
                    }

                    try
                    {
                        var listInvoices = new List<InvoiceBO>();
                        var usingInvoiceType = objUser.USINGINVOICETYPE;
                        var groupInvoice = importInvoices.GroupBy(x => x.INVOICEID).Select(y => y.ToList()).ToList();
                        foreach (var invoice in groupInvoice)
                        {
                            decimal totalM = 0;
                            InvoiceBO objInvocie = invoice[0];
                            objInvocie.INVOICETYPE = 1;
                            objInvocie.INVOICESTATUS = 1;
                            objInvocie.PAYMENTSTATUS = 1;
                            objInvocie.CURRENCY = string.IsNullOrWhiteSpace(objInvocie.CURRENCY) ? "VND" : objInvocie.CURRENCY;
                            objInvocie.EXCHANGERATE = objInvocie.EXCHANGERATE == 0 ? 1 : objInvocie.EXCHANGERATE;
                            objInvocie.COMNAME = objUser.COMNAME;
                            objInvocie.COMTAXCODE = objUser.COMTAXCODE;
                            objInvocie.COMADDRESS = objUser.COMADDRESS;
                            objInvocie.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                            objInvocie.QUANTITYPLACE = objUser.QUANTITYPLACE;
                            objInvocie.PRICEPLACE = objUser.PRICEPLACE;
                            objInvocie.MONEYPLACE = objUser.MONEYPLACE;
                            objInvocie.USINGINVOICETYPE = usingInvoiceType;
                            objInvocie.SIGNINDEX = objInvocie.INVOICEID;//gán số thứ tự từ file excel sang SIGNINDEX để lưu database
                            if (!string.IsNullOrWhiteSpace(objInvocie.FROMDATESTR))
                            {
                                objInvocie.FROMDATE = DateTime.ParseExact(objInvocie.FROMDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (!string.IsNullOrWhiteSpace(objInvocie.TODATESTR))
                            {
                                objInvocie.TODATE = DateTime.ParseExact(objInvocie.TODATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (!string.IsNullOrWhiteSpace(objInvocie.DELIVERYORDERDATESTR))
                            {
                                objInvocie.DELIVERYORDERDATE = DateTime.ParseExact(objInvocie.DELIVERYORDERDATESTR, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }

                            /* 
                             * PHIẾU XUẤT KHO:
                             * Kiểm tra ngày xuất kho và ngày nhập kho. Ngày xuất kho không nhỏ hơn ngày nhập kho
                             * False => Reject
                             */
                            if (objUser.USINGINVOICETYPE == (int)AccountObjectType.PHIEUXUATKHO
                                && (objInvocie.FROMDATE == null || objInvocie.DELIVERYORDERDATE == null || string.IsNullOrEmpty(objInvocie.CUSNAME)))
                            {
                                objInvocie.ISIMPORTSUCCESS = false;
                            }

                            //Chi tiết hóa đơn
                            List<InvoiceDetailBO> objInvoiceDetail = new List<InvoiceDetailBO>();
                            foreach (var invoiceDetail in invoice)
                            {
                                objInvoiceDetail.Add(invoiceDetail.LISTPRODUCT[0]);
                                switch (objUser.USINGINVOICETYPE)
                                {
                                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                                        totalM += invoiceDetail.LISTPRODUCT[0].TOTALMONEY;
                                        break;
                                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                                        totalM += invoiceDetail.LISTPRODUCT[0].TOTALMONEY;
                                        break;
                                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                                        totalM += invoiceDetail.LISTPRODUCT[0].TOTALMONEY;
                                        break;
                                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                                        totalM += invoiceDetail.LISTPRODUCT[0].TOTALPAYMENT;
                                        break;
                                }
                            }
                            switch (objUser.USINGINVOICETYPE)
                            {
                                case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                                    objInvocie.TOTALMONEY = totalM;
                                    break;
                                case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                                    totalM += objInvocie.TOTALPAYMENT;
                                    break;
                            }

                            objInvocie.LISTPRODUCT = objInvoiceDetail;
                            listInvoices.Add(objInvocie);
                            totalInvoiceMoney += totalM;
                        }
                        CacheHelper.Set("ListInvoices", listInvoices);

                        //Session["ListInvoices"] = listInvoices;
                        return new JsonResult()
                        {
                            Data = new { ListInvoices = listInvoices, TotalInvoiceMoney = totalInvoiceMoney },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };
                    }
                    catch (Exception exx)
                    {
                        ConfigHelper.Instance.WriteLog($"Lỗi nhóm thông tin hóa đơn groupInvoice.", exx, MethodBase.GetCurrentMethod().Name, "groupInvoice");
                    }
                }
                return Json(new { rs = false, msg = "File không đúng chuẩn, vui lòng tải file Excel mẫu kế bên" });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy dữ liệu để thêm mới.", objEx, MethodBase.GetCurrentMethod().Name, "PreiviewInoiceData");
                return Json(new { rs = false, msg = "Lỗi lấy dữ liệu để thêm mới." });
            }
        }
        public ActionResult ImportDataInvoice()
        {
            var lstInvoice = CacheHelper.Get("ListInvoices");
            if (lstInvoice == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });
            List<InvoiceBO> invoices = (List<InvoiceBO>)lstInvoice;
            if (invoices == null) Json(new { rs = false, msg = "Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập." });

            string msg = SaveDataImport(invoices);

            if (msg.Length > 0)
                return Json(new { rs = true, msg = msg });

            return Json(new { rs = false, msg = "Nhập khẩu hóa đơn không thành công, liên hệ với quản trị để được hỗ trợ." });
        }

        /// <summary>
        /// truongnv 20200404
        /// Lưu thông tin hóa đơn
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public string SaveDataImport(List<InvoiceBO> invoices)
        {
            string msg = string.Empty;
            ServiceResult serviceResult = new ServiceResult();
            Dictionary<long, InvoiceBO> dicResultInvoices;
            Dictionary<int, InvoiceBO> dicImportInvoiceError;
            try
            {
                serviceResult = SaveDataInvoice(invoices, out dicResultInvoices, out dicImportInvoiceError);
                if (serviceResult.ErrorCode == 0)
                {
                    var dicSaveCustomerInvoice = dicResultInvoices;
                    int start_hour = DateTime.Now.Hour;
                    int start_minute = DateTime.Now.AddMinutes(1).Minute;
                    int start_second = DateTime.Now.AddSeconds(60).Second;
                    MyScheduler.IntervalInMinutes(start_hour, start_minute, start_second, () => { SaveCustomerInvoice(dicSaveCustomerInvoice); dicSaveCustomerInvoice = null; });

                    return serviceResult.Message;
                }
                return msg;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataImport");
                msg = "Lỗi thêm mới hóa đơn try-catch.";
            }
            return msg;
        }

        /// <summary>
        /// TuyenNV 20201112
        /// Lưu dữ liệu hóa đơn vào DB
        /// </summary>
        /// <returns></returns>

        private static ConcurrentBag<InvoiceBO> errorInvoices = new ConcurrentBag<InvoiceBO>();
        private ServiceResult SaveDataInvoice(List<InvoiceBO> invoices, out Dictionary<long, InvoiceBO> dicResultInvoices, out Dictionary<int, InvoiceBO> dicImportInvoiceError)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            ServiceResult serviceResult = new ServiceResult();
            dicResultInvoices = null;
            dicImportInvoiceError = null;
            List<Task> tasks = new List<Task>();
            int totalCount = invoices.Count;
            int insertRowcount = 0;
            string msg = string.Empty;
            ConcurrentBag<long> invcInserts = new ConcurrentBag<long>();
            try
            {
                ProgressViewModel model = new ProgressViewModel();
                model.TotalRow = totalCount;
                invoices = invoices.OrderBy(x => x.SIGNINDEX).ToList();
                // Sắp xếp thứ tự import theo số thứ tự đánh trong file excel
                var user = Session[ConfigHelper.User] as AccountBO;
                string userID = user.USERNAME;
                int taskNum = 20;
                int qtyperTask = 0;
                CalTaskNumber(totalCount, ref qtyperTask, ref taskNum);
                //List<Task> lstTasks = new List<Task>();
                for (int i = 0; i < taskNum; i++)
                {
                    int tempI = i;
                    var items = invoices.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
                    var task = Task.Run(() =>
                    {
                        foreach (var item in items)
                        {
                            var id = invoiceBLL.InsertWithTask(item);
                            if (id <= 0)
                            {
                                ConfigHelper.Instance.WriteLog($"Id: {item.CUSTOMERCODE}", "Lỗi import", MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
                                errorInvoices.Add(item);
                            }
                            else
                            {
                                insertRowcount = insertRowcount + 1;
                                invcInserts.Add(id);
                            }
                            model.CurrentRow = invcInserts.Count;
                            model.IsSuccess = invcInserts.Count == model.TotalRow;
                            GlobalHost.ConnectionManager.GetHubContext<SignlRConf>().Clients.Group(userID).newMessageReceived(model);
                        }
                    }).ContinueWith((t) =>
                    {
                        if (t.IsFaulted) ConfigHelper.Instance.WriteLog(t.Exception.ToString(), t.Exception, MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
                    });
                    //lstTasks.Add(task);
                }

                msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> hóa đơn.";
                serviceResult.ErrorCode = 0;
                serviceResult.Message = msg;
                await Task.Factory.StartNew(() => Console.Write("1"));
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
                msg = "Lỗi khi thêm mới hóa đơn try-catch.";
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = msg;
                if (invcInserts.Any())
                {
                    invoiceBLL.RollBackInvoice(invcInserts.ToList());
                }
            }
            return serviceResult;
        }

        public ActionResult ActionAfterImport()
        {
            try
            {
                if (errorInvoices == null)
                    return Json(new { rs = false, msg = "Không có hóa đơn lỗi.", JsonRequestBehavior.AllowGet });
                List<InvoiceBO> listErrorInvoices = errorInvoices.ToList();
                // Ghi ra file lỗi
                string fileErrorPath = HttpContext.Server.MapPath($"~/NOVAON_FOLDER/{objUser.COMTAXCODE}/errorImport.txt");
                string content = string.Empty;
                foreach (var item in listErrorInvoices)
                    content +="ID: " + item.INVOICEID + " - MaKH: " + item.CUSTOMERCODE + Environment.NewLine;

                System.IO.File.WriteAllText(fileErrorPath, content);
                return Json(new { rs = true, msg = errorInvoices.Count > 0 ? $"/NOVAON_FOLDER/{objUser.COMTAXCODE}/errorImport.txt" : null, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { rs = false, msg = "Lỗi ActionAfterImport", JsonRequestBehavior.AllowGet });
            }
            finally
            {
                errorInvoices = new ConcurrentBag<InvoiceBO>();
            }
        }


        //private ServiceResult SaveDataInvoice(List<InvoiceBO> invoices, out Dictionary<long, InvoiceBO> dicResultInvoices, out Dictionary<int, InvoiceBO> dicImportInvoiceError)
        //{
        //    InvoiceBLL invoiceBLL = new InvoiceBLL();
        //    ServiceResult serviceResult = new ServiceResult();
        //    ConcurrentDictionary<long, InvoiceBO> dicResult = new ConcurrentDictionary<long, InvoiceBO>();
        //    ConcurrentDictionary<int, InvoiceBO> dicError = new ConcurrentDictionary<int, InvoiceBO>();
        //    List<Task> tasks = new List<Task>();
        //    int totalCount = invoices.Count;
        //    int insertRowcount = 0;
        //    string msg = string.Empty;
        //    List<long> invcInserts = new List<long>();
        //    try
        //    {
        //        ProgressViewModel model = new ProgressViewModel();
        //        model.TotalRow = totalCount;
        //        invoices = invoices.OrderBy(x => x.SIGNINDEX).ToList();
        //        // Sắp xếp thứ tự import theo số thứ tự đánh trong file excel
        //        var user = Session[ConfigHelper.User] as AccountBO;
        //        string userID = user.USERNAME;
        //        int taskNum = 20;
        //        int qtyperTask = 0;
        //        CalTaskNumberInvoice(totalCount, ref qtyperTask, ref taskNum);
        //        //List<Task> lstTasks = new List<Task>();
        //        for (int i = 0; i < taskNum; i++)
        //        {
        //            int tempI = i;
        //            var items = invoices.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
        //            try
        //            {
        //                var task = Task.Run(() =>
        //                {
        //                    Thread.Sleep(5);
        //                    foreach (var item in items)
        //                    {
        //                        var id = invoiceBLL.InsertWithTask(item);
        //                        if (id <= 0)
        //                            ConfigHelper.Instance.WriteLog($"Id: {id}", "Lỗi import", MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
        //                        invcInserts.Add(id);
        //                        insertRowcount = insertRowcount + 1;
        //                        model.CurrentRow = invcInserts.Count;
        //                        model.IsSuccess = invcInserts.Count == model.TotalRow;
        //                        GlobalHost.ConnectionManager.GetHubContext<SignlRConf>().Clients.Group(userID).newMessageReceived(model);
        //                    }
        //                });
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }

        //            //lstTasks.Add(task);
        //        }

        //        msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> hóa đơn.";
        //        serviceResult.ErrorCode = 0;
        //        serviceResult.Message = msg;
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
        //        msg = "Lỗi khi thêm mới hóa đơn try-catch.";
        //        serviceResult.ErrorCode = 1000;
        //        serviceResult.Message = msg;
        //        if (invcInserts.Any())
        //        {
        //            invoiceBLL.RollBackInvoice(invcInserts);
        //        }
        //    }
        //    dicResultInvoices = dicResult.ToDictionary(x => x.Key, x => x.Value);
        //    dicImportInvoiceError = dicError.ToDictionary(x => x.Key, x => x.Value);
        //    return serviceResult;// cái này nó vô nghĩa, vì bên back end a ko có dùng result này, còn e là return về 1 cái json, thằng back end nó gọi vào Json dó lient62
        //}

        //private ServiceResult SaveDataInvoice(List<InvoiceBO> invoices, out Dictionary<long, InvoiceBO> dicResultInvoices, out Dictionary<int, InvoiceBO> dicImportInvoiceError)
        //{
        //    ServiceResult serviceResult = new ServiceResult();
        //    string msg = string.Empty;
        //    dicResultInvoices = new Dictionary<long, InvoiceBO>();
        //    dicImportInvoiceError = new Dictionary<int, InvoiceBO>();
        //    int totalCount = 0;
        //    int insertRowcount = 0;
        //    try
        //    {
        //        InvoiceBLL invoiceBLL = new InvoiceBLL();
        //        totalCount = invoices.Count;
        //        int startRow = 0;
        //        invoices = invoices.OrderBy(x => x.INVOICEID).ToList(); // Sắp xếp thứ tự import theo số thứ tự đánh trong file excel
        //        foreach (var invoice in invoices)
        //        {
        //            if (!invoice.ISIMPORTSUCCESS)
        //                continue;
        //            invoice.REFERENCECODE = ReferenceCode.GenerateReferenceCode();
        //            long result = invoiceBLL.CreateInvoice(invoice);
        //            startRow++;
        //            if (result < 0)
        //                dicImportInvoiceError.Add(startRow, invoice);
        //            else
        //            {
        //                insertRowcount++;
        //                msg = $"Thêm mới thành công <b>{insertRowcount}/{totalCount}</b> hóa đơn.";
        //                dicResultInvoices.Add(result, invoice);
        //            }
        //        }
        //        serviceResult.ErrorCode = 0;
        //        serviceResult.Message = msg;
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveDataInvoice");
        //        msg = "Lỗi khi thêm mới hóa đơn try-catch.";
        //        serviceResult.ErrorCode = 1000;
        //        serviceResult.Message = msg;
        //    }
        //    return serviceResult;
        //}

        /// <summary>
        /// truongnv 20200404\
        /// Tính giá sản phẩm đối với nước/điện
        /// </summary>
        /// <param name="objP"></param>
        /// <param name="objP2"></param>
        /// <param name="quantity"></param>
        /// <param name="apartno"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        private string CaculatorProductPrice(List<ProductBO> objP, List<ProductBO> objP2, int quantity, int apartno, out decimal money)
        {
            string msg = string.Empty;
            money = 0;
            decimal price1 = 0, price2 = 0, price3 = 0, price4 = 0, price5 = 0, price6 = 0;
            ProductBO objPrice = null;
            try
            {
                if (apartno == 1 && objP.Count > 0)
                {
                    //Biểu giá 1: Dùng cho các hộ sinh hoạt bình thường có 1 hộ khẩu, 1 công tơ			
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 0 && x.TOLEVEL == 50);
                    if (objPrice != null) price1 = objPrice.PRICE;
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 51 && x.TOLEVEL == 100);
                    if (objPrice != null) price2 = objPrice.PRICE;
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 101 && x.TOLEVEL == 200);
                    if (objPrice != null) price3 = objPrice.PRICE;
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 201 && x.TOLEVEL == 300);
                    if (objPrice != null) price4 = objPrice.PRICE;
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 301 && x.TOLEVEL == 400);
                    if (objPrice != null) price5 = objPrice.PRICE;
                    objPrice = objP.FirstOrDefault(x => x.FROMLEVEL == 401);
                    if (objPrice != null) price6 = objPrice.PRICE;

                    if (quantity <= 50)
                    {
                        money = quantity * price1;
                    }
                    if (quantity > 50 && quantity <= 100)
                    {
                        money += 50 * price1 + (quantity - 50) * price2;
                    }
                    if (quantity > 100 && quantity <= 200)
                    {
                        money += 50 * price1 + 50 * price2 + (quantity - 100) * price3;
                    }
                    if (quantity > 200 && quantity <= 300)
                    {
                        money += 50 * price1 + 50 * price2 + 100 * price3 + (quantity - 200) * price4;
                    }
                    if (quantity > 300 && quantity <= 400)
                    {
                        money += 50 * price1 + 50 * price2 + 100 * price3 + 100 * price4 + (quantity - 300) * price5;
                    }
                    if (quantity > 400)
                    {
                        money += 50 * price1 + 50 * price2 + 100 * price3 + 100 * price4 + 100 * price5 + (quantity - 400) * price6;
                    }
                }
                else if (objP2.Count > 0)
                {
                    //Biểu giá 2: Dùng cho các hộ sinh hoạt bình thường có 2 hộ khẩu, nhưng dùng 1 công tơ				
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 0 && x.TOLEVEL == 100);
                    if (objPrice != null) price1 = objPrice.PRICE;
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 101 && x.TOLEVEL == 200);
                    if (objPrice != null) price2 = objPrice.PRICE;
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 201 && x.TOLEVEL == 400);
                    if (objPrice != null) price3 = objPrice.PRICE;
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 401 && x.TOLEVEL == 600);
                    if (objPrice != null) price4 = objPrice.PRICE;
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 601 && x.TOLEVEL == 800);
                    if (objPrice != null) price5 = objPrice.PRICE;
                    objPrice = objP2.FirstOrDefault(x => x.FROMLEVEL == 801);
                    if (objPrice != null) price6 = objPrice.PRICE;

                    if (quantity <= 100)
                    {
                        money = quantity * price1;
                    }
                    if (quantity > 100 && quantity <= 200)
                    {
                        money += 100 * price1 + (quantity - 100) * price2;
                    }
                    if (quantity > 200 && quantity <= 400)
                    {
                        money += 100 * price1 + 100 * price2 + (quantity - 200) * price3;
                    }
                    if (quantity > 400 && quantity <= 600)
                    {
                        money += 100 * price1 + 100 * price2 + 200 * price3 + (quantity - 400) * price4;
                    }
                    if (quantity > 600 && quantity <= 800)
                    {
                        money += 100 * price1 + 100 * price2 + 200 * price3 + 200 * price4 + (quantity - 600) * price5;
                    }
                    if (quantity > 800)
                    {
                        money += 100 * price1 + 100 * price2 + 200 * price3 + 200 * price4 + 200 * price5 + (quantity - 800) * price6;
                    }
                }

                //if (quantity <= 50)
                //{
                //    money = quantity * apartno * price1;
                //}
                //if (quantity > 50 && quantity <= 100)
                //{
                //    money += 50 * apartno * price1 + (quantity - 50) * apartno * price2;
                //}
                //if (quantity > 100 && quantity <= 200)
                //{
                //    money += 50 * apartno * price1 + 50 * apartno * price2 + (quantity - 100) * apartno * price3;
                //}
                //if (quantity > 200 && quantity <= 300)
                //{
                //    money += 50 * apartno * price1 + 50 * apartno * price2 + 100 * apartno * price3 + (quantity - 200) * apartno * price4;
                //}
                //if (quantity > 300 && quantity <= 400)
                //{
                //    money += 50 * apartno * price1 + 50 * apartno * price2 + 100 * apartno * price3 + 100 * apartno * price4 + (quantity - 300) * apartno * price5;
                //}
                //if (quantity > 400)
                //{
                //    money += 50 * apartno * price1 + 50 * apartno * price2 + 100 * apartno * price3 + 100 * apartno * price4 + 100 * apartno * price5 + (quantity - 400) * price6;
                //}
            }
            catch (Exception ex)
            {
                msg = "Lỗi tính toán số tiền của hóa đơn.";
                ConfigHelper.Instance.WriteLog(msg, ex, MethodBase.GetCurrentMethod().Name, "CaculatorProductPrice");
            }
            return msg;
        }

        #region Lấy thông tin doanh nghiệp theo mã số thuế
        public async Task<JsonResult> LoadInfoByTaxcode(string taxcode)
        {
            try
            {
                string taxCode = taxcode.Trim();
                string resultGetToken = string.Empty;
                string hParam = string.Empty;

                //Get html from a uri
                if (string.IsNullOrEmpty(resultGetToken))
                {
                    resultGetToken = await GetData(ConfigHelper.UriEnterpriseInfo);
                }

                //Detect data to find necessary information
                if (resultGetToken != null)
                {
                    var index = resultGetToken.IndexOf("ctl00$hdParameter");
                    hParam = resultGetToken.Substring((index + "ctl00$hdParameter".Length + 32), 83);
                }

                var input = new { searchField = taxCode, h = hParam };
                string inputJson = (new JavaScriptSerializer()).Serialize(input);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigHelper.UriEnterpriseInfoSearch);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Proxy = null;
                httpWebRequest.UseDefaultCredentials = true;

                //2.Add the container with the active
                CookieContainer cc = new CookieContainer();

                //3.Must assing a cookie container for the request to pull the cookies
                httpWebRequest.CookieContainer = cc;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(inputJson);
                }
                var finalResult = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResult = await streamReader.ReadToEndAsync();
                    streamReader.Close();
                }
                Info info = Newtonsoft.Json.JsonConvert.DeserializeObject<Info>(finalResult);
                info.ResponseData = info.d.Where(x => x.Enterprise_Gdt_Code == taxCode).FirstOrDefault();

                return Json(new { rs = true, msg = "Lấy thông tin doanh nghiệp thành công.", data = info.ResponseData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin doanh nghiệp qua API", objEx, MethodBase.GetCurrentMethod().Name, "LoadInfoByTaxcode");
                return Json(new { rs = false, msg = "Không tìm thấy thông tin doanh nghiệp. Vui lòng nhập thủ công." });
            }
        }

        //public async Task<JsonResult> LoadInfoByTaxcode(string taxcode)
        //{
        //    try
        //    {
        //        string taxCode = taxcode.Trim();
        //        string resultGetToken = string.Empty;
        //        string hParam = string.Empty;
        //        string p = "";

        //        //Get html from a uri
        //        if (string.IsNullOrEmpty(resultGetToken))
        //        {
        //            resultGetToken = await GetData("https://dichvuthongtin.dkkd.gov.vn/auth/Public/LogOn.aspx?ReturnUrl=%2fonline%2fdefault.aspx");
        //        }

        //        //Detect data to find necessary information
        //        if (resultGetToken != null)
        //        {
        //            var index = resultGetToken.IndexOf("ctl00$hdParameter");
        //            hParam = resultGetToken.Substring((index + "ctl00$hdParameter".Length + 32), 83);

        //            var index1 = resultGetToken.IndexOf("ctl00$nonceKeyFld");
        //            p = resultGetToken.Substring((index1 + "ctl00$nonceKeyFld".Length + 32), 36);
        //        }

        //        string inputJson = "ctl00%24SM=ctl00%24C%24UpdatePanel1%7Cctl00%24C%24UC_ENT_LIST1%24BtnFilter&ctl00%24nonceKeyFld="+p+"&ctl00%24hdParameter="+hParam+"&ctl00%24FldSearch=&ctl00%24FldSearchID=&ctl00%24searchtype=1&ctl00%24C%24UC_ENT_LIST1%24ENTERPRISE_GDT_CODEFilterFld=0106579683";

        //        var url = "https://dichvuthongtin.dkkd.gov.vn/inf/Forms/Searches/EnterpriseSearchList.aspx?h=145a9";
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        //        httpWebRequest.Method = "POST";
        //        httpWebRequest.Proxy = null;
        //        httpWebRequest.UseDefaultCredentials = true;

        //        //2.Add the container with the active
        //        CookieContainer cc = new CookieContainer();

        //        //3.Must assing a cookie container for the request to pull the cookies
        //        httpWebRequest.CookieContainer = cc;
        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            await streamWriter.WriteAsync(inputJson);
        //        }
        //        var finalResult = "";
        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            finalResult = await streamReader.ReadToEndAsync();
        //            streamReader.Close();
        //        }
        //        Info info = Newtonsoft.Json.JsonConvert.DeserializeObject<Info>(finalResult);
        //        info.ResponseData = info.d.Where(x => x.Enterprise_Gdt_Code == taxCode).FirstOrDefault();

        //        return Json(new { rs = true, msg = "Lấy thông tin doanh nghiệp thành công.", data = info.ResponseData }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception objEx)
        //    {
        //        ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin doanh nghiệp qua API", objEx, MethodBase.GetCurrentMethod().Name, "LoadInfoByTaxcode");
        //        return Json(new { rs = false, msg = "Không tìm thấy thông tin doanh nghiệp. Vui lòng nhập thủ công." });
        //    }
        //}

        public async Task<string> GetData(string uri)
        {
            string output = string.Empty;
            if (!string.IsNullOrEmpty(uri))
            {
                //1.Create the request object
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Proxy = null;
                request.UseDefaultCredentials = true;
                //2.Add the container with the active
                request.CookieContainer = new CookieContainer();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    output = await sr.ReadToEndAsync();
                }
            }
            return output;
        }
        #endregion
        public JsonResult LoadCurrency()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new System.Net.WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
                client.Headers.Add("Access-Control-Allow-Origin", "*");
                client.Headers.Add("Accept-Language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
                client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                string URLTaxCode = ConfigurationManager.AppSettings["URLTaxCode"];
                var url = ConfigurationManager.AppSettings["URLCurrency"];

                string content = client.DownloadString(url);
                content = content.Replace(")", "").Replace("(", "");
                CURRENCYAPI obj = (CURRENCYAPI)JsonConvert.DeserializeObject(content, typeof(CURRENCYAPI));

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin tỷ giá qua API", objEx, MethodBase.GetCurrentMethod().Name, "LoadInfoByTaxcode");
                return Json(new { rs = false, reponseText = "Không thể lấy thông tin tỷ giá, vui lòng nhập thủ công." });
            }
        }

        #region Dải hóa đơn chờ
        public ActionResult GetInvoiceNumerWaiting(int itemPerPage, int currentPage)
        {
            try
            {
                FormSearchNumber form = new FormSearchNumber
                {
                    CURRENTPAGE = currentPage,
                    COMTAXCODE = objUser.COMTAXCODE,
                    ITEMPERPAGE = itemPerPage
                };
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.GetNumberWaiting(form);
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % form.ITEMPERPAGE == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / form.ITEMPERPAGE.Value;
                else
                    TotalPages = TotalRow / form.ITEMPERPAGE.Value + 1;

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceNumerWaiting");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, TotalPages, TotalRow, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetInvoiceNumerWaiting");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddNumberWaiting(InvoiceNumberBO numberWaiting)
        {
            try
            {
                if (numberWaiting.FROMNUMBER > numberWaiting.TONUMBER)
                    return Json(new { rs = false, msg = $"Dải số đến không được nhỏ hơn số bắt đầu" }, JsonRequestBehavior.AllowGet);
                numberWaiting.COMTAXCODE = objUser.COMTAXCODE;
                NumberBLL numberBLL = new NumberBLL();
                //var check = numberBLL.AddNumberWaitingcheck(numberWaiting);
                //if (check.Count > 0)
                //    return Json(new { rs = false, msg = $"Lỗi thêm mới dải hóa đơn, vui lòng kiểm tra lại" }, JsonRequestBehavior.AllowGet);
                var result = numberBLL.AddNumberWaiting(numberWaiting);
                if (numberBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(numberBLL.ResultMessageBO.Message, numberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddNumberWaiting");
                    return Json(new { rs = false, msg = numberBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "AddNumberWaiting");
                return Json(new { rs = false, msg = $"Lỗi {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateNumberWaiting(InvoiceNumberBO numberWaiting)
        {
            try
            {
                numberWaiting.COMTAXCODE = objUser.COMTAXCODE;
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.UpdateNumberWaiting(numberWaiting);
                if (numberBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(numberBLL.ResultMessageBO.Message, numberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateNumberWaiting");
                    return Json(new { rs = false, msg = numberBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "UpdateNumberWaiting");
                return Json(new { rs = false, msg = $"Lỗi {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //Lấy danh sách thông báo phát hành
        public ActionResult GetNumber(FormSearchNumber form)
        {
            try
            {
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.GetNumber(form);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceNumerWaiting");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetNumber");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceTaxRateType(FormSearchNumber form)
        {
            try
            {
                form.COMTAXCODE = objUser.COMTAXCODE;
                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.GetNumber(form);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceNumerWaiting");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetNumber");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult ReadNumberToWords(decimal number, string currency)
        //{
        //    try
        //    {
        //        var obj = ReadNumberToCurrencyWords.ConvertToWordsWithPostfix(number, currency);
        //        return Json(new { rs = true, data = obj });
        //    }
        //    catch (Exception objEx)
        //    {
        //        ConfigHelper.Instance.WriteLog("Lỗi chuyển đổi số thành chữ số", objEx, MethodBase.GetCurrentMethod().Name, "ReadNumberToWords");
        //        return Json(new { rs = false, reponseText = "Không thể chuyển số thành chữ số. Vui lòng thử lại!" });
        //    }
        //}

        public JsonResult ReadNumberToWords(decimal number, string currency, int numberPlace)
        {
            try
            {
                var obj = ReadNumberToCurrencyWords.ConvertToWordsWithPostfixNumberPlace(number, currency, numberPlace);
                return Json(new { rs = true, data = obj });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi chuyển đổi số thành chữ số", objEx, MethodBase.GetCurrentMethod().Name, "ReadNumberToWords");
                return Json(new { rs = false, reponseText = "Không thể chuyển số thành chữ số. Vui lòng thử lại!" });
            }
        }

        public ActionResult AddReleaseNotice(InvoiceNumberBO numberWaiting)
        {
            try
            {
                if (numberWaiting.FROMNUMBER > numberWaiting.TONUMBER)
                    return Json(new { rs = false, msg = $"Dải số đến không được lớn hơn số bắt đầu" }, JsonRequestBehavior.AllowGet);
                try
                {
                    numberWaiting.FROMTIME = DateTime.ParseExact(numberWaiting.STRFROMTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    return Json(new { rs = false, msg = $"Vui lòng kiểm tra lại định dạng thời gian: dd/MM/yyyy (ngày/tháng/năm)" }, JsonRequestBehavior.AllowGet);
                }

                numberWaiting.COMTAXCODE = objUser.COMTAXCODE;
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.AddReleaseNotice(numberWaiting);
                if (numberBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(numberBLL.ResultMessageBO.Message, numberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddNumberWaiting");
                    return Json(new { rs = false, msg = numberBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "AddNumberWaiting");
                return Json(new { rs = false, msg = $"Lỗi thêm dải chờ hóa đơn" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Email
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
                    Username = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEACCOUNT : ConfigurationManager.AppSettings["UsernameEmail"],
                    Password = objUser.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL ? objUser.MAILSERVICEPASSWORD : ConfigurationManager.AppSettings["PasswordEmail"],
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
                ConfigHelper.Instance.WriteLog("Lỗi gửi email!", ex, MethodBase.GetCurrentMethod().Name, "SendCancelEmail");
                return Json(new { rs = false, msg = "Gửi mail không thành công!" }, JsonRequestBehavior.AllowGet);
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
        #endregion

        public ActionResult AddReport(ReportBO report)
        {
            try
            {
                report.REPORTTIME = DateTime.ParseExact(report.STRREPORTTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
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
        /// Lưu và ký biên bản hóa đơn
        /// truongnv 20200218
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public ActionResult SaveAndSignDocumentPDF(ReportBO report)
        {
            try
            {
                report.REPORTTIME = DateTime.ParseExact(report.STRREPORTTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
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
        /// Cập nhật biên bản hóa đơn
        /// truongnv 20200220 
        /// </summary>
        /// <param name="report">Đối tượng biên bản</param>
        /// <returns></returns>
        public ActionResult UpdateReport(ReportBO report)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                InvoiceBO invoice = invoiceBLL.GetInvoiceById(report.INVOICEID);
                report.REPORTTIME = DateTime.ParseExact(report.STRREPORTTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                int reportType = int.Parse(report.REPORTTYPE);
                string message = reportType == 1 ? "Hủy" : "Điều chỉnh";
                var invoiceSignedTime = invoice.SIGNEDTIME;
                if (reportType == 2)
                {
                    InvoiceBO refInvoice = invoiceBLL.GetInvoiceById(invoice.REFERENCE);
                    invoiceSignedTime = refInvoice.SIGNEDTIME;
                }
                if (report.REPORTTIME < invoiceSignedTime)
                {
                    return Json(new { rs = false, msg = $"Ngày lập biên bản: <b class='text-success'>{report.REPORTTIME.ToString("dd/MM/yyyy")}</b> không nhỏ hơn ngày ký hóa đơn: <b class='text-danger'>{invoiceSignedTime.ToString("dd/MM/yyyy")}</b>." });
                }
                if (invoice != null)
                {
                    ReportBLL reportBLL = new ReportBLL();
                    var linkReport = invoiceBLL.GenerateReportToPdf(report, false);

                    if (!string.IsNullOrEmpty(linkReport))
                    {
                        // Cập nhật biên bản 
                        report.LINK = linkReport;
                        bool result = reportBLL.UpdateReport(report);
                        if (!result)
                            return Json(new { rs = false, msg = $"Lỗi cập nhật biên bản hóa đơn <b> {message} </b>." }, JsonRequestBehavior.AllowGet);

                        // Cập nhật lại lý do hủy hoặc điều chỉnh
                        invoice.CANCELREASON = report.REASON;
                        result = invoiceBLL.UpdateReportInvoice(invoice.ID, report.REASON, reportType);
                        if (!result)
                        {
                            ConfigHelper.Instance.WriteLog(reportBLL.ResultMessageBO.Message, reportBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateReport");
                            return Json(new { rs = false, msg = reportBLL.ResultMessageBO.Message });
                        }

                        return Json(new { rs = true, msg = $"Cập nhật biên bản <b> {message} </b> thành công." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateReport");
                        return Json(new { rs = false, msg = $"Không lưu được biên bản <b> {message} </b> vào folder!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { rs = false, msg = $"Không lấy được thông tin hóa đơn <b> {message} </b>." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm biên bản", ex, MethodBase.GetCurrentMethod().Name, "UpdateReport");
                return Json(new { rs = false, msg = $"Biên bản đã tồn tại cho hóa đơn này." }, JsonRequestBehavior.AllowGet);
            }
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
                return Json(new { rs = true, msg = "Tải lên thành công.", dstFilePath });
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải lên được file";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "UploadFileReport");
                return Json(new { rs = false, msg = msg });
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

        public ActionResult ExportExcel(FormSearchInvoice form)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                List<InvoiceBO> result = new List<InvoiceBO>();
                string strFileName = "Danh_sach_hoa_don_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
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
                form.USINGINVOICETYPE = objUser.USINGINVOICETYPE;

                if (form.INVOICESTATUS == (int)INVOICE_STATUS.WAITING)
                    result = invoiceBLL.ExportInvoiceExcelByInitDate(form);
                else
                    result = invoiceBLL.ExportInvoiceExcelBySignDate(form);

                return PushFileCustomizationForInvoice(result, strFileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách hóa đơn.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Không thể xuất danh sách hóa đơn." });
            }
        }

        /// <summary>
        /// TuyenNV 20201109
        /// Chọn hóa đơn cần export ra file excel.
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        public ActionResult DownloadInvoiceExcel(string idInvoices)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                string strFileName = "Danh_sach_hoa_don_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                List<InvoiceBO> result = new List<InvoiceBO>();
                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> arr = new List<string>();

                for (int i = 0; i < lstInvoiceid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstInvoiceid[i], 0);
                    //arr.Add($"'{vVal}'");
                    arr.Add($"{vVal}");
                }
                string ids = string.Join(",", arr);
                result = invoiceBLL.ExportInvoiceExcelByIds(ids);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReport");
                }
                return PushFileCustomizationForInvoice(result, strFileName);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "DownloadInvoiceExcel");
                return Json(new { success = false, responseText = "Không thể xuất danh sách hóa đơn." });
            }
        }

        byte[] GetFile(string s)
        {
            using (FileStream fs = System.IO.File.OpenRead(s))
            {
                //System.IO.FileStream fs = System.IO.File.OpenRead(s);
                byte[] data = new byte[fs.Length];
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new IOException(s);
                return data;
            }
        }
        public ActionResult Downloadfile(string link, string fileName)
        {
            try
            {
                //Loại bỏ domain
                //link = link.Replace(ConfigHelper.UriInvoiceFolder, "");

                //Tách lấy tên file
                string[] path = link.Split('/');

                //Đưa về đường dẫn vật lý
                link = ConfigHelper.PhysicalInvoiceFolder + link.Replace('/', '\\');

                //Đọc file
                byte[] fileBytes = GetFile(link);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tải file.", objEx, MethodBase.GetCurrentMethod().Name, "Downloadfile");
                return null;
            }
        }

        #region Thông báo phát hành

        /// <summary>
        /// Tải biên bản thông báo phát hành
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        public ActionResult DownloadIssuedReleaseDocument(InvoiceNumberBO numberBO)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                string html = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Server.MapPath("~/TemplateFiles/quyet_dinh_su_dung_hoa_don.html")))
                {
                    html = reader.ReadToEnd();
                }

                html = html.Replace("{{COMNAME}}", objUser.COMNAME);
                html = html.Replace("{{FORMCODE}}", numberBO.FORMCODE);
                html = html.Replace("{{SYMBOLCODE}}", numberBO.SYMBOLCODE);
                html = html.Replace("{{CURRENTDATE}}", DateTime.Now.ToString("dd/MM/yyyy"));
                html = html.Replace("{{DATE}}", DateTime.Now.Day.ToString("D2"));
                html = html.Replace("{{MONTH}}", DateTime.Now.Month.ToString("D2"));
                html = html.Replace("{{YEAR}}", DateTime.Now.Year.ToString());

                string fileName = "QUYET_DINH_SU_DUNG_HOA_DON.doc";
                System.IO.File.WriteAllText(HttpContext.Server.MapPath("~/TemplateFiles/template.html"), html);
                byte[] fileBytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/TemplateFiles/template.html"));
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file biên bản phát hành.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadReleaseDocument");
                return Json(new { rs = false, msg });
            }
        }

        /// <summary>
        /// Tải biên bản thông báo phát hành
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        public ActionResult DownloadReleaseDocument(InvoiceNumberBO numberBO)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                string html = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Server.MapPath("~/TemplateFiles/ReleaseDocumentReport.html")))
                {
                    html = reader.ReadToEnd();
                }

                html = html.Replace("{INVOICETYPE}", GetInvoiceNameXmlFile(objUser.USINGINVOICETYPE));
                html = html.Replace("{COMNAME}", objUser.COMNAME);
                html = html.Replace("{COMADDRESS}", objUser.COMADDRESS);
                html = html.Replace("{COMTAXCODE}", objUser.COMTAXCODE);
                html = html.Replace("{COMPHONENUMBER}", objUser.COMPHONENUMBER);

                var quantity = (numberBO.TONUMBER - numberBO.FROMNUMBER) + 1;
                html = html.Replace("{FORMCODE}", numberBO.FORMCODE);
                html = html.Replace("{SYMBOLCODE}", numberBO.SYMBOLCODE);
                html = html.Replace("{QUANTITY}", quantity.ToString());
                html = html.Replace("{FROMNUMBER}", numberBO.FROMNUMBER.ToString());
                html = html.Replace("{TONUMBER}", numberBO.TONUMBER.ToString());
                html = html.Replace("{STARTDATE}", numberBO.STRFROMTIME);

                string fileName = "Bien_Ban_Thong_Bao_Phat_Hanh.doc";
                System.IO.File.WriteAllText(HttpContext.Server.MapPath("~/TemplateFiles/template.html"), html);
                byte[] fileBytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/TemplateFiles/template.html"));
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file biên bản phát hành.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadReleaseDocument");
                return Json(new { rs = false, msg });
            }
        }


        /// <summary>
        /// Tải biên bản thông báo phát hành xml import vào phần mềm của thuế
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        /// 
        public ActionResult DownloadReleaseDocumentXML(InvoiceNumberBO numberBO)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                HSoThueDTu hstdt = new HSoThueDTu();
                HSoThueDTuHSoKhaiThue hskt = new HSoThueDTuHSoKhaiThue();
                HSoThueDTuHSoKhaiThueTTinChung ttc = new HSoThueDTuHSoKhaiThueTTinChung()
                {
                    TTinDVu = new HSoThueDTuHSoKhaiThueTTinChungTTinDVu()
                    {
                        maDVu = "ONFINANCE",
                        tenDVu = "Onfinance.asia",
                        pbanDVu = "1.0.0.0",
                        ttinNhaCCapDVu = "ONFINANCE"
                    },
                    TTinTKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue()
                    {
                        TKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue()
                        {
                            maTKhai = 106,
                            tenTKhai = "Thông báo phát hành hóa đơn (TB01/AC)",
                            moTaBMau = "",
                            pbanTKhaiXML = "2.1.2",
                            loaiTKhai = "C",
                            soLan = 0,
                            KyKKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue()
                            {
                                kieuKy = "D",
                                kyKKhai = DateTime.Now.ToString("dd/MM/yyyy"),
                                kyKKhaiTuNgay = DateTime.Now.ToString("dd/MM/yyyy"),
                                kyKKhaiDenNgay = DateTime.Now.ToString("dd/MM/yyyy"),
                                kyKKhaiTuThang = "",
                                kyKKhaiDenThang = ""
                            },
                            maCQTNoiNop = objUser.TAXDEPARTEMENTCODE,
                            tenCQTNoiNop = objUser.TAXDEPARTEMENT,
                            ngayLapTKhai = DateTime.Now,
                            GiaHan = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan()
                            {
                                maLyDoGiaHan = "",
                                lyDoGiaHan = ""
                            },
                            nguoiKy = "",
                            ngayKy = DateTime.Now,
                            nganhNgheKD = "",
                        },
                        NNT = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT()
                        {
                            mst = objUser.COMTAXCODE,
                            tenNNT = objUser.COMNAME,
                            dchiNNT = objUser.COMADDRESS,
                            phuongXa = "",
                            maHuyenNNT = "",
                            tenHuyenNNT = "",
                            maTinhNNT = "",
                            tenTinhNNT = "",
                            dthoaiNNT = "",
                            faxNNT = "",
                            emailNNT = ""
                        }
                    }
                };

                HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon hd = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon()
                {
                    ChiTiet = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTiet()
                    {
                        ID = "ID_1",
                        tenLoaiHDon = GetInvoiceNameXmlFile(objUser.USINGINVOICETYPE),
                        mauSo = numberBO.FORMCODE,
                        kyHieu = numberBO.SYMBOLCODE,
                        soLuong = (numberBO.TONUMBER - numberBO.FROMNUMBER) + 1,
                        tuSo = numberBO.FROMNUMBER.ToString("D7"),
                        denSo = numberBO.TONUMBER.ToString("D7"),
                        ngayBDauSDung = DateTime.Now.Date.AddDays(2),
                        DoanhNghiepIn = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietDoanhNghiepIn()
                        {
                            ten = "Công ty cổ phần tập đoàn truyền thông và công nghệ NOVA",
                            mst = "0101990346"
                        },
                        HopDongDatIn = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietHopDongDatIn()
                        {
                            so = "",
                            ngay = ""
                        }
                    }
                };
                HSoThueDTuHSoKhaiThueCTieuTKhaiChinh ctkc = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinh()
                {
                    HoaDon = hd,
                    DonViChuQuan = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinhDonViChuQuan()
                    {
                        ten = "",
                        mst = ""
                    },
                    tenCQTTiepNhan = objUser.TAXDEPARTEMENT,
                    nguoiDaiDien = objUser.COMLEGALNAME,
                    ngayBCao = DateTime.Now
                };

                hskt.TTinChung = ttc;
                hskt.CTieuTKhaiChinh = ctkc;
                hstdt.HSoKhaiThue = hskt;
                string fileName = "Bien_Ban_Thong_Bao_Phat_Hanh.xml";
                string filePath = HttpContext.Server.MapPath("~/" + "Baocaotinhhinhsudunghoadon.xml");
                // Ghi dữ liệu vào file
                XMLHelper.SerializationXml(hstdt, filePath);
                // Download file
                return Downloadfile(filePath, fileName);
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file biên bản phát hành.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadReleaseDocumentXML");
                return Json(new { rs = false, msg });
            }
        }

        /// <summary>
        /// Tải mẫu hóa đơn phát hành
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        public ActionResult DownloadReleaseInvoiceTemplate(InvoiceNumberBO numberBO)
        {
            try
            {
                AccountBLL accountBLL = new AccountBLL();
                var objCustomer = accountBLL.GetInfoUserByUserName(objUser.USERNAME, objUser.COMTAXCODE);
                var lstProduct = new List<InvoiceDetailBO>();
                for (var i = 0; i < 10; i++)
                {
                    var p = new InvoiceDetailBO();
                    lstProduct.Add(p);
                }
                var model = new InvoiceBO()
                {
                    INVOICESTATUS = -1,
                    LISTPRODUCT = lstProduct,
                    COMNAME = objCustomer.COMNAME,
                    COMTAXCODE = objCustomer.COMTAXCODE,
                    COMADDRESS = objCustomer.COMADDRESS,
                    COMPHONENUMBER = objCustomer.COMPHONENUMBER,
                    COMACCOUNTNUMBER = objCustomer.COMACCOUNTNUMBER,
                    COMBANKNAME = objCustomer.COMBANKNAME
                };
                var pageSize = "A4";
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        pageSize = "A5";
                        break;
                }
                var content = RenderViewToString(ControllerContext, "~" + numberBO.TEMPLATEPATH, model, true);

                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();

                FileResult fileResult = new FileContentResult(objGlobalBLL.ConvertStringToPDF(content, null, pageSize), "application/pdf");
                fileResult.FileDownloadName = "TemplateInvoice_" + objUser.COMTAXCODE + ".pdf";
                return fileResult;
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file DownloadReleaseInvoiceTemplate";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadReleaseInvoiceTemplate");
                return Json(new { rs = false, msg });

            }
        }

        /// <summary>
        /// Tải mẫu hóa đơn chuyển đổi
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        public ActionResult DownloadConvertibleInvoiceTemplate(InvoiceNumberBO numberBO)
        {
            try
            {
                AccountBLL accountBLL = new AccountBLL();
                var objCustomer = accountBLL.GetInfoUserByUserName(objUser.USERNAME, objUser.COMTAXCODE);
                var lstProduct = new List<InvoiceDetailBO>();
                for (var i = 0; i < 10; i++)
                {
                    var p = new InvoiceDetailBO();
                    lstProduct.Add(p);
                }
                var model = new InvoiceBO()
                {
                    ID = -1,
                    INVOICESTATUS = -1,
                    COMNAME = objCustomer.COMNAME,
                    COMTAXCODE = objCustomer.COMTAXCODE,
                    COMADDRESS = objCustomer.COMADDRESS,
                    COMPHONENUMBER = objCustomer.COMPHONENUMBER,
                    COMACCOUNTNUMBER = objCustomer.COMACCOUNTNUMBER,
                    COMBANKNAME = objCustomer.COMBANKNAME,
                    LISTPRODUCT = lstProduct,
                    ISCONVERTED = true
                };
                var pageSize = "A4";
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        pageSize = "A5";
                        break;
                }
                var content = RenderViewToString(ControllerContext, "~" + numberBO.TEMPLATEPATH, model, true);

                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();

                FileResult fileResult = new FileContentResult(objGlobalBLL.ConvertStringToPDF(content, null, pageSize), "application/pdf");
                fileResult.FileDownloadName = "TemplateConvertibleInvoice_" + objUser.COMTAXCODE + ".pdf";
                return fileResult;
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file DownloadConvertibleInvoiceTemplate.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadConvertibleInvoiceTemplate");
                return Json(new { rs = false, msg });

            }
        }

        /// <summary>
        /// Tải mẫu hóa đơn chiết khấu
        /// </summary>
        /// <param name="numberBO">Đối tượng number tương đương bảng pm_number</param>
        /// <returns></returns>
        public ActionResult DownloadDiscountInvoiceTemplate(InvoiceNumberBO numberBO)
        {
            try
            {
                AccountBLL accountBLL = new AccountBLL();
                var objCustomer = accountBLL.GetInfoUserByUserName(objUser.USERNAME, objUser.COMTAXCODE);
                var lstProduct = new List<InvoiceDetailBO>();
                for (var i = 0; i < 10; i++)
                {
                    var p = new InvoiceDetailBO();
                    lstProduct.Add(p);
                }
                var model = new InvoiceBO()
                {
                    ID = -1,
                    INVOICESTATUS = -1,
                    COMNAME = objCustomer.COMNAME,
                    COMTAXCODE = objCustomer.COMTAXCODE,
                    COMADDRESS = objCustomer.COMADDRESS,
                    COMPHONENUMBER = objCustomer.COMPHONENUMBER,
                    COMACCOUNTNUMBER = objCustomer.COMACCOUNTNUMBER,
                    COMBANKNAME = objCustomer.COMBANKNAME,
                    LISTPRODUCT = lstProduct,
                    DISCOUNTTYPE = "CHIET_KHAU_THEO_HANG_HOA"
                };
                var content = RenderViewToString(ControllerContext, "~" + numberBO.TEMPLATEPATH, model, true);

                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();

                FileResult fileResult = new FileContentResult(objGlobalBLL.ConvertStringToPDF(content, null), "application/pdf");
                fileResult.FileDownloadName = "TemplateDiscountInvoice_" + objUser.COMTAXCODE + ".pdf";
                return fileResult;
            }
            catch (Exception objEx)
            {
                string msg = "Lỗi không tải xuống được file DownloadDiscountInvoiceTemplate.";
                ConfigHelper.Instance.WriteLog(msg, objEx, MethodBase.GetCurrentMethod().Name, "DownloadDiscountInvoiceTemplate");
                return Json(new { rs = false, msg });

            }
        }

        /// <summary>
        /// Xem mẫu hóa đơn thông báo phát hành, mẫu chuyển đổi.
        /// </summary>
        /// <param name="numberBO"></param>
        /// <returns></returns>
        public ActionResult PreviewInvoiceTemplate(InvoiceNumberBO numberBO, bool isConvert)
        {
            try
            {
                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();
                AccountBLL accountBLL = new AccountBLL();
                var objCustomer = accountBLL.GetInfoUserByUserName(objUser.USERNAME, objUser.COMTAXCODE);
                var lstProduct = new List<InvoiceDetailBO>();
                for (var i = 0; i < 10; i++)
                {
                    var p = new InvoiceDetailBO();
                    lstProduct.Add(p);
                }
                var model = new InvoiceBO()
                {
                    ID = isConvert ? -1 : 0,
                    INVOICESTATUS = -1,
                    LISTPRODUCT = lstProduct,
                    COMNAME = objCustomer.COMNAME,
                    COMTAXCODE = objCustomer.COMTAXCODE,
                    COMADDRESS = objCustomer.COMADDRESS,
                    COMPHONENUMBER = objCustomer.COMPHONENUMBER,
                    COMACCOUNTNUMBER = objCustomer.COMACCOUNTNUMBER,
                    COMBANKNAME = objCustomer.COMBANKNAME,
                    ISCONVERTED = isConvert
                };
                var pageSize = "A4";
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        pageSize = "A5";
                        break;
                }

                var content = RenderViewToString(ControllerContext, "~" + numberBO.TEMPLATEPATH, model, true);

                string tempFileName = isConvert ? "/TemplateConvertibleInvoice_" + objUser.COMTAXCODE + ".pdf" : "/TemplateInvoice_" + objUser.COMTAXCODE + ".pdf";

                string fileName = HttpContext.Server.MapPath("~/NOVAON_FOLDER/" + objUser.COMTAXCODE + tempFileName);
                var isSaveSuccess = SaveFile(fileName, objGlobalBLL.ConvertStringToPDF(content, null, pageSize));
                if (isSaveSuccess)
                    return Json(new { rs = true, msg = "NOVAON_FOLDER/" + objUser.COMTAXCODE + tempFileName });
                return Json(new { rs = false, msg = "Lấy mẫu hóa đơn không thành công." });
            }
            catch (Exception objEx)
            {
                return Json(new { rs = false, msg = "Lấy mẫu hóa đơn không thành công." });
            }
        }

        #endregion

        /// <summary>
        /// Lấy ngày ký của hóa đơn có số lớn nhất nhỏ hơn số cần ký trong dải chờ hóa đơn
        /// </summary>
        /// <param name="form"></param>
        /// <param name="number">Số hóa đơn cần ký</param>
        /// <returns></returns>
        public ActionResult CheckDateOfPreviousInvoice(InvoiceBO form, long number)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.CheckDateOfPreviousInvoice(form, number);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetEmailHistoryByInvoiceId");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                if (result == null)
                {
                    return Json(new { rs = false, result }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được hóa đơn với số hóa đơn cho trước.", ex, MethodBase.GetCurrentMethod().Name, "GetEmailHistoryByInvoiceId");
                return Json(new { rs = false, msg = $"Số hóa đơn {number - 1} không tồn tại hoặc không hợp lệ để kiểm tra." }, JsonRequestBehavior.AllowGet);
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
                var invoiceSignedTime = invoice.SIGNEDTIME;
                if (report.REPORTTYPE == "2")
                {
                    if (invoice.REFERENCE != 0) // Kiểm tra xem đã có hóa đơn điểu chỉnh chưa?
                    {
                        InvoiceBO refInvoice = invoiceBLL.GetInvoiceById(invoice.REFERENCE);
                        invoiceSignedTime = refInvoice.SIGNEDTIME;
                    }
                    else
                    {
                        InvoiceBO refInvoice = invoiceBLL.GetInvoiceById(invoice.ID);
                        invoiceSignedTime = refInvoice.SIGNEDTIME;
                    }
                }
                if (report.REPORTTIME < invoiceSignedTime)
                {
                    return $"Ngày lập biên bản: <b class='text-success'>{report.REPORTTIME.ToString("dd/MM/yyyy")}</b> không nhỏ hơn ngày ký hóa đơn: <b class='text-danger'>{invoiceSignedTime.ToString("dd/MM/yyyy")}</b>.";
                }
                if (report.REPORTTYPE == "1")
                {
                    var linkReport = invoiceBLL.GenerateReportToPdf(report, isSigned);
                    if (!string.IsNullOrEmpty(linkReport))
                    {
                        // Kiểm tra hàm lưu và ký có tồn tại biên bản chưa. Có => Ký. Chưa => Lưu và ký
                        var tmpReport = reportBLL.GetReportByInvoiceIdReportType(report.INVOICEID, report.REPORTTYPE);
                        report.ID = tmpReport != null ? tmpReport.ID : 0;
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
                    var tmpReport = reportBLL.GetReportByInvoiceIdReportType(report.INVOICEID, report.REPORTTYPE);
                    report.ID = tmpReport != null ? tmpReport.ID : 0;
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

        public ActionResult CreateFilePdfToView(long invoiceId)
        {
            ServiceResult serviceResult = SaveTemplateInvoice(invoiceId);
            if (serviceResult.ErrorCode == 1000)
                return Json(new { rs = false, msg = serviceResult.Message });

            return Json(new { rs = true, msg = serviceResult.Message });
        }

        private void CreateFolderFile(InvoiceBO invoice, string rootPath, out string fileName)
        {
            fileName = string.Empty;
            try
            {
                string branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                string branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                string branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");

                string branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();

                string path = string.Format($"{rootPath}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");
                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                fileName = dir + "/" + invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".xml";
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi tạo thư mục chứa file. KH: {objUser.COMTAXCODE}", ex, MethodBase.GetCurrentMethod().Name, "CreateFolderFile");
            }
        }

        private ServiceResult SaveCustomerInvoice(Dictionary<long, InvoiceBO> dicResultInvoices)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                if (dicResultInvoices == null)
                {
                    serviceResult.ErrorCode = 1000;
                    serviceResult.Message = "Lỗi tạo mẫu xml hóa đơn.";
                    return serviceResult;
                }
                Parallel.ForEach(dicResultInvoices, item =>
                {
                    var invoice = item.Value;
                    CustomerBO customerBO = new CustomerBO()
                    {
                        CUSID = invoice.CUSID,
                        COMTAXCODE = invoice.COMTAXCODE,
                        CUSTAXCODE = invoice.CUSTAXCODE,
                        CUSBUYER = invoice.CUSBUYER,
                        CUSNAME = invoice.CUSNAME,
                        CUSADDRESS = invoice.CUSADDRESS,
                        CUSPHONENUMBER = invoice.CUSPHONENUMBER,
                        CUSEMAIL = invoice.CUSEMAIL,
                        CUSPAYMENTMETHOD = invoice.CUSPAYMENTMETHOD,
                        CUSBANKNAME = invoice.CUSBANKNAME,
                        CUSACCOUNTNUMBER = invoice.CUSACCOUNTNUMBER
                    };

                    CustomerBLL customerBLL = new CustomerBLL();
                    string msg = customerBLL.CheckCustomerDuplicateTaxCode(customerBO);
                    if (msg.Length == 0)
                    {
                        customerBLL.AddCustomer(customerBO);
                    }
                });

                serviceResult.ErrorCode = 0;
                serviceResult.Message = string.Empty;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "SaveCustomerInvoice");
                serviceResult.ErrorCode = 1000;
                serviceResult.Message = "Lỗi tạo mẫu xml hóa đơn.";
            }
            return serviceResult;
        }

        /// <summary>
        /// Clear Only Session Values   
        /// </summary>
        private void ClearSession()
        {
            try
            {
                if (Session["FullPath"] != null)
                {
                    Session.Remove("FullPath");
                }
                if (Session["dtInvoice"] != null)
                {
                    Session.Remove("dtInvoice");
                }
                if (Session["ColumnInvoiceFinance"] != null)
                {
                    Session.Remove("ColumnInvoiceFinance");
                }

                if (Session["ListInvoices"] != null)
                {
                    Session.Remove("ListInvoices");
                }
            }
            catch { }
        }

        /// <summary>
        /// tuyennv 20200410
        /// Xóa hóa đơn. Cập nhập trạng thái active = false trong db
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
        /// Xóa hẳn hóa đơn trong db và xóa thư mục file vật lý.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult RemovedInvoice(string ids)
        {
            try
            {
                string[] lstInvoiceid = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<string> arr = new List<string>();
                for (int i = 0; i < lstInvoiceid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstInvoiceid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string id = string.Join(",", arr);
                string msg = invoiceBLL.DeletedInvoice(id);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                // Xóa thư mục file nếu tồn tại
                foreach (var item in id.Split(','))
                {
                    var dirItem = Server.MapPath("~/NOVAON_FOLDER/" + objUser.COMTAXCODE + "/" + item + "");
                    DirectoryInfo di = new DirectoryInfo(dirItem);
                    if (Directory.Exists(dirItem))
                    {
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                        di.Delete();
                    }
                }
                return Json(new { rs = true, msg = $"Xóa thành công {lstInvoiceid.Length}/{lstInvoiceid.Length} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemovedInvoice");
            }

            return Json(new { rs = true, msg = "Xóa thành công" });
        }

        /// <summary>
        /// Xóa hẳn tất cả hóa đơn trong db và xóa thư mục file vật lý.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult RemovedAllInvoice()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                List<InvoiceBO> listDeletedInvoices = invoiceBLL.DeletedAllInvoice(objUser.COMTAXCODE, objUser.USINGINVOICETYPE);
                if (listDeletedInvoices == null || listDeletedInvoices.Count == 0)
                    return Json(new { rs = false, msg = "Không tôn tại hóa đơn cần xóa." });

                // Xóa thư mục file nếu tồn tại
                foreach (var item in listDeletedInvoices)
                {
                    var dirItem = Server.MapPath("~/NOVAON_FOLDER/" + objUser.COMTAXCODE + "/" + item.ID + "");
                    DirectoryInfo di = new DirectoryInfo(dirItem);
                    if (Directory.Exists(dirItem))
                    {
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                        di.Delete();
                    }
                }
                return Json(new { rs = true, msg = $"Xóa thành công {listDeletedInvoices.Count}/{listDeletedInvoices.Count} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "RemovedAllInvoice");
                return Json(new { rs = false, msg = "Có vấn đề khi xóa tất cả hóa đơn." });
            }
        }


        /// <summary>
        /// tuyennv 20201017
        /// Cập nhập trạng thái thanh toán hóa đơn
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        public ActionResult UpdatePaymentStatus(string idInvoices, int paymentStatus)
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
                string msg = invoiceBLL.UpdatePaymentStatus(ids, paymentStatus);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = $"Cập nhật thành công {lstInvoiceid.Length}/{lstInvoiceid.Length} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "UpdatePaymentStatus");
                return Json(new { rs = true, msg = "Cập nhật không thành công" });
            }
        }

        /// <summary>
        /// tuyennv 20201023
        /// Phân quyền hóa đơn cho user cụ thể theo email
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <param name="partnerEmail"></param>
        /// <returns></returns>
        public ActionResult UpdatePartner(string idInvoices, string partnerEmail)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                AccountBLL accountBLL = new AccountBLL();

                partnerEmail = partnerEmail.Trim();
                string msg = accountBLL.CheckUserByTaxCodeUserName(objUser.COMTAXCODE, partnerEmail);
                if (msg.Length == 0)
                    return Json(new { rs = false, msg = $"Không tồn tại người dùng có email: {partnerEmail}" });

                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> arr = new List<string>();
                for (int i = 0; i < lstInvoiceid.Length; i++)
                {
                    var vVal = CommonFunction.NullSafeInteger(lstInvoiceid[i], 0);
                    arr.Add($"'{vVal}'");
                }
                string ids = string.Join(",", arr);
                msg = invoiceBLL.UpdatePartner(ids, partnerEmail);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = $"Cập nhật thành công {lstInvoiceid.Length}/{lstInvoiceid.Length} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "UpdatePartner");
                return Json(new { rs = true, msg = "Cập nhật không thành công" });
            }
        }

        /// <summary>
        /// TuyenNV: In hóa đơn hàng loạt.
        /// Tạo các file PDF nếu chưa có. Sau đó merge lại thành 1 file.
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        //public ActionResult MergingPdfFile(string idInvoices)
        //{
        //    InvoiceBLL invoiceBLL = new InvoiceBLL();
        //    string msg = string.Empty;
        //    string outputPdfPath = string.Empty;
        //    string filePath = string.Empty;
        //    string dir = string.Empty;
        //    var usingInvoiceType = (int)Session["USINGINVOICETYPEID"];
        //    var comTaxCode = objUser.COMTAXCODE;
        //    //List<PdfReader> pdfReaderList = new List<PdfReader>();
        //    ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>> dicDataLinkNotFound = new ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>>();
        //    ConcurrentBag<string> listPath = new ConcurrentBag<string>();
        //    List<Task> tasks = new List<Task>();

        //    try
        //    {
        //        string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var invoiceidx in lstInvoiceid)
        //        {
        //            Thread.Sleep(50);
        //            var t = new Task(() =>
        //            {
        //                var invoiceId = CommonFunction.NullSafeInteger(invoiceidx, 0);

        //                var invoice = invoiceBLL.GetInvoiceById(invoiceId, usingInvoiceType);
        //                filePath = $"/{comTaxCode}/{invoiceId}/{ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-")}/{comTaxCode}_{invoiceId}_{invoice.INVOICETYPE}.pdf";
        //                dir = Server.MapPath("~/NOVAON_FOLDER" + filePath);

        //                listPath.Add(dir);
        //                // Kiểm tra xem tồn tại file pdf hay chưa? nếu chưa thì tạo file
        //                if (!System.IO.File.Exists(dir))
        //                {
        //                    var lstProducts = invoiceBLL.GetInvoiceDetail(invoiceId);
        //                    dicDataLinkNotFound.TryAdd(invoice, lstProducts);
        //                }
        //            });
        //            tasks.Add(t);
        //            t.ConfigureAwait(false);
        //            t.Start();
        //        }
        //        Task.WaitAll(tasks.ToArray());

        //        // Tạo các file còn thiếu ở list dicDataLinkNotFound
        //        if (dicDataLinkNotFound.Count > 0)
        //        {
        //            List<string> lstPaths;
        //            msg = CreateFileInputFolder(dicDataLinkNotFound.ToDictionary(x => x.Key, x => x.Value), out lstPaths);
        //        }

        //        outputPdfPath = Server.MapPath("~/Uploads/") + objUser.COMTAXCODE + "_novaon_invoice.pdf";
        //        if (System.IO.File.Exists(outputPdfPath))
        //        {
        //            System.IO.File.Delete(outputPdfPath);
        //        }

        //        // Merge các file PDF lại. Phải làm thế này mục đích để tránh số thứ tự hóa đơn trong file ghép bị nhảy lung tung do chạy nhiều luồng.
        //        var tempMergeList = listPath.ToList();
        //        tempMergeList.Sort();

        //        // Dùng thư viên PdfSharp. Không dùng itextsharp => Lỗi merge trùng file
        //        PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
        //        foreach (string file in tempMergeList)
        //        {
        //            PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(file, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
        //            int count = inputDocument.PageCount;
        //            for (int idx = 0; idx < count; idx++)
        //            {
        //                PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
        //                outputDocument.AddPage(page);
        //            }
        //        }
        //        outputDocument.Save(outputPdfPath);

        //        //foreach (var path in tempMergeList)
        //        //{
        //        //    PdfReader pdfReader = new PdfReader(path);
        //        //    pdfReaderList.Add(pdfReader);
        //        //}

        //        //Document document = new Document();
        //        //document.SetPageSize(PageSize.A4);

        //        //PdfCopy pdfCopyProvider = new PdfCopy(document, new FileStream(outputPdfPath, FileMode.Create));
        //        //document.Open();
        //        //foreach (PdfReader reader in pdfReaderList)
        //        //{
        //        //    for (int i = 1; i <= reader.NumberOfPages; i++)
        //        //    {
        //        //        PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, i);
        //        //        pdfCopyProvider.AddPage(importedPage);
        //        //    }
        //        //    reader.Close();
        //        //}
        //        //document.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "MergingPdfFile");
        //        return Json(new { rs = true, msg = ex.ToString() });
        //    }
        //    return Json(new { rs = true, msg = objUser.COMTAXCODE + "_novaon_invoice.pdf" });
        //}

        public ActionResult MergingPdfFile(string idInvoices)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            string msg = string.Empty;
            string outputPdfPath = string.Empty;
            string filePath = string.Empty;
            string dir = string.Empty;
            var usingInvoiceType = (int)Session["USINGINVOICETYPEID"];
            var comTaxCode = objUser.COMTAXCODE;
            //List<PdfReader> pdfReaderList = new List<PdfReader>();
            ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>> dicDataLinkNotFound = new ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>>();
            ConcurrentDictionary<long, string> listPath = new ConcurrentDictionary<long, string>();
            List<Task> tasks = new List<Task>();

            try
            {
                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var invoiceidx in lstInvoiceid)
                {
                    Thread.Sleep(50);
                    var t = new Task(() =>
                    {
                        var invoiceId = CommonFunction.NullSafeInteger(invoiceidx, 0);

                        var invoice = invoiceBLL.GetInvoiceById(invoiceId, usingInvoiceType);
                        filePath = $"/{comTaxCode}/{invoiceId}/{ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-")}/{comTaxCode}_{invoiceId}_{invoice.INVOICETYPE}.pdf";
                        dir = Server.MapPath("~/NOVAON_FOLDER" + filePath);

                        listPath.TryAdd(invoice.NUMBER, dir);
                        // Kiểm tra xem tồn tại file pdf hay chưa? nếu chưa thì tạo file
                        if (!System.IO.File.Exists(dir))
                        {
                            var lstProducts = invoiceBLL.GetInvoiceDetail(invoiceId);
                            dicDataLinkNotFound.TryAdd(invoice, lstProducts);
                        }
                    });
                    tasks.Add(t);
                    t.ConfigureAwait(false);
                    t.Start();
                }
                Task.WaitAll(tasks.ToArray());

                // Tạo các file còn thiếu ở list dicDataLinkNotFound
                if (dicDataLinkNotFound.Count > 0)
                {
                    List<string> lstPaths;
                    msg = CreateFileInputFolder(dicDataLinkNotFound.ToDictionary(x => x.Key, x => x.Value), out lstPaths);
                }

                outputPdfPath = Server.MapPath("~/Uploads/") + objUser.COMTAXCODE + "_novaon_invoice.pdf";
                if (System.IO.File.Exists(outputPdfPath))
                {
                    System.IO.File.Delete(outputPdfPath);
                }

                // Merge các file PDF lại. Phải làm thế này mục đích để tránh số thứ tự hóa đơn trong file ghép bị nhảy lung tung do chạy nhiều luồng.
                var tempMergeList = listPath.OrderBy(x => x.Key).ToList();
                //tempMergeList.Sort();

                // Dùng thư viên PdfSharp. Không dùng itextsharp => Lỗi merge trùng file
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                foreach (var file in tempMergeList)
                {
                    PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(file.Value, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                    int count = inputDocument.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                        outputDocument.AddPage(page);
                    }
                }
                outputDocument.Save(outputPdfPath);

                //foreach (var path in tempMergeList)
                //{
                //    PdfReader pdfReader = new PdfReader(path);
                //    pdfReaderList.Add(pdfReader);
                //}

                //Document document = new Document();
                //document.SetPageSize(PageSize.A4);

                //PdfCopy pdfCopyProvider = new PdfCopy(document, new FileStream(outputPdfPath, FileMode.Create));
                //document.Open();
                //foreach (PdfReader reader in pdfReaderList)
                //{
                //    for (int i = 1; i <= reader.NumberOfPages; i++)
                //    {
                //        PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                //        pdfCopyProvider.AddPage(importedPage);
                //    }
                //    reader.Close();
                //}
                //document.Close();
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "MergingPdfFile");
                return Json(new { rs = true, msg = ex.ToString() });
            }
            return Json(new { rs = true, msg = objUser.COMTAXCODE + "_novaon_invoice.pdf" });
        }

        /// <summary>
        /// TuyenNV: Tạo lại file PDF
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        public ActionResult ReMergingPdfFile(string idInvoices)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            string msg = string.Empty;
            var usingInvoiceType = (int)Session["USINGINVOICETYPEID"];
            ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>> dicDataLinkNotFound = new ConcurrentDictionary<InvoiceBO, List<InvoiceDetailBO>>();
            List<Task> tasks = new List<Task>();

            try
            {
                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var invoiceidx in lstInvoiceid)
                {
                    //lock (_locker)
                    //{
                    //    var t = new Task(() =>
                    //    {
                    //        var invoiceId = CommonFunction.NullSafeInteger(invoiceidx, 0);
                    //        var invoice = invoiceBLL.GetInvoiceById(invoiceId, usingInvoiceType);
                    //        var lstProducts = invoiceBLL.GetInvoiceDetail(invoiceId);
                    //        dicDataLinkNotFound.TryAdd(invoice, lstProducts);
                    //    });
                    //    tasks.Add(t);
                    //    t.ConfigureAwait(false);
                    //    t.Start();
                    //}

                    var t = new Task(() =>
                    {
                        var invoiceId = CommonFunction.NullSafeInteger(invoiceidx, 0);
                        var invoice = invoiceBLL.GetInvoiceById(invoiceId, usingInvoiceType);
                        var lstProducts = invoiceBLL.GetInvoiceDetail(invoiceId);
                        dicDataLinkNotFound.TryAdd(invoice, lstProducts);
                    });
                    tasks.Add(t);
                    t.ConfigureAwait(false);
                    t.Start();

                }
                Task.WaitAll(tasks.ToArray());
                // Tạo các file còn thiếu ở list dicDataLinkNotFound
                List<string> lstPaths;
                msg = CreateFileInputFolder(dicDataLinkNotFound.ToDictionary(x => x.Key, x => x.Value), out lstPaths);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "MergingPdfFile");
                return Json(new { rs = true, msg = ex.ToString() });
            }
            return Json(new { rs = true, msg = "Tạo lại hóa đơn PDF thành công. Vui lòng chọn và in lại." });
        }

        private string CreateFileInputFolder(Dictionary<InvoiceBO, List<InvoiceDetailBO>> dicDataLinkNotFound, out List<string> listFilePaths)
        {
            string msg = string.Empty;
            listFilePaths = new List<string>();
            List<string> lstPathInsert = new List<string>();
            try
            {
                List<Task> tasks = new List<Task>();
                bool pageSizeA5 = true;
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        pageSizeA5 = false;
                        break;
                }
                foreach (var item in dicDataLinkNotFound)
                {
                    Thread.Sleep(50);
                    var invoice = item.Key;
                    var lstProducts = item.Value;
                    string htmlContent = "";
                    string filePath = "";
                    List<string> content = new List<string>();
                    if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENNUOC)
                        htmlContent = GetContentViewString(invoice, invoice.NUMBER);
                    else
                        content = GetContentViewStringMuliplePage(invoice, invoice.NUMBER);

                    var t = new Task(() =>
                    {
                        if (pageSizeA5)
                        {
                            var temp = SendHtmlStringToSelectPDF(invoice, content);
                            string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
                            filePath = SaveInvoiceFile(temp, invoice, fileName);
                        }
                        else
                        {
                            long invoiceid;
                            RenderHtmlToPdf(invoice, htmlContent, out filePath, out invoiceid);
                        }
                        lstPathInsert.Add(Server.MapPath("~/NOVAON_FOLDER" + filePath));
                    });
                    tasks.Add(t);
                    t.ConfigureAwait(false);
                    t.Start();
                }
                Task.WaitAll(tasks.ToArray());
                if (lstPathInsert.Count > 0)
                    listFilePaths = lstPathInsert;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "CreateFileInputFolder");
            }
            return msg;
        }

        //private string CreateFileInputFolder(Dictionary<InvoiceBO, List<InvoiceDetailBO>> dicDataLinkNotFound, out List<string> listFilePaths)
        //{
        //    string msg = string.Empty;
        //    listFilePaths = new List<string>();
        //    List<string> lstPathInsert = new List<string>();
        //    try
        //    {
        //        List<Task> tasks = new List<Task>();
        //        bool pageSizeA5 = true;
        //        switch (objUser.USINGINVOICETYPE)
        //        {
        //            case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
        //                pageSizeA5 = false;
        //                break;
        //        }
        //        foreach (var item in dicDataLinkNotFound)
        //        {
        //            var invoice = item.Key;
        //            var lstProducts = item.Value;
        //            string htmlContent = "";
        //            string filePath = "";
        //            List<string> content = new List<string>();
        //            if (invoice.USINGINVOICETYPE == (int)AccountObjectType.HOADONTIENNUOC)
        //                htmlContent = GetContentViewString(invoice, invoice.NUMBER);
        //            else
        //                content = GetContentViewStringMuliplePage(invoice, invoice.NUMBER);

        //            var t = new Task(() =>
        //            {
        //                Thread.Sleep(100);
        //                if (pageSizeA5)
        //                {
        //                    var temp = SendHtmlStringToSelectPDF(invoice, content);
        //                    if (temp.Result != null)
        //                    {
        //                        //Lưu file vào thư mục
        //                        string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
        //                        filePath = SaveInvoiceFile(temp.Result, invoice, fileName);
        //                    }
        //                }
        //                else
        //                {
        //                    long invoiceid;
        //                    RenderHtmlToPdf(invoice, htmlContent, out filePath, out invoiceid);
        //                }
        //                lstPathInsert.Add(Server.MapPath("~/NOVAON_FOLDER" + filePath));
        //            });
        //            tasks.Add(t);
        //            t.Start();
        //        }
        //        Task.WaitAll(tasks.ToArray());
        //        if (lstPathInsert.Count > 0)
        //            listFilePaths = lstPathInsert;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = ex.ToString();
        //        ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "CreateFileInputFolder");
        //    }
        //    return msg;
        //}

        public byte[] ParseHtml(String html)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StringReader sr = new StringReader(html);
                Document pdfDoc = new Document(PageSize.A5, 10f, 10f, 100f, 0f);
                pdfDoc.SetPageSize(PageSize.A5.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                // XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Khôi phục hóa đơn đã xóa
        /// </summary>
        /// <param name="idInvoices">danh sách id hóa đơn cần khôi phục</param>
        /// <returns></returns>
        public ActionResult RecoverInvoice(string idInvoices)
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
                string msg = invoiceBLL.RecoverInvoice(ids);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                return Json(new { rs = true, msg = $"Phục hồi thành công {lstInvoiceid.Length}/{lstInvoiceid.Length} hóa đơn." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), $"Lỗi phục hóa đơn bạn vui lòng thử lại", MethodBase.GetCurrentMethod().Name, "RecoverInvoice");
            }

            return Json(new { rs = true, msg = "Phục hồi thành công" });
        }

        /// <summary>
        /// Xem nhanh hóa đơn
        /// </summary>
        /// <param name="invoiceId">id hóa đơn cần xem</param>
        /// <returns></returns>
        public ActionResult QuickviewInvoice(long invoiceId)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                InvoiceBO invoice = invoiceBLL.GetInvoiceById(invoiceId);
                List<InvoiceDetailBO> lstInvoiceDetail = invoiceBLL.GetInvoiceDetail(invoiceId);
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                //Lấy detail
                // Kiểm tra khuyến mãi nếu có thì gán RETAILPRICE = 0
                foreach (var item in lstInvoiceDetail)
                {
                    if (item.ISPROMOTION)
                    {
                        item.RETAILPRICE = 0;
                    }
                    item.QUANTITY = Math.Round(item.QUANTITY, 2);
                }
                invoice.LISTPRODUCT = lstInvoiceDetail;
                var content = GetContentMultiplePage(invoice, lstInvoiceDetail);
                string totalContent = "";
                foreach (var i in content)
                {
                    totalContent = totalContent + i;
                }
                string fileHtml = Server.MapPath("~/Uploads/") + "quickView.html";
                System.IO.File.WriteAllText(fileHtml, totalContent);
                return Json(new { rs = true, data = "/Uploads/quickView.html?v=" + DateTime.Now.Millisecond });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "QuickviewInvoice");
                return Json(new { rs = false, data = "" });
            }
        }

        /// <summary>
        /// Chuyển đổi hóa đơn ngay tại màn hình quản lý hóa đơn
        /// </summary>
        /// <param name="invoice">obj invoice</param>
        /// <returns></returns>
        public ActionResult QuickviewInvoiceConvert(InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();


                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();
                //Lấy hóa đơn theo mã hóa đơn
                var objInvoice = invoiceBLL.GetInvoiceById(invoice.ID, invoice.USINGINVOICETYPE);
                objInvoice.LISTPRODUCT = invoiceBLL.GetInvoiceDetail(invoice.ID);

                //Lấy loại hóa đơn
                var invoiceType = objGlobalBLL.GetInvoiceTypeByID(4);

                //Gán biến là hóa đơn chuyển đổi để render ra mẫu hóa đơn chuyển đổi
                objInvoice.ISCONVERTED = true;
                objInvoice.CONVERTTIME = DateTime.Now;
                objInvoice.CONVERTUSERNAME = invoice.NAMECONVERT;
                objInvoice.INVOICETYPENAME = invoiceType.INVOICETYPENAME;

                List<InvoiceDetailBO> lstInvoiceDetail = invoiceBLL.GetInvoiceDetail(invoice.ID);
                // Tạo mã QR code gán vào thuộc tính QRCODEBASE64 = src hình ảnh bên template view
                invoice.QRCODEBASE64 = QRCodeHelper.CreateQRCode("https://onfinance.asia/tracuu/?referencecode=" + invoice.REFERENCECODE);
                var objGenerateInvoice = GetInvoiceContent(objInvoice, objInvoice.NUMBER);
                if (objGenerateInvoice != null)
                {
                    string fileName = objInvoice.COMTAXCODE + "_" + invoice.ID + "_" + objInvoice.INVOICETYPE + ".pdf";
                    string fileConvertedPath = invoiceBLL.SaveInvoiceFile(objGenerateInvoice, objInvoice, fileName);
                    foreach (var item in lstInvoiceDetail)
                    {
                        if (item.ISPROMOTION)
                        {
                            item.RETAILPRICE = 0;
                        }
                        item.QUANTITY = Math.Round(item.QUANTITY, 2);
                    }
                    objInvoice.LISTPRODUCT = lstInvoiceDetail;
                    objInvoice.SIGNLINK = fileConvertedPath;
                    var content = GetContentMultiplePage(objInvoice, lstInvoiceDetail);
                    string totalContent = "";
                    foreach (var i in content)
                    {
                        totalContent = totalContent + i;
                    }
                    string fileHtml = Server.MapPath("~/Uploads/") + "quickView.html";
                    System.IO.File.WriteAllText(fileHtml, totalContent);
                    return Json(new { rs = true, data = "/Uploads/quickView.html" });
                }
                else
                {
                    ConfigHelper.Instance.WriteLog("Lỗi chuyển đổi string sang pdf", "", MethodBase.GetCurrentMethod().Name, "QuickviewInvoiceConvert");
                    return Json(new { rs = false, msg = $"Không thể chuyển đổi hóa đơn, vui lòng thử lại." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { rs = false, data = "" });
            }
        }

        /// <summary>
        /// Chuyển đổi hóa đơn ở màn hình quản lý hóa đơn
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public ActionResult InvoiceConvert(InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL objInvoiceBLL = new InvoiceBLL();
                DS.BusinessLogic.GlobalBLL objGlobalBLL = new DS.BusinessLogic.GlobalBLL();

                //Lấy hóa đơn theo mã hóa đơn
                var objInvoice = objInvoiceBLL.GetInvoiceById(invoice.ID, invoice.USINGINVOICETYPE);
                objInvoice.LISTPRODUCT = objInvoiceBLL.GetInvoiceDetail(invoice.ID);

                //Lấy loại hóa đơn
                var invoiceType = objGlobalBLL.GetInvoiceTypeByID(4);

                //Gán biến là hóa đơn chuyển đổi để render ra mẫu hóa đơn chuyển đổi
                objInvoice.ISCONVERTED = true;
                objInvoice.CONVERTTIME = DateTime.Now;
                objInvoice.CONVERTUSERNAME = invoice.NAMECONVERT;
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

        #region Zalo 
        /// <summary>
        /// Gửi tin qua zalo theo sdt
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public ActionResult SendZalo(InvoiceBO invoice, string note)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                string code = (objUser.ZALOACCESSTOKEN == null || string.IsNullOrEmpty(objUser.ZALOACCESSTOKEN)) ? ConfigurationManager.AppSettings["AccessTokenZalo"] : objUser.ZALOACCESSTOKEN;
                string inputFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];
                ZaloClient client = new ZaloClient(code);
                //lấy thông tin khách hàng quan tâm theo sdt
                var resultInfo = client.getProfileOfFollower(invoice.CUSPHONENUMBER);
                DataUserInfo user = Newtonsoft.Json.JsonConvert.DeserializeObject<DataUserInfo>(resultInfo.ToString());

                //lấy file HĐ
                ServiceResult serviceResult = SaveTemplateInvoice(invoice.ID);
                if (serviceResult.ErrorCode == 1000)
                    return Json(new { rs = false, msg = serviceResult.Message }, JsonRequestBehavior.AllowGet);
                invoice = invoiceBLL.GetInvoiceById(invoice.ID, (int)Session["USINGINVOICETYPEID"]);
                if (invoice == null)
                    return Json(new { rs = false, msg = "Lỗi lấy thông tin hóa đơn gửi zalo." }, JsonRequestBehavior.AllowGet);
                string signLink = invoice.SIGNLINK == null ? "" : invoice.SIGNLINK;
                var dstFilePathPdf = Server.MapPath("~/" + inputFolder + signLink);

                //lấy mã token để gửi file
                ZaloFile file = new ZaloFile(dstFilePathPdf);
                file.setMediaTypeHeader("application/pdf");
                var result = client.uploadFileForOfficialAccountAPI(file);
                DataToken token = Newtonsoft.Json.JsonConvert.DeserializeObject<DataToken>(result.ToString());
                //gửi tin văn bản và file
                var message = String.Format(ConfigurationManager.AppSettings["MessageZalo"], objUser.COMNAME, CommonFunction.FormatMoney(invoice.TOTALPAYMENT, 0, ",", "."));
                var resultMsg = client.sendTextMessageToUserId(user.data.user_id, message);
                if (!string.IsNullOrEmpty(note))
                    resultMsg = client.sendTextMessageToUserId(user.data.user_id, note);
                var resultFile = client.sendFileToUserId(user.data.user_id, token.data.token);

                return Json(new { rs = true, result = "Gửi tin thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi gửi tin trên zalo", objEx, MethodBase.GetCurrentMethod().Name, "SendZalo");
                return Json(new { rs = false, msg = "Không tìm thấy bạn bè trên zalo. Vui lòng gửi quan tâm để tiếp tục gửi tin" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gửi zalo tới nhiều khách hàng
        /// </summary>
        /// <param name="invoices"></param>
        /// <returns></returns>
        public ActionResult SendMultipleZalo(List<InvoiceBO> invoices)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                string code = (objUser.ZALOACCESSTOKEN == null || string.IsNullOrEmpty(objUser.ZALOACCESSTOKEN)) ? ConfigurationManager.AppSettings["AccessTokenZalo"] : objUser.ZALOACCESSTOKEN;
                string inputFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];
                ZaloClient client = new ZaloClient(code);
                foreach (var item in invoices)
                {
                    //lấy thông tin khách hàng quan tâm theo sdt
                    var resultInfo = client.getProfileOfFollower(item.CUSPHONENUMBER);
                    DataUserInfo user = Newtonsoft.Json.JsonConvert.DeserializeObject<DataUserInfo>(resultInfo.ToString());
                    ServiceResult serviceResult = SaveTemplateInvoice(item.ID);
                    InvoiceBO invoice = new InvoiceBO();
                    if (serviceResult.ErrorCode == 1000)
                        return Json(new { rs = false, msg = serviceResult.Message }, JsonRequestBehavior.AllowGet);
                    invoice = invoiceBLL.GetInvoiceById(item.ID, (int)Session["USINGINVOICETYPEID"]);
                    if (invoice == null)
                        return Json(new { rs = false, msg = "Lỗi lấy thông tin hóa đơn gửi zalo." }, JsonRequestBehavior.AllowGet);
                    string signLink = item.SIGNLINK == null ? "" : item.SIGNLINK;
                    var dstFilePathPdf = Server.MapPath("~/" + inputFolder + signLink);

                    //lấy mã token để gửi file
                    ZaloFile file = new ZaloFile(dstFilePathPdf);
                    file.setMediaTypeHeader("application/pdf");
                    var result = client.uploadFileForOfficialAccountAPI(file);
                    DataToken token = Newtonsoft.Json.JsonConvert.DeserializeObject<DataToken>(result.ToString());
                    //gửi tin văn bản và file
                    var message = String.Format(ConfigurationManager.AppSettings["MessageZalo"], objUser.COMNAME, CommonFunction.FormatMoney(item.TOTALPAYMENT, 0, ",", "."));
                    var resultMsg = client.sendTextMessageToUserId(user.data.user_id, message);
                    var resultFile = client.sendFileToUserId(user.data.user_id, token.data.token);
                }
                return Json(new { rs = true, msg = "Gửi tin thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi gửi tin trên zalo", objEx, MethodBase.GetCurrentMethod().Name, "SendMultipleZalo");
                return Json(new { rs = false, msg = "Không tìm thấy bạn bè trên zalo. Vui lòng gửi quan tâm để tiếp tục gửi tin" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Xem lịch sử gửi tin cuả từng khách hàng theo sdt
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public ActionResult GetHistoryZalo(InvoiceBO invoice)
        {
            try
            {
                string code = ConfigurationManager.AppSettings["AccessTokenZalo"];
                ZaloClient client = new ZaloClient(code);
                //Lấy thông tin khách hàng quan tâm theo số điện thoại
                JObject resultInfo = client.getProfileOfFollower(invoice.CUSPHONENUMBER);
                DataUserInfo user = Newtonsoft.Json.JsonConvert.DeserializeObject<DataUserInfo>(resultInfo.ToString());

                //Lấy lịch sử gửi tin cho khách hàng theo user_id
                JObject HistoryMsg = client.getListConversationWithUser(Convert.ToInt64(user.data.user_id), 0, 5);
                MSGUser listMSG = Newtonsoft.Json.JsonConvert.DeserializeObject<MSGUser>(HistoryMsg.ToString());
                return Json(new { rs = true, result = listMSG.data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách lịch sử gửi mail của từng khách hàng", objEx, MethodBase.GetCurrentMethod().Name, "SendZalo");
                return Json(new { rs = false, msg = "Khách hàng này chưa quan tâm" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}