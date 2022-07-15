using DS.BusinessLogic.Account;
using DS.BusinessObject.Account;
using DS.BusinessObject.Company;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Company;
using SAB.Library.Core.Crypt;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Company
{

    public class CompanyBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public CompanyBLL()
        {
        }

        public CompanyBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region methods
        public CompanyBO GetInfoEnterprise(string comtaxcode)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetInfoEnterprise(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin doanh nghiệp theo email.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public CompanyBO GetTaxCode(string comtaxcode)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetTaxCode(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin doanh nghiệp theo mã số thuế.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<CompanyBO> GetAllCompany(CompanySearchFormBO companySearch)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetAllCompany(companySearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách doanh nghiệp");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<AccountBO> GetAllUser(string comtaxcode, int? limit, int? offset)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetAllUserByComtaxCode(comtaxcode, "", limit, offset);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách user theo mã số thuế.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<AccountBO> GetAllUserByComtaxCode(string comtaxcode, string keyword, int? limit, int? offset)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetAllUserByComtaxCode(comtaxcode, keyword, limit, offset);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách user theo mã số thuế.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public bool UpdateEnterpriseInfo(CompanyBO enterprise, AccountBO objUser)
        {
            IData objIData;
            if (objDataAccess == null)
            {
                objIData = Data.CreateData(ConfigHelper.Instance.GetConnectionStringDS(), false);
                objDataAccess = objIData;
            }
            else
                objIData = objDataAccess;
            objIData.BeginTransaction();

            try
            {
                bool result = false;
                bool isInsertable = false;
                CompanyDAO objCompanyDAO = new CompanyDAO(objIData);
                AccountDAO objAccountDAO = new AccountDAO(objIData);
                if (enterprise.COMTAXCODE == null || enterprise.COMTAXCODE == "")
                {
                    return false;
                }
                //kiểm tra công ty nào đã xài mã số thuế này hay chưa.
                var check = objCompanyDAO.GetAllUserByComtaxCode(enterprise.COMTAXCODE, null, null);
                if (check.Count > 0)
                {
                    if (check.First().PHONENUMBER == check.First().PHONENUMBER)
                        isInsertable = true;
                    //else if (check.USERNAME == check.USERNAME)
                    //    isInsertable = true;
                    else
                        isInsertable = false;
                }
                else //nếu check null, nghĩa là chưa có công ty nào dùng comtaxcode này
                {
                    isInsertable = true;
                }
                if (isInsertable == true)
                {
                    string comtaxcode = objCompanyDAO.UpdateEnterpriseInfo(enterprise);
                    if (comtaxcode != null || comtaxcode != "")
                    {

                        objUser.COMTAXCODE = comtaxcode;
                        result = objAccountDAO.UpdateUser(objUser);
                    }
                }
                objIData.CommitTransaction();
                return result;
            }
            catch (Exception objEx)
            {
                objIData.RollBackTransaction();
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool AddEnterpriseInfoFreeVersion(CompanyBO enterprise, AccountBO objUser)
        {
            IData objIData;
            if (objDataAccess == null)
            {
                objIData = Data.CreateData(ConfigHelper.Instance.GetConnectionStringDS(), false);
                objDataAccess = objIData;
            }
            else
                objIData = objDataAccess;
            objIData.BeginTransaction();

            try
            {
                bool result = false;
                bool isInsertable = false;
                CompanyDAO objCompanyDAO = new CompanyDAO(objIData);
                AccountDAO objAccountDAO = new AccountDAO(objIData);
                if (enterprise.COMTAXCODE == null || enterprise.COMTAXCODE == "")
                {
                    return false;
                }
                //kiểm tra công ty nào đã xài mã số thuế này hay chưa.
                var check = objCompanyDAO.GetAllUserByComtaxCode(enterprise.COMTAXCODE, null, null);
                if (check.Count > 0)
                {
                    if (check.First().PHONENUMBER == check.First().PHONENUMBER)
                        isInsertable = true;
                    else
                        isInsertable = false;
                }
                else //nếu check null, nghĩa là chưa có công ty nào dùng comtaxcode này
                {
                    isInsertable = true;
                }
                if (isInsertable == true)
                {
                    string comtaxcode = objCompanyDAO.AddEnterpriseInfoFreeVersion(enterprise);
                    if (comtaxcode != null || comtaxcode != "")
                    {
                        objUser.PRECOMTAXCODE = objUser.COMTAXCODE;
                        objUser.COMTAXCODE = comtaxcode;
                        result = objAccountDAO.UpdateUserFreeVersion(objUser);
                    }
                }
                objIData.CommitTransaction();
                return result;
            }
            catch (Exception objEx)
            {
                objIData.RollBackTransaction();
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        //public bool UpdateEnterpriseInfo(CompanyBO enterprise, AccountBO objUser)
        //{
        //    IData objIData;
        //    if (objDataAccess == null)
        //    {
        //        objIData = Data.CreateData(ConfigHelper.Instance.GetConnectionStringDS(), false);
        //        objDataAccess = objIData;
        //    }
        //    else
        //        objIData = objDataAccess;
        //    objIData.BeginTransaction();

        //    try
        //    {
        //        bool result = false;
        //        bool isInsertable = false;
        //        CompanyDAO objCompanyDAO = new CompanyDAO(objIData);
        //        AccountDAO objAccountDAO = new AccountDAO(objIData);
        //        if (enterprise.COMTAXCODE == null || enterprise.COMTAXCODE == "")
        //        {
        //            return false;
        //        }
        //        //kiểm tra công ty nào đã xài mã số thuế này hay chưa.
        //        var check = objCompanyDAO.GetAllUserByComtaxCode(enterprise.COMTAXCODE, null, null);
        //        if (check.Count > 0)
        //        {
        //            if (check.First().PHONENUMBER == check.First().PHONENUMBER)
        //                isInsertable = true;
        //            //else if (check.USERNAME == check.USERNAME)
        //            //    isInsertable = true;
        //            else
        //                isInsertable = false;
        //        }
        //        else //nếu check null, nghĩa là chưa có công ty nào dùng comtaxcode này
        //        {
        //            isInsertable = true;
        //        }
        //        if (isInsertable == true)
        //        {
        //            string comtaxcode = objCompanyDAO.UpdateEnterpriseInfo(enterprise);
        //            if (comtaxcode != null || comtaxcode != "")
        //            {

        //                objUser.COMTAXCODE = comtaxcode;
        //                result = objAccountDAO.UpdateUser(objUser);
        //            }
        //        }
        //        return result;
        //    }
        //    catch (Exception objEx)
        //    {
        //        this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin doanh nghiệp.");
        //        objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
        //        return false;
        //    }
        //}

        public bool UpdateEnterpriseInfoAdminPage(CompanyBO enterprise)
        {
            IData objIData;
            if (objDataAccess == null)
            {
                objIData = Data.CreateData(ConfigHelper.Instance.GetConnectionStringDS(), false);
                objDataAccess = objIData;
            }
            else
                objIData = objDataAccess;
            objIData.BeginTransaction();

            try
            {
                if (string.IsNullOrEmpty(enterprise.COMTAXCODE))
                {
                    return false;
                }

                CompanyDAO objCompanyDAO = new CompanyDAO();
                AccountDAO objAccountDAO = new AccountDAO();
                //kiểm tra công ty nào đã xài mã số thuế này hay chưa.

                //Nếu là thêm mới doanh nghiệp thì enterprise.ID = 0
                if (enterprise.ID == null)
                {
                    //Kiểm tra xem đã tồn tại doanh nghiệp ứng với mã số thuế này
                    var result = objCompanyDAO.GetTaxCode(enterprise.COMTAXCODE);
                    if (result != null)
                    {
                        return false;
                    }
                    else
                    {
                        var addCompanyStatus = objCompanyDAO.UpdateEnterpriseInfo(enterprise);
                        if (string.IsNullOrEmpty(addCompanyStatus))
                            return false;
                        AccountBO account = new AccountBO()
                        {
                            COMTAXCODE = enterprise.COMTAXCODE,
                            EMAIL = enterprise.COMEMAIL,
                            PASSWORD = Cryptography.HashingSHA512(enterprise.COMTAXCODE),
                            ISADMIN = true
                        };
                        var addUserStatus = objAccountDAO.UpdateUser(account);
                    }
                }
                else
                {
                    var addCompanyStatus = objCompanyDAO.UpdateEnterpriseInfo(enterprise);
                    if (string.IsNullOrEmpty(addCompanyStatus))
                        return false;
                }
                objIData.CommitTransaction();
                return true;
            }
            catch (Exception objEx)
            {
                objIData.RollBackTransaction();
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra MST đã tồn tại hay chưa
        /// truongnv 20200221
        /// </summary>
        /// <param name="cusTaxCode">MST</param>
        /// <param name="cusName">Tên doanh nghiệp</param>
        /// <returns></returns>
        public string CheckCompanyDuplicateTaxCode(string cusTaxCode, string cusName)
        {
            string msg = string.Empty;
            try
            {
                CompanyDAO companyDL = new CompanyDAO();
                return companyDL.CheckCompanyDuplicateTaxCode(cusTaxCode, cusName);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi kiểm tra MST của doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = $"Không lấy được thông tin MST của doanh nghiệp.";
            }
            return msg;
        }

        /// <summary>
        /// Lấy ra thông tin doanh nghiệp
        /// truongnv 20200221
        /// </summary>
        /// <param name="companyId">ID doanh nghiệp</param>
        /// <param name="companyName">Tên doanh nghiệp</param>
        /// <returns></returns>
        public CompanyBO GetCompanyInfoByID(long companyId, string companyName)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetCompanyInfoByID(companyId, companyName);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin doanh nghiệp theo ID và Name.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Lấy ra thông tin doanh nghiệp
        /// truongnv 20200227
        /// </summary>
        /// <param name="companyId">ID doanh nghiệp</param>
        /// <returns></returns>
        public CompanyBO GetDataCompanyByID(long companyId)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetDataCompanyByID(companyId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin doanh nghiệp theo ID và Name.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra hóa đơn đã phát hành hay chưa
        /// truongnv 20200221
        /// </summary>
        /// <param name="cusTaxCode">MST</param>
        /// <param name="formCode">Mẫu số</param>
        /// <param name="symbolCode">Ký hiệu</param>
        /// <returns></returns>
        public string CheckInvoiceRelease(string cusTaxCode, string formCode, string symbolCode)
        {
            string msg = string.Empty;
            try
            {

                CompanyDAO companyDL = new CompanyDAO();
                return companyDL.CheckInvoiceRelease(cusTaxCode, formCode, symbolCode);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi kiểm tra MST của doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = $"Lỗi kiểm tra hóa đơn đã phát hành.";
            }
            return msg;
        }

        /// <summary>
        /// Kiểm tra hóa đơn còn số phát hành không
        /// truongnv 20200221
        /// </summary>
        /// <param name="cusTaxCode">MST</param>
        /// <param name="formCode">Mẫu số</param>
        /// <param name="symbolCode">Ký hiệu</param>
        /// <returns></returns>
        public string CheckInvoiceNumberRelease(string cusTaxCode, string formCode, string symbolCode)
        {
            string msg = string.Empty;
            try
            {

                CompanyDAO companyDL = new CompanyDAO();
                return companyDL.CheckInvoiceNumberRelease(cusTaxCode, formCode, symbolCode);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi kiểm tra hóa đơn đã phát hành.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = $"Lỗi kiểm tra hóa đơn đã phát hành.";
            }
            return msg;
        }

        /// <summary>
        /// truongnv 20200304
        /// Lấy thông tin doanh nghiệp trang cài đặt tài khoản
        /// </summary>
        /// <param name="comtaxcode">MST</param>
        /// <returns></returns>
        public CompanyBO GetInfoEnterpriseByTaxCode(string comtaxcode)
        {
            try
            {
                CompanyDAO objCompanyDAO = new CompanyDAO();
                return objCompanyDAO.GetInfoEnterpriseByTaxCode(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin doanh nghiệp theo email.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        public string ChangStatus(string comataxcode, string email,bool actived)
        {
            string msg = string.Empty;
            try
            {
                CompanyDAO companyDAO = new CompanyDAO();
                msg = companyDAO.ChangStatus(comataxcode, email, actived);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }
        //Cập nhật tham số làm tròn
        public string SaveParameter(CompanyBO company)
        {
            string msg = string.Empty;
            try {
                CompanyDAO companyDAO = new CompanyDAO();
                msg = companyDAO.SaveParameter(company);
            } catch(Exception objex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objex, "Lỗi cập nhật ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objex.Message;
            }
            return msg;
        }
        
        #endregion
    }
}
