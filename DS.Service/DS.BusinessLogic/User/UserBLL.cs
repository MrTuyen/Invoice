using DS.BusinessObject.Account;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.User;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Customer;
using DS.DataObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Customer
{
    public class UserBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public UserBLL()
        {
        }

        public UserBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        public List<RoleBO> GetRole()
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetRole();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách role");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<RoleBO>();
            }
        }

        public List<RoleDetailBO> GetRoleDetail()
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetDetailRole();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách role detail");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<RoleDetailBO>();
            }
        }

        public List<UserRoleDetailBO> GetRoleDetailByUserId(string email, string comTaxCode)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.GetRoleDetailByUserId(email, comTaxCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "GetRoleDetailByUserId. Email: " + email);
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<UserRoleDetailBO>();
            }
        }

        public string DeleteUserRole(string email, string comTaxCode)
        {
            string msg = string.Empty;
            try
            {
                UserDAO userDAO = new UserDAO();
                msg = userDAO.DeleteUserRole(email, comTaxCode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "DeleteUserRole. Email: " + email);
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        public string UpdateUserRole(UserRoleDetailBO userRoleDetail)
        {
            string msg = string.Empty;
            try
            {
                UserDAO userDAO = new UserDAO();
                msg = userDAO.UpdateUserRole(userRoleDetail);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "UpdateUserRole. UserRoleDetail.USERNAME: " + userRoleDetail.USERNAME + ", UserRoleDetail.ROLEDETAILID: " + userRoleDetail.ROLEDETAILID);
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        public List<UserBO> SearchUser(FormSearchUser form)
        {
            try
            {
                UserDAO userDAO = new UserDAO();
                return userDAO.SearchUser(form);
            }
            catch (Exception ex)
            {
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return new List<UserBO>();
        }
        #endregion
    }
}
