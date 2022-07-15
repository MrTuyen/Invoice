using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.API
{
    [Serializable]
    public class InvoiceDetailAPIBO
    {
        public string productname { get; set; }

        public string unit { get; set; }

        public int quantity { get; set; }

        public decimal price { get; set; }

        public bool promotion { get; set; }
    }
}
