using Novaon_AME_SvcTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Novaon_AME_SvcTool.DAO
{
    public class InvoiceDAO : BaseDAO
    {
        public List<InvoiceBO> GetInvoices(string query)
        {
            try
            {
                var dataSet = sqlUtilities.ExecuteQueryWithQuery(query);
                List<InvoiceBO> listInvoices = dataSet.Tables[0].ToList<InvoiceBO>();
                return listInvoices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateInvoices(string query)
        {
            try
            {
                var isSucess = sqlUtilities.ExecuteNonQueryWithQuery(query);
                return isSucess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
