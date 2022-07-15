using DS.BusinessLogic.Account;
using DS.Common.Helpers;
using SAB.Library.Core.Crypt;
using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.Mvc;


namespace erp.onfinance.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            objUser = null;
            Session[ConfigHelper.User] = null;

            return RedirectToAction("Login", "Account");
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        public ActionResult ChangePassword(string password, string newpassword, string renewpassword)
        {
            //Kiểm tra mật khẩu cũ
            var PassMD5 = Cryptography.HashingSHA512(password);
            if (PassMD5 != objUser.PASSWORD)
            {
                return Json(new { rs = false, msg = "Mật khẩu cũ không đúng. Vui lòng kiểm tra lại." });
            }

            //Kiểm tra mật khẩu mới
            Regex regexForPassword = new Regex(".{8,30}");
            if (regexForPassword.IsMatch(newpassword) == false)
            {
                return Json(new { rs = false, msg = "Mật khẩu mới phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
            }
            if (newpassword != renewpassword)
            {
                return Json(new { rs = false, msg = "Lặp lại mật khẩu mới không đúng. Vui lòng xác nhận lại mật khẩu mới cho đúng." });
            }

            var newPass = SAB.Library.Core.Crypt.Cryptography.HashingSHA512(newpassword);
            objUser.PASSWORD = newPass;

            AccountBLL objAccountBLL = new AccountBLL();
            objAccountBLL.UpdateUserPassWord(objUser);

            if (objAccountBLL.ResultMessageBO.IsError)
            {
                ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Account");
                return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
            }

            return Json(new { rs = true, msg = "Đổi mật khẩu thành công!" });

        }

        public ActionResult UnauthorisedRequest(string username, string password)
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
                var User = objAccountBLL.GetInfoUser(username.ToLower(), "0106579683-999");
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UnauthorisedRequest");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }

                if (User == null)
                {
                    return Json(new { rs = false, msg = "Tài khoản không tồn tại!" });
                }
                else if (User.PASSWORD != PassMD5 && User.PASSWORDTEMP != PassMD5)
                {
                    return Json(new { rs = false, msg = "Sai mật khẩu!" });
                }
                else if (!User.ISADMIN)
                {
                    return Json(new { rs = false, msg = "Tài khoản không có quyền truy cập" });
                }

                objUser = User;
                Session[ConfigHelper.User] = User;
                return Json(new { rs = true, User }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi đăng nhập";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "Login");
                return Json(new { rs = false, msg = $"Lỗi {error}" }, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult UnauthorisedRequest(string username, string password)
        //{
        //    //string MAC_Address = this.GetMACAddress();
        //    return Json(new { rs = true });
        //}

        public ActionResult RefreshSession()
        {
            if (Session[ConfigHelper.User] == null)
            {
                return Json(new { rs = false, msg = "SessionNull!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { rs = true, msg = "Session in!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            return sMacAddress;
        }
    }
}