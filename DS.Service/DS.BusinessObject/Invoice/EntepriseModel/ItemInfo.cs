using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice.EntepriseModel
{
    public class ItemInfo
    {
        public string Name { get; set; }
        public string Ho_Address { get; set; }
        public string Enterprise_Gdt_Code { get; set; }
    }
    public class Info
    {
        public List<ItemInfo> d { get; set; }
        public ItemInfo ResponseData { get; set; }
    }
}
