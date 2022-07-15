using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.CurrencyUnit
{
     public class CurrencyUnitBO
    {
        public int ID { get; set; }
        public string CURRENCYUNIT { get; set; }
        public bool ISACTIVED { get; set; }
        
        public long TOTALROW { get; set; }
        public bool ISSELECTED { get; set; }
        public string COMTAXCODE { get; set; }
    }
}
