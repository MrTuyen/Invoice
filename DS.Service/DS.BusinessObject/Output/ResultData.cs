using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class ResultData
    {
        public bool Success { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessge { get; set; }

        public object Data { get; set; }
    }
}
