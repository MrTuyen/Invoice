using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DS.API.Dto.Order
{
    public class TokenResponse
    {
        public string token { get; set; }
        public string tokenType { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}