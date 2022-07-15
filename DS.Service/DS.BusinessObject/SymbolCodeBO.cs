using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class SymbolCodeBO
    {
        public int SYMBOLCODEID { get; set; }
        public int FROMNUMBER { get; set; }
        public int TONUMBER { get; set; }
        public string USERNAME { get; set; }
        public string TAXCODE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVE { get; set; }
    }
}
