using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Product
{
    public class FormSearchProduct
    {
        public string COMTAXCODE { get; set; }
        public string KEYWORD { get; set; }
        public int? PRODUCTTYPE { get; set; }
        public string CATEGORY { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
