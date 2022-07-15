using DS.BusinessObject.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DS.API.DTO.Invoice
{
    public class InvoiceRequest
    {
        public string partnerid { get; set; }
        public FormSearchInvoice invoice { get; set; }
    }
}