using DS.BusinessObject.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class SignInvoiceOutput
    {
        public InvoiceBO InvoiceData { get; set; }

        public string base64xmlSigned { get; set; }

        public string htmlContent { get; set; }
    }
}
