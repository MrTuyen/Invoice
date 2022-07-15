using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.API
{
    [Serializable]
    public class ProductDetailAPIBO
    {
        public string productname { get; set; }
        public int producttype { get; set; }
        public string sku { get; set; }
        public string category { get; set; }
        public double price { get; set; }
        public string quantityunit { get; set; }
    }
}
