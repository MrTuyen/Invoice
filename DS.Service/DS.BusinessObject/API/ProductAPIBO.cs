using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.API
{
    [Serializable]
    public class ProductAPIBO
    {
        /// <summary>
        /// Chuỗi mã hóa, do Onfinance Invoice cung cấp
        /// </summary>
        public string secret { get; set; }

        /// <summary>
        /// Mã đối tác, do Onfinance Invoice cung cấp
        /// </summary>
        public int partner_id { get; set; }

        public List<ProductDetailAPIBO> products { get; set; }
    }
}
