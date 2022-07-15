using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice.API.Enumeration
{
    public class EnumCommon
    {
        /// <summary>
        /// Khai báo danh sách mã lỗi dùng chung
        /// 0	Thành công
        /// -1	Thất bại không rõ nguyên nhân
        /// </summary>
        public enum ErrorCode
        {
            Success = 0,
            Fail = -1
        }

        /// <summary>
        /// Khai báo danh mục PRODUCTTYPE
        /// 1: Hàng hóa,2: Dịch vụ
        /// </summary>
        public enum ProductType
        {
            Product = 1,
            Service = 2
        }
    }
}