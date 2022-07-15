using DS.BusinessObject.CurrencyUnit;
using DS.Common.Helpers;
using DS.DataObject.CurrencyUnit;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.CurrencyUnit
{
    public class CurrencyUnitBLL:BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public CurrencyUnitBLL()
        {
        }

        public CurrencyUnitBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        //lấy danh sách tiền thanh toán
        public List<CurrencyUnitBO> GetAllCurrencyUnit(string keyword, int pagesize, int offset,string comtaxcode)
        {
            try
            {
                CurrencyUnitDAO objCurrencyDAO = new CurrencyUnitDAO();
                return objCurrencyDAO.GetCurrencyUnit(keyword, pagesize, offset,comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách tiền thanh toán");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        /// <summary>
        /// Cập nhật thông tin tiền thanh toán
        /// </summary>
        /// <returns></returns>
        public bool SaveCurrencyUnit(CurrencyUnitBO currency)
        {
            try
            {
                CurrencyUnitDAO currencyDAO = new CurrencyUnitDAO();
                return currencyDAO.SaveCurrencyUnit(currency);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Thông tin tiền thanh toán này đã tồn tại vui lòng thử lại với tên khác");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        //xóa tiền thanh toán
        public string RemoveCurrencyUnit(string id)
        {
            try
            {
                CurrencyUnitDAO currencyDAO = new CurrencyUnitDAO();
                var msg = currencyDAO.RemoveCurrencyUnit(id);

                return msg;
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi xóa tiền thanh toán");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return this.ErrorMsg;
            }
        }
        #endregion
    }
}
