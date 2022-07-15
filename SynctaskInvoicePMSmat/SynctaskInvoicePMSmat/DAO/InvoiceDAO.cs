using SynctaskInvoicePMSmat.Common;
using SynctaskInvoicePMSmat.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SynctaskInvoicePMSmat.DAO
{
    public class InvoiceDAO : BaseDAO
    {
        public List<InvoiceBO> GetInvoices(DateTime requestedTime)
        {
            try
            {
                string spName = "[dbo].[USP_NOVAON_GET_INVOICES]";
                Dictionary<string, SqlParameter> cmdParameters = new Dictionary<string, SqlParameter>();
                cmdParameters["RequestedTime"] = new SqlParameter("@RequestedTime", requestedTime);
                var dataSet = sqlUtilities.ExecuteQuery(spName, cmdParameters);
                List<InvoiceBO> listInvoices = dataSet.Tables[0].ToList<InvoiceBO>();
                return listInvoices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvoiceBO> GetInvoicesFromCacheFile(string listInvoiceId)
        {
            try
            {
                string spName = "[dbo].[USP_NOVAON_GET_INVOICES_FROM_CACHE_FILE]";
                Dictionary<string, SqlParameter> cmdParameters = new Dictionary<string, SqlParameter>();
                cmdParameters["ListInvoiceIds"] = new SqlParameter("@ListInvoiceIds", listInvoiceId);
                var dataSet = sqlUtilities.ExecuteQuery(spName, cmdParameters);
                List<InvoiceBO> listInvoices = dataSet.Tables[0].ToList<InvoiceBO>();
                return listInvoices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateInvoices(string listMAHD)
        {
            try
            {
                string spName = "[dbo].[USP_NOVAON_UPDATE_INVOICES]";
                Dictionary<string, SqlParameter> cmdParameters = new Dictionary<string, SqlParameter>();
                cmdParameters["PMSMAT_INVOICEID"] = new SqlParameter("@PMSMAT_INVOICEID", listMAHD);
                var isSucess = sqlUtilities.ExecuteNonQuery(spName, cmdParameters);
                return isSucess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
