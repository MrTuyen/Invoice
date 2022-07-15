using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Logs
{
    //thông tin search log
    public class FormSearchLogs
    {
        public string COMTAXCODE { get; set; }
        public string KEYWORD { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
