using DS.BusinessObject.Meter;
using DS.Common.Helpers;
using DS.DataObject.Meter;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Meter
{
    public class MeterBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public MeterBLL()
        {
        }

        public MeterBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Function
        public List<MeterBO> GetMeterListByCustaxcode(string custaxcode)
        {
            try
            {
                MeterDAO oDL = new MeterDAO();
                return oDL.GetMeterListByCustaxcode(custaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<MeterBO>();
            }
        }

        public List<MeterBO> GetMeterListByComtaxcode(string custaxcode)
        {
            try
            {
                MeterDAO oDL = new MeterDAO();
                return oDL.GetMeterListByComtaxcode(custaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<MeterBO>();
            }
        }

        public List<MeterBO> GetListMeterCodeByInvoiceID(long invoiceId)
        {
            try
            {
                MeterDAO oDL = new MeterDAO();
                List<MeterBO> lst = new List<MeterBO>();
                var electricMeterList = oDL.GetListMeterCodeByInvoiceID(invoiceId);
                for (int i = 0; i < electricMeterList.Count; i++)
                {
                    electricMeterList[i].STT = i + 1;
                    lst.Add(electricMeterList[i]);
                }
                return lst;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<MeterBO>();
            }
        }
        public List<MeterBO> GetMeter(FormSearchMeter form)
        {
            try
            {
                MeterDAO MeterDAO = new MeterDAO();
                return MeterDAO.GetMeter(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách công tơ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<MeterBO>();
            }
        }
        public string AddCommonMeter(MeterBO meter)
        {
            string msg = string.Empty;
            try
            {
                MeterDAO oDL = new MeterDAO();
                return oDL.AddCommonMeter(meter);
            }
            catch
            {
                msg = "Lỗi thêm mới danh mục công tơ.";
            }
            return msg;
        }
        public bool UpdateMeter(MeterBO meter)
        {
            try
            {
                MeterDAO productDAO = new MeterDAO();
                return productDAO.UpdateMeter(meter);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật công tơ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Tải lên danh sách công cơ Excel
        /// </summary>
        /// <returns></returns>
        public bool ImportMeter(List<MeterBO> meters)
        {
            try
            {
                MeterDAO oDL = new MeterDAO();
                foreach (var obj in meters)
                {
                    oDL.AddCommonMeter(obj);
                }
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm sản phẩm từ Excel");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        public string DeleteMeter(string meterids)
        {
            string msg = string.Empty;
            try
            {
                MeterDAO meterDAO = new MeterDAO();
                msg = meterDAO.DeleteMeter(meterids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }

        public int CheckInvoice(string metercode)
        {
            try
            {
                MeterDAO meterDAO = new MeterDAO();
                return meterDAO.CkeckInvoice(metercode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách chi tiết hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return 0;
            }
        }
        #endregion
    }
}
