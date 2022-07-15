using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Customer
{
    public class ImportCustomerTitleBO
    {
        public string MYFIELD { get; set; }
        public string YOURFIELD { get; set; }
        public bool ISDIFFERENT { get; set; }
    }
    public class ImportCustomerBO
    {
        public string CUSID { get; set; }
        public string CUSNAME { get; set; }
        public string CUSPHONENUMBER { get; set; }
        public string CUSBUYER { get; set; }
        public string CUSEMAIL { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSWEBSITE { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSPAYMENTMETHOD { get; set; }
        public string CUSBANKNAME { get; set; }
        public string CUSACCOUNTNUMBER { get; set; }
        public int CUSCURRENCYUNIT { get; set; }
        public string CUSNOTE { get; set; }
    }
}
