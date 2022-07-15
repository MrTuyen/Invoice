using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendEmailService
{
    public class HelperDAO : BaseDAO
    {
        public List<EmailDataBO> GetEmail(string lstEmailTo)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_get");
                objIData.AddParameter("p_emailtolist", lstEmailTo);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<EmailDataBO>();
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


        public bool UpdateStatusEmail(long id, int status)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_email_update");
                objIData.AddParameter("p_id", id);
                objIData.AddParameter("p_content", null);
                objIData.AddParameter("p_status", status);
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

    }

}
