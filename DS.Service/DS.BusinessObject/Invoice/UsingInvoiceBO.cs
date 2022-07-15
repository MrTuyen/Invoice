using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice
{
    public class UsingInvoiceBO
    {
        public string FORMCODE { get; set; }
        public string SYMBOLCODE { get; set; }
        public long ROOT_FROMNUMBER { get; set; }
        public long ROOT_TONUMBER { get; set; }
        public long FROMNUMBER { get; set; }
        public long TONUMBER { get; set; }
        public long SUM_PRE_TERM { get; set; }
        public long PRE_TERM_RETAIN_FROMNUMBER { get; set; }
        public long PRE_TERM_RETAIN_TONUMBER { get; set; }
        public long BUY_INTERM_FROMNUMBER { get; set; }
        public long BUY_INTERM_TONUMBER { get; set; }
        public long ALL_USED_FROMNUMBER { get; set; }
        public long ALL_USED_TONUMBER { get; set; }
        public long ALL_USED_SUM { get; set; }
        public long REAL_USED_SUM { get; set; }
        public long DELETED_USED_SUM { get; set; }
        public string DELETED_USED_DETAIL_LIST { get; set; }
        public string LAST_RETAIN_FROMNUMBER { get; set; }
        public string LAST_RETAIN_TONUMBER { get; set; }
        public long LAST_RETAIN_SUM { get; set; }
    }
}
