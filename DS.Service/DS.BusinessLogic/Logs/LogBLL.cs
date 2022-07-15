using DS.BusinessObject.Logs;
using DS.Common.Helpers;
using DS.DataObject.Logs;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Logs
{
    public class LogBLL:BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public LogBLL()
        {
        }

        public LogBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Function
        //lấy ra danh sách logs
        public List<LogBO> GetLogs(FormSearchLogs form)
        {
            try
            {
                LogDAO oDL = new LogDAO();
                return oDL.GetLogs(form);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<LogBO>();
            }
        }
        #endregion
    }
}
