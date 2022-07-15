using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA_Invoice.Models
{
    public class Data
    {
        public string Base64Pdf { get; set; }
        public string Base64Xml { get; set; }
    }

    public class ResultDataInvoice
    {
        public bool rs { get; set; }

        public object result { get; set; }

        public long TotalPages { get; set; }

        public long TotalRow { get; set; }

        public int CurrentPage { get; set; }
    }
}