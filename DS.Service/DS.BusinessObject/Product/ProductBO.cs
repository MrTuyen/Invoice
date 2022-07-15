using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Product
{
    public class ProductBO
    {
        public long ID { get; set; }
        public string COMUSERNAME { get; set; }
        public string COMNAME { get; set; }
        public string COMADDRESS { get; set; }
        public string COMTAXCODE { get; set; }
        public int PRODUCTTYPE { get; set; }
        public string PRODUCTTYPENAME { get; set; }
        public string SKU { get; set; }
        public string PRODUCTNAME { get; set; }
        /// <summary>
        /// PRODUCTNAME_UNICODE phục vụ Elasticsearch
        /// </summary>
        public string PRODUCTNAME_UNICODE { get; set; }
        public string CATEGORY { get; set; }
        public string CATEGORYNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal PRICE { get; set; }
        public string QUANTITYUNIT { get; set; }
        public string IMAGE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISSELECTED { get; set; }
        public string NOTE { get; set; }
        public long TOTALROW { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string PartnerID { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Mã số công tơ điện (Dùng cho kh sử dụng hóa đơn tiền điện)
        /// </summary>
        public string METERCODE { get; set; }

        /// <summary>
        /// Tên công tơ
        /// </summary>
        public string METERNAME { get; set; }

        /// <summary>
        /// Hệ số nhân
        /// </summary>
        public int FACTOR { get; set; }

        /// <summary>
        /// mức
        /// </summary>
        public int LEVEL { get; set; }

        /// <summary>
        /// Từ  mức
        /// </summary>
        public int FROMLEVEL { get; set; }

        /// <summary>
        /// Đến mức
        /// </summary>
        public int TOLEVEL { get; set; }

        public int TEMPAPARTNO { get; set; } = 0;

        public int GROUPID { get; set; }
    }

    public class ProductModel
    {
        public long ID { get; set; }
        public string COMUSERNAME { get; set; }
        public string COMNAME { get; set; }
        public string COMADDRESS { get; set; }
        public string COMTAXCODE { get; set; }
        public int PRODUCTTYPE { get; set; }
        public string PRODUCTTYPENAME { get; set; }
        public string SKU { get; set; }
        public string PRODUCTNAME { get; set; }
        /// <summary>
        /// PRODUCTNAME_UNICODE phục vụ Elasticsearch
        /// </summary>
        public string PRODUCTNAME_UNICODE { get; set; }
        public string CATEGORY { get; set; }
        public string CATEGORYNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal PRICE { get; set; }
        public string QUANTITYUNIT { get; set; }
        public string IMAGE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISSELECTED { get; set; }
        public string NOTE { get; set; }
        public long TOTALROW { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string PartnerID { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Mã số công tơ điện (Dùng cho kh sử dụng hóa đơn tiền điện)
        /// </summary>
        public string METERCODE { get; set; }

        /// <summary>
        /// Tên công tơ
        /// </summary>
        public string METERNAME { get; set; }

        /// <summary>
        /// Hệ số nhân
        /// </summary>
        public int FACTOR { get; set; }

        /// <summary>
        /// mức
        /// </summary>
        public int LEVEL { get; set; }

        /// <summary>
        /// Từ  mức
        /// </summary>
        public int FROMLEVEL { get; set; }

        /// <summary>
        /// Đến mức
        /// </summary>
        public int TOLEVEL { get; set; }

        public int TEMPAPARTNO { get; set; } = 0;
    }

    public class ProductModelLevel
    {
        public long ID { get; set; }
        public string COMUSERNAME { get; set; }
        public string COMNAME { get; set; }
        public string COMADDRESS { get; set; }
        public string COMTAXCODE { get; set; }
        public int PRODUCTTYPE { get; set; }
        public string PRODUCTTYPENAME { get; set; }
        public string SKU { get; set; }
        public string PRODUCTNAME { get; set; }
        /// <summary>
        /// PRODUCTNAME_UNICODE phục vụ Elasticsearch
        /// </summary>
        public string PRODUCTNAME_UNICODE { get; set; }
        public string CATEGORY { get; set; }
        public string CATEGORYNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal PRICE { get; set; }
        public string QUANTITYUNIT { get; set; }
        public string IMAGE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISSELECTED { get; set; }
        public string NOTE { get; set; }
        public long TOTALROW { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string PartnerID { get; set; }

        /// <summary>
        /// Cột này không có trong database của onfinance thêm vào để kiểm tra trên api cung câp cho đối tác
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Mã số công tơ điện (Dùng cho kh sử dụng hóa đơn tiền điện)
        /// </summary>
        public string METERCODE { get; set; }

        /// <summary>
        /// Tên công tơ
        /// </summary>
        public string METERNAME { get; set; }

        /// <summary>
        /// Hệ số nhân
        /// </summary>
        public int FACTOR { get; set; }

        /// <summary>
        /// mức
        /// </summary>
        public int LEVEL { get; set; }

        /// <summary>
        /// Từ  mức
        /// </summary>
        public int FROMLEVEL { get; set; }

        /// <summary>
        /// Đến mức
        /// </summary>
        public int TOLEVEL { get; set; }

        public int TEMPAPARTNO { get; set; } = 0;
    }

    public class SuggestModel
    {
        public string IDObj { get; set; }
        public string LabelObj { get; set; }
        public Int64 NumberObj { get; set; }
        public Int64 DecimalObj { get; set; }
        public string DescriptionObj { get; set; }
        public string ImageObj { get; set; }
        public string TypeObj { get; set; }
    }
}
