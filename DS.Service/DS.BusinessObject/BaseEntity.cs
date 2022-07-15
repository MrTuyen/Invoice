using DS.BusinessObject.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    /// <summary>
    /// Truongnv 20200301
    /// Base entity chứa các cột mặc định có của hệ thống
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Người tạo
        /// </summary>
        public string CREATEDBY { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public string MODIFIEDBY { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime MODIFIEDDATE { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public string IPADDRESS { get; set; }

        public BaseEntity()
        {
            AccountBO objUser = AccountBO.Current.CurrentUser();
            if (objUser != null) {
                this.MODIFIEDBY = objUser.EMAIL; this.CREATEDBY = objUser.EMAIL;
            }
            this.IPADDRESS = Common.Helpers.CommonFunction.GetLocalIPAddress();
            this.MODIFIEDDATE = DateTime.Now;
        }
    }

    public class PDFContentPage
    {
        public string HtmlContent { get; set; }
        public string BaseUrl { get; set; }
        public string PageSize { get; set; }

    }
}
