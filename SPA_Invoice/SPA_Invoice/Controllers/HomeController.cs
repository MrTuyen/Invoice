using DS.BusinessLogic.Account;
using DS.BusinessLogic.Company;
using DS.BusinessLogic.EmailSender;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Number;
using DS.BusinessObject;
using DS.BusinessObject.Company;
using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using DS.Common.Enums;
using DS.Common.Helpers;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPA_Invoice.Common;
using SPA_Invoice.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;

namespace SPA_Invoice.Controllers
{
    public class HomeController : BaseController
    {
        private static IHubContext hubContext;

        public HomeController()
        {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<SignlRConf>();
        }

        /// <summary>
        /// Kiểm tra khách hàng mở email. Cập nhật lại trạng thái email.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public bool Logo(string code)
        {
            if (code == "{EMAILID}")
                return false;
            long emailId = long.Parse(code);
            EmailBLL emailBLL = new EmailBLL();
            emailBLL.UpdateEmail(new EmailBO() { ID = emailId, CONTENT = null, STATUS = 2 });
            return true;
        }

        #region HSM
        /// <summary>
        /// Ký dải chờ hóa đơn HSM
        /// truongnv 20200227
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="signTime"></param>
        /// <param name="numberWaitingId"></param>
        /// <param name="waitingNumber"></param>
        /// <returns></returns>
        /// 
        public ActionResult SignWaitingHSMApi(long invoiceId, long numberId, long tempNumberWaiting, string signTime)
        {
            string msg = string.Empty;
            string signLinkXml = string.Empty;

            long nextNumber = 0;
            InvoiceNumberBO tempWaitingNumberObj = new InvoiceNumberBO();
            try
            {
                //Kiểm tra xem khách hàng có sử dụng chữ ký số hsm không
                AccountBLL accountBLL = new AccountBLL();
                UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(objUser.USERNAME, objUser.COMTAXCODE);
                if (userModel == null && string.IsNullOrWhiteSpace(userModel.APIURL))
                    return Json(new { rs = true, msg = "Khách hàng chưa đăng ký, ký số bằng HSM." });

                //Kiểm tra dữ liệu hóa đơn trước khi ký
                msg = ValidateBeforeSign(numberId, tempNumberWaiting, ref nextNumber, ref tempWaitingNumberObj);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });

                TempData["NextNumber"] = nextNumber;

                ViewBag.Date = signTime.Split('/');

                // Lấy thông tin hóa đơn
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                InvoiceBO invoice = invoiceBLL.GetInvoiceById(invoiceId);
                if (invoice == null)
                {
                    msg = "Không lấy được thông tin hóa đơn.";
                    return Json(new { rs = false, msg = msg, JsonRequestBehavior.AllowGet });
                }

                //Kiểm tra ký hiệu hóa đơn đang ký có khớp hay không
                if (invoice.SYMBOLCODE != tempWaitingNumberObj.SYMBOLCODE)
                {
                    msg = "Bạn không thể ký dải chờ của ký hiệu này cho ký hiệu khác.";
                    return Json(new { rs = false, msg = msg, JsonRequestBehavior.AllowGet });
                }
                //Lấy hóa đơn mẫu
                var bTemplateInvoice = GetInvoiceContent(invoice, nextNumber);
                if (bTemplateInvoice == null)
                {
                    msg = "Lấy mẫu hóa đơn không thành công.";
                    return Json(new { rs = false, msg = msg, JsonRequestBehavior.AllowGet });

                }
                //Template Xml CreateFileInvoiceXML
                //signLinkXml = invoiceBLL.CreateFileInvoiceWaitingXML(invoice, nextNumber, signTime, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE));
                signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, invoiceBLL.GetInvoiceDetail(invoice.ID), nextNumber, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE), signTime);
                if (signLinkXml.Length == 0)
                    return Json(new { rs = false, msg = "Không tạo được file xml.", JsonRequestBehavior.AllowGet });

                //Đường dẫn thư mục chứa file
                var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                //Thông tin xml cần ký
                string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                if (base64Xml == null)
                    return Json(new { rs = false, msg = "Lỗi chuyển đổi sang xml base64.", JsonRequestBehavior.AllowGet });

                //Tích hợp ký số bên Cyber HSM
                var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
                task.Wait();
                if (task.Result.status == 1000) // Thành công
                {
                    //Gán thời gian ký hóa đơn
                    DateTime dtsignTime = DateTime.ParseExact(signTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    msg = SaveInvoiceSigned(invoiceId, nextNumber, task.Result.base64xmlSigned, invoice, dtsignTime, numberId);
                    invoice.NUMBER = nextNumber;
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = "Lỗi cập nhật thông tin hóa đơn." });
                    return Json(new { rs = true, msg = $"Phát hành hóa đơn thành công.", info = invoice });
                }
                else
                    return Json(new { rs = false, msg = $"Phát hành hóa đơn lỗi." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFile");
                return Json(new { rs = false, msg = "Chưa lấy được file hóa đơn để ký. Vui lòng xem lại!" });
            }
        }

        /// <summary>
        /// Ký hóa đơn xml bằng công cụ HSM
        /// truongnv 20200226
        /// </summary>
        /// <param name="idInvoice">ID hóa đơn</param>
        /// <returns></returns>
        public ActionResult SignXmlHSMApi(long idInvoice)
        {
            var signLinkXml = string.Empty;
            long nextNumber = 0;
            string msg = string.Empty;
            try
            {
                //Kiểm tra xem khách hàng có sử dụng chữ ký số hsm không
                AccountBLL accountBLL = new AccountBLL();
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(objUser.USERNAME, objUser.COMTAXCODE);
                if (userModel == null && string.IsNullOrWhiteSpace(userModel.APIURL))
                    return Json(new { rs = true, msg = "Khách hàng chưa đăng ký, ký số bằng HSM." });

                msg = CheckNumberWaiting(idInvoice, ref nextNumber);
                if (msg.Length > 0)
                {
                    return Json(new { rs = false, msg, JsonRequestBehavior.AllowGet });
                }
                // Tránh lỗi trả về nextNumber = -1. Nếu = -1 thì lấy lại.
                if (nextNumber < 0)
                {
                    CheckNumberWaiting(idInvoice, ref nextNumber);
                }
                TempData["NextNumber"] = nextNumber;
                
                // Lấy thông tin hóa đơn
                InvoiceBO invoice = invoiceBLL.GetInvoiceById(idInvoice);
                invoice.LISTPRODUCT = invoiceBLL.GetInvoiceDetail(invoice.ID);
                if (invoice == null)
                {
                    msg = "Không lấy được thông tin hóa đơn.";
                    return Json(new { rs = false, msg, JsonRequestBehavior.AllowGet });
                }

                invoice.CUSEMAIL = string.IsNullOrEmpty(invoice.CUSEMAIL) ? invoice.CUSEMAILSEND : invoice.CUSEMAIL;
                //Template Xml
                signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, invoice.LISTPRODUCT, nextNumber, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE));
                if (signLinkXml.Length == 0)
                    return Json(new { rs = false, msg = "Không tạo được file xml.", JsonRequestBehavior.AllowGet });

                //Đường dẫn thư mục chứa file
                var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                //Thông tin xml cần ký
                string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                if (base64Xml == null)
                    return Json(new { rs = false, msg = "Lỗi chuyển đổi sang xml base64.", JsonRequestBehavior.AllowGet });

                //Tích hợp ký số bên Cyber HSM
                var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
                task.Wait();
                if (task.Result.status == 1000) // Thành công
                {
                    //Gán thời gian ký hóa đơn
                    DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    msg = SaveInvoiceSigned(idInvoice, nextNumber, task.Result.base64xmlSigned, invoice, dtSigntime);
                    invoice.NUMBER = nextNumber;
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = "Lỗi cập nhật thông tin hóa đơn." });
                    return Json(new { rs = true, msg = $"Phát hành hóa đơn thành công.", info = invoice });
                }
                else
                    return Json(new { rs = false, msg = $"Phát hành hóa đơn không thành công." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFile");
                return Json("Chưa lấy được file hóa đơn để ký. Vui lòng xem lại!", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// truongnv 20200421
        /// Ký hàng loạt
        /// </summary>
        /// <param name="idInvoices"></param>
        /// <returns></returns>
        private static List<InvoiceBO> listGlobalInvoiceSendMail = new List<InvoiceBO>();
        public ActionResult signXmlHSMApiMultiple(string idInvoices)
        {
            NumberBLL numberBLL = new NumberBLL();

            var signLinkXml = string.Empty;
            string msgSignSuccess = string.Empty;
            int rowSigned = 0;
            int totalRowSign = 0;
            string msg = string.Empty;

            var dicImportInvoiceError = new Dictionary<long, long>();
            var dicResultInvoices = new Dictionary<InvoiceBO, string>();
            List<SignInvoiceOutput> signInvoiceOutputs = new List<SignInvoiceOutput>();
            ConcurrentBag<SignInvoiceData> signInvoiceDatas = new ConcurrentBag<SignInvoiceData>(); // Thread safe while using Parallel
            var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"]; //Đường dẫn thư mục chứa file

            InvoiceBLL invoiceBLL = new InvoiceBLL();
            try
            {
                //Kiểm tra xem khách hàng có sử dụng chữ ký số hsm không
                AccountBLL accountBLL = new AccountBLL();
                UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(objUser.USERNAME, objUser.COMTAXCODE);
                if (userModel == null && string.IsNullOrWhiteSpace(userModel.APIURL))
                    return Json(new { rs = true, msg = "Khách hàng chưa đăng ký, ký số bằng HSM." });

                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                totalRowSign = lstInvoiceid.Length;

                // Lấy ra chi tiết hóa đơn
                Parallel.ForEach(lstInvoiceid, inv =>
                {
                    long idInvoice = CommonFunction.NullSafeLonger(inv, 0);
                    InvoiceBO invoice = invoiceBLL.GetInvoiceById(idInvoice,(int)Session["USINGINVOICETYPEID"]);
                    invoice.CUSEMAIL = string.IsNullOrEmpty(invoice.CUSEMAIL) ? invoice.CUSEMAILSEND : invoice.CUSEMAIL;
                    var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                    SignInvoiceData obj = new SignInvoiceData();
                    obj.CusCode = invoice.CUSTOMERCODE;
                    obj.InvoiceData = invoice;
                    obj.lstDetails = lstProducts;
                    signInvoiceDatas.Add(obj);
                });

                //Sắp xếp lại dữ liệu
                //var oDatas = signInvoiceDatas.OrderBy(x => x.CusCode).ToList();
                var oDatas = signInvoiceDatas.OrderBy(x => x.InvoiceData.SIGNINDEX).ToList();

                var invoicesCount = oDatas.Count;
                // Lấy ra list số cần ký theo số lượng chọn hóa đơn
                List<NextNumber> listNextNumbers = null;
                if (listNextNumbers == null)
                {
                    listNextNumbers = numberBLL.GetNextNumberByInvoiceIdUSB(oDatas[0].InvoiceData.ID, invoicesCount);
                    if (listNextNumbers.Count == 0)
                    {
                        return new JsonResult()
                        {
                            Data = new { rs = false, msg = "Phát hành không thành công. Dải hóa đơn này đã hết số." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };
                    }

                    var listNextNumbersCount = listNextNumbers.Count;
                    if (listNextNumbersCount < invoicesCount)
                    {
                        return new JsonResult()
                        {
                            Data = new { rs = false, msg = $"Phát hành không thành công. Số lượng số còn lại là {listNextNumbersCount}. Số hóa đơn bạn chọn là {invoicesCount}." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };
                    }
                    ConfigHelper.Instance.WriteLog($"List so tiep theo {string.Join(",", listNextNumbers.Select(x => x.ID))}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple=>B2");                   
                }

                // Gán số cho invoice object để tạo file xml
                for (int i = 0; i < invoicesCount; i++)
                {
                    oDatas[i].InvoiceData.NUMBER = listNextNumbers[i].ID;
                }

                string invoiceName = GetInvoiceNameXmlFile(objUser.USINGINVOICETYPE);
                // Chia luồng khi data đã được chuẩn bị sẵn.

                Dictionary<long, string> listErrorInvoice = new Dictionary<long, string>();
                List<InvoiceBO> listErrorInvoices = new List<InvoiceBO>();
                int taskNum = 20;
                int qtyperTask = 0;
                CalTaskNumber(invoicesCount, ref qtyperTask, ref taskNum);
                List<InvoiceBO> listInvoicesSendMail = new List<InvoiceBO>();
                DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string invoiceFolder = ConfigurationManager.AppSettings["SignHSMFolder"]; //Đường dẫn thư mục chứa file
                var userID = objUser.USERNAME;
                Session["InvoiceId"] = oDatas.FirstOrDefault().InvoiceData.ID;
                Session["CurrentNumber"] = listNextNumbers.LastOrDefault().ID;
                ProgressViewModel model = new ProgressViewModel();
                model.TotalRow = invoicesCount;
                List<Task> listTask = new List<Task>();
                for (int i = 0; i < taskNum; i++)
                {
                    int tempI = i;
                    var items = oDatas.Skip(tempI * qtyperTask).Take(qtyperTask).ToList();
                    var task = Task.Run(() =>
                    {
                        foreach (var item in items)
                        {
                            InvoiceBO invoice = item.InvoiceData;
                            invoice.LISTPRODUCT = item.lstDetails;
                            //Template Xml
                            string invTypeName = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                            invoice.SIGNLINK = string.IsNullOrEmpty(invoice.SIGNLINK) ? string.Format("/{0}/{1}/{5}/{2}_{3}_{4}.pdf", invoice.COMTAXCODE, invoice.ID, invoice.COMTAXCODE, invoice.ID, invoice.INVOICETYPE, invTypeName) : invoice.SIGNLINK;

                            string dstFilePathXml = string.Empty;
                            signLinkXml = invoiceBLL.CreateFileInvoiceXMLAPI(invoice, invoice.LISTPRODUCT, invoice.NUMBER, invoiceFolder, invoiceName);
                            dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                            if (signLinkXml.Length == 0)
                            {
                                listErrorInvoices.Add(invoice);
                                listErrorInvoice.Add(invoice.ID, "Lỗi tạo file xml");
                            }

                            //Thông tin xml cần ký
                            string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                            if (base64Xml == null)
                            {
                                listErrorInvoices.Add(invoice);
                                listErrorInvoice.Add(invoice.ID, "Lỗi chuyển đổi sang xml base64.");
                            }

                            var taskCyber = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));// nếu ko đc, chỗ này em bỏ đi để nó chạy cái GlobalHost thoi
                            if (taskCyber.Result.status == 1000) // Thành công
                            {
                                //Gán thời gian ký hóa đơn
                                msg = SaveInvoiceSigned(invoice.ID, invoice.NUMBER, taskCyber.Result.base64xmlSigned, invoice, dtSigntime);
                                if (msg.Length > 0)
                                {
                                    listErrorInvoices.Add(invoice);
                                    listErrorInvoice.Add(invoice.ID, "Lưu cập nhật sau ký thất bại.");
                                    continue; 
                                }
                                rowSigned = rowSigned + 1;
                                model.CurrentRow = rowSigned;
                                model.IsSuccess = rowSigned == model.TotalRow;
                                GlobalHost.ConnectionManager.GetHubContext<SignlRConf>().Clients.Group(userID).newMessageReceivedSignInvoice(model);
                                listInvoicesSendMail.Add(invoice);
                                listGlobalInvoiceSendMail = listInvoicesSendMail;
                            }
                            else
                            {
                                //Cho ký lại nhưng hóa đơn khi gửi sang Cyber trả lại lSsỗi lần 1
                                var reSign = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));

                                if (reSign.Result.status == 1000) // Thành công
                                {
                                    //Gán thời gian ký hóa đơn
                                    msg = SaveInvoiceSigned(invoice.ID, invoice.NUMBER, taskCyber.Result.base64xmlSigned, invoice, dtSigntime);
                                    if (msg.Length > 0)
                                    {
                                        listErrorInvoices.Add(invoice);
                                        listErrorInvoice.Add(invoice.ID, "Lưu cập nhật sau ký thất bại.");
                                        continue;
                                    }
                                    rowSigned = rowSigned + 1;
                                    model.CurrentRow = rowSigned;
                                    model.IsSuccess = rowSigned == model.TotalRow;
                                    GlobalHost.ConnectionManager.GetHubContext<SignlRConf>().Clients.Group(userID).newMessageReceivedSignInvoice(model);
                                    listInvoicesSendMail.Add(invoice);
                                    listGlobalInvoiceSendMail = listInvoicesSendMail;
                                }
                                else
                                {
                                    msgSignSuccess = "không tạo được chữ ký HSM.";
                                    listErrorInvoices.Add(invoice);
                                    listErrorInvoice.Add(invoice.ID, msgSignSuccess);
                                    ConfigHelper.Instance.WriteLog($"Phát hành hóa đơn không thành công. Kết quả trả về từ Cyber: {taskCyber.Result.status}. Des: {taskCyber.Result.description}. InvoiceID = {invoice.ID}. Number = {invoice.NUMBER}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
                                }
                            }
                        }
                    });

                    listTask.Add(task);
                }

                Task.WaitAll(listTask.ToArray());
                // Cho ký lại nhưng hóa đơn khi gửi sang Cyber trả lại lỗi lần 2
                if (listErrorInvoices != null && listErrorInvoices.Count > 0)
                {
                    foreach (var invoice in listErrorInvoices)
                    {
                        string invTypeName = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                        invoice.SIGNLINK = string.IsNullOrEmpty(invoice.SIGNLINK) ? string.Format("/{0}/{1}/{5}/{2}_{3}_{4}.pdf", invoice.COMTAXCODE, invoice.ID, invoice.COMTAXCODE, invoice.ID, invoice.INVOICETYPE, invTypeName) : invoice.SIGNLINK;

                        string dstFilePathXml = string.Empty;
                        signLinkXml = invoiceBLL.CreateFileInvoiceXMLAPI(invoice, invoice.LISTPRODUCT, invoice.NUMBER, invoiceFolder, invoiceName);
                        dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                        if (signLinkXml.Length == 0)
                        {
                            listErrorInvoices.Add(invoice);
                            listErrorInvoice.Add(invoice.ID, "Lỗi tạo file xml");
                        }

                        //Thông tin xml cần ký
                        string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                        if (base64Xml == null)
                        {
                            listErrorInvoices.Add(invoice);
                            listErrorInvoice.Add(invoice.ID, "Lỗi chuyển đổi sang xml base64.");
                        }

                        var taskCyber = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
                        if (taskCyber.Result.status == 1000) // Thành công
                            msg = SaveInvoiceSigned(invoice.ID, invoice.NUMBER, taskCyber.Result.base64xmlSigned, invoice, dtSigntime);
                    }
                }

                // Update current number
                if (Session["InvoiceId"] == null || Session["CurrentNumber"] == null)
                {
                    ConfigHelper.Instance.WriteLog("Ký hàng loạt hsm. Cập nhật số hiện tại.", "Null Session", MethodBase.GetCurrentMethod().Name, "UpdateAfterSigning");
                    return Json(new { rs = false, msg = "Đã hết thời gian thao tác.", JsonRequestBehavior.AllowGet });
                }
                var isSuccess = numberBLL.UpdateCurrentNumber(int.Parse(Session["InvoiceId"].ToString()), int.Parse(Session["CurrentNumber"].ToString()));
                if (!isSuccess)
                    return Json(new { rs = false, msg = "Update current number failed.", JsonRequestBehavior.AllowGet });

                return Json(new { rs = true, msg = $"Phát hành hóa đơn thành công", isNoti = false });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
                UpdateCurrentNumber(); // Xử lý cập nhật số hóa đơn lớn nhất đã phát hành vào số hiện tại nếu có lỗi.
                return Json(new { rs = false, msg = "Phát hành hóa đơn không thành công.", JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult UpdateAfterSigning() {
            try
            {
                // Update current number
                if (Session["InvoiceId"] == null || Session["CurrentNumber"] == null)
                {
                    ConfigHelper.Instance.WriteLog("Ký hàng loạt hsm. Cập nhật số hiện tại.", "Null Session", MethodBase.GetCurrentMethod().Name, "UpdateAfterSigning");
                    return Json(new { rs = false, msg = "Đã hết thời gian thao tác. Null invoiceid, current number session.", JsonRequestBehavior.AllowGet });
                }
                NumberBLL numberBLL = new NumberBLL();
                var isSuccess = numberBLL.UpdateCurrentNumber(int.Parse(Session["InvoiceId"].ToString()), int.Parse(Session["CurrentNumber"].ToString()));
                if (!isSuccess)
                    return Json(new { rs = false, msg = "Update current number failed.", JsonRequestBehavior.AllowGet });

                // Cấu hình gửi email tự động sau khi phát hành thành công nếu trong hóa đơn có email của khách hàng
                if (listGlobalInvoiceSendMail == null)
                {
                    ConfigHelper.Instance.WriteLog("Ký hàng loạt hsm.Gửi email hàng loạt.", "Null Email list", MethodBase.GetCurrentMethod().Name, "UpdateAfterSigning");
                    return Json(new { rs = false, msg = "Đã hết thời gian thao tác. Null Email list.", JsonRequestBehavior.AllowGet });
                }
                List<InvoiceBO> listInvoicesSendMail = listGlobalInvoiceSendMail;
                SendMultipleEmail(listInvoicesSendMail);

                return Json(new { rs = true, msg = "Update current number and Send mail.", JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Ký hàng loạt hsm. Cập nhật số hiện tại.", ex, MethodBase.GetCurrentMethod().Name, "UpdateAfterSigning");
                return Json(new { rs = false, msg = "Update current number failed.", JsonRequestBehavior.AllowGet });
            }
            finally
            {
                listGlobalInvoiceSendMail = new List<InvoiceBO>();
            }
        }

        private void UpdateCurrentNumber()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                NumberBLL numberBLL = new NumberBLL();
                // Update current number
                if (Session["InvoiceId"] == null || Session["CurrentNumber"] == null)
                    ConfigHelper.Instance.WriteLog("Ký hàng loạt hsm. Cập nhật số hiện tại.", "Null Session", MethodBase.GetCurrentMethod().Name, "UpdateCurrentNumber");

                var inv = invoiceBLL.GetInvoiceById(long.Parse(Session["InvoiceId"].ToString()));
                var maxInvoiceNumber = invoiceBLL.GetMaxNumberInvoice(new FormSearchInvoice() { COMTAXCODE = inv.COMTAXCODE, FORMCODE = inv.FORMCODE, SYMBOLCODE = inv.SYMBOLCODE });

                var isSuccess = numberBLL.UpdateCurrentNumber(inv.ID, inv.NUMBER);
                if (!isSuccess)
                    ConfigHelper.Instance.WriteLog("Update current number failed.", "Update current number failed.", MethodBase.GetCurrentMethod().Name, "UpdateCurrentNumber");
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex.InnerException.ToString(), MethodBase.GetCurrentMethod().Name, "UpdateCurrentNumber");
            }
        }


        //public ActionResult signXmlHSMApiMultiple(string idInvoices)
        //{
        //    var signLinkXml = string.Empty;
        //    long nextNumber;
        //    string msgSignSuccess = string.Empty;
        //    int rowSigned = 0;
        //    int totalRowSign = 0;
        //    string msg = string.Empty;

        //    var dicImportInvoiceError = new Dictionary<long, long>();
        //    var dicResultInvoices = new Dictionary<InvoiceBO, string>();
        //    List<SignInvoiceOutput> signInvoiceOutputs = new List<SignInvoiceOutput>();
        //    //List<SignInvoiceData> signInvoiceDatas = new List<SignInvoiceData>();
        //    ConcurrentBag<SignInvoiceData> signInvoiceDatas = new ConcurrentBag<SignInvoiceData>(); // Thread safe while using Parallel
        //    var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"]; //Đường dẫn thư mục chứa file

        //    InvoiceBLL invoiceBLL = new InvoiceBLL();
        //    try
        //    {
        //        //Kiểm tra xem khách hàng có sử dụng chữ ký số hsm không
        //        AccountBLL accountBLL = new AccountBLL();
        //        UserModel userModel = accountBLL.CheckUserNameUsedKysoHSM(objUser.USERNAME, objUser.COMTAXCODE);
        //        if (userModel == null && string.IsNullOrWhiteSpace(userModel.APIURL))
        //            return Json(new { rs = true, msg = "Khách hàng chưa đăng ký, ký số bằng HSM." });

        //        string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //        totalRowSign = lstInvoiceid.Length;

        //        // Lấy ra chi tiết hóa đơn
        //        Parallel.ForEach(lstInvoiceid, inv =>
        //        {
        //            long idInvoice = CommonFunction.NullSafeLonger(inv, 0);
        //            InvoiceBO invoice = invoiceBLL.GetInvoiceById(idInvoice, (int)Session["USINGINVOICETYPEID"]);
        //            invoice.CUSEMAIL = string.IsNullOrEmpty(invoice.CUSEMAIL) ? invoice.CUSEMAILSEND : invoice.CUSEMAIL;
        //            var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
        //            SignInvoiceData obj = new SignInvoiceData();
        //            obj.CusCode = invoice.CUSTOMERCODE;
        //            obj.InvoiceData = invoice;
        //            obj.lstDetails = lstProducts;
        //            signInvoiceDatas.Add(obj);
        //        });


        //        ConfigHelper.Instance.WriteLog($"Bắt đầu ký {totalRowSign} HĐ. KH: {objUser.COMTAXCODE}.StartTime:{DateTime.Now}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple=>B1");

        //        //Sắp xếp lại dữ liệu
        //        //var oDatas = signInvoiceDatas.OrderBy(x => x.CusCode).ToList();
        //        var oDatas = signInvoiceDatas.OrderBy(x => x.InvoiceData.SIGNINDEX).ToList();

        //        //Thực hiện ký
        //        List<InvoiceBO> listInvoicesSendMail = new List<InvoiceBO>();
        //        foreach (var item in oDatas)
        //        {
        //            InvoiceBO invoice = item.InvoiceData;
        //            long idInvoice = invoice.ID;
        //            msg = CheckNumberWaiting(invoice.COMTAXCODE, invoice.FORMCODE, invoice.SYMBOLCODE, out nextNumber);
        //            if (msg.Length > 0)
        //            {
        //                return Json(new { rs = false, msg, JsonRequestBehavior.AllowGet });
        //            }
        //            // Tránh lỗi trả về nextNumber = -1. Nếu = -1 thì lấy lại.
        //            if (nextNumber < 0)
        //            {
        //                CheckNumberWaiting(invoice.COMTAXCODE, invoice.FORMCODE, invoice.SYMBOLCODE, out nextNumber);
        //            }
        //            //Template Xml
        //            string invTypeName = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
        //            invoice.SIGNLINK = string.IsNullOrEmpty(invoice.SIGNLINK) ? string.Format("/{0}/{1}/{5}/{2}_{3}_{4}.pdf", invoice.COMTAXCODE, invoice.ID, invoice.COMTAXCODE, invoice.ID, invoice.INVOICETYPE, invTypeName) : invoice.SIGNLINK;

        //            string dstFilePathXml = string.Empty;
        //            signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, item.lstDetails, nextNumber, GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE));
        //            invoice.SIGNLINK = string.IsNullOrEmpty(invoice.SIGNLINK) ? string.Format("/{0}/{1}/{5}/{2}_{3}_{4}.pdf", invoice.COMTAXCODE, invoice.ID, invoice.COMTAXCODE, invoice.ID, invoice.INVOICETYPE, invTypeName) : invoice.SIGNLINK;

        //            dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

        //            if (signLinkXml.Length == 0)
        //                return Json(new { rs = false, msg = "Không tạo được file xml.", JsonRequestBehavior.AllowGet });

        //            //Thông tin xml cần ký
        //            string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
        //            if (base64Xml == null)
        //                return Json(new { rs = false, msg = "Lỗi chuyển đổi sang xml base64.", JsonRequestBehavior.AllowGet });

        //            //Tích hợp ký số bên Cyber HSM
        //            var task = Task.Run(() => CommonFunction.SignInvoice(base64Xml, userModel.APIURL, userModel.APIID, userModel.SECRET));
        //            if (task.Result.status == 1000) // Thành công
        //            {
        //                //Gán thời gian ký hóa đơn
        //                DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //                msg = SaveInvoiceSigned(invoice.ID, nextNumber, task.Result.base64xmlSigned, invoice, dtSigntime);
        //                invoice.NUMBER = nextNumber;
        //                invoice.SIGNTIME = dtSigntime;
        //                invoice.SIGNEDTIME = dtSigntime;
        //                invoice.INVOICESTATUS = 2;

        //                if (msg.Length > 0)
        //                {
        //                    dicImportInvoiceError.Add(idInvoice, idInvoice);
        //                    continue; //return Json(new { rs = false, msg = "Lỗi cập nhật thông tin hóa đơn." });
        //                }

        //                listInvoicesSendMail.Add(invoice);
        //                rowSigned++;
        //            }
        //            else
        //            {
        //                dicImportInvoiceError.Add(idInvoice, idInvoice);
        //                msgSignSuccess = "không tạo được chữ ký HSM.";
        //                ConfigHelper.Instance.WriteLog($"Phát hành hóa đơn không thành công. Kết quả trả về từ Cyber: {task.Result.status}. Des: {task.Result.description}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
        //            }
        //        }

        //        ConfigHelper.Instance.WriteLog($"Kết thúc ký {totalRowSign} HĐ.KH: {objUser.COMTAXCODE}.EndTime:{DateTime.Now}", string.Empty, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple=>B2");
        //        if (rowSigned > 0)
        //        {
        //            // Cấu hình gửi email tự động sau khi phát hành thành công nếu trong hóa đơn có email của khách hàng
        //            SendMultipleEmail(listInvoicesSendMail);
        //            return Json(new { rs = true, msg = $"Phát hành hóa đơn thành công {rowSigned}/{totalRowSign} hóa đơn.", rowData = JsonConvert.SerializeObject(dicImportInvoiceError), JsonRequestBehavior.AllowGet });
        //        }
        //        else
        //            return Json(new { rs = false, msg = $"Phát hành hóa đơn không thành công. Chi tiết {msgSignSuccess}", rowData = JsonConvert.SerializeObject(dicImportInvoiceError), JsonRequestBehavior.AllowGet });
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "signXmlHSMApiMultiple");
        //        return Json(new { rs = false, msg = "Phát hành hóa đơn không thành công.", JsonRequestBehavior.AllowGet });
        //    }
        //}
        #endregion

        #region Ký hóa đơn chờ
        public JsonResult GetFileWaiting(string invoiceId, string signTime, string numberWaitingId, string waitingNumber)
        {
            try
            {
                string[] d = signTime.Split('/');
                ViewBag.Date = d;
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                NumberBLL numberBLL = new NumberBLL();
                var id = int.Parse(invoiceId);
                var numberId = int.Parse(numberWaitingId);
                var signLinkPdf = string.Empty;
                var signLinkXml = string.Empty;
                long nextNumber = 0;
                long tempNumberWaiting = long.Parse(waitingNumber);
                var tempWaitingNumberObj = new InvoiceNumberBO();
                try
                {
                    tempWaitingNumberObj = numberBLL.GetWaitingNumberByWaitingNumberId(numberId);
                    if (tempWaitingNumberObj == null)
                    {
                        ConfigHelper.Instance.WriteLog("Không tồn tại dải hóa đơn.", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                        return new JsonResult()
                        {
                            Data = new { rs = false, msg = "Không tồn tại dải hóa đơn." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };
                    }
                    else
                    {
                        var countNumbers = (tempWaitingNumberObj.TONUMBER - tempWaitingNumberObj.FROMNUMBER) + 1;
                        var lstUsedNumbers = tempWaitingNumberObj.USEDNUMBER;

                        if (tempNumberWaiting <= 0)
                        {
                            ConfigHelper.Instance.WriteLog("Lỗi lấy dải hóa đơn", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                            return new JsonResult()
                            {
                                Data = new { rs = false, msg = "Số hóa đơn không được nhỏ hơn 0." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                MaxJsonLength = Int32.MaxValue
                            };
                        }
                        if (tempNumberWaiting < tempWaitingNumberObj.FROMNUMBER)
                        {
                            ConfigHelper.Instance.WriteLog($"Lỗi lấy dải hóa đơn. tempNumberWaiting: {tempNumberWaiting}, tempWaitingNumberObj.FROMNUMBER: {tempWaitingNumberObj.FROMNUMBER}", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                            return new JsonResult()
                            {
                                Data = new { rs = false, msg = "Số hóa đơn không nhỏ hơn số bắt đầu." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                MaxJsonLength = Int32.MaxValue
                            };
                        }
                        if (tempNumberWaiting > tempWaitingNumberObj.TONUMBER)
                        {
                            ConfigHelper.Instance.WriteLog("Lỗi lấy dải hóa đơn", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                            return new JsonResult()
                            {
                                Data = new { rs = false, msg = "Số hóa đơn không lớn hơn số đến." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                MaxJsonLength = Int32.MaxValue
                            };
                        }
                        if (lstUsedNumbers != null)
                        {
                            if (countNumbers == lstUsedNumbers.Split(',').Count())
                            {
                                ConfigHelper.Instance.WriteLog("Lỗi lấy dải hóa đơn", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                                return new JsonResult()
                                {
                                    Data = new { rs = false, msg = "Dải hóa đơn này đã hết số." },
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                    MaxJsonLength = Int32.MaxValue
                                };
                            }
                            foreach (var item in lstUsedNumbers.Split(','))
                            {
                                if (item == waitingNumber)
                                {
                                    ConfigHelper.Instance.WriteLog($"Số {tempNumberWaiting} đã được sử dụng.", "", MethodBase.GetCurrentMethod().Name, "GetFileWaiting");
                                    return new JsonResult()
                                    {
                                        Data = new { rs = false, msg = "Số đã được sử dụng." },
                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                        MaxJsonLength = Int32.MaxValue
                                    };
                                }
                            }
                        }
                        nextNumber = tempNumberWaiting;
                        TempData["NextNumber"] = nextNumber;
                    }
                }
                catch (Exception ex)
                {
                    ConfigHelper.Instance.WriteLog("Lỗi lấy dải hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetNumberBydInvoiceId");
                    return new JsonResult()
                    {
                        Data = new { rs = false, msg = "Vui lòng xem lại thông báo phát hành dải hóa đơn." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }

                // get invoice's information
                var invoice = invoiceBLL.GetInvoiceById(id);
                if (invoice.SYMBOLCODE != tempWaitingNumberObj.SYMBOLCODE)
                {
                    ConfigHelper.Instance.WriteLog("Lỗi lấy dải hóa đơn của ký hiệu khác.", "", MethodBase.GetCurrentMethod().Name, "GetNumberBydInvoiceId");
                    return new JsonResult()
                    {
                        Data = new { rs = false, msg = "Bạn không thể ký dải chờ của ký hiệu này cho ký hiệu khác."},
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }

                string invoiceName = GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE);
                //signLinkXml = invoiceBLL.CreateFileInvoiceWaitingXML(invoice, nextNumber, signTime, invoiceName);
                signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, invoiceBLL.GetInvoiceDetail(invoice.ID) ,nextNumber, invoiceName, signTime);
                var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                Data data = new Data()
                {
                    Base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml))
                };
                return new JsonResult()
                {
                    Data = new { rs = true, msg = "Phát hành hóa đơn thành công.", data = data },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đọc file hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetFile");
                return Json("Chưa lấy được file hóa đơn để ký. Vui lòng xem lại!", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveFileWaiting(string invoiceId, string signTime, string numberWaitingId, string base64Xml, string subject)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                NumberBLL numberBLL = new NumberBLL();
                var id = int.Parse(invoiceId);
                var numberId = int.Parse(numberWaitingId);
                long tempNextNumber = 0;
                string[] d = signTime.Split('/');
                ViewBag.Date = d;
                DateTime dtSignTime = DateTime.ParseExact(signTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                List<InvoiceBO> listInvoicesSendMail = new List<InvoiceBO>();
                var msg = "";
                long.TryParse(TempData["NextNumber"].ToString(), out tempNextNumber);
                if (tempNextNumber <= 0)
                {
                    ConfigHelper.Instance.WriteLog(TempData["NextNumber"] + "Lỗi số hóa đơn kỳ lấy từ tempdata bị mất.", "", MethodBase.GetCurrentMethod().Name, "tempNextNumber");
                    return Json(new { rs = false, msg = "Phát hành không thành công. Vui lòng phát hành lại.", JsonRequestBehavior.AllowGet });
                }
                var invoice = invoiceBLL.GetInvoiceById(long.Parse(invoiceId));
                msg = SaveInvoiceSigned(id, tempNextNumber, base64Xml, invoice, dtSignTime, numberId);
                if (string.IsNullOrEmpty(msg))
                    listInvoicesSendMail.Add(invoice);
                // Cấu hình gửi email tự động sau khi phát hành thành công nếu trong hóa đơn có email của khách hàng
                if (objUser.AUTOSENDMAIL)
                {
                    SendMultipleEmail(listInvoicesSendMail);
                }
                return Json(new { rs = true, msg = $"Phát hành thành công", JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), "", MethodBase.GetCurrentMethod().Name, "SaveFileWaitting");
                return Json(new { rs = false, msg = "Phát hành hóa đơn không thành công. Vui lòng phát hành lại.", JsonRequestBehavior.AllowGet });
            }
        }
        #endregion

        #region Ký hóa đơn thường với USB. 1 hóa đơn hoặc nhiều hóa đơn.
        //[AuthorizeUser(Role = (int)UserRole.KY)]
        public ActionResult SignXmlUSBApiMultiple(string idInvoices)
        {
            NumberBLL numberBLL = new NumberBLL();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            var signLinkXml = string.Empty;
            string msgSignSuccess = string.Empty;
            int totalRowSign = 0;
            string msg = string.Empty;
            List<Data> listData = new List<Data>();
            List<SignInvoiceData> signInvoiceDatas = new List<SignInvoiceData>();
            var inputFolder = ConfigurationManager.AppSettings["InputInvoiceFolder"]; //Đường dẫn thư mục chứa file

            try
            {
                if (string.IsNullOrEmpty(idInvoices))
                {
                    return new JsonResult()
                    {
                        Data = new { rs = false, msg = "Phát hành hóa đơn không thành công. Bạn chưa chọn hóa đơn để ký." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                string[] lstInvoiceid = idInvoices.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                totalRowSign = lstInvoiceid.Length;
                var invoiceQuantities = int.Parse(ConfigHelper.AllowSigningNumberoOfInvoice);
                if (totalRowSign > invoiceQuantities)
                {
                    return new JsonResult()
                    {
                        Data = new { rs = false, msg = $"Phát hành hóa đơn không thành công. Số lượng hóa đơn tối đa khi ký hàng loạt bằng USB không lớn hơn: {invoiceQuantities}/ 1 lần ký." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                //// Kiểm tra đang ký
                //objNumber = numberBLL.GetNumberRecord(long.Parse(lstInvoiceid[0]));
                //if (objNumber == null)
                //{
                //    return new JsonResult()
                //    {
                //        Data = new { rs = false, msg = "Phát hành hóa đơn không thành công. Lỗi kiểm tra trạng thái đang ký." },
                //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //        MaxJsonLength = Int32.MaxValue
                //    };
                //}
                //if (objNumber.ISSIGNING)
                //{
                //    return new JsonResult()
                //    {
                //        Data = new { rs = false, msg = "Có người đang phát hành. Trong cùng một thời điểm không thể có nhiều hơn hai người phát hành." },
                //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //        MaxJsonLength = Int32.MaxValue
                //    };
                //}


                //Lấy ra chi tiết hóa đơn
                var context = System.Web.HttpContext.Current;
                var usingInvoiceType = (int)context.Session["USINGINVOICETYPEID"];
                Parallel.ForEach(lstInvoiceid, inv =>
                {
                    long idInvoice = CommonFunction.NullSafeLonger(inv, 0);
                    InvoiceBO invoice = invoiceBLL.GetInvoiceById(idInvoice, context, usingInvoiceType);//invoiceBLL.GetInvoiceByIdAPI(idInvoice);
                    invoice.CUSEMAIL = string.IsNullOrEmpty(invoice.CUSEMAIL) ? invoice.CUSEMAILSEND : invoice.CUSEMAIL;
                    var lstProducts = invoiceBLL.GetInvoiceDetail(invoice.ID);
                    SignInvoiceData obj = new SignInvoiceData();
                    obj.CusCode = invoice.CUSTOMERCODE;
                    obj.InvoiceData = invoice;
                    obj.lstDetails = lstProducts;
                    signInvoiceDatas.Add(obj);
                });

                // Sắp xếp theo ID và lọc những hóa đơn chưa ký nếu người dùng ký song song trước sau. Người 1 chọn hóa đơn x,y,z người 2 cũng x,y,z nhưng người 1 đã ký
                signInvoiceDatas = signInvoiceDatas.Where(x => x.InvoiceData.INVOICESTATUS == (int)INVOICE_STATUS.WAITING).OrderBy(x => x.InvoiceData.ID).ToList();
                // Kiểm tra sau khi lọc hóa đơn đã ký nếu = 0 => return 
                if (signInvoiceDatas.Count == 0)
                {
                    return new JsonResult()
                    {
                        Data = new { rs = false, msg = "Phát hành không thành công. Hóa đơn đã được ký." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
                //Thực hiện ký
                var signInvoiceDatasCount = signInvoiceDatas.Count;
                List<NextNumber> listNextNumbers = null;
                for (int i = 0; i < signInvoiceDatasCount; i++)
                {
                    var item = signInvoiceDatas[i];
                    // Kiểm tra hóa đơn đã được ký chưa. Nếu ký rồi thì bỏ qua không ký hóa đơn đó nữa
                    InvoiceBO invoice = item.InvoiceData;
                    long idInvoice = invoice.ID;
                    if (listNextNumbers == null)
                    {
                        listNextNumbers = numberBLL.GetNextNumberByInvoiceIdUSB(idInvoice, signInvoiceDatasCount);
                        if (listNextNumbers.Count == 0)
                        {
                            return new JsonResult()
                            {
                                Data = new { rs = false, msg = "Phát hành không thành công. Dải hóa đơn này đã hết số." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                MaxJsonLength = Int32.MaxValue
                            };
                        }

                        var listNextNumbersCount = listNextNumbers.Count;
                        if (listNextNumbersCount < signInvoiceDatasCount)
                        {
                            return new JsonResult()
                            {
                                Data = new { rs = false, msg = $"Phát hành không thành công. Số lượng số còn lại là {listNextNumbersCount}. Số hóa đơn bạn chọn là {signInvoiceDatasCount}." },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                MaxJsonLength = Int32.MaxValue
                            };
                        }

                        if (listNextNumbersCount == 1)
                        {
                            // Kiểm tra dải chờ
                            var preNumber = listNextNumbers[0].ID - 1;
                            var preNumberInvoice = invoiceBLL.GetInvoiceByNumber(new FormSearchInvoice() { NUMBER = preNumber, COMTAXCODE = invoice.COMTAXCODE, FORMCODE = invoice.FORMCODE, SYMBOLCODE = invoice.SYMBOLCODE });
                            if (preNumberInvoice != null)
                            {
                                if (DateTime.Compare(preNumberInvoice.SIGNEDTIME, DateTime.Now) > 0)
                                {
                                    msg = $"Bạn đang ký số {listNextNumbers[0].ID}. Số {preNumber} nằm trong dải chờ và có ngày ký {preNumberInvoice.SIGNEDTIME.ToString("dd/MM/yyyy")} lớn hơn ngày hiện tại. " +
                                        $"Vui lòng chú ý ngày ký số lớn ngày lớn số nhỏ ngày nhỏ. " + 
                                        $"<br><text class='text-danger'>Giải pháp:</text> Tạo dải chờ cho số {listNextNumbers[0].ID}.";
                                    return new JsonResult()
                                    {
                                        Data = new { rs = false, msg = msg },
                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                        MaxJsonLength = Int32.MaxValue
                                    };
                                }
                            }
                        }
                    }
                    //Template Xml
                    string invTypeName = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                    invoice.SIGNLINK = string.IsNullOrEmpty(invoice.SIGNLINK) ? string.Format("/{0}/{1}/{5}/{2}_{3}_{4}.pdf", invoice.COMTAXCODE, invoice.ID, invoice.COMTAXCODE, invoice.ID, invoice.INVOICETYPE, invTypeName) : invoice.SIGNLINK;

                    string dstFilePathXml = string.Empty;
                    string invoiceName = GetInvoiceNameXmlFile(invoice.USINGINVOICETYPE);
                    signLinkXml = invoiceBLL.CreateFileInvoiceXML(invoice, item.lstDetails, listNextNumbers[i].ID, invoiceName);
                    dstFilePathXml = Server.MapPath("~/" + inputFolder + signLinkXml);

                    if (signLinkXml.Length == 0)
                        return new JsonResult()
                        {
                            Data = new { rs = false, msg = "Phát hành không thành công. Không tạo được file xml." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };

                    //Thông tin xml cần ký
                    string base64Xml = Convert.ToBase64String(System.IO.File.ReadAllBytes(dstFilePathXml));
                    if (base64Xml == null)
                        return new JsonResult()
                        {
                            Data = new { rs = false, msg = "Phát hành không thành công. Lỗi chuyển đổi sang xml base64." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };

                    Data data = new Data()
                    {
                        Base64Xml = base64Xml
                    };
                    listData.Add(data);
                }
                //// Thiết lập trạng thái đang ký
                //msg = SetSigningStatus(objNumber.ID, true);
                //if (msg.Length > 0)
                //{
                //    return new JsonResult()
                //    {
                //        Data = new { rs = false, msg = "Phát hành không thành công. Thiết lập trạng thái đang ký lỗi." },
                //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //        MaxJsonLength = Int32.MaxValue
                //    };
                //}
                string listInvoiceIdString = string.Join(";", signInvoiceDatas.Select(x => x.InvoiceData.ID));
                string listNextNumberString = string.Join(";", listNextNumbers.Select(x => x.ID));
                return new JsonResult()
                {
                    Data = new { rs = true, msg = "Phát hành hóa đơn thành công.", data = listData, data2 = listInvoiceIdString, data3 = listNextNumberString },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception ex)
            {
                //msg = numberBLL.SetSigningStatus(objNumber.ID, false);
                ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "signXmlUSBApiMultiple");
                return new JsonResult()
                {
                    Data = new { rs = false, msg = "Phát hành hóa đơn không thành công. Vui lòng phát hành lại." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
        }

        public ActionResult SaveFileUSB(string listInvoiceId, List<string> base64Xml, string cerFileInfo, string listNextNumber)
        {
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            NumberBLL numberBLL = new NumberBLL();
            var listId = listInvoiceId.Split(';');
            var listNumber = listNextNumber.Split(';');
            var listIdCount = listId.Length;
            string msg = "";
            string cerFileName = $"{ConfigHelper.OutputInvoiceFolder}/{objUser.COMTAXCODE}/ChungChiChuKySo_{objUser.COMTAXCODE}.cer";
            List<InvoiceBO> listInvoicesSendMail = new List<InvoiceBO>();

            try
            {
                if (listNumber.Count() != listId.Count())
                    return Json(new { rs = false, msg = "Phát hành không thành công. Số lượng id hóa đơn và số hóa đơn không bằng nhau.", JsonRequestBehavior.AllowGet });

                var isSaveCerFileSuccess = SaveFile2Disk(Server.MapPath("~/" + cerFileName), cerFileInfo);
                if (!isSaveCerFileSuccess)
                    return Json(new { rs = false, msg = "Phát hành không thành công. Lưu file chứng chỉ không thành công.", JsonRequestBehavior.AllowGet });

                DateTime dtSigntime = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                for (int i = 0; i < listIdCount; i++)
                {
                    var invoice = invoiceBLL.GetInvoiceById(long.Parse(listId[i]));
                    msg = SaveInvoiceSigned(invoice.ID, long.Parse(listNumber[i]), base64Xml[i], invoice, dtSigntime);
                    if (string.IsNullOrEmpty(msg))
                        listInvoicesSendMail.Add(invoice);
                }

                // Cấu hình gửi email tự động sau khi phát hành thành công nếu trong hóa đơn có email của khách hàng
                if (objUser.AUTOSENDMAIL)
                    SendMultipleEmail(listInvoicesSendMail);

                return Json(new { rs = true, msg = "Phát hành hóa đơn thành công.", JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), "Phát hành không thành công USB", MethodBase.GetCurrentMethod().Name, "SaveFileUSB");
                return Json(new { rs = false, msg = "Phát hành không thành công. Vui lòng phát hành lại.", JsonRequestBehavior.AllowGet });
            }
            finally
            {
                // Cập nhật lại trạng thái cho pm_number về trạng thái không ký. pm_number.IsSigning = false
                //var objNumber = numberBLL.GetNumberRecord(long.Parse(listId[0]));
                //SetSigningStatus(objNumber.ID, false);
            }
        }

        #endregion

        #region Hàm chung
        /// <summary>
        /// Kiểm tra dữ liệu hóa đơn trước khi ký
        /// truongnv 20200227
        /// </summary>
        /// <param name="numberId"></param>
        /// <param name="tempNumberWaiting"></param>
        /// <param name="nextNumber"></param>
        /// <param name="tempWaitingNumberObj"></param>
        /// <returns></returns>
        private string ValidateBeforeSign(long numberId, long tempNumberWaiting, ref long nextNumber, ref InvoiceNumberBO tempWaitingNumberObj)
        {
            string msg = string.Empty;
            try
            {
                NumberBLL numberBLL = new NumberBLL();
                tempWaitingNumberObj = numberBLL.GetWaitingNumberByWaitingNumberId(numberId);
                if (tempWaitingNumberObj == null)
                {
                    msg = "Không tồn tại dải hóa đơn.";
                    return msg;
                }

                var countNumbers = (tempWaitingNumberObj.TONUMBER - tempWaitingNumberObj.FROMNUMBER) + 1;
                var lstUsedNumbers = tempWaitingNumberObj.USEDNUMBER;

                if (tempNumberWaiting <= 0)
                {
                    msg = "Số hóa đơn không được nhỏ hơn 0.";
                    return msg;
                }
                if (tempNumberWaiting < tempWaitingNumberObj.FROMNUMBER)
                {
                    msg = "Số hóa đơn không nhỏ hơn số bắt đầu.";
                    return msg;
                }
                if (tempNumberWaiting > tempWaitingNumberObj.TONUMBER)
                {
                    msg = "Số hóa đơn không lớn hơn số đến.";
                    return msg;
                }

                if (lstUsedNumbers != null)
                {
                    var usedNumbers = lstUsedNumbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (countNumbers == usedNumbers.Count())
                    {
                        msg = "Dải hóa đơn này đã hết số.";
                        return msg;
                    }

                    foreach (var item in usedNumbers)
                    {
                        if (item == tempNumberWaiting.ToString())
                        {
                            msg = $"Số <b> {item} </b> đã được sử dụng.";
                            return msg;
                        }
                    }
                }

                nextNumber = tempNumberWaiting;
            }
            catch (Exception ex)
            {
                msg = "Lỗi kiểm tra dải hóa dơn chờ. methodName (ValidateBeforeSign).";
            }
            return msg;
        }

        /// <summary>
        /// Check dải chờ hóa đơn trước khi phát hành
        /// truongnv 20200227
        /// </summary>
        /// <param name="invoiceId">ID  hóa đơn</param>
        /// <returns></returns>
        public string CheckNumberWaiting(long invoiceId, ref long nextNumber)
        {
            string msg = string.Empty;
            try
            {
                NumberBLL numberBLL = new NumberBLL();
                var tempNextNumber = numberBLL.GetNumberByInvoiceId2(invoiceId);
                if (tempNextNumber == 0)
                    msg = "Dải hóa đơn này đã hết số.";
                else
                    nextNumber = tempNextNumber;
            }
            catch (Exception ex)
            {
                msg = "Lỗi kiểm tra dải hóa đơn chờ.";
                throw ex;
            }
            return msg;
        }

        /// <summary>
        /// Check dải chờ hóa đơn trước khi phát hành
        /// truongnv 20200227
        /// </summary>
        /// <param name="invoiceId">ID  hóa đơn</param>
        /// <returns></returns>
        public string CheckNumberWaiting(string comtaxcode, string formcode, string symbolcode, out long nextNumber)
        {
            string msg = string.Empty;
            nextNumber = 0;
            try
            {
                NumberBLL numberBLL = new NumberBLL();
                var tempNextNumber = numberBLL.GetNextNumberByInvoiceId(comtaxcode, formcode, symbolcode);
                if (tempNextNumber == 0)
                    msg = "Dải hóa đơn này đã hết số.";
                else
                    nextNumber = tempNextNumber;
            }
            catch (Exception ex)
            {
                msg = "Lỗi kiểm tra dải hóa đơn chờ.";
                throw ex;
            }
            return msg;
        }

        /// <summary>
        /// Cập nhật thông tin hóa đơn
        /// truongnv 20200227
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="tempNextNumber"></param>
        /// <param name="base64xmlSigned"></param>
        /// <param name="invoice"></param>
        /// <param name="numberWaitingId">ID bảng chứa số hóa đơn chờ</param>
        /// <returns></returns>
        public string SaveInvoiceSigned(long invoiceId, long tempNextNumber, string base64xmlSigned, InvoiceBO invoice, DateTime dtSignTime, long numberWaitingId = 0)
        {
            string msg = string.Empty;
            string filePathXml = string.Empty;
            string fileName = invoice.COMTAXCODE + "_" + invoice.ID + "_" + invoice.INVOICETYPE + ".pdf";
            try
            {
                var outputFolder = ConfigHelper.OutputInvoiceFolder;

                string signLinkRef = invoice.SIGNLINK;
                if (string.IsNullOrWhiteSpace(invoice.SIGNLINK))
                    signLinkRef = CreateFileInvoicePath(invoice, fileName);

                invoice.SIGNLINK = signLinkRef;

                string pathXml = outputFolder + invoice.SIGNLINK.Substring(0, invoice.SIGNLINK.Length - 4) + ".xml";

                //Lưu file XML đã ký thành công
                var outputFileNameXml = Server.MapPath("~/" + pathXml);
                msg = CommonFunction.Base64StringToFile(base64xmlSigned, outputFileNameXml);
                if (msg.Length > 0)
                {
                    msg = "Lưu file XML không thành công.";
                    return msg;
                }

                //Sum total size file
                var sizeXml = Checksum.CreateMD5(outputFileNameXml);

                //Cập nhật hóa đơn
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                long nextNumber = invoiceBLL.UpdateDataInvoice(invoiceId, sizeXml, invoice.SIGNLINK, tempNextNumber, dtSignTime);
                if (nextNumber <= 0)
                {
                    msg = $"Số hóa đơn hiện tại đã hết số.{nextNumber}.tempNextNumber:{tempNextNumber}";
                    return msg;
                }

                //Cập nhật số hóa đơn hiện tại
                bool isSuccess = false;
                NumberBLL numberBLL = new NumberBLL();
                if (numberWaitingId == 0)
                    isSuccess = numberBLL.UpdateCurrentNumber(invoiceId, tempNextNumber);
                else 
                    isSuccess = numberBLL.UpdateCurrentWaittingNumber(numberWaitingId, tempNextNumber);

                if (isSuccess == false)
                {
                    msg = "Lỗi cập nhật giá trị số hóa đơn hiện tại.";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Phát hành hóa đơn không thành công.", ex, MethodBase.GetCurrentMethod().Name, "SaveInvoiceSigned");
                msg = "Lỗi khi cập nhật thông tin ký hóa đơn.";
            }
            return msg;
        }

        public string CreateFileInvoicePath(InvoiceBO invoice, string fileName)
        {
            try
            {
                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(invoice.COMTAXCODE) ? "COMTAXCODE/" : invoice.COMTAXCODE + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoice.ID.ToString()) ? "INVOICEID/" : invoice.ID.ToString() + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoice.INVOICETYPENAME.ToString()) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoice.INVOICETYPENAME.ToString())).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;

                fileName = filePath.Replace(HttpContext.Server.MapPath("~/" + root), "").Replace('\\', '/');

                return fileName;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Không tạo được đường dẫn file.", objEx, MethodBase.GetCurrentMethod().Name, "CreateFileInvoicePath");
                fileName = string.Empty;
                return fileName;
            }
        }

        public void UpdateSigningStatus(string listInvoiceId, List<string> base64Xml)
        {
            try
            {
                NumberBLL numberBLL = new NumberBLL();
                var listId = listInvoiceId.Split(';');
                var objNumber = numberBLL.GetNumberRecord(long.Parse(listId[0]));
                SetSigningStatus(objNumber.ID, false);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Update trạng thái ký không thành công. Trạng thái ký hóa đơn đang là TRUE", ex, MethodBase.GetCurrentMethod().Name, "UpdateSigningStatus");
            }
        }

        private string SetSigningStatus(long numberId, bool signingStatus)
        {
            NumberBLL numberBLL = new NumberBLL();
            var msg = numberBLL.SetSigningStatus(numberId, signingStatus);
            return msg;
        }

        private bool SaveFile2Disk(string fileName, string base64String)
        {
            try
            {
                byte[] dataBuffer = Convert.FromBase64String(base64String);
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Chưa lưu được file vào thư mục của bạn", ex, MethodBase.GetCurrentMethod().Name, "SaveFile2Disk");
                return false;
            }
        }
        #endregion

        #region Đăng ký free version
        public ActionResult AddEnterpriseInfoFreeVersion(CompanyBO enterprise)
        {
            try
            {
                CompanyBLL companyBLL = new CompanyBLL();
                AccountBLL accountBLL = new AccountBLL();
                enterprise.ISFREETRIAL = true;
                enterprise.COMLOGO = null;
                var result = companyBLL.AddEnterpriseInfoFreeVersion(enterprise, objUser);
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateEnterpriseInfo");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
                }
                if (result)
                {
                    objUser = accountBLL.GetInfoUser(objUser.USERNAME, enterprise.COMTAXCODE);
                    //objUser.ISFREETRIAL = true; // assign free trial for global objUser
                }
                return Json(new { rs = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi cập nhật thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "UpdateEnterpriseInfo");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Trang tổng quan
        public JsonResult GetInvoice(string dateRange)
        {
            try
            {
                string[] d = dateRange.Split(';');
                DateTime fromDate = DateTime.ParseExact(d[0], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(d[1], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);

                InvoiceBLL objInvoiceBLL = new InvoiceBLL();

                List<InvoiceBO> lst = objInvoiceBLL.GetInvoiceByDate(objUser.COMTAXCODE, fromDate, toDate);
                if (objInvoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objInvoiceBLL.ResultMessageBO.Message, objInvoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return Json(new { rs = false, msg = objInvoiceBLL.ResultMessageBO.Message });
                }

                lst = lst.FindAll(x => x.USINGINVOICETYPE == objUser.USINGINVOICETYPE);
                //Invoice money
                lst = lst.Where(x => x.INVOICESTATUS == 2).ToList();
                var TotalMoneyBeforVAT = lst.Sum(x => x.TOTALMONEY);
                var TotalMoneyAfterVAT = lst.Sum(x => x.TOTALPAYMENT);
                var TotalMoneyVAT = lst.Sum(x => x.TAXMONEY);

                var chartData = lst.OrderBy(x => x.SIGNEDTIME)
                    .GroupBy(x => x.SIGNEDTIME.Date)
                    .Select(x => new
                    {
                        Date = x.Key.ToString("yyyy-MM-dd"),
                        BeforVAT = x.Sum(xx => xx.TOTALMONEY),
                        AfterVAT = x.Sum(xx => xx.TOTALPAYMENT),
                        VAT = x.Sum(xx => xx.TAXMONEY),
                        CountUse = x.Where(xx => xx.INVOICETYPE == 1).Count(),
                        CountCancel = x.Where(xx => xx.INVOICETYPE == 3).Count(),
                        CountReplace = x.Where(xx => xx.INVOICETYPE == 6).Count()
                    }).ToList();


                return Json(new { rs = true, msg = "Thành công", TotalMoneyBeforVAT, TotalMoneyAfterVAT, TotalMoneyVAT, chartData });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được danh sách", objEx, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return Json(new { rs = false, msg = "Lỗi khi lấy thông tin sản phẩm." });
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Home()
        {
            return PartialView();
        }
        #endregion
    }
}