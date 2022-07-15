using DS.BusinessLogic.Account;
using DS.Common.Helpers;
using SAB.Library.Core.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace TaxReport.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                var PassMD5 = Cryptography.HashingSHA512(password);
                AccountBLL objAccountBLL = new AccountBLL();
                var User = objAccountBLL.GetInfoUser(username, "0106579683-999");
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "Login");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                if (User != null && (User.PASSWORD == PassMD5 || User.PASSWORDTEMP == PassMD5))
                    ;
                else
                {
                    return Json(new { rs = false, msg = "Thông tin đăng nhập không đúng, vui lòng thử lại!" });
                }
                objUser = User;
                Session[ConfigHelper.User] = User;

                return Json(new { rs = true, User }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}