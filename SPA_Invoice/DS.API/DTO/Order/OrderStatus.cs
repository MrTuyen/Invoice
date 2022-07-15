using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DS.API.Dto.Order
{
    public class OrderStatus
    {
        public string OrderCode { get; set; }
        public string status { get; set; }
        public string partnerCode { get; set; }
    }
}