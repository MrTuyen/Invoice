using DS.BusinessObject.EmailSender;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DS.DataObject.Email
{
    public class EmailDAO : BaseDAO
    {
        #region Constructor

        public EmailDAO() : base()
        {
        }

        public EmailDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region methods

        public Int64 AddEmail(EmailBO email)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_add");
                objIData.AddParameter("p_mailto", email.MAILTO);
                objIData.AddParameter("p_recievername", email.RECIEVERNAME);
                objIData.AddParameter("p_content", email.CONTENT);
                objIData.AddParameter("p_attachementlink", email.ATTACHEMENTLINK);
                objIData.AddParameter("p_status", email.STATUS);
                objIData.AddParameter("p_mailtype", email.MAILTYPE);
                objIData.AddParameter("p_comtaxcode", email.COMTAXCODE);
                objIData.AddParameter("p_invoiceid", email.INVOICEID);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
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

        public Int64 UpdateEmail(EmailBO email)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_update");
                objIData.AddParameter("p_id", email.ID);
                objIData.AddParameter("p_content", email.CONTENT);
                objIData.AddParameter("p_status", email.STATUS);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                return Convert.ToInt64(id);
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

        public List<EmailBO> GetEmailHistoryByInvoiceId(long invoiceId)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_history_get");
                objIData.AddParameter("p_invoiceid", invoiceId);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<EmailBO>();
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

        public List<EmailBO> GetEmailHistoryByComtaxcode(FormSearchEmail form)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_history_get_by_comtaxcode");
                objIData.AddParameter("p_comtaxcode", form.COMTAXCODE);
                objIData.AddParameter("p_pagesize", form.ITEMPERPAGE);
                objIData.AddParameter("p_offset", form.OFFSET);
                objIData.AddParameter("p_mailstatus", form.STATUS);
                objIData.AddParameter("p_mailtype", form.MAILTYPE);
                objIData.AddParameter("p_fromdate", form.FROMDATE);
                objIData.AddParameter("p_todate", form.TODATE);
                objIData.AddParameter("p_number", form.NUMBER);
                objIData.AddParameter("p_cusname", form.CUSNAME);
                objIData.AddParameter("p_mailto", form.MAILTO);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<EmailBO>();
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

        #endregion
    }
}
