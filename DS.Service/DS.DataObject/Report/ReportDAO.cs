using DS.BusinessObject.Report;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Report
{
    public class ReportDAO: BaseDAO
    {
        #region Constructor

        public ReportDAO() : base()
        {
        }

        public ReportDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods

        public bool AddReport(ReportBO report)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_report_add");
                objIData.AddParameter("p_invoiceid", report.INVOICEID);
                objIData.AddParameter("p_reporttype", report.REPORTTYPE);
                objIData.AddParameter("p_comname", report.COMNAME);
                objIData.AddParameter("p_comaddress", report.COMADDRESS);
                objIData.AddParameter("p_comtaxcode", report.COMTAXCODE);
                objIData.AddParameter("p_comphone", report.COMPHONENUMBER);
                objIData.AddParameter("p_comlegalname", report.COMLEGALNAME);
                objIData.AddParameter("p_comrole", report.COMROLE);
                objIData.AddParameter("p_cusname", report.CUSNAME);
                objIData.AddParameter("p_cusaddress", report.CUSADDRESS);
                objIData.AddParameter("p_custaxcode", report.CUSTAXCODE);
                objIData.AddParameter("p_cusphone", report.CUSPHONENUMBER);
                objIData.AddParameter("p_cusdelegate", report.CUSDELEGATE);
                objIData.AddParameter("p_cusrole", report.CUSROLE);
                objIData.AddParameter("p_reason", report.REASON);
                objIData.AddParameter("p_link", report.LINK);
                objIData.AddParameter("p_reportstatus", report.REPORTSTATUS);
                objIData.AddParameter("p_reporttime", report.REPORTTIME);
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

        public bool UpdateReport(ReportBO report)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_report_update");
                objIData.AddParameter("p_id", report.ID);
                objIData.AddParameter("p_invoiceid", report.INVOICEID);
                objIData.AddParameter("p_reporttype", report.REPORTTYPE);
                objIData.AddParameter("p_comphone", report.COMPHONENUMBER);
                objIData.AddParameter("p_comlegalname", report.COMLEGALNAME);
                objIData.AddParameter("p_comrole", report.COMROLE);
                objIData.AddParameter("p_cusaddress", report.CUSADDRESS);
                objIData.AddParameter("p_custaxcode", report.CUSTAXCODE);
                objIData.AddParameter("p_cusphone", report.CUSPHONENUMBER);
                objIData.AddParameter("p_cusdelegate", report.CUSDELEGATE);
                objIData.AddParameter("p_cusrole", report.CUSROLE);
                objIData.AddParameter("p_reason", report.REASON);
                objIData.AddParameter("p_link", report.LINK);
                objIData.AddParameter("p_reportstatus", report.REPORTSTATUS);
                objIData.AddParameter("p_reporttime", report.REPORTTIME);
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

        public bool UpdateModifiedReport(ReportBO report)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_report_modified_update");
                objIData.AddParameter("p_id", report.ID);
                objIData.AddParameter("p_invoiceid", report.INVOICEID);
                objIData.AddParameter("p_reporttype", report.REPORTTYPE);
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

        public ReportBO GetReportByInvoiceIdReportType(long invoiceId, string reportType)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_report_get_by_invoiceid_reporttype");
                objIData.AddParameter("p_invoiceid", invoiceId);
                objIData.AddParameter("p_reporttype", reportType);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ReportBO>();
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
        #endregion
    }

}
