using DS.BusinessLogic.Account;
using DS.BusinessLogic.Company;
using DS.BusinessLogic.Customer;
using DS.BusinessObject.Account;
using DS.BusinessObject.User;
using DS.Common.Helpers;
using SAB.Library.Core.Crypt;
using SPA_Invoice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static DS.Common.Enums.EnumHelper;

namespace SPA_Invoice.Controllers
{
    public class UserController : BaseController
    {
        public PartialViewResult Index()
        {
            return PartialView();
        }

        public ActionResult GetAllUserByComtaxCode(string keyword, int page = 1)
        {
            try
            {
                int limit = 10;
                int offset = (page - 1) * limit;
                CompanyBLL companyBLL = new CompanyBLL();
                List<AccountBO> result = companyBLL.GetAllUserByComtaxCode(objUser.COMTAXCODE, keyword, limit, offset);
                long totalPages = 0;
                var totalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (totalRow % limit == 0)
                    totalRow = totalRow == 0 ? 1 : totalRow / limit;
                else
                    totalPages = (totalRow / 10) + 1;
                if (companyBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(companyBLL.ResultMessageBO.Message, companyBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                    return Json(new { rs = false, msg = companyBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, totalPages, totalRow, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetCompany");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRole()
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                List<RoleBO> result = userBLL.GetRole();
                if (userBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRole");
                    return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
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

        public ActionResult GetRoleDetail()
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                List<RoleDetailBO> result = userBLL.GetRoleDetail();
                if (userBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                    return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
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

        public ActionResult GetRoleDetailByUserId(string email)
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                List<UserRoleDetailBO> result = userBLL.GetRoleDetailByUserId(email, objUser.COMTAXCODE);
                if (userBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetRoleDetail");
                    return Json(new { rs = false, msg = userBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi lấy thông tin doanh nghiệp";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "GetRoleDetailByUserId");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(Role = (int)UserRole.PHAN_QUYEN_NGUOI_DUNG)]
        public ActionResult UpdateUserRole(string email, List<UserRoleDetailBO> userRoleDetail)
        {
            try
            {
                UserBLL userBLL = new UserBLL();
                AccountBLL accountBLL = new AccountBLL();
                if (string.IsNullOrEmpty(email.Trim()))
                    return Json(new { rs = false, msg = "Không tồn tại người dùng để phân quyền. Vui lòng kiểm tra lại." });
                string msg = accountBLL.CheckUserByTaxCodeUserName(objUser.COMTAXCODE, email.Trim());
                if (msg.Length <= 0)
                    return Json(new { rs = false, msg = "Không tồn tại người dùng để phân quyền. Vui lòng kiểm tra lại." });
                // Delete all user's roles
                msg = userBLL.DeleteUserRole(email.Trim(), objUser.COMTAXCODE);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = "Lỗi xóa quyền người dùng" });
                // Update all user's roles
                if (userRoleDetail != null)
                {
                    foreach (var item in userRoleDetail)
                    {
                        item.COMTAXCODE = objUser.COMTAXCODE;
                        item.USERNAME.Trim();
                        userBLL.UpdateUserRole(item);
                    }
                }
                if (userBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(userBLL.ResultMessageBO.Message, userBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateUserRole");
                    return Json(new { rs = false, msg = "Lỗi cập nhật phân quyền người dùng \n" + userBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, msg = "Cập nhật phân quyền thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = "Lỗi cập nhật phân quyền người dùng";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "UpdateUserRole");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(Role = (int)UserRole.CAP_NHAT_THONG_TIN_NGUOI_DUNG)]
        public ActionResult AddUser(RegisterForm registerForm)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");
                Regex regexForEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (registerForm.EMAIL == null || regexForEmail.IsMatch(registerForm.EMAIL) == false)
                {
                    return Json(new { rs = false, msg = "Định dạng Email không hợp lệ. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                }
                if (registerForm.PASSWORD == null || regexForPassword.IsMatch(registerForm.PASSWORD) == false)
                {
                    return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                }
                registerForm.UTMSOURCE = "System";
                var PassMD5 = Cryptography.HashingSHA512(registerForm.PASSWORD);
                registerForm.PASSWORD = PassMD5;
                registerForm.COMTAXCODE = objUser == null ? "0106579683-999" : objUser.COMTAXCODE;
                registerForm.ISADMIN = false;
                AccountBLL accountBLL = new AccountBLL();
                string msg = accountBLL.CheckUserByTaxCodeUserName(registerForm.COMTAXCODE, registerForm.EMAIL);
                if (msg.Length > 0)
                    return Json(new { rs = false, msg = msg });
                var result = accountBLL.CreateUser(registerForm);
                if (result)
                    return Json(new { rs = true, msg = "Thêm mới người dùng thành công." });
                return Json(new { rs = false, msg = "Lỗi thêm mới người dùng. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                string error = "Lỗi thêm mới người dùng";
                ConfigHelper.Instance.WriteLog(error, ex, MethodBase.GetCurrentMethod().Name, "AddUser");
                return Json(new { rs = false, msg = error }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(Role = (int)UserRole.CAP_NHAT_THONG_TIN_NGUOI_DUNG)]
        public ActionResult UpdateUser(AccountBO registerForm)
        {
            try
            {
                Regex regexForPassword = new Regex(".{8,30}");
                Regex regexForEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (registerForm.EMAIL == null || regexForEmail.IsMatch(registerForm.EMAIL) == false)
                    return Json(new { rs = false, msg = "Định dạng Email không hợp lệ. Vui lòng kiểm tra lại. " }, JsonRequestBehavior.AllowGet);
                if (!string.IsNullOrEmpty(registerForm.PASSWORD))
                {
                    if (registerForm.PASSWORD == null || regexForPassword.IsMatch(registerForm.PASSWORD) == false)
                        return Json(new { rs = false, msg = "Mật khẩu phải có ít nhất 8 ký tự." }, JsonRequestBehavior.AllowGet);
                    registerForm.PASSWORD = Cryptography.HashingSHA512(registerForm.PASSWORD);
                }
                AccountBLL objAccountBLL = new AccountBLL();
                var result = objAccountBLL.UpdateUser(registerForm);
                if (objAccountBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(objAccountBLL.ResultMessageBO.Message, objAccountBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "UpdateUser");
                    return Json(new { rs = false, msg = objAccountBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, result, msg = "Cập nhật thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi cập nhật người dùng", ex, MethodBase.GetCurrentMethod().Name, "UpdateUser");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(Role = (int)UserRole.CAP_NHAT_THONG_TIN_NGUOI_DUNG)]
        public ActionResult DeleteUser(AccountBO account)
        {
            try
            {
                AccountBLL objAccountBLL = new AccountBLL();
                var result = objAccountBLL.DeleteUser(account);
                if (result)
                {
                    UserBLL userBLL = new UserBLL();
                    var msg = userBLL.DeleteUserRole(account.EMAIL.Trim(), account.COMTAXCODE);
                    if (msg.Length > 0)
                        return Json(new { rs = false, msg = "Lỗi xóa quyền người dùng" });
                }
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

        /// <summary>
        /// Tìm kiếm user
        /// </summary>
        /// <param name="strKeyword"></param>
        /// <param name="intPageSize"></param>
        /// <returns></returns>
        public ActionResult SuggestUserByObject(string strKeyword, int intPageSize)
        {
            try
            {
                UserBLL uBLL = new UserBLL();
                FormSearchUser form = new FormSearchUser();
                form.CURRENTPAGE = 1;
                form.ITEMPERPAGE = 10;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.KEYWORD = strKeyword;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                List<UserBO> listUser = uBLL.SearchUser(form);

                return Json(new { rs = true, listResult = listUser });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không thể lấy thông tin user.", ex, "SuggestUserByObject", "Common");
                return Json(new { rs = false, msg = "Không tìm thấy thông tin người dùng." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}