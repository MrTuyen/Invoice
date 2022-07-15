using DS.BusinessObject.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice.API.DTO.Invoice
{
    public class InvoiceRequest
    {
        public string partnerid { get; set; }
        public FormSearchInvoice invoice { get; set; }
    }

    public class ListInvoiceIdRequest
    {
        public string COMUSERNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public string IDS { get; set; }
    }

    public class TokenModel
    {
        public string id { get; set; }
        public string taxCode { get; set; }
        public string comName { get; set; }
        public string appId { get; set; }
    }

    public class ResponseInvoiceInfo
    {
        public long ID { get; set; }
        public string INVOICECODE { get; set; }
        public string REFERENCECODE { get; set; }
        public long NUMBER { get; set; }
        public DateTime SIGNEDTIME { get; set; }
        public string SIGNLINK { get; set; }
    }
}