using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class APIEnterpriseResponse
    {
        public bool success { get; set; }
        public EnterpriseInfo data { get; set; }
    }
    public class EnterpriseInfo
    {
        public string ComName { get; set; }
        public string ComAddress { get; set; }
        public string Source { get; set; }
        public string IsMainMode { get; set; }
    }
}
