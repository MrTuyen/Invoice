using DS.BusinessLogic.Account;
using DS.BusinessLogic.Company;
using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Invoice;
using DS.BusinessLogic.Number;
using DS.BusinessObject;
using DS.BusinessObject.Account;
using DS.BusinessObject.Company;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using SAB.Library.Core.Crypt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace erp.onfinance.Controllers
{
    public class ManagerController : BaseController
    {
        // GET: ManagerUser
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Company()
        {
            return View();
        }

        public ActionResult User()
        {
            return View();
        }

        public ActionResult GetCompany(string keyword, int currentPage)
        {
            var itemPerPage = 10;
            var offset = (currentPage - 1) * itemPerPage;
            CompanySearchFormBO companySearch = new CompanySearchFormBO
            {
                COMTAXCODE = null,
                USERNAME = "administrator",
                KEYWORD = keyword,
                ITEMPERPAGE = itemPerPage,
                OFFSET = offset
            };
            try
            {
                CompanyBLL companyBLL = new CompanyBLL();
                List<CompanyBO> result = companyBLL.GetAllCompany(companySearch);
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
                }
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;
                return Json(new { rs = true, result, TotalPages }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetCompany");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetInvoice(string keyword, int currentPage)
        {
            var itemPerPage = 10;
            var offset = (currentPage - 1) * itemPerPage;
            InvoiceSearchFormBO invoiceSearch = new InvoiceSearchFormBO
            {
                COMTAXCODE = null,
                COMPANYNAME = "",
                KEYWORD = keyword,
                ITEMPERPAGE = itemPerPage,
                OFFSET = offset
            };
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                List<InvoiceBO> result = invoiceBLL.GetAllInvoice(invoiceSearch);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;
                return Json(new { rs = true, result, TotalPages }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetCompany");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateEnterpriseInfo(CompanyBO enterprise)
        {
            try
            {
                enterprise.USERNAME = "administrator";
                CompanyBLL companyBLL = new CompanyBLL();
                var result = companyBLL.UpdateEnterpriseInfoAdminPage(enterprise);
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateEnterpriseInfo");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
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

        public ActionResult GetAllUserByComtaxCode(string comtaxcode)
        {
            try
            {
                int limit = 10;
                int offset = 0;
                CompanyBLL companyBLL = new CompanyBLL();
                List<AccountBO> result = companyBLL.GetAllUser(comtaxcode, limit, offset);
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetCompany");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllReleaseNotice(FormSearchNumber form)
        {
            try
            {
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

        public ActionResult AddReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                try
                {
                    invoiceNumber.FROMTIME = DateTime.ParseExact(invoiceNumber.STRFROMTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    return Json(new { rs = false, msg = $"Vui lòng kiểm tra lại định dạng thời gian: dd/MM/yyyy (ngày/tháng/năm)" }, JsonRequestBehavior.AllowGet);
                }
                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.AddReleaseNotice(invoiceNumber);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReleaseNotice");
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

        public ActionResult SaveReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                try
                {
                    invoiceNumber.FROMTIME = DateTime.ParseExact(invoiceNumber.STRFROMTIME, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    return Json(new { rs = false, msg = $"Vui lòng kiểm tra lại định dạng thời gian: dd/MM/yyyy (ngày/tháng/năm)" }, JsonRequestBehavior.AllowGet);
                }
                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.SaveReleaseNotice(invoiceNumber);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReleaseNotice");
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

        public ActionResult DeleteReleaseNotice(InvoiceNumberBO releaseNotice)
        {
            try
            {
                //Nếu thông báo phát hành không phải ở trạng thái nháp thì trả về cảnh báo người dùng
                if (releaseNotice.STATUS != 4)
                    return Json(new { rs = false, msg = "Thất bại, bạn chỉ được thực hiện thao tác xóa đối với các thông báo phát hành đang ở trạng thái nháp" }, JsonRequestBehavior.AllowGet);


                NumberBLL invoiceBLL = new NumberBLL();
                var result = invoiceBLL.DeleteReleaseNotice(releaseNotice.ID);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddReleaseNotice");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                if (result)
                    //Xóa hóa đơn mẫu đối với những thông báo phát hành chưa duyệt
                    if (releaseNotice.FROMNUMBER < 1)
                    {
                        string path = $"{ConfigurationManager.AppSettings["CompanyPath"]}{releaseNotice.TEMPLATEPATH}";

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm dải chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetNumber");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddUser(AccountBO account)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");
                if (!string.IsNullOrEmpty(account.PASSWORD))
                {
                    if (regexForPassword.IsMatch(account.PASSWORD) == false)
                    {
                        return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                    }
                    account.PASSWORD = Cryptography.HashingSHA512(account.PASSWORD);
                }
                AccountBLL objAccountBLL = new AccountBLL();

                //Kiểm tra xem tên đăng nhập và mã số thuế đã tồn tại chưa
                string msg = objAccountBLL.CheckUserByTaxCodeUserName(account.COMTAXCODE, account.EMAIL);
                if (msg.Length > 0)
                {
                    return Json(new { rs = false, msg = msg });
                }
                account.USERNAME = account.EMAIL;
                var result = objAccountBLL.UpdateUser(account);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddUser");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật user", ex, MethodBase.GetCurrentMethod().Name, "AddUser");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// truongnv 20200304
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ActionResult UpdateUser(AccountBO account)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");
                if (!string.IsNullOrEmpty(account.PASSWORD))
                {
                    if (regexForPassword.IsMatch(account.PASSWORD) == false)
                    {
                        return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                    }
                    account.PASSWORD = Cryptography.HashingSHA512(account.PASSWORD);
                }
                AccountBLL objAccountBLL = new AccountBLL();
                account.USERNAME = account.COMTAXCODE;
                var result = objAccountBLL.UpdateUser(account);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddUser");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật user", ex, MethodBase.GetCurrentMethod().Name, "AddUser");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateInvoiceStatus(InvoiceBO invoice)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                var result = invoiceBLL.UpdateInvoiceStatus(invoice);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateEnterpriseInfo");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
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
    }
}