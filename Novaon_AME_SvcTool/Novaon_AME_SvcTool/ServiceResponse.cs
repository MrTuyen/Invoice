using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaon_AME_SvcTool
{
    public class ServiceResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Time { get; set; }
    }
}
