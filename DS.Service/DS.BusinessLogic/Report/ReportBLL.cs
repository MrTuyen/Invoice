using DS.BusinessObject.Report;
using DS.Common.Helpers;
using DS.DataObject.Report;
using SAB.Library.Data;
using System;
using System.Reflection;

namespace DS.BusinessLogic.Report
{
    public class ReportBLL: BaseBLL
    {

        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public ReportBLL()
        {
        }

        public ReportBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        public bool AddReport(ReportBO report)
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO();
                return reportDAO.AddReport(report);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm biên bản!");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public bool UpdateReport(ReportBO report)
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO();
                return reportDAO.UpdateReport(report);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi chỉnh sửa biên bản!");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public bool UpdateModifiedReport(ReportBO report)
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO();
                return reportDAO.UpdateModifiedReport(report);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi chỉnh sửa biên bản điều chỉnh!");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public ReportBO GetReportByInvoiceIdReportType(long invoiceId, string reportType)
        {
            try
            {
                ReportDAO reportDAO = new ReportDAO();
                return reportDAO.GetReportByInvoiceIdReportType(invoiceId, reportType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin biên bản!");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }
        #endregion
    }
}
