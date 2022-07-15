using DS.BusinessObject.Account;
using DS.BusinessObject.Company;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DS.DataObject.Company
{
    public class CompanyDAO : BaseDAO
    {
        #region Constructor

        public CompanyDAO() : base()
        {
        }

        public CompanyDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region methods
        public CompanyBO GetInfoEnterprise(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_getbyid");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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
        /// truongnv 20200304
        /// Lấy thông tin doanh nghiệp trang cài đặt tài khoản
        /// </summary>
        /// <param name="comtaxcode">MST</param>
        /// <returns></returns>
        public CompanyBO GetInfoEnterpriseByTaxCode(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_get_info_by_taxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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

        public CompanyBO GetTaxCode(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_getby_taxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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

        public List<CompanyBO> GetAllCompany(CompanySearchFormBO companySearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_getby_taxcode");
                objIData.AddParameter("p_comtaxcode", companySearch.COMTAXCODE);
                objIData.AddParameter("p_username", companySearch.USERNAME);
                objIData.AddParameter("p_keyword", companySearch.KEYWORD);
                objIData.AddParameter("p_pagesize", companySearch.ITEMPERPAGE);
                objIData.AddParameter("p_offset", companySearch.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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

        public List<AccountBO> GetAllUserByComtaxCode(string comtaxcode, int? limit, int? offset)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get_bycomtaxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                objIData.AddParameter("p_limit", limit);
                objIData.AddParameter("p_offset", offset);
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
        public List<AccountBO> GetAllUserByComtaxCode(string comtaxcode, string keyword, int? limit, int? offset)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.system_user_get_bycomtaxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                objIData.AddParameter("p_keyword", keyword);
                objIData.AddParameter("p_limit", limit);
                objIData.AddParameter("p_offset", offset);
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
        public string UpdateEnterpriseInfo(CompanyBO enterprise)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_update");
                objIData.AddParameter("p_id", enterprise.ID);
                objIData.AddParameter("p_comtaxcode", enterprise.COMTAXCODE);
                objIData.AddParameter("p_comname", enterprise.COMNAME);
                objIData.AddParameter("p_comlegalname", enterprise.COMLEGALNAME);
                objIData.AddParameter("p_comsize", enterprise.COMSIZE);
                objIData.AddParameter("p_comactivity", enterprise.COMACTIVITY);
                objIData.AddParameter("p_comemail", enterprise.COMEMAIL);
                objIData.AddParameter("p_comwebsite", enterprise.COMWEBSITE);
                objIData.AddParameter("p_comaccountnumber", enterprise.COMACCOUNTNUMBER);
                objIData.AddParameter("p_combankname", enterprise.COMBANKNAME);
                objIData.AddParameter("p_comaddress", enterprise.COMADDRESS);
                objIData.AddParameter("p_comlogo", enterprise.COMLOGO);
                objIData.AddParameter("p_comphonenumber", enterprise.COMPHONENUMBER);
                objIData.AddParameter("p_mailserviceid", enterprise.MAILSERVICEID);
                objIData.AddParameter("p_mailserviceaccount", enterprise.MAILSERVICEACCOUNT);
                objIData.AddParameter("p_mailservicepassword", enterprise.MAILSERVICEPASSWORD);
                objIData.AddParameter("p_username", enterprise.USERNAME);
                objIData.AddParameter("p_isdeleted", enterprise.ISDELETED);
                objIData.AddParameter("p_taxdepartementcode", enterprise.TAXDEPARTEMENTCODE);
                objIData.AddParameter("p_taxdepartement", enterprise.TAXDEPARTEMENT);
                objIData.AddParameter("p_usinginvoicetype", enterprise.USINGINVOICETYPE);
                objIData.AddParameter("p_usinginvoicetypetmp", enterprise.USINGINVOICETYPETMP);
                objIData.AddParameter("p_autosendmail", enterprise.AUTOSENDMAIL);
                objIData.AddParameter("p_zaloaccesstoken", enterprise.ZALOACCESSTOKEN);
                objIData.AddParameter("p_showmenuwaitingsign", enterprise.SHOWMENUWAITINGSIGN);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);

                return id;
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

        public string AddEnterpriseInfoFreeVersion(CompanyBO enterprise)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_add_free_version");
                objIData.AddParameter("p_id", enterprise.ID);
                objIData.AddParameter("p_comtaxcode", enterprise.COMTAXCODE);
                objIData.AddParameter("p_comname", enterprise.COMNAME);
                objIData.AddParameter("p_comlegalname", enterprise.COMLEGALNAME);
                objIData.AddParameter("p_comsize", enterprise.COMSIZE);
                objIData.AddParameter("p_comactivity", enterprise.COMACTIVITY);
                objIData.AddParameter("p_comemail", enterprise.COMEMAIL);
                objIData.AddParameter("p_comwebsite", enterprise.COMWEBSITE);
                objIData.AddParameter("p_comaccountnumber", enterprise.COMACCOUNTNUMBER);
                objIData.AddParameter("p_combankname", enterprise.COMBANKNAME);
                objIData.AddParameter("p_comaddress", enterprise.COMADDRESS);
                objIData.AddParameter("p_comlogo", enterprise.COMLOGO);
                objIData.AddParameter("p_comphonenumber", enterprise.COMPHONENUMBER);
                objIData.AddParameter("p_mailserviceid", enterprise.MAILSERVICEID);
                objIData.AddParameter("p_mailserviceaccount", enterprise.MAILSERVICEACCOUNT);
                objIData.AddParameter("p_mailservicepassword", enterprise.MAILSERVICEPASSWORD);
                objIData.AddParameter("p_username", enterprise.USERNAME);
                objIData.AddParameter("p_isdeleted", enterprise.ISDELETED);
                objIData.AddParameter("p_taxdepartementcode", enterprise.TAXDEPARTEMENTCODE);
                objIData.AddParameter("p_taxdepartement", enterprise.TAXDEPARTEMENT);
                objIData.AddParameter("p_usinginvoicetype", enterprise.USINGINVOICETYPE);
                objIData.AddParameter("p_usinginvoicetypetmp", enterprise.USINGINVOICETYPETMP);
                objIData.AddParameter("p_isfreetrial", enterprise.ISFREETRIAL);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);

                return id;
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
                IData objIData = this.CreateIData();
                BeginTransactionIfAny(objIData);
                if (cusName == "") // for API
                {
                    var id = objIData.ExecQueryToString($"SELECT 1 FROM ds_masterdata.pm_company WHERE ds_masterdata.pm_company.comtaxcode = '{cusTaxCode}'");
                    CommitTransactionIfAny(objIData);
                    if (string.IsNullOrWhiteSpace(id))
                        msg = $"MST {cusTaxCode} không tồn tại. Vui lòng kiểm tra lại.";
                }
                else // for normal case
                {
                    objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_check_duplicate_taxcode");
                    objIData.AddParameter("p_comtaxcode", cusTaxCode);
                    objIData.AddParameter("p_comname", cusName);
                    var id = objIData.ExecStoreToString();
                    CommitTransactionIfAny(objIData);
                    if (string.IsNullOrWhiteSpace(id))
                        msg = $"MST {cusTaxCode} không tồn tại. Vui lòng kiểm tra lại.";
                }
            }
            catch (Exception ex)
            {
                msg = $"Không lấy được thông tin MST của doanh nghiệp.";
                throw ex;
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
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_get_info_by_id");
                objIData.AddParameter("p_comid", companyId);
                objIData.AddParameter("p_comname", companyName);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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
        /// Lấy ra thông tin doanh nghiệp
        /// truongnv 20200227
        /// </summary>
        /// <param name="companyId">ID doanh nghiệp</param>
        /// <param name="companyName">Tên doanh nghiệp</param>
        /// <returns></returns>
        public CompanyBO GetDataCompanyByID(long companyId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_get_data_by_id");
                objIData.AddParameter("p_comid", companyId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CompanyBO>();
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
                IData objIData = this.CreateIData();
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_check_invoice_release");
                objIData.AddParameter("p_comtaxcode", cusTaxCode);
                objIData.AddParameter("p_formcode", formCode);
                objIData.AddParameter("p_symbolcode", symbolCode);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (!string.IsNullOrWhiteSpace(id))
                    msg = $"Hóa đơn mẫu số {formCode} và ký hiệu {symbolCode} này đã phát hành. Vui lòng kiểm tra lại.";
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra hóa đơn đã phát hành.";
                throw ex;
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
                IData objIData = this.CreateIData();
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_number_check_release_bytaxcode");
                objIData.AddParameter("p_comtaxcode", cusTaxCode);
                objIData.AddParameter("p_formcode", formCode);
                objIData.AddParameter("p_symbolcode", symbolCode);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (string.IsNullOrWhiteSpace(id))
                    msg = $"Hóa đơn mẫu số {formCode} và ký hiệu {symbolCode} này đã hết số phát hành. Vui lòng kiểm tra lại.";
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra hóa đơn đã phát hành.";
                throw ex;
            }
            return msg;
        }
        public string ChangStatus(string comataxcode, string email,bool actived)
        {
            IData objIData = this.CreateIData();

            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                if(actived ==true)
                {
                    objIData.ExecUpdate($"UPDATE ds_masterdata.system_user SET isactived = false WHERE comtaxcode IN (SELECT comtaxcode FROM ds_masterdata.pm_company WHERE email= '{email}')");
                }
                else
                {
                    objIData.ExecUpdate($"UPDATE ds_masterdata.system_user SET isactived = true WHERE comtaxcode IN (SELECT comtaxcode FROM ds_masterdata.pm_company WHERE email= '{email}')");
                }
                CommitTransactionIfAny(objIData);
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
            return string.Empty;
        }
        #endregion
        //cập nhật tham số làm tròn
        public string SaveParameter(CompanyBO company)
        {
            IData objIData = this.CreateIData();
            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_company_update_parameter");
                objIData.AddParameter("p_comtaxcode", company.COMTAXCODE);
                objIData.AddParameter("p_quantity", company.QUANTITYPLACE);
                objIData.AddParameter("p_price", company.PRICEPLACE);
                objIData.AddParameter("p_money", company.MONEYPLACE);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);
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
            return msg;
        }
    }
}
