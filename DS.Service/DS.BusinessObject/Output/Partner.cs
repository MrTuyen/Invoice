using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    [Serializable]
    public class Partner
    {
        /// <summary>
        /// ID đối tác
        /// </summary>
        public int Partner_id { get; set; }

        /// <summary>
        /// Mã xác thực đối tác
        /// </summary>
        public string Secret { get; set; }
    }
}
