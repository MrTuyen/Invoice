namespace DS.Common.Enums
{
    public class EnumHelper
    {
        public enum INVOICE_TYPE
        {
            VAT = 1, //giá trị gia tăng
            RETAIL = 2, //bán lẻ
            CANCEL = 3, //hủy bỏ
            CONVERT = 4, //chuyển đổi
            MODIFIED = 5, //điều chỉnh
            ALTERNATIVE = 6, //thay thế,
            RECEIPT = 7,// Biên lai thu phí, lệ phí
        }

        public enum INVOICE_STATUS
        {
            WAITING = 1,
            CONFIRMED = 2,
            SENT = 3
        }

        public enum EREPORTTYPE
        {
            Cancel = 1, //Biển bản hủy
            Change = 2 //Biển bản điều chỉnh
        }
        public enum MODIFIED_INVOICE_METHOD_TYPE
        {
            INCREASE = 1, //Điều chỉnh tăng
            DECREASE = 2, //Điều chỉnh giảm
            INFORMATION = 3 //Điều chỉnh thông tin
        }
        public enum TAX_TYPE
        {
            NO = - 1, //Không chịu thuế
            ZERO = 0, //Chịu thuế 0%
            FIVE = 5, //Chịu thuế 5%
            TEN = 10, //Chịu thuế 10%

            // Hóa đơn bán hàng
            ONE = 1, //Chịu thuế 1%
            TWO = 2, //Chịu thuế 2%
            THREE = 3 //Chịu thuế 3%
        }

        public enum EMAIL_SERVICE_TYPE
        {
            NOVAON = 1,
            GMAIL = 2
        }

        public enum AccountObjectType
        {
            HOADONGTGT = 0,
            HOADONTRUONGHOC = 1,
            HOADONTIENDIEN = 2,
            HOADONBANHANG = 3,
            HOADONTIENNUOC = 4,
            PHIEUXUATKHO = 5
        }
        public enum InvoiceStatus
        {
            CHUAKYSOTIEPTHEO = 1,
            DAKYSOTIEPTHEO = 2
        }
        public enum UserRole
        {
            // 1. Quản lý hóa đơn
            THEM_MOI_HOA_DON = 1,
            CAP_NHAT_HOA_DON = 2,
            KY = 3,
            XEM_HOA_DON = 4,
            GUI_EMAIL = 5,
            NGHIEP_VU_HOA_DON = 6,
            IMPORT_EXCEL_HOA_DON = 7,

            // 2. Dải hóa đơn chờ
            THEM_DAI_CHO = 8,
            KY_DAI_CHO = 9,

            // 3. Khách hàng
            THEM_MOI_KHACH_HANG = 10,
            CHINH_SUA_KHACH_HANG = 11,
            IMPORT_EXCEL_KHACH_HANG = 12,

            // 4. Hàng hóa, dịch vụ
            THEM_MOI_HANG_HOA = 13,
            CHINH_SUA_HANG_HOA = 14,
            IMPORT_EXCEL_HANG_HOA = 15,

            // 5. Báo cáo, thống kê
            LOC = 16,
            TAI_XUONG = 17,

            // 6. Quản trị người dùng
            CAP_NHAT_THONG_TIN_NGUOI_DUNG = 18,
            PHAN_QUYEN_NGUOI_DUNG = 19,

            // 7. Thông báo phát hành
            THEM_MAU_HOA_DON = 20

        }
    }
}
