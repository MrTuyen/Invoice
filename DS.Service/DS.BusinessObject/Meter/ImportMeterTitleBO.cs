using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Meter
{
    public class ImportMeterTitleBO
    {
        public string MYFIELD { get; set; }
        public string YOURFIELD { get; set; }
        public bool ISDIFFERENT { get; set; }
        public string EXPLAINT { get; set; }
        public List<string> ConfigFields { get; set; }
        public bool Required { get; set; }
    }

    public class ImportMeterBO
    {
        public string CODE { get; set; }

        public string METERNAME { get; set; }

        public bool ISACTIVE { get; set; }

        public string CUSTAXCODE { get; set; }

        public string COMTAXCODE { get; set; }

        public string PRODUCTCODE { get; set; }

        public int FACTOR { get; set; }

        public string PRODUCTCODELIST { get; set; }

        public int APARTMENTNO { get; set; }
    }
}
