using DS.BusinessObject.PaymentMethod;
using DS.Common.Helpers;
using DS.DataObject.PaymentMethod;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.PaymentMethod
{
    public class PaymentMethodBLL:BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public PaymentMethodBLL()
        {
        }

        public PaymentMethodBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        //lấy danh sách HTTT
        public List<PaymentMethodBO> GetAllPaymentMethod(string keyword, int pagesize, int offset,string comtaxcode)
        {
            try
            {
                PaymentMethodDAO objPaymentDAO = new PaymentMethodDAO();
                return objPaymentDAO.GetPaymentMethod(keyword, pagesize, offset,comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách HTTT");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        /// <summary>
        /// Cập nhật thông tin HTTT
        /// </summary>
        /// <returns></returns>
        public bool SavePaymentMethod(PaymentMethodBO payment)
        {
            try
            {
                PaymentMethodDAO paymentDAO = new PaymentMethodDAO();
                return paymentDAO.SavePaymentMethod(payment);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Tên HTTT này đã tồn tại vui lòng thử lại với tên khác");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        //xóa HTTT
        public string RemovePaymentMethod(string id)
        {
            try
            {
                PaymentMethodDAO paymentDAO = new PaymentMethodDAO();
                var msg= paymentDAO.RemovePaymentMehtod(id);

                return msg;
            }
            catch(Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi xóa HTTT");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return this.ErrorMsg;
            }
        }
        #endregion
    }
}
