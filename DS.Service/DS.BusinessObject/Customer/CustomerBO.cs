using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Customer
{
    public enum TYPECHANGE
    {
        Add = 1,
        Edit = 2,
        Copy = 3
    }

    public class CustomerBO
    {
        public int TYPECHANGE { get; set; }
        public Int64 ID { get; set; }
        public string COMUSERNAME { get; set; }
        public string CUSID { get; set; }
        public string COMADDRESS { get; set; }
        public string COMTAXCODE { get; set; }
        public string CUSBUYER { get; set; }
        public string CUSPHONENUMBER { get; set; }
        public string CUSNAME { get; set; }
        public string CUSEMAIL { get; set; }
        public string CUSTAXCODE { get; set; }
        public string CUSWEBSITE { get; set; }
        public string CUSADDRESS { get; set; }
        public string CUSPAYMENTMETHOD { get; set; }
        public string CUSBANKNAME { get; set; }
        public string CUSACCOUNTNUMBER { get; set; }
        public string CUSCURRENCYUNIT { get; set; }
        public string CUSNOTE { get; set; }
        public DateTime INITTIME { get; set; }
        public bool ISDELETED { get; set; }
        public bool ISSELECTED { get; set; }
        public decimal TOTALPAYMENT { get; set; }
        public long TOTALROW { get; set; }
        public int CUSTOMERTYPE { get; set; }

        public decimal TOTALPAYMENTPAID { get; set; }
        public decimal TOTALPAYMENTUNPAID { get; set; }
        public decimal TOTALPAYMENTUNCONFIRMED { get; set; }
        /// <summary>
        /// Phục vụ cho elasticsearch
        /// </summary>
        public string keySearch { get; set; }

        /// <summary>
        /// Mã công tơ điện dùng cho kh sử dụng hóa đơn tiền điện
        /// </summary>
        public string METERCODE { get; set; }

  
    }

    public class TaxAPI
    {
        public string companyName { get; set; }
        public string shortName { get; set; }
        public string taxCode { get; set; }
        public string address { get; set; }
        public string owner { get; set; }
    }

    public class TTDNAPI
    {
        public string code { get; set; }
        public TaxAPI data { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
    public class CURRENCYAPI
    {
        public List<Item> items { get; set; }
    }
    public class Item
    {
        public string type { get; set; }
        public string imageurl { get; set; }
        public string muatienmat { get; set; }
        public string muack { get; set; }
        public string bantienmat { get; set; }
        public string banck { get; set; }
    }
    //lấy thông tin người dùng zalo
    public class DataUserInfo
    {
        public InfoUser data { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }
    public class InfoUser
    {
        public string avatar { get; set; }
        public Avatar avatars { get; set; }
        public string birth_date { get; set; }
        public string display_name { get; set; }
        public Tag tags_and_notes_infor { get; set; }
        public string user_gender { get; set; }
        public string user_id { get; set; }
        public string user_id_by_app { get; set; }
    }
    public class Avatar
    {
        public string a240 { get; set; }
        public string b120 { get; set; }
    }
    public class Tag
    {
        public string note { get; set; }
        public string tag_names { get; set; }
    }
    //lấy mã token để gửi file 
    public class InfoToken
    {
        public string token { get; set; }
    }
    public class DataToken
    {
        public InfoToken data { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }
    //lấy lịch sử gửi tin zalo của từng khách hàng

    public class MSGUser
    {
        public List<DetailMSG> data { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }
    public class DetailMSG
    {
        public string from_avatar { get; set; }
        public string from_display_name { get; set; }
        public long from_id { get; set; }
        public string message_id { get; set; }
        public int src { get; set; }
        public long time { get; set; }
        public string to_avatar { get; set; }
        public string to_display_name { get; set; }
        public long to_id { get; set; }
        public string type { get; set; }
    }
}