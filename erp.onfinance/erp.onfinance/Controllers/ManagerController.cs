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
        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult StatisticInWeek()
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
                //USERNAME = "administrator",
                USERNAME = null,
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
                if (!string.IsNullOrEmpty(enterprise.USINGINVOICETYPETMP))
                {
                    enterprise.USINGINVOICETYPETMP = null;
                }
                foreach (string i in enterprise.ARRUSINGTYPETMP[0].Split(','))
                {
                    if (enterprise.ARRUSINGTYPETMP.Length == 1)
                    {
                        enterprise.USINGINVOICETYPE = Convert.ToInt32(i);
                    }
                    enterprise.USINGINVOICETYPETMP = i + "," + enterprise.USINGINVOICETYPETMP;
                }
                enterprise.USINGINVOICETYPETMP = enterprise.USINGINVOICETYPETMP.Remove(enterprise.USINGINVOICETYPETMP.Length - 1);
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
                {
                    //Xóa hóa đơn mẫu đối với những thông báo phát hành chưa duyệt
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
                ConfigHelper.Instance.WriteLog("Lỗi xoa chờ hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "DeleteReleaseNotice");
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
                account.ISADMIN = true;
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
                account.ISADMIN = true;
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

        public ActionResult DeleteUser(AccountBO account)
        {
            try
            {
                AccountBLL objAccountBLL = new AccountBLL();
                var result = objAccountBLL.DeleteUser(account);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "AddUser");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi delete user", ex, MethodBase.GetCurrentMethod().Name, "DeleteUser");
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
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateInvoiceStatus");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi UpdateInvoiceStatus";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "UpdateInvoiceStatus");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        //báo cáo tình hình sử dụng hóa đơn
        public ActionResult Getforstatistics(string keyword, int currentPage, string statuscus)
        {
            try
            {
                var itemPerPage = 10;
                var offset = (currentPage - 1) * itemPerPage;
                InvoiceSearchFormBO invoiceSearch = new InvoiceSearchFormBO
                {
                    KEYWORD = keyword,
                    COMPANYNAME = keyword,
                    ITEMPERPAGE = itemPerPage,
                    OFFSET = offset,
                    STATUS = string.IsNullOrEmpty(statuscus) ? (int?)null : Convert.ToInt32(statuscus)
                };

                InvoiceBLL invoiceBLL = new InvoiceBLL();
                List<InvoiceSummary> result = invoiceBLL.Getforstatistics(invoiceSearch);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                var lst = result.GroupBy(x => x.ID).Select(x => x.ToList()).ToList();
                result.Clear();
                foreach (var l in lst)
                {
                    var t = l[0];
                    t.ListSummaryDetail = l.Select(x => new InvoiceSummaryDetail()
                    {
                        FORMCODE = x.FORMCODE,
                        SYMBOLCODE = x.SYMBOLCODE,
                        FROMNUMBER = x.FROMNUMBER,
                        TONUMBER = x.TONUMBER,
                        TOTALNUMBER = x.TOTALNUMBER,
                        USEDNUMBER = x.USEDNUMBER,
                        NUMBERSTATUSNAME = x.NUMBERSTATUSNAME,
                        NUMBERSTATUSID = x.NUMBERSTATUSID
                    }).ToList();
                    result.Add(t);
                }

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;
                return Json(new { rs = true, result, TotalPages /*,TotalCusAll,TotalCusFee,TotalCusFree,TotalCusActive,TotalCusNoActive*/ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "Getforstatistics");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetStatisticTopTen(int currentPage, InvoiceSearchFormBO statistic)
        {
            try
            {
                var itemPerPage = 10;
                var offset = (currentPage - 1) * itemPerPage;

                statistic.COMPANYNAME = statistic.KEYWORD;
                statistic.ITEMPERPAGE = itemPerPage;
                statistic.OFFSET = offset;

                if (statistic.STATUS != 6)
                {
                    var time = DateTimeHelper.ThisWeek(DateTime.Now);
                    switch (statistic.STATUS)
                    {
                        case 1:
                            time = new DateTimeHelper.DateRange() { Start = new DateTime(2000, 1, 1), End =  DateTime.Now};
                            break;
                        case 2:
                            time = DateTimeHelper.ThisWeek(DateTime.Now);
                            break;
                        case 3:
                            time = DateTimeHelper.LastWeek(DateTime.Now);
                            break;
                        case 4:
                            time = DateTimeHelper.ThisMonth(DateTime.Now);
                            break;
                        case 5:
                            time = DateTimeHelper.LastMonth(DateTime.Now);
                            break;
                        default:
                            break;
                    }

                    statistic.FROMTIME = string.IsNullOrEmpty(time.Start.ToString("dd/MM/yyyy")) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(time.Start.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    statistic.TOTIME = string.IsNullOrEmpty(time.End.ToString("dd/MM/yyyy")) ? DateTime.Now : DateTime.ParseExact(time.End.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }

                InvoiceBLL invoiceBLL = new InvoiceBLL();
                List<InvoiceSummary> result = invoiceBLL.GetStatisticTopTen(statistic);
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetStatisticTopTen");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                var lst = result.GroupBy(x => x.ID).Select(x => x.ToList()).ToList();
                result.Clear();
                foreach (var l in lst)
                {
                    var t = l[0];
                    var listSummaryDetail = l.Select(x => new InvoiceSummaryDetail()
                    {
                        FORMCODE = x.FORMCODE,
                        SYMBOLCODE = x.SYMBOLCODE,
                        FROMNUMBER = x.FROMNUMBER,
                        TONUMBER = x.TONUMBER,
                        TOTALNUMBER = x.TOTALNUMBER,
                        USEDNUMBER = x.USEDNUMBER,
                        NUMBERSTATUSNAME = x.NUMBERSTATUSNAME,
                        NUMBERSTATUSID = x.NUMBERSTATUSID
                    }).ToList();
                    t.ListSummaryDetail = listSummaryDetail;
                    t.TOTALUSEDNUMBER = listSummaryDetail.Sum(x => x.USEDNUMBER);
                    result.Add(t);
                }

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % itemPerPage == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / itemPerPage;
                else
                    TotalPages = TotalRow / itemPerPage + 1;

                result = result.OrderByDescending(x => x.TOTALUSEDNUMBER).Take(10).ToList();
                return Json(new { rs = true, result, TotalPages /*,TotalCusAll,TotalCusFee,TotalCusFree,TotalCusActive,TotalCusNoActive*/ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "Getforstatistics");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSumany()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                CompanySummary result = invoiceBLL.GetSumany();
                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = invoiceBLL.ResultMessageBO.Message });
                }

                
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "Getforstatistics");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangStatusUser(string comataxcode, string email,bool actived)
        {
            try
            {
                CompanyBLL companyBLL = new CompanyBLL();
                    string msg = companyBLL.ChangStatus(comataxcode, email, actived);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = $"Lỗi cập nhật" });

                    return Json(new { rs = true, msg = $"Cập nhật thành công người dùng." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "ChangStatusUser");
            }

            return Json(new { rs = true, msg = "Cập nhật người dùng" });
        }
    }
}