using DS.BusinessObject.Account;
using DS.BusinessObject.Output;
using DS.Common.Helpers;
using DS.DataObject.Account;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Account
{
    public class AccountBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public AccountBLL()
        {
        }

        public AccountBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Thêm người dùng đăng nhập từ tài khoản Google
        /// </summary>
        /// <returns></returns>
        public bool CreateUser(RegisterForm registerForm)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.CreateUser(registerForm);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm người dùng đăng nhập thông qua Google Sign In");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        public bool UpdateUser(AccountBO account)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.UpdateUser(account);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật người dùng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool DeleteUser(AccountBO account)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.DeleteUser(account);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật người dùng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem trong bảng system_user đã tồn tại người dùng với tài khoản này chưa
        /// truongnv 20200304
        /// </summary>
        /// <param name="comtaxcode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckUserByTaxCodeUserName(string comtaxcode, string userName)
        {
            string msg = string.Empty;
            try
            {
                AccountDAO accountDL = new AccountDAO();
                return accountDL.CheckUserByTaxCodeUserName(comtaxcode, userName);
            }
            catch (Exception ex)
            {
                msg = $"Lỗi khi thực hiện kiểm tra doanh nghiệp.";
            }
            return msg;
        }

        public bool UpdateUserPassWord(AccountBO account)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.UpdateUserPassWord(account);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật người dùng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <returns></returns>
        public AccountBO GetInfoUserByEmail(string email)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.GetInfoUserByEmail(email);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <returns></returns>
        public AccountBO GetInfoUser(string username, string comtaxcode)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.GetInfoUser(username, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// truongnv 20200302
        /// </summary>
        /// <returns></returns>
        public List<AccountBO> GetListInfoUser(string username)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.GetListInfoUser(username);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Cập nhật ngày cập nhật cuối 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string UpdateModifiedDateByAccount(string userName, string comTaxCode)
        {
            string msg = string.Empty;
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.UpdateModifiedDateByAccount(userName, comTaxCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thời gian đăng nhập cuối của người dùng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return msg;
        }

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <returns></returns>
        public AccountBO GetInfoUserByUserName(string username, string comtaxcode)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.GetInfoUserByUserName(username, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        //public AccountBO GetInfoUserByComtaxCode(string comtaxcode)
        //{
        //    try
        //    {
        //        AccountDAO objAccountDAO = new AccountDAO();
        //        return objAccountDAO.GetInfoUserByComtaxcode(comtaxcode);
        //    }
        //    catch (Exception objEx)
        //    {
        //        this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng bằng mã thuế công ty.");
        //        objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
        //        return null;
        //    }
        //}

        /// <summary>
        /// đổi mật khẩu người dùng
        /// </summary>
        /// <returns></returns>
        public bool ChangePassword(string username, string password)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.ChangePassword(username, password);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi đổi mật khẩu người dùng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Lưu mật khẩu mới
        /// </summary>
        /// <returns></returns>
        public bool ForgotPassword(string username, string password)
        {
            try
            {

                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.ForgotPassword(username, password);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi quên mật khẩu.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdatePhoneNumber(string phonenumber, string email)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.UpdatePhoneNumber(phonenumber, email);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số điện thoại.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }


        /// <summary>
        /// Lấy thông tin người dùng theo tài khoản để kiểm tra xem người dùng có sử dụng chữ ký số HSM không
        /// </summary>
        /// <returns></returns>
        public UserModel CheckUserNameUsedKysoHSM(string username, string comtaxcode)
        {
            try
            {
                AccountDAO objAccountDAO = new AccountDAO();
                return objAccountDAO.CheckUserNameUsedKysoHSM(username, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin người dùng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        public string SaveColumnExcel(string columnExcel,string comtaxcode)
        {
            try
            {
                AccountDAO account = new AccountDAO();
                return account.SaveColumnExcel(columnExcel, comtaxcode);
            }catch(Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi lưu Column");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        #endregion
    }
}
