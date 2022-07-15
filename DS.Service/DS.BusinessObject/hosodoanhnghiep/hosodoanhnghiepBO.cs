using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.hosodoanhnghiep
{
    public class hosodoanhnghiepBO
    {
        public string LINKDETAIL { get; set; }
        public string COMNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public string COMADDRESS { get; set; }
        public string COMLEGAL { get; set; }
        public string COMPHONE { get; set; }
        public string COMACTIVETIME { get; set; }
        public string COMACTIVITYTIME { get; set; }
        public string STATUS { get; set; }
        public string KEYWORD { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
