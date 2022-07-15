using System;
using System.Configuration;

namespace SynctaskInvoicePMSmat.Common
{
    public static class ConfigHelper
    {
        public static int setIntervalTime = Convert.ToInt32(ConfigurationManager.AppSettings["SetIntervalTime"]);
        public static string LogPath = ConfigurationManager.AppSettings["LogPath"];
        public static string NOVAON_CacheFilePath = ConfigurationManager.AppSettings["NOVAON_CacheFilePath"];
        public static string NOVAON_AppId = ConfigurationManager.AppSettings["NOVAON_AppId"];

        public static string NOVAON_GetTokenUrl = ConfigurationManager.AppSettings["NOVAON_GetTokenUrl"];
        public static string InvoiceApiUrl = ConfigurationManager.AppSettings["NOVAON_InvoiceApiUrl"];
        public static string OrgComtaxcode = ConfigurationManager.AppSettings["NOVAON_OrgComtaxcode"];
        public static string OrgComName = ConfigurationManager.AppSettings["NOVAON_OrgComName"];
        public static string OrgComAddress = ConfigurationManager.AppSettings["NOVAON_OrgAddress"];
        public static int OrgUsingInvoiceType = Convert.ToInt32(ConfigurationManager.AppSettings["NOVAON_OrgUsingInvoiceType"]);
        public static int OrgTaxRate = Convert.ToInt32(ConfigurationManager.AppSettings["NOVAON_OrgTaxRate"]);
        public static string OrgFormCode = ConfigurationManager.AppSettings["NOVAON_OrgFormCode"];
        public static string OrgSymbolCode = ConfigurationManager.AppSettings["NOVAON_OrgSymbolCode"];

        public static string SetDateTime = ConfigurationManager.AppSettings["SetDateTime"];
        public static string SetDateTimeValue = ConfigurationManager.AppSettings["SetDateTimeValue"];
    }
}
