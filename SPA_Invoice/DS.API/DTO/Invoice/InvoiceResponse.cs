using DS.BusinessObject.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DS.API.DTO.Invoice
{
    public class InvoiceResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public List<InvoiceBO> data { get; set; }
        public string time { get; set; }
    }
}