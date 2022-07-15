using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Meter
{
    public class MeterBO
    {
        public long ID { get; set; }

        public string CODE { get; set; }

        public string METERNAME { get; set; }

        public bool ISACTIVE { get; set; }

        public string CUSTAXCODE { get; set; }

        public string COMTAXCODE { get; set; }

        public string PRODUCTCODE { get; set; }

        public int FACTOR { get; set; }

        public string PRODUCTCODELIST { get; set; }

        public int APARTMENTNO { get; set; }
        public int TOTALROW { get; set; }

        public bool ISSELECTED { get; set; }

        public int STT { get; set; }
    }
}
