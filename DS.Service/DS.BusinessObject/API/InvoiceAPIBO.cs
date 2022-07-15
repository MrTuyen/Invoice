using DS.BusinessObject.Invoice;
using DS.BusinessObject.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.API
{
    [Serializable]
    public class InvoiceAPIBO
    {
        /// <summary>
        /// Chuỗi mã hóa, do Onfinance Invoice cung cấp
        /// </summary>
        public string secret { get; set; }

        /// <summary>
        /// Mã đối tác, do Onfinance Invoice cung cấp
        /// </summary>
        public int partner_id { get; set; }
        public string formcode { get; set; }
        public string symbolcode { get; set; }
        public string customer_name { get; set; }
        public string company { get; set; }
        public string taxcode { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public int payment_method { get; set; }
        public string bank { get; set; }
        public string account_number { get; set; }
        public int currency_unit { get; set; }

        public List<InvoiceDetailAPIBO> products { get; set; }
    }
}
