using DS.BusinessObject.Account;
using DS.BusinessObject.Customer;
using DS.BusinessObject.User;
using SAB.Library.Data;
using System;
using System.Collections.Generic;

namespace DS.DataObject.Customer
{
    public class UserDAO : BaseDAO
    {
        #region Constructor
        public UserDAO() : base()
        {
        }
        public UserDAO(IData objIData)
            : base(objIData)
        {
        }
        #endregion
        #region Methods
        //Hiển thị danh sách role
        public List<RoleBO> GetRole()
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_role_get");
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<RoleBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<RoleDetailBO> GetDetailRole()
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_roledetail_get");
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<RoleDetailBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<UserRoleDetailBO> GetRoleDetailByUserId(string email, string comTaxCode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_roledetail_get_by_userid");
                objIData.AddParameter("p_email", email);
                objIData.AddParameter("p_comtaxcode", comTaxCode);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<UserRoleDetailBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string DeleteUserRole(string email, string comTaxCode)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"DELETE FROM ds_masterdata.pm_user_roledetail " +
                           $"WHERE ds_masterdata.pm_user_roledetail.username = '{email}' " +
                           $"AND ds_masterdata.pm_user_roledetail.comtaxcode = '{comTaxCode}'";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public string UpdateUserRole(UserRoleDetailBO userRoleDetail)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            string query = $"INSERT INTO ds_masterdata.pm_user_roledetail (username, roledetailid, comtaxcode) " +
                           $"VALUES ('{userRoleDetail.USERNAME}', {userRoleDetail.ROLEDETAILID}, '{userRoleDetail.COMTAXCODE}')";
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<UserBO> SearchUser(FormSearchUser form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_search_auto_complete");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_keyword", form.KEYWORD);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);

                var reader = objIData.ExecStoreToDataReader();
                var list = new List<UserBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        #endregion
    }
}
