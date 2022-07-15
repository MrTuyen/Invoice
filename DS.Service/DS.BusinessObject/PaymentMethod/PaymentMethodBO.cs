using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.PaymentMethod
{
    public class PaymentMethodBO
    {
        public int ID { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public bool ISACTIVED { get; set; }
        public long TOTALROW { get; set; }
        public bool ISSELECTED { get; set; }
        public string COMTAXCODE { get; set; }
    }
}
