using DS.BusinessObject.QuantityUnit;
using DS.Common.Helpers;
using DS.DataObject.QuantityUnit;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.QuantityUnit
{
    public class QuantityUnitBLL: BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public QuantityUnitBLL()
        {
        }

        public QuantityUnitBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        //lấy danh sách đơn vị tính
        public List<QuantityUnitBO> GetAllQuantityUnit(string keyword, int pagesize, int offset,string comtaxcode)
        {
            try
            {
                QuantityUnitDAO objQuantityDAO = new QuantityUnitDAO();
                return objQuantityDAO.GetQuantityUnit(keyword, pagesize, offset,comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách đơn vị tính");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        /// <summary>
        /// Cập nhật thông tin đơn vị tính
        /// </summary>
        /// <returns></returns>
        public bool SaveQuantityUnit(QuantityUnitBO quantity)
        {
            try
            {
                QuantityUnitDAO quantityDAO = new QuantityUnitDAO();
                return quantityDAO.SaveQuantityUnit(quantity);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Tên đơn vị tính này đã tồn tại vui lòng thử lại với tên khác");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        //xóa đơn vị tính
        public string RemoveQuantityUnit(string id)
        {
            try {
                QuantityUnitDAO quantityDAO = new QuantityUnitDAO();
                return quantityDAO.RemoveQuantityUnit(id);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi xóa đơn vị tính");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return this.ErrorMsg;
            }
        }
        #endregion
    }
}
