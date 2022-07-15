using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class FormCodeBO
    {
        public int ID { get; set; }
        public string FORMCODEID { get; set; }
        public string FORMCODENAME { get; set; }
        public string USERNAME { get; set; }
        public string TAXCODE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVE { get; set; }
    }
}
