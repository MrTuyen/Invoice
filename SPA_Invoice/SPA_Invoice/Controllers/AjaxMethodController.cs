using DS.BusinessLogic.Account;
using DS.BusinessObject.Account;
using DS.BusinessObject.Output;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Controllers
{
    public class AjaxMethodController : BaseController
    {
        // GET: AjaxMethod
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy thông tin khách hàng sử dụng chữ ký số HSM
        /// truongnv 20200226
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxMethod()
        {
            try
            {
                AccountBLL accountBLL = new AccountBLL();
                UserModel person = accountBLL.CheckUserNameUsedKysoHSM(objUser.USERNAME, objUser.COMTAXCODE);
                if (person != null && !string.IsNullOrWhiteSpace(person.APIURL))
                {
                    return Json(new { rs = true, msg = "OK" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch { }
            return Json(new { rs = false, msg = "Error" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gán lại thông tin doanh nghiệp khi người dùng chọn lại doanh nghiệp
        /// truongnv 20200302
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetSessionData(string comTaxCode)
        {
            try
            {
                if (Session["ONFINANCEUSERS"] == null)
                    return Json(new { rs = false, msg = "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại để tiếp tục làm việc." }, JsonRequestBehavior.AllowGet);

                List<AccountBO> objUsers = (List<AccountBO>)Session["ONFINANCEUSERS"];
                var user = objUsers.FirstOrDefault<AccountBO>(x => x.COMTAXCODE.Equals(comTaxCode, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    AccountBLL accountBLL = new AccountBLL();
                    AccountBO objuser = accountBLL.GetInfoUser(user.USERNAME, user.COMTAXCODE);
                    Session[ConfigHelper.User] = objuser;

                    string msg = accountBLL.UpdateModifiedDateByAccount(user.EMAIL, user.COMTAXCODE);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = msg }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { rs = false, msg = "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại để tiếp tục làm việc." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thay đổi đơn vị phát hành hóa đơn.", ex, "ResetSessionData", "ResetSessionData");
                return Json(new { rs = false, msg = "Lỗi thay đổi đơn vị phát hành hóa đơn." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { rs = true, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gán lại thông tin doanh nghiệp khi người dùng chọn lại doanh nghiệp
        /// truongnv 20200302
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveChangeUsingInvoiceType(int typeid,string typename)
        {
            try
            {
                if (Session[ConfigHelper.User] == null)
                    return Json(new { rs = false, msg = "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại để tiếp tục làm việc." }, JsonRequestBehavior.AllowGet);

                objUser.USINGINVOICETYPE = typeid;
                Session["USINGINVOICETYPEID"] = objUser.USINGINVOICETYPE;
                Session["USINGINVOICETYPENAME"] = typename;
            }
            catch(Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi thay đổi loại hóa đơn sử dụng.", ex, "SaveChangeUsingInvoiceType", "SaveChangeUsingInvoiceType");
                return Json(new { rs = false, msg = "Lỗi thay đổi loại hóa đơn sử dụng." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { rs = true, msg = "Thành công" }, JsonRequestBehavior.AllowGet);
        }
    }
}