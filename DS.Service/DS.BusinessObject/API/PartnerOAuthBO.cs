using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DS.BusinessObject.API
{
    public class PartnerOAuthBO
    {
        private static PartnerOAuthBO _instance;

        public static PartnerOAuthBO Current
        {
            get { return _instance ?? (_instance = new PartnerOAuthBO()); }
        }

        public PartnerOAuthBO CurrentPartner()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                    return session[ConfigHelper.Partner] as PartnerOAuthBO;
                }
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Không lấy được thông tin đối tác", objEx, "CurrentPartner", "PartnerOAuthBO");
            }
            return null;
        }

        public long ID { get; set; }
        public string PARTNERID { get; set; }
        public string PARTNERNAME { get; set; }
        public bool ISACTIVED { get; set; }
        public bool ISDELETED { get; set; }
        public DateTime INITTIME { get; set; }
        public string TOKEN { get; set; }
        public string CREATEDBYUSER { get; set; }
    }
}

