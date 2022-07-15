using DS.BusinessObject.Account;
using DS.BusinessObject.Customer;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Customer;
using DS.DataObject.Invoice;
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
    public class CustomerBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public CustomerBLL()
        {
        }

        public CustomerBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Lấy tất cả thông tin khách hàng
        /// </summary>
        /// <returns></returns>

        public List<CustomerBO> GetCustomer(FormSearchCustomer form)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.GetCustomer(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<CustomerBO>();
            }
        }

        /// <summary>
        /// Cập nhật thông tin Khách hàng
        /// </summary>
        /// <returns></returns>
        public bool AddCustomer(CustomerBO customer)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.AddCustomer(customer);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm mới khách hàng. Mã khách hàng không được giống nhau.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateCustomer(CustomerBO customer)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.UpdateCustomer(customer);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin khách hàng. Mã khách hàng không được giống nhau.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool SaveCustomerList(List<CustomerBO> customers)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                foreach (var customer in customers)
                {
                    var result = customerDAO.AddCustomer(customer);
                }
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi khi lưu hàng loạt thông tin khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool DeactiveCustomer(string customers)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.DeactiveCustomer(customers);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật thông tin khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public List<CustomerBO> ES_CustomerByKeyword(string strKeyword, string comTaxcode, int pagesize, int pageindex)
        {
            try
            {
                var keywords = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(strKeyword));
                //var str = ElaHelper.GenTerm("CHAI");
                string kwfieldEN = "keySearch";
                string[] arrKeywords = keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string rsl_en = kwfieldEN + ":" + "(" + comTaxcode + ") AND " + "((*" + string.Join("*) AND (*", arrKeywords) + "*))";


                //Search
                var result = ElasticIndexer.Current.IndexClient.Search<CustomerBO>(s => s
                    .Index("customer_invoice").Type("index")
                    .From(pageindex * pagesize)
                    .Size(pagesize)
                    .Query(q => q.QueryString(qs => qs.Query(rsl_en)))
                );

                return result.Documents.ToList();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi gọi Elasticsearch customer");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
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
            try
            {

                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.CheckCustomerDuplicateTaxCode(customer);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi kiểm tra MST của khách hàng.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = $"Không lấy được thông tin MST của khách hàng.";
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
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.CheckCustomerByCusId(customer);
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra Mã khách hàng.";
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, msg);
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return msg;
        }

        public List<CustomerBO> SearchCustomer(FormSearchCustomer form)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.SearchCustomer(form);
            }
            catch (Exception ex)
            {
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
            }
            return new List<CustomerBO>();
        }
        public string DeleteCustomer(string customerids)
        {
            string msg = string.Empty;
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                msg = customerDAO.DeleteCustomer(customerids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }
        //kiểm tra khách hàng có tồn tại trong invoice hay k
        public string CheckCustomer(string cusid,string comtaxcode)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.CkeckCustomer(cusid,comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
        }
        //kiểm tra xem khách hàng có tồn tại trong công tơ hay không
        public string CheckMeter(string cusid, string comtaxcode)
        {
            try
            {
                CustomerDAO customerDAO = new CustomerDAO();
                return customerDAO.CkeckMeter(cusid, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
        }
        #endregion
    }
}
