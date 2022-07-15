using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class ResultMessageBO
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string MessageDetail { get; set; }
        public SystemErrorBO.ErrorTypes ErrorType { get; set; }
        public int ErrorCode { get; set; }
        public string Parram { get; set; }
    }
}
