using DS.BusinessObject.Invoice;
using System.Collections.Generic;

namespace Invoice.API.DTO.Invoice
{
    public class InvoiceResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<InvoiceBO> data { get; set; }
        public List<int> IndexCreated { get; set; }
        public List<long> InvoiceCreated { get; set; }
        public string Time { get; set; }
    }
}