using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Product
{
    public class ImportProductTitleBO
    {
        public string MYFIELD { get; set; }
        public string YOURFIELD { get; set; }
        public bool ISDIFFERENT { get; set; }
        public bool ISMANDATORY { get; set; }
        public List<string> ConfigFields { get; set; }
        public bool Required { get; set; }
    }
    public class ImportProductBO
    {
        public int PRODUCTTYPE { get; set; }
        public string PRODUCTTYPENAME { get; set; }
        public string PRODUCTNAME { get; set; }
        public string SKU { get; set; }
        public string CATEGORY { get; set; }
        public string DESCRIPTION { get; set; }
        public Single PRICE { get; set; }
        public string QUANTITYUNIT { get; set; }
        public int TAXRATE { get; set; }

        /// <summary>
        /// Từ  mức
        /// </summary>
        public int FROMLEVEL { get; set; }

        /// <summary>
        /// Đến mức
        /// </summary>
        public int TOLEVEL { get; set; }

        public int GROUPID { get; set; }
    }
}
