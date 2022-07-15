using SynctaskInvoicePMSmat.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynctaskInvoicePMSmat.DAO
{
    public class BaseDAO
    {
        protected SqlUtilities sqlUtilities;
       
        protected BaseDAO()
        {
            sqlUtilities = new SqlUtilities();
        }
    }
}
