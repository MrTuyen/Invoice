using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Company
{
    public class CompanyBO
    {
        public int? ID { get; set; }
        public string USERNAME { get; set; }
        public string COMNAME { get; set; }
        public string COMLEGALNAME { get; set; }
        public string COMTAXCODE { get; set; }
        public string COMSIZE { get; set; }
        public string COMACTIVITY { get; set; }
        public string COMEMAIL { get; set; }
        public string COMPHONENUMBER { get; set; }
        public string COMWEBSITE { get; set; }
        public string COMACCOUNTNUMBER { get; set; }
        public string COMBANKNAME { get; set; }
        public string COMADDRESS { get; set; }
        public string COMLOGO { get; set; }
        public int MAILSERVICEID { get; set; }
        public string MAILSERVICEACCOUNT { get; set; }
        public string MAILSERVICEPASSWORD { get; set; }
        public bool AUTOSENDMAIL { get; set; }
        public bool ISDELETED { get; set; }
        public long TOTALROW { get; set; }
        public string TAXDEPARTEMENT { get; set; }
        public string TAXDEPARTEMENTCODE { get; set; }

        public int USINGINVOICETYPE { get; set; } = 0;

        public string USINGINVOICETYPETMP { get; set; }

        public string[] ARRUSINGTYPETMP { get; set; }

        public bool ISFREETRIAL { get; set; }

        #region Hỗ trợ làm tròn
        //Số lượng làm tròn
        public int QUANTITYPLACE { get; set; }
        // đơn giá làm tròn
        public int PRICEPLACE { get; set; }
        //thành tiền làm tròn
        public int MONEYPLACE { get; set; }
        #endregion

        //Zalo accesstoken 
        public string ZALOACCESSTOKEN { get; set; }

        public bool SHOWMENUWAITINGSIGN { get; set; }

        public int EMAILTEMPLATEID { get; set; }

        public string EMAILTEMPLATECONTENT { get; set; }

    }


    public class CompanySearchFormBO
    {
        public string COMTAXCODE { get; set; }
        public string USERNAME { get; set; }
        public string KEYWORD { get; set; }
        public int? ITEMPERPAGE { get; set; }
        public int? OFFSET { get; set; }
    }
}
