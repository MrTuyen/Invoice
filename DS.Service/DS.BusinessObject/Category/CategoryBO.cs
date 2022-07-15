using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Category
{
    public class CategoryBO
    { 
        public long ID { get; set; }
        public int CATEGORYID { get; set; }
        public string CATEGORY { get; set; }
        public string COMTAXCODE { get; set; }
        public string COMUSERNAME { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVE { get; set; }
        public bool ISSUBCATEGORY { get; set; }
        public long TOTALROW { get; set; }
        public bool ISSELECTED { get; set; }
    }
}
