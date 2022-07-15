using DS.BusinessLogic.Customer;
using DS.BusinessLogic.Product;
using DS.BusinessObject.Account;
using DS.Common.Helpers;
using System.IO;
using System.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using ClosedXML.Excel;
using DS.BusinessLogic.Account;
using DS.BusinessObject.Company;
using DS.BusinessLogic.Company;
using SAB.Library.Core.Crypt;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Product;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using static DS.Common.Enums.EnumHelper;
using DS.BusinessLogic.EmailSender;
using DS.BusinessObject.EmailSender;
using ZaloDotNetSDK;
using ZaloCSharpSDK;

namespace SPA_Invoice.Controllers
{
    public class SettingsController : BaseController
    {
        public PartialViewResult Index()
        {
            return PartialView();
        }

        public PartialViewResult Announcement()
        {
            return PartialView();
        }
        public ActionResult Parameters()
        {
            return PartialView();
        }
        public PartialViewResult AccountSetting()
        {
            AccountBLL objAccountBLL = new AccountBLL();
            var User = objAccountBLL.GetInfoUser(objUser.EMAIL, objUser.COMTAXCODE);
            if (objAccountBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
            }
            objUser = User;
            Session[ConfigHelper.User] = User;
            ViewBag.User = objUser;
            return PartialView();
        }

        public PartialViewResult UserSettings()
        {
            AccountBLL objAccountBLL = new AccountBLL();
            var User = objAccountBLL.GetInfoUser(objUser.EMAIL, objUser.COMTAXCODE);
            if (objAccountBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
            }
            objUser = User;
            Session[ConfigHelper.User] = User;
            ViewBag.User = objUser;
            return PartialView();
        }

        public PartialViewResult AttachementFile()
        {
            return PartialView();
        }

        public PartialViewResult Tool()
        {
            return PartialView();
        }

        public ActionResult GetUserInfo()
        {
            try
            {
                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUser(objUser.EMAIL, objUser.COMTAXCODE);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                objUser = User;
                Session[ConfigHelper.User] = User;
                return Json(new { rs = true, objUser });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin người dùng", ex, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string SaveFile(string base64String, CompanyBO companyBo)
        {
            try
            {
                string root = ConfigurationManager.AppSettings["ImageFolder"]; // physical folder in local or virtual path on cloud server by getting the appconfig 
                string branchComTaxCode = "/" + (string.IsNullOrEmpty(companyBo.COMTAXCODE) ? "COMTAXCODE/" : companyBo.COMTAXCODE + "/");
                string path = string.Format($"{root}{branchComTaxCode}") + "/";
                string title = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(companyBo.COMNAME));

                // checking path is exist if not create the folder to download file 
                DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string fileName = dir + ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(companyBo.COMNAME)).Replace(" ", "-") + ".jpg";

                // save file to folder
                byte[] dataBuffer = Convert.FromBase64String(base64String);
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    if (dataBuffer.Length > 0)
                    {
                        fileStream.Write(dataBuffer, 0, dataBuffer.Length);
                    }
                }

                // return file name to save into database
                return fileName.Replace(Server.MapPath("~/" + root), "");
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu hình ảnh avatar", ex, MethodBase.GetCurrentMethod().Name, "SaveFile");
                return null;
            }
        }

        public ActionResult UpdateUser(AccountBO account)
        {
            try
            {
                AccountBLL objAccountBLL = new AccountBLL();

                var result = objAccountBLL.UpdateUser(account);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateUser");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (!result)
                {
                    return Json(new { rs = false, msg = "Lỗi cập nhật nhật dữ liệu cho user." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật thông tin người dùng", ex, MethodBase.GetCurrentMethod().Name, "UpdateUser");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }

        }

        //Cập nhật mật khẩu người dùng - Đổi mật khẩu
        public ActionResult UserUpdatePassword(AccountBO account)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");
                if (regexForPassword.IsMatch(string.IsNullOrEmpty(account.NEWPASSWORD) ? "" : account.NEWPASSWORD) == false)
                {
                    return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                }
                if (regexForPassword.IsMatch(string.IsNullOrEmpty(account.RENEWPASSWORD) ? "" : account.RENEWPASSWORD) == false)
                {
                    return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                }
                var PassMD5 = account.PASSWORDHIDDEN == null ? null : Cryptography.HashingSHA512(account.PASSWORDHIDDEN);
                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUser(objUser.EMAIL, objUser.COMTAXCODE);
                if (User != null && (User.PASSWORD == PassMD5 || User.PASSWORDTEMP == PassMD5))
                    ;
                else
                {
                    return Json(new { rs = false, msg = "Mật khẩu cũ không đúng!" });
                }
                if (account.NEWPASSWORD != account.RENEWPASSWORD)
                    return Json(new { rs = false, msg = "Mật khẩu mới và mật khẩu mới nhập lại không trùng nhau!" });

                account.COMTAXCODE = objUser.COMTAXCODE;
                account.PASSWORD = Cryptography.HashingSHA512(account.NEWPASSWORD);
                var result = objAccountBLL.UpdateUserPassWord(account);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UserUpdatePassword");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (!result)
                {
                    return Json(new { rs = false, msg = "Lỗi cập nhật nhật mật khẩu người dùng." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật mật khẩu người dùng", ex, MethodBase.GetCurrentMethod().Name, "UserUpdatePassword");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEnterpriseInfo()
        {
            try
            {
                CompanyBLL objCompanyBLL = new CompanyBLL();
                string comtaxcode = "";
                if (Session[ConfigHelper.User] != null)
                {
                    AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
                    comtaxcode = objUser.COMTAXCODE;
                }
                else
                {
                    return Json(new { rs = false, msg = "Lỗi lấy thông tin doanh nghiệp. Người dùng chưa đăng nhập." }, JsonRequestBehavior.AllowGet);
                }

                var Company = objCompanyBLL.GetInfoEnterpriseByTaxCode(objUser.COMTAXCODE);
                if (Company == null) //nếu tài khoản mới vào lần đầu, chưa lưu thông tin doanh nghiệp
                {
                    Company = new CompanyBO
                    {
                        COMTAXCODE = comtaxcode
                    };
                }
                if (Company.MAILSERVICEPASSWORD != null && Company.MAILSERVICEACCOUNT != null)
                    Company.MAILSERVICEPASSWORD = Cryptography.Decrypt(Company.MAILSERVICEPASSWORD, Company.MAILSERVICEACCOUNT);

                if (string.IsNullOrEmpty(Company.COMLOGO))
                {
                    Company.COMLOGO = "/Images/64px_image-add.png";
                }
                else
                {
                    Company.COMLOGO = "/NOVAON_FOLDER" + Company.COMLOGO;
                }

                if (objCompanyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetEnterpriseInfo");
                    return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
                }
                Company.USINGINVOICETYPE = objUser.USINGINVOICETYPE;
                AccountBO tempObjUser = objUser;
                tempObjUser.PASSWORD = null;
                tempObjUser.PASSWORDTEMP = null;
                return Json(new { rs = true, Company, User = tempObjUser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin doanh nghiệp", ex, MethodBase.GetCurrentMethod().Name, "GetEnterpriseInfo");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveInfoEnterprise(CompanyBO enterprise)
        {
            try
            {
                CompanyBLL objCompanyBLL = new CompanyBLL();

                if (Session[ConfigHelper.User] != null)
                {
                    AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
                    enterprise.COMTAXCODE = enterprise.COMTAXCODE == null || enterprise.COMTAXCODE == "" ? objUser.COMTAXCODE : enterprise.COMTAXCODE;
                }
                else
                {
                    return Json(new { rs = false, msg = "Lỗi cập nhật nhật thông tin doanh nghiệp. Người dùng chưa đăng nhập." }, JsonRequestBehavior.AllowGet);
                }

                if (enterprise.COMLOGO != null)
                {
                    string[] image = enterprise.COMLOGO.Split(',');
                    if (image.Count() > 1)
                    {
                        enterprise.COMLOGO = SaveFile(image[1], enterprise);
                    }
                    else
                    {
                        enterprise.COMLOGO = image[0].Replace("/" + ConfigurationManager.AppSettings["ImageFolder"], "");
                    }
                }

                var result = objCompanyBLL.UpdateEnterpriseInfo(enterprise, objUser);
                if (objCompanyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveInfoEnterprise");
                    return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
                }
                if (!result)
                {
                    return Json(new { rs = false, msg = "Lỗi cập nhật thông tin doanh nghiệp." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { rs = true, objUser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi cập nhật thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "SaveInfoEnterprise");
                return Json(new { rs = false, error }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult SaveInfoEnterprise(CompanyBO enterprise)
        //{
        //    try
        //    {
        //        CompanyBLL objCompanyBLL = new CompanyBLL();

        //        if (Session[ConfigHelper.User] != null)
        //        {
        //            AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
        //            enterprise.COMTAXCODE = enterprise.COMTAXCODE == null || enterprise.COMTAXCODE == "" ? objUser.COMTAXCODE : enterprise.COMTAXCODE;
        //        }
        //        else
        //        {
        //            return Json(new { rs = false, msg = "Lỗi cập nhật nhật thông tin doanh nghiệp. Người dùng chưa đăng nhập." }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (enterprise.COMLOGO != null)
        //        {
        //            string[] image = enterprise.COMLOGO.Split(',');
        //            if (image.Count() > 1)
        //            {
        //                enterprise.COMLOGO = SaveFile(image[1], enterprise);
        //            }
        //            else
        //            {
        //                enterprise.COMLOGO = image[0].Replace("/" + ConfigurationManager.AppSettings["ImageFolder"], "");
        //            }
        //        }

        //        var result = objCompanyBLL.UpdateEnterpriseInfo(enterprise, objUser);
        //        if (objCompanyBLL.ResultMessageBO.IsError)
        //        {
        //            ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveInfoEnterprise");
        //            return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
        //        }
        //        if (!result)
        //        {
        //            return Json(new { rs = false, msg = "Lỗi cập nhật thông tin doanh nghiệp." }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { rs = true, objUser }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = "Lỗi cập nhật thông tin doanh nghiệp";
        //        ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "SaveInfoEnterprise");
        //        return Json(new { rs = false, error }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        public ActionResult SaveEmailConfig(CompanyBO enterprise)
        {
            try
            {
                CompanyBLL objCompanyBLL = new CompanyBLL();
                string comtaxcode = "";
                if (Session[ConfigHelper.User] != null)
                {
                    AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
                    comtaxcode = objUser.COMTAXCODE;
                }
                else
                {
                    return Json(new { rs = false, msg = "Lỗi lấy thông tin doanh nghiệp. Người dùng chưa đăng nhập." }, JsonRequestBehavior.AllowGet);
                }

                var Company = objCompanyBLL.GetInfoEnterprise(objUser.EMAIL);
                if (Company == null) //nếu tài khoản mới vào lần đầu, chưa lưu thông tin doanh nghiệp
                {
                    return Json(new { rs = false, msg = "Lỗi cập nhật thông tin dịch vụ email. Tài khoản cần phải lưu mã số thuế trước." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (enterprise.MAILSERVICEID == (int)EMAIL_SERVICE_TYPE.GMAIL)
                    {
                        if (enterprise.MAILSERVICEPASSWORD == null || enterprise.MAILSERVICEACCOUNT == null)
                            return Json(new { rs = false, msg = "Vui lòng nhập đầy đủ thông tin." }, JsonRequestBehavior.AllowGet);
                    }
                    var result = objCompanyBLL.UpdateEnterpriseInfo(enterprise, objUser);
                    if (result == true)
                    {
                        Company = objCompanyBLL.GetInfoEnterprise(objUser.EMAIL);
                        // Cập nhật objUser hiện tại
                        objUser.AUTOSENDMAIL = enterprise.AUTOSENDMAIL;
                        objUser.MAILSERVICEID = enterprise.MAILSERVICEID;
                        objUser.MAILSERVICEACCOUNT = enterprise.MAILSERVICEACCOUNT;
                        objUser.MAILSERVICEPASSWORD = enterprise.MAILSERVICEPASSWORD;
                    }
                }
                if (objCompanyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveEmailConfig");
                    return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, Company }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cấu hình email gửi thư", ex, MethodBase.GetCurrentMethod().Name, "SaveEmailConfig");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckSetupEmailConfig(CompanyBO enterprise)
        {
            try
            {
                EmailData emailData = new EmailData()
                {
                    Username = enterprise.MAILSERVICEACCOUNT,
                    Password = enterprise.MAILSERVICEPASSWORD,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    MailTo = "onfinance@novaon.asia",
                    Subject = "Kiểm tra thiết lập email",
                    Content = "",
                };
                var isSentSuccess = EmailSender.CheckSendMail(emailData);
                if (isSentSuccess)
                    return Json(new { rs = true, msg = "Thiết lập thành công." }, JsonRequestBehavior.AllowGet);
                return Json(new { rs = false, msg = "Thiết lập không thành công. Vui lòng kiểm tra lại tài khoản hoặc mật khẩu." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = "Thiết lập không thành công. Vui lòng kiểm tra lại tài khoản hoặc mật khẩu." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportExcelIsCategory(bool hasProductData, bool hasCustomerData)
        {
            try
            {
                if (hasProductData == true && hasCustomerData == false)
                {
                    // đặt tên file xuất ra file theo tên 
                    string strFileName = "Danh_sach_san_pham" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                    DataTable dtReport = GetProductData();
                    return PushFile(dtReport, strFileName);
                }
                else if (hasProductData == false && hasCustomerData == true)
                {
                    // đặt tên file xuất ra file theo tên 
                    string strFileName = "Danh_sach_khach_hang" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                    DataTable dtReport = GetCustomerData();
                    return PushFile(dtReport, strFileName);
                }
                else if (hasProductData == true && hasCustomerData == true)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(GetProductData(), "Danh_sach_hanghoa_dichvu");
                        wb.Worksheets.Add(GetCustomerData(), "Danh_sach_khachhang");
                        //Export the Excel file.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=DS_khachang_sanpham.xlsx");
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách sản phẩm." });
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách sản phẩm.", objEx, MethodBase.GetCurrentMethod().Name, "ExportExcell");
                return Json(new { success = false, responseText = "Lỗi khi xuất excel danh sách sản phẩm." });
            }
        }

        public DataTable GetCustomerData()
        {
            try
            {
                CustomerBLL customerBLL = new CustomerBLL();
                FormSearchCustomer search = new FormSearchCustomer
                {
                    COMTAXCODE = objUser.COMTAXCODE
                };
                List<CustomerBO> result = customerBLL.GetCustomer(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new System.Data.DataTable();
                dtReport.Columns.Add("TÊN KHÁCH HÀNG");
                dtReport.Columns.Add("TÊN DOANH NGHIỆP");
                dtReport.Columns.Add("MÃ SỐ THUẾ");
                dtReport.Columns.Add("SỐ ĐIỆN THOẠI");
                dtReport.Columns.Add("EMAIL");
                dtReport.Columns.Add("WEBSITE");
                dtReport.Columns.Add("ĐỊA CHỈ");
                dtReport.Columns.Add("HÌNH THỨC THANH TOÁN");
                dtReport.Columns.Add("NGÂN HÀNG");
                dtReport.Columns.Add("SỐ TÀI KHOẢN");
                dtReport.Columns.Add("ĐƠN VỊ TIỀN TỆ");
                dtReport.Columns.Add("GHI CHÚ");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["TÊN KHÁCH HÀNG"] = row.CUSBUYER;
                    dr["TÊN DOANH NGHIỆP"] = row.CUSNAME;
                    dr["MÃ SỐ THUẾ"] = row.CUSTAXCODE;
                    dr["SỐ ĐIỆN THOẠI"] = row.CUSPHONENUMBER;
                    dr["EMAIL"] = row.CUSEMAIL;
                    dr["WEBSITE"] = row.CUSWEBSITE;
                    dr["ĐỊA CHỈ"] = row.CUSADDRESS;
                    dr["HÌNH THỨC THANH TOÁN"] = row.CUSPAYMENTMETHOD;
                    dr["NGÂN HÀNG"] = row.CUSBANKNAME;
                    dr["SỐ TÀI KHOẢN"] = row.CUSACCOUNTNUMBER;
                    dr["ĐƠN VỊ TIỀN TỆ"] = row.CUSCURRENCYUNIT;
                    dr["GHI CHÚ"] = row.CUSNOTE;
                    dtReport.Rows.Add(dr);
                }

                return dtReport;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách khách hàng.", objEx, MethodBase.GetCurrentMethod().Name, "GetCustomerData");
                return null;
            }
        }

        public DataTable GetProductData()
        {
            try
            {
                ProductBLL productBLL = new ProductBLL();
                FormSearchProduct search = new FormSearchProduct
                {
                    COMTAXCODE = objUser.COMTAXCODE
                };
                List<ProductBO> result = productBLL.GetProduct(search);

                // tạo nhãn dữ liệu
                DataTable dtReport = new DataTable();
                dtReport.Columns.Add("TÊN SẢN PHẨM");
                dtReport.Columns.Add("SKU");
                dtReport.Columns.Add("LOẠI");
                dtReport.Columns.Add("DANH MỤC");
                dtReport.Columns.Add("MÔ TẢ");
                dtReport.Columns.Add("ĐƠN VỊ TÍNH");
                dtReport.Columns.Add("ĐƠN GIÁ");

                //add dòng dữ liệu vào
                foreach (var row in result)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["TÊN SẢN PHẨM"] = row.PRODUCTNAME;
                    dr["SKU"] = row.SKU;
                    dr["LOẠI"] = Convert.ToInt32(row.PRODUCTTYPE) == 1 ? "Hàng hóa" : "Dịch vụ";
                    dr["DANH MỤC"] = row.CATEGORYNAME;
                    dr["MÔ TẢ"] = row.DESCRIPTION;
                    dr["ĐƠN VỊ TÍNH"] = row.QUANTITYUNIT;
                    dr["ĐƠN GIÁ"] = row.PRICE;
                    dtReport.Rows.Add(dr);
                }
                return dtReport;
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi khi xuất excel danh sách sản phẩm.", objEx, MethodBase.GetCurrentMethod().Name, "GetProductData");
                return null;
            }
        }
        public ActionResult SaveParameter(CompanyBO enterprise)
        {
            try
            {
                CompanyBLL companyBLL = new CompanyBLL();
                var result = companyBLL.SaveParameter(enterprise);
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveParameter");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
                }
                if (string.IsNullOrEmpty(result))
                {
                    // Cập nhật objUser hiện tại
                    objUser.QUANTITYPLACE = enterprise.QUANTITYPLACE;
                    objUser.PRICEPLACE = enterprise.PRICEPLACE;
                    objUser.MONEYPLACE = enterprise.MONEYPLACE;
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lưu doanh nghiệp", ex, MethodBase.GetCurrentMethod().Name, "SaveParameter");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveZaloOAAccessToken(CompanyBO enterprise)
        {
            CompanyBLL objCompanyBLL = new CompanyBLL();
            var result = objCompanyBLL.UpdateEnterpriseInfo(enterprise, objUser);
            if (result == true)
            {
                // Cập nhật objUser hiện tại
                objUser.ZALOACCESSTOKEN = enterprise.ZALOACCESSTOKEN.Trim();
            }
            if (objCompanyBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objCompanyBLL.ResultMessageBO.Message, objCompanyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "SaveEmailConfig");
                return Json(new { rs = false, msg = objCompanyBLL.ResultMessageBO.Message });
            }
            return Json(new { rs = true, msg = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEmailTemplate(CompanyBO enterprise)
        {

            try
            {
                string fileName = enterprise.EMAILTEMPLATEID == 1 ? "EmailTemplate.html" : "EmailCancelTemplate.html";
                string fullPathFileName = HttpContext.Server.MapPath($"~/NOVAON_FOLDER/{objUser.COMTAXCODE}/" + fileName);
                string content = enterprise.EMAILTEMPLATECONTENT;

                System.IO.File.WriteAllText(fullPathFileName, content);
                return Json(new { rs = true, msg = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog(ex.InnerException.ToString(), ex.InnerException.ToString(), MethodBase.GetCurrentMethod().Name, "SaveEmailTemplate");
                return Json(new { rs = false, msg = "Cập nhật không thành công." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadEmailTemplate(CompanyBO enterprise)
        {
            string fileName = enterprise.EMAILTEMPLATEID == 1 ? "EmailTemplate.html" : "EmailCancelTemplate.html";
            string fullPathFileName = HttpContext.Server.MapPath($"~/NOVAON_FOLDER/{objUser.COMTAXCODE}/" + fileName);
            string body = string.Empty;
            // Read email template file of own company
            try
            {
                using (StreamReader reader = new StreamReader(fullPathFileName))
                {
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {

            }

            if (string.IsNullOrEmpty(body))
            {
                fullPathFileName = HttpContext.Server.MapPath($"~/TemplateFiles/" + fileName);
                using (StreamReader reader = new StreamReader(fullPathFileName))
                {
                    body = reader.ReadToEnd();
                }
            }

            return Json(new { rs = true, msg = body }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestoreDefault(CompanyBO enterprise)
        {
            string fileName = enterprise.EMAILTEMPLATEID == 1 ? "EmailTemplate.html" : "EmailCancelTemplate.html";
            string fullPathFileName = HttpContext.Server.MapPath($"~/NOVAON_FOLDER/{objUser.COMTAXCODE}/" + fileName);
            string body = string.Empty;
            // Read email template file of own company
            using (StreamReader reader = new StreamReader(HttpContext.Server.MapPath($"~/TemplateFiles/" + fileName)))
            {
                body = reader.ReadToEnd();
            }

            System.IO.File.WriteAllText(fullPathFileName, body);
            return Json(new { rs = true, msg = body }, JsonRequestBehavior.AllowGet);
        }
    }
}