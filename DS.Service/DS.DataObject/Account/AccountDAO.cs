using DS.BusinessObject.Account;
using DS.BusinessObject.Output;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Account
{
    public class AccountDAO : BaseDAO
    {
        #region Constructor

        public AccountDAO() : base()
        {
        }

        public AccountDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods
        public bool CreateUser(RegisterForm registerForm)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_add");
                objIData.AddParameter("p_email", registerForm.EMAIL);
                objIData.AddParameter("p_phonenumber", registerForm.PHONENUMBER);
                objIData.AddParameter("p_password", registerForm.PASSWORD);
                objIData.AddParameter("p_utmcode", registerForm.UTMSOURCE);
                objIData.AddParameter("p_id", registerForm.ID);
                objIData.AddParameter("p_comtaxcode", registerForm.COMTAXCODE);
                objIData.AddParameter("p_fullname", registerForm.FULLNAME);
                objIData.AddParameter("p_isactive", registerForm.ISACTIVED);
                objIData.AddParameter("p_isadmin", registerForm.ISADMIN);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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
        public bool UpdateUser(AccountBO account)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_update");
                objIData.AddParameter("p_email", account.EMAIL);
                objIData.AddParameter("p_phonenumber", account.PHONENUMBER);
                objIData.AddParameter("p_name", account.FULLNAME);
                objIData.AddParameter("p_avatar", "");
                objIData.AddParameter("p_address", account.ADDRESS);
                objIData.AddParameter("p_comtaxcode", account.COMTAXCODE);
                objIData.AddParameter("p_username", account.USERNAME);
                objIData.AddParameter("p_password", account.PASSWORD);
                objIData.AddParameter("p_isactive", account.ISACTIVED);
                objIData.AddParameter("p_isadmin", account.ISADMIN);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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

        public bool DeleteUser(AccountBO account)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"DELETE FROM ds_masterdata.system_user" +
                               $" WHERE comtaxcode = '{account.COMTAXCODE}' AND email = '{account.EMAIL}'";
                BeginTransactionIfAny(objIData);
                objIData.ExecQueryToString(query);
                CommitTransactionIfAny(objIData);
                return true;
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

        public bool UpdateUserFreeVersion(AccountBO account)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_update_free_version");
                objIData.AddParameter("p_email", account.EMAIL);
                objIData.AddParameter("p_precomtaxcode", account.PRECOMTAXCODE);
                objIData.AddParameter("p_comtaxcode", account.COMTAXCODE);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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

        /// <summary>
        /// Kiểm tra xem trong bảng system_user đã tồn tại người dùng với tài khoản này chưa
        /// truongnv 20200304
        /// </summary>
        /// <param name="comtaxcode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckUserByTaxCodeUserName(string comtaxcode, string userName)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_check_duplicate_taxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                objIData.AddParameter("p_email", userName);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (!string.IsNullOrWhiteSpace(id))
                    msg = $"MST {comtaxcode} và {userName} đã tồn tại. Vui lòng kiểm tra lại.";
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
            return msg;
        }

        public bool UpdateUserPassWord(AccountBO account)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_update_password");
                objIData.AddParameter("p_email", account.EMAIL);
                objIData.AddParameter("p_comtaxcode", account.COMTAXCODE);
                objIData.AddParameter("p_password", account.PASSWORD);

                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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

        // lấy thông tin đăng nhập của khách hàng
        public AccountBO GetInfoUserByEmail(string email)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get_by_email");
                objIData.AddParameter("p_email", email);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<AccountBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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

        // lấy thông tin đăng nhập của khách hàng
        public AccountBO GetInfoUser(string username, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get");
                objIData.AddParameter("p_username", username);
                objIData.AddParameter("p_comtaxcode", comtaxcode); 
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<AccountBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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

        /// <summary>
        /// truongnv 202003002
        /// lấy ra danh sách user theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<AccountBO> GetListInfoUser(string username)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get_list_user_info");
                objIData.AddParameter("p_username", username);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<AccountBO>();
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

        /// <summary>
        /// Cập nhật ngày cập nhật cuối 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string UpdateModifiedDateByAccount(string userName, string comTaxCode)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_update_lastupdate");
                objIData.AddParameter("p_email", userName);
                objIData.AddParameter("p_comcode", comTaxCode); 
                msg = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
            }
            catch (Exception objEx)
            {
                msg = "Không cập nhật được thông tin người dùng.";
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
            return msg;
        }

        public AccountBO GetInfoUserByUserName(string username, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get_by_username");
                objIData.AddParameter("p_username", username);
                objIData.AddParameter("p_comtaxcode", comtaxcode); 
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<AccountBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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
        //Đổi mật khẩu
        public bool ChangePassword(string username, string password)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_changepass");
                objIData.AddParameter("p_username", username);
                objIData.AddParameter("p_password", password);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
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

        //Quên mật khẩu -> Tạo ra 1 mật khẩu tạm lưu vào pm_customer_password
        public bool ForgotPassword(string username, string password)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_password_add");
                objIData.AddParameter("p_email", username);
                objIData.AddParameter("p_password", password);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                DisconnectIData(objIData);
            }
        }


        public bool UpdatePhoneNumber(string phonenumber, string email)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_add_phone_number");
                objIData.AddParameter("p_phonenumber", phonenumber);
                objIData.AddParameter("p_email", email);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                DisconnectIData(objIData);
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo tài khoản để kiểm tra xem người dùng có sử dụng chữ ký số HSM không
        /// </summary>
        /// <returns></returns>
        public UserModel CheckUserNameUsedKysoHSM(string username, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_check_used_kyso_hsm_by_username");
                objIData.AddParameter("p_username", username);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                 var list = new List<UserModel>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
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
        // Lưu column excel khi import
        public string SaveColumnExcel(string columnExcel, string comtaxcode)
        {
            IData obj = this.CreateIData();
            try {
                BeginTransactionIfAny(obj);
                obj.ExecQueryToString($"UPDATE ds_masterdata.system_user SET mapcolumnexcel ='{columnExcel}' WHERE comtaxcode = '{comtaxcode}'");
                CommitTransactionIfAny(obj);
                return string.Empty;
            }
            catch(Exception ex)
            {
                RollBackTransactionIfAny(obj);
                throw ex;
            }finally
            {
                this.DisconnectIData(obj);
            }
        }
        #endregion
    }

}


