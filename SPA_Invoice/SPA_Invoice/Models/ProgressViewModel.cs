using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA_Invoice.Models
{
    public class ProgressViewModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("currentRow")]
        public int CurrentRow { get; set; }
        [JsonProperty("totalRow")]
        public int TotalRow { get; set; }
    }
}