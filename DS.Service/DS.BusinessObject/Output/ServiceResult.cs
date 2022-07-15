using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Output
{
    public class ServiceResult
    {
        /// <summary>
        /// ID được thêm thành công
        /// </summary>
        public object ObjectID { get; set; }

        /// <summary>
        /// Mã code trả về: 
        /// 0	Thành công
        ////-1	Thất bại không rõ nguyên nhân
        ////97	Sai secret key
        ////01	partner_id không trùng khớp với secret key
        ////02	Mẫu hóa đơn không tồn tại
        ////03	Ký hiệu hóa đơn không tồn tại
        ////04	Đã hết số hóa đơn, vui lòng đăng ký phát hành mới hóa đơn
        ////05	Lỗi tham số đầu vào

        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Thông báo trả ra
        /// </summary>
        public string Message { get; set; }
    }
}
