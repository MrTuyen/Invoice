using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.QuantityUnit
{
    public class QuantityUnitBO
    {
        public int ID { get; set; }
        public string QUANTITYUNIT { get; set; }
        public long TOTALROW { get; set; }
        public bool ISSELECTED { get; set; }
        public bool ISACTIVED { get; set; }
        public string COMTAXCODE { get; set; }
    }
}
