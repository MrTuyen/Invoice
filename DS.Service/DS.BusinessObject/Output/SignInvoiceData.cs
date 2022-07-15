using DS.BusinessObject.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class SignInvoiceData
    {
        public string CusCode { get; set; }

        public InvoiceBO InvoiceData { get; set; }

        public List<InvoiceDetailBO> lstDetails { get; set; }
    }
}
