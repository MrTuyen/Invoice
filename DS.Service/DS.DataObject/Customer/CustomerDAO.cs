using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Customer
{
    public class CustomerDAO : BaseDAO
    {
        #region Constructor

        public CustomerDAO() : base()
        {
        }

        public CustomerDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods

        //Hiển thị danh sách khách hàng

        public List<CustomerBO> GetCustomer(FormSearchCustomer form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_get");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_keyword", form.KEYWORD);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CustomerBO>();
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

        //Tạo mới hoặc cập nhật sản phẩm 
        public bool AddCustomer(CustomerBO customer)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_add");
                objIData.AddParameter("p_cusid", string.IsNullOrEmpty(customer.CUSID) ? Guid.NewGuid().ToString() : customer.CUSID);
                objIData.AddParameter("p_comtaxcode", customer.COMTAXCODE);
                objIData.AddParameter("p_buyer", string.IsNullOrEmpty(customer.CUSBUYER) ? customer.CUSNAME : customer.CUSBUYER);
                objIData.AddParameter("p_phonenumber", customer.CUSPHONENUMBER);
                objIData.AddParameter("p_name", customer.CUSNAME);
                objIData.AddParameter("p_email", customer.CUSEMAIL);
                objIData.AddParameter("p_taxcode", customer.CUSTAXCODE);
                objIData.AddParameter("p_website", customer.CUSWEBSITE);
                objIData.AddParameter("p_address", customer.CUSADDRESS);
                objIData.AddParameter("p_paymentmethod", customer.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_bankname", customer.CUSBANKNAME);
                objIData.AddParameter("p_accountnumber", customer.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_currencyunit", customer.CUSCURRENCYUNIT);
                objIData.AddParameter("p_note", customer.CUSNOTE);
                objIData.AddParameter("p_metercode", customer.METERCODE);
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
        /// Kiểm tra MST đã tồn tại hay chưa
        /// truongnv 20200219
        /// </summary>
        /// <param name="cusTaxCode">MST</param>
        /// <returns></returns>
        public string CheckCustomerDuplicateTaxCode(CustomerBO customer)
        {
            string msg = string.Empty;
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_check_duplicate_taxcode");
                objIData.AddParameter("p_custaxcode", customer.CUSTAXCODE);
                objIData.AddParameter("p_custname", customer.CUSNAME);
                objIData.AddParameter("p_comtaxcode", customer.COMTAXCODE);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (!string.IsNullOrWhiteSpace(id))
                    msg = $"MST <b>{customer.CUSTAXCODE}</b> đã tồn tại";
            }
            catch (Exception ex)
            {
                msg = $"Không lấy được thông tin MST của khách hàng.";
                RollBackTransactionIfAny(objIData);
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
            return msg;
        }

        /// <summary>
        /// Kiểm tra Mã khách hàng đã tồn tại chưa
        /// truongnv 2020025
        /// </summary>
        /// <param name="cusId">mã khách hàng</param>
        /// <returns></returns>
        public string CheckCustomerByCusId(CustomerBO customer)
        {
            string msg = string.Empty;
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_check_duplicate_byid");
                objIData.AddParameter("p_cusid", customer.CUSID);
                objIData.AddParameter("p_comtaxcode", customer.COMTAXCODE);

                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (!string.IsNullOrWhiteSpace(id))
                    msg = $"Mã khách hàng {customer.CUSID} đã tồn tại";
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra Mã khách hàng.";
                RollBackTransactionIfAny(objIData);
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
            return msg;
        }

        public bool UpdateCustomer(CustomerBO customer)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_update");
                objIData.AddParameter("p_id", customer.ID);
                objIData.AddParameter("p_cusid", customer.CUSID);
                objIData.AddParameter("p_comtaxcode", customer.COMTAXCODE);
                objIData.AddParameter("p_buyer", customer.CUSBUYER);
                objIData.AddParameter("p_phonenumber", customer.CUSPHONENUMBER);
                objIData.AddParameter("p_name", customer.CUSNAME);
                objIData.AddParameter("p_email", customer.CUSEMAIL);
                objIData.AddParameter("p_taxcode", customer.CUSTAXCODE);
                objIData.AddParameter("p_website", customer.CUSWEBSITE);
                objIData.AddParameter("p_address", customer.CUSADDRESS);
                objIData.AddParameter("p_paymentmethod", customer.CUSPAYMENTMETHOD);
                objIData.AddParameter("p_bankname", customer.CUSBANKNAME);
                objIData.AddParameter("p_accountnumber", customer.CUSACCOUNTNUMBER);
                objIData.AddParameter("p_currencyunit", customer.CUSCURRENCYUNIT);
                objIData.AddParameter("p_note", customer.CUSNOTE);
                objIData.AddParameter("p_isdeleted", customer.ISDELETED);
                objIData.AddParameter("p_metercode", customer.METERCODE);
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

        public bool DeactiveCustomer(string customers)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_delete_customer");
                objIData.AddParameter("p_id", customers);
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

        public List<CustomerBO> SearchCustomer(FormSearchCustomer form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_customer_search_auto_complete");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_keyword", form.KEYWORD);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<CustomerBO>();
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
        public string DeleteCustomer(string customerIds)
        {
            IData objIData = this.CreateIData();

            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                //objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_delete_invocie");
                //objIData.AddParameter("p_ids", invoiceIds);
                objIData.ExecUpdate($"UPDATE ds_masterdata.pm_customer SET isdeleted = true WHERE pm_customer.id IN({customerIds})");
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
        //kiểm tra khách hàng có tồn tại trong invoice hay không
        public string CkeckCustomer(string cusid,string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader= objIData.ExecQueryToString($"SELECT COUNT(*) FROM ds_masterdata.pm_invoice WHERE customercode  IN ({cusid}) AND active=FALSE AND comtaxcode='"+comtaxcode+"'");
                Convert.ToString(reader);
                CommitTransactionIfAny(objIData);
                return reader;
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
        //kiểm tra khách hàng có tồn tại trong công tơ không
        public string CkeckMeter(string cusid, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToString($"SELECT COUNT(*) FROM ds_masterdata.common_meter WHERE custaxcode  IN ({cusid}) AND isactive = TRUE AND comtaxcode = '" + comtaxcode + "'");
                Convert.ToString(reader);
                CommitTransactionIfAny(objIData);
                return reader;
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
