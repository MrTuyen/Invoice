using DS.BusinessLogic.Account;
using DS.BusinessLogic.Customer;
using DS.BusinessLogic.EmailSender;
using DS.BusinessObject.Account;
using DS.BusinessObject.EmailSender;
using DS.BusinessObject.Output;
using DS.BusinessObject.User;
using DS.Common.Enums;
using DS.Common.Helpers;
using SAB.Library.Core.Crypt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult PhoneNumber(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        //Trả về View đăng ký
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            objUser = null;
            Session[ConfigHelper.User] = null;

            return RedirectToAction("Login", "Account");
        }


        //Hàm đăng ký với dữ liệu gửi lên
        [HttpPost]
        public ActionResult Register(RegisterForm registerForm)
        {
            try
            {
                Regex regexForPhoneNumber = new Regex(@"^(09|03|07|05)+([0-9]{8})$");
                Regex regexForPassword = new Regex(".{8,30}");
                Regex regexForEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (registerForm.PHONENUMBER == null || regexForPhoneNumber.IsMatch(registerForm.PHONENUMBER) == false)
                {
                    return Json(new { rs = false, msg = "Số điện thoại bạn nhập không hợp lệ. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                }
                if (registerForm.EMAIL == null || regexForEmail.IsMatch(registerForm.EMAIL) == false)
                {
                    return Json(new { rs = false, msg = "Định dạng Email không hợp lệ. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                }
                if (registerForm.PASSWORD == null || regexForPassword.IsMatch(registerForm.PASSWORD) == false)
                {
                    return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                }
                registerForm.UTMSOURCE = "Link";
                var PassMD5 = Cryptography.HashingSHA512(registerForm.PASSWORD);
                registerForm.PASSWORD = PassMD5;
                if (objUser == null)
                {
                    registerForm.COMTAXCODE = "0106579683-999";
                }
                AccountBLL objAccountBLL = new AccountBLL();
                var result = objAccountBLL.CreateUser(registerForm);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Register");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (result)
                {
                    var User = objAccountBLL.GetInfoUserByEmail(registerForm.EMAIL);
                    if (objAccountBLL.ResultMessageBO.IsError)
                    {
                        ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Login");
                        return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                    }
                    objUser = User;
                    Session[ConfigHelper.User] = User;
                }
                if (!result)
                {
                    return Json(new { rs = false, msg = "Dữ liệu đã tồn tại trên hệ thống, vui lòng thử lại!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { rs = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đăng ký", ex, MethodBase.GetCurrentMethod().Name, "Register");
                return Json(new { rs = false, msg = "Không thể đăng ký dùng thử." }, JsonRequestBehavior.AllowGet);
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
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật thông tin User", ex, MethodBase.GetCurrentMethod().Name, "UpdateUser");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");

                if (regexForPassword.IsMatch(password) == false)
                {
                    return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                }
                var PassMD5 = Cryptography.HashingSHA512(password);
                AccountBLL objAccountBLL = new AccountBLL();
                var users = objAccountBLL.GetListInfoUser(username.ToLower());
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Login");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }

                //Nếu không lấy được thông tin người dùng
                if (users == null || users.Count == 0)
                    return Json(new { rs = false, msg = "Không tìm thấy người dùng trong hệ thống." });

                var user = users[0];
                if (user == null || (user.PASSWORD != PassMD5 && user.PASSWORDTEMP != PassMD5))
                {
                    return Json(new { rs = false, msg = "Thông tin đăng nhập không đúng, vui lòng thử lại!" });
                }


                objUser = objAccountBLL.GetInfoUser(user.USERNAME, user.COMTAXCODE);
                if (objUser == null)
                {
                    return Json(new { rs = false, msg = "Không lấy được thông tin người dùng." });
                }

                Session["ONFINANCEUSERS_LISTTEMP"] = null;
                Session["USINGINVOICETYPENAME"] = null;
                Session["USINGINVOICETYPEID"] = null;

                string invoiveTypeName = string.Empty;
                if (objUser.USINGINVOICETYPETMP != null)
                {
                    string[] usingInvoiceTypeTmp = objUser.USINGINVOICETYPETMP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    objUser.USINGINVOICETYPE = CommonFunction.NullSafeInteger(usingInvoiceTypeTmp[0], -1);

                    List<UsingInvoiceTypeBO> lstInvoiceTypeTmp = new List<UsingInvoiceTypeBO>();
                    if (usingInvoiceTypeTmp.Length > 1)
                    {
                        foreach (var item in usingInvoiceTypeTmp)
                        {
                            var id = CommonFunction.NullSafeInteger(item, 0);
                            UsingInvoiceTypeBO obj = new UsingInvoiceTypeBO();
                            if (id == (int)EnumHelper.AccountObjectType.HOADONGTGT)
                                invoiveTypeName = "Hóa đơn GTGT";
                            else if (id == (int)EnumHelper.AccountObjectType.HOADONBANHANG)
                                invoiveTypeName = "Hóa đơn bán hàng";
                            else if (id == (int)EnumHelper.AccountObjectType.HOADONTIENNUOC)
                                invoiveTypeName = "Hóa đơn tiền nước";
                            else if (id == (int)EnumHelper.AccountObjectType.HOADONTIENDIEN)
                                invoiveTypeName = "Hóa đơn tiền điện";
                            else if (id == (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC)
                                invoiveTypeName = "Hóa đơn trường học";
                            else if (id == (int)EnumHelper.AccountObjectType.PHIEUXUATKHO)
                                invoiveTypeName = "Phiếu xuất kho";

                            obj.ID = id;
                            obj.TypeName = invoiveTypeName;
                            lstInvoiceTypeTmp.Add(obj);
                        }
                        Session["ONFINANCEUSERS_LISTTEMP"] = lstInvoiceTypeTmp;
                    }
                }
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)EnumHelper.AccountObjectType.HOADONGTGT:
                        invoiveTypeName = "Hóa đơn GTGT";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONBANHANG:
                        invoiveTypeName = "Hóa đơn bán hàng";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTIENNUOC:
                        invoiveTypeName = "Hóa đơn tiền nước";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTIENDIEN:
                        invoiveTypeName = "Hóa đơn tiền điện";
                        break;
                    case (int)EnumHelper.AccountObjectType.HOADONTRUONGHOC:
                        invoiveTypeName = "Hóa đơn trường học";
                        break;
                    case (int)EnumHelper.AccountObjectType.PHIEUXUATKHO:
                        invoiveTypeName = "Phiếu xuất kho";
                        break;
                }
                Session["USINGINVOICETYPENAME"] = invoiveTypeName;
                Session["USINGINVOICETYPEID"] = objUser.USINGINVOICETYPE;
                objUser.USERNAME = objUser.USERNAME == null ? objUser.EMAIL : objUser.USERNAME;
                GetRoleDetailByCurrentUser();
                Session[ConfigHelper.User] = objUser;
                Session["ONFINANCEUSERS"] = users;
                return Json(new { rs = true, objUser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đăng nhập", ex, MethodBase.GetCurrentMethod().Name, "Login");
                return Json(new { rs = false, msg = $"Đăng nhập không thành công." }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetRoleDetailByCurrentUser()
        {
            try
            {
                //if (objUser.USERROLES == null)
                //{
                //    UserBLL userBLL = new UserBLL();
                //    List<UserRoleDetailBO> result = new List<UserRoleDetailBO>();
                //    if (objUser.ISADMIN)
                //    {
                //        List<RoleDetailBO> temp = userBLL.GetRoleDetail();
                //        if (userBLL.ResultMessageBO.IsError)
                //        {
                //            ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                //            return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
                //        }
                //        foreach (var item in temp)
                //        {
                //            result.Add(new UserRoleDetailBO()
                //            {
                //                COMTAXCODE = objUser.COMTAXCODE,
                //                USERNAME = objUser.EMAIL,
                //                ROLEDETAILID = item.ID
                //            });
                //        }
                //    }
                //    else
                //    {
                //        result = userBLL.GetRoleDetailByUserId(objUser.EMAIL, objUser.COMTAXCODE);
                //        if (userBLL.ResultMessageBO.IsError)
                //        {
                //            ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                //            return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
                //        }
                //    }
                //    objUser.USERROLES = result.Select(x => x.ROLEDETAILID).ToList();
                //    return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
                //}
                //return Json(new { rs = true, result = objUser.USERROLES }, JsonRequestBehavior.AllowGet);
                UserBLL userBLL = new UserBLL();
                List<UserRoleDetailBO> result = new List<UserRoleDetailBO>();
                if (objUser.ISADMIN)
                {
                    List<RoleDetailBO> temp = userBLL.GetRoleDetail();
                    if (userBLL.ResultMessageBO.IsError)
                    {
                        ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                        return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
                    }
                    foreach (var item in temp)
                    {
                        result.Add(new UserRoleDetailBO()
                        {
                            COMTAXCODE = objUser.COMTAXCODE,
                            USERNAME = objUser.EMAIL,
                            ROLEDETAILID = item.ID
                        });
                    }
                }
                else
                {
                    result = userBLL.GetRoleDetailByUserId(objUser.EMAIL, objUser.COMTAXCODE);
                    if (userBLL.ResultMessageBO.IsError)
                    {
                        ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                        return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
                    }
                }
                objUser.USERROLES = result.Select(x => x.ROLEDETAILID).ToList();
                return Json(new { rs = true, result = objUser.USERROLES }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy quyền user hiện tại.";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetRoleDetailByCurrentUser");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult GetUserDetail(string username)
        //{
        //    try
        //    {
        //        AccountBLL objAccountBLL = new AccountBLL();
        //        var User = objAccountBLL.GetInfoUser(username);
        //        if (objAccountBLL.ResultMessageBO.IsError)
        //        {
        //            ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUserDetail");
        //            return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
        //        }
        //        return Json(new { rs = true, User }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //Hàm đăng nhập nhanh với mạng xã hội

        [HttpPost]
        public ActionResult QuickLogin(RegisterForm registerForm)
        {
            try
            {
                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUserByEmail(registerForm.EMAIL);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "QuickLogin");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (User == null)
                {
                    if (objUser == null)
                    {
                        registerForm.COMTAXCODE = "0106579683-999";
                    }
                    registerForm.ISACTIVED = true;
                    var result = objAccountBLL.CreateUser(registerForm);
                    if (result)
                        //User = objAccountBLL.GetInfoUserByEmail(registerForm.EMAIL, objUser.COMTAXCODE);
                        User = objAccountBLL.GetInfoUserByEmail(registerForm.EMAIL);
                    else
                        return Json(new { rs = false, msg = "Lỗi đăng nhập bằng tài khoản Google. Vui lòng thử lại." });
                }
                objUser = User;
                Session[ConfigHelper.User] = User;
                //if (string.IsNullOrEmpty(User.PHONENUMBER))
                //    return View("/Account/PhoneNumber");
                return Json(new { rs = true, User });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đăng nhập bằng tài khoản Google", ex, MethodBase.GetCurrentMethod().Name, "QuickLogin");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Hàm quên mật khẩu với email hoặc số điện thoại
        [HttpPost]
        public ActionResult ForgotPassword(string username)
        {
            try
            {
                Regex regexForPhoneNumber = new Regex(@"^(09|03|07|05)+([0-9]{8})$");
                Regex regexForEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                Regex isPhoneNumber = new Regex(@"^([0-9])+([0-9])$");
                if (string.IsNullOrEmpty(username))
                {
                    username = "";
                    return Json(new { rs = false, msg = "Số điện thoại hoặc email đang để trống!" }, JsonRequestBehavior.AllowGet);
                }
                if (isPhoneNumber.IsMatch(username) == true)// nếu người nhập nhập số điện thoại thì kiểm tra định dạng cảu số điện thoại
                {
                    if (regexForPhoneNumber.IsMatch(username) == false)
                    {
                        return Json(new { rs = false, msg = "Số điện thoại không hợp lệ. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                    }
                }
                else //các trường hợp còn lại kiểm tra định dạng email
                {
                    if (regexForEmail.IsMatch(username) == false)
                    {
                        return Json(new { rs = false, msg = "Email không đúng định dạng. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                    }
                }

                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUserByEmail(username);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ForgotPasswoard");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (User != null && (User.EMAIL == username || User.PHONENUMBER == username))
                    ;
                else
                {
                    return Json(new { rs = false, msg = "Dữ liệu bạn nhập không tồn tại trên hệ thống, vui lòng thử lại!" });
                }
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                string randPassword = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
                var newPassword = Cryptography.HashingSHA512(randPassword);
                var result = objAccountBLL.ForgotPassword(username, newPassword);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ForgotPasswoard");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }

                EmailSender.MailSender(new EmailData
                {
                    Username = ConfigurationManager.AppSettings["UsernameEmail"],//"onfinance@novaon.asia",
                    Password = ConfigurationManager.AppSettings["PasswordEmail"],//"!@#123qwe",
                    Host = "smtp.gmail.com",
                    Port = 587,
                    MailTo = User.EMAIL,
                    Subject = "[Onfinance] Reset mật khẩu",
                    Content = $"<!DOCTYPE><html xmlns=" + "http://www.w3.org/1999/xhtml" + "><header><meta http-equiv=" + "Content - Type" + " content=" + "text / html; charset = UTF - 8" + " /><meta name=" + "viewport" + " content=" + "width = device - width,initial - scale = 1.0" + "/></header><body>" +
                              $"Xin chào <b>{User.FULLNAME}</b>!<br><br> Mật khẩu đăng nhập của bạn là: <b><h4>{randPassword}</h4></b><br>" +
                              $"Vui lòng đăng nhập tại đây: https://e.onfinance.asia/user-account/login và đội mật khẩu tại: <b><h4>Cài đặt -> Thông tin tài khoản</h4></b> <br>" +
                              $"Mật khẩu mới có hiệu lực trong 60 phút.</body></html>"
                });


                objUser = null;
                Session[ConfigHelper.User] = null;

                return Json(new { rs = true, data = username, msg = "Mật khẩu đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến hoặc trong tin nhắn rác." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi quên mật khẩu", ex, MethodBase.GetCurrentMethod().Name, "ForgotPassword");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReSendEmail(string username)
        {
            return View();
        }

        public ActionResult ChangePasswordAction(ChangePasswordForm changePasswordForm)
        {
            try
            {
                if (!MethodHelper.ValidatePassword(changePasswordForm.NEWPASSWORD))
                {
                    return Json(new { rs = false, msg = "Mật khẩu bao từ 8 ký tự trở lên bao gồm chữ in hoa, chữ thường và chữ số!" });
                }
                var PassMD5 = Cryptography.HashingSHA512(changePasswordForm.PASSWORD);
                if (PassMD5 == objUser.PASSWORD || PassMD5 == objUser.PASSWORDTEMP) ;
                else
                    return Json(new { rs = false, msg = "Mật khẩu cũ không đúng. Vui lòng kiểm tra lại." });
                if (changePasswordForm.NEWPASSWORD != changePasswordForm.RENEWPASSWORD)
                    return Json(new { rs = false, msg = "Mật khẩu mới không trùng khớp" });
                var newPass = Cryptography.HashingSHA512(changePasswordForm.NEWPASSWORD);
                AccountBLL objAccountBLL = new AccountBLL();
                var result = objAccountBLL.ChangePassword(objUser.USERNAME, newPass);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ChangePasswordAction");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (!result)
                {
                    return Json(new { rs = true, msg = "Đổi mật khẩu thất bại!" });
                }
                objUser.PASSWORD = changePasswordForm.NEWPASSWORD;
                return Json(new { rs = true, msg = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi đổi mật khẩu", ex, MethodBase.GetCurrentMethod().Name, "ChangePasswordAction");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PhoneNumber(string phonenumber, string email)
        {
            try
            {
                string comtaxCode = "0106579683-999";
                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUser(email, comtaxCode);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ForgotPasswoard");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                //if (User != null && (User.PHONENUMBER == phonenumber))
                //{
                //    return Json(new { rs = false, msg = "Số điện thoại này đã tồn tại trên hệ thông, vui lòng thử lại bằng số điện thoại khác!" });
                //}
                var result = objAccountBLL.UpdatePhoneNumber(phonenumber, email);
                if (result == true)
                {
                    Session[ConfigHelper.User] = objAccountBLL.GetInfoUser(phonenumber, comtaxCode);
                }
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "ForgotPasswoard");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, msg = "Mật khẩu đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến hoặc trong tin nhắn rác." });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thêm số điện thoại lần đầu đăng nhập bằng tài khoản Google", ex, MethodBase.GetCurrentMethod().Name, "PhoneNumber");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult Logout()
        //{
        //    if (Session[ConfigHelper.User] == null)
        //        return Json(new { rs = false, msg = "SessionNull!" }, JsonRequestBehavior.AllowGet);
        //    else
        //        return Json(new { rs = true, msg = "Session in!" }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult RefreshSession()
        {
            if (Session[ConfigHelper.User] == null)
                return Json(new { rs = false, msg = "SessionNull!" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { rs = true, msg = "Session in!" }, JsonRequestBehavior.AllowGet);
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            string sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
                if (sMacAddress == string.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            return sMacAddress;
        }
    }
}