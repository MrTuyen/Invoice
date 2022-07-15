using DS.BusinessObject.User;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DS.BusinessObject.Account
{
    [Serializable]
    public class AccountBO
    {
        private static AccountBO _instance;

        public static AccountBO Current
        {
            get { return _instance ?? (_instance = new AccountBO()); }
        }

        public AccountBO CurrentUser()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                    return session[ConfigHelper.User] as AccountBO;
                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Không lấy được thông tin user", objEx, "CurrentUser", "AccountModel");
            }
            return null;
        }


        /// <remarks/>
        #region Thông tin cá nhân (thông tin đăng ký)
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string NEWPASSWORD { get; set; }
        public string RENEWPASSWORD { get; set; }
        public string PASSWORDHIDDEN { get; set; }
        public string PASSWORDTEMP { get; set; }
        public string FULLNAME { get; set; }
        public DateTime INITTIME { get; set; }
        public string PHONENUMBER { get; set; }
        public string EMAIL { get; set; }
        public string UTMSOURCE { get; set; }
        public string ADDRESS { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISADMIN { get; set; }
        public int TOTALROW { get; set; }

        #endregion

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
        public string TAXDEPARTEMENT { get; set; }
        public string TAXDEPARTEMENTCODE { get; set; }
        public int MAILSERVICEID { get; set; }
        public string MAILSERVICEACCOUNT { get; set; }
        public string MAILSERVICEPASSWORD { get; set; }

        /// <summary>
        /// Khách hàng sử dụng loại hóa đơn là gì
        /// </summary>
        public int USINGINVOICETYPE { get; set; }

        public string USINGINVOICETYPETMP { get; set; }

        public string PRECOMTAXCODE { get; set; }
        public bool ISFREETRIAL { get; set; }
        //Save Column Excel khi import\

        public string MAPCOLUMNEXCEL { get; set; }
        public bool AUTOSENDMAIL { get; set; }

        // Số lượng số thập phân lấy sau dấu phẩy
        public int QUANTITYPLACE { get; set; }
        public int PRICEPLACE { get; set; }
        public int MONEYPLACE { get; set; }

        // Phân quyền
        public List<int> USERROLES { get; set; }
        // ZaloOA AccessToken
        public string ZALOACCESSTOKEN { get; set; }

        public bool SHOWMENUWAITINGSIGN { get; set; }
    }

    public class RegisterForm
    {
        public string COMTAXCODE { get; set; }
        public string PHONENUMBER { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string ID { get; set; }
        public string UTMSOURCE { get; set; }
        public string FULLNAME { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISADMIN { get; set; }
    }

    public class LoginForm
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string AVATAR { get; set; }
        public string EMAIL { get; set; }
        public string UTMSOURCE { get; set; }
    }

    public class ChangePasswordForm
    {
        public string PASSWORD { get; set; }
        public string NEWPASSWORD { get; set; }
        public string RENEWPASSWORD { get; set; }
    }

    public class UpdateInformationForm
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string AVATAR { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string PHONENUMBER { get; set; }
    }
}
