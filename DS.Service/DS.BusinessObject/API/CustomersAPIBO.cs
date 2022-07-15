using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.API
{
    [Serializable]
    public class CustomersAPIBO
    {
        public string cusid { get; set; }
        public string cusname { get; set; }
        public string custaxcode { get; set; }
        public string cusphonenumber { get; set; }
        public string cusaddress { get; set; }
        public string cusemail { get; set; }
        public string cuspaymentmethod { get; set; }
        public string cusbankname { get; set; }
        public string cusaccountnumber { get; set; }
        public string cuscurrencyunit { get; set; }
        public string cusnote { get; set; }
    }
}
