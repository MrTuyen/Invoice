using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class CurrencyBO
    {
        public List<Item> items { get; set; }
    }
    public class Item
    {
        public string type { get; set; }
        public string imageurl { get; set; }
        public string muatienmat { get; set; }
        public string muack { get; set; }
        public string bantienmat { get; set; }
        public string banck { get; set; }
    }
}
