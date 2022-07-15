using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Logs
{
    //Class Log
    public class LogBO
    {
        public long? ID { get; set; }
        public int? ACTIONID { get; set; }
        public string ACTIONNAME { get; set; }
        public long OBJECTID { get; set; }
        public string OBJECTNAME { get; set; }
        public string USERNAME { get; set; }
        public string IPADDRESS { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime LOGTIME { get; set; }
        public int TOTALROW { get; set; }
        public bool ISSELECTED { get; set; }
        public string COMTAXCODE { get; set; }
    }
}
