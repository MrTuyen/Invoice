using SAB.Library.Core.FileService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.Common.Helpers
{
    public class ConfigHelper
    {
        #region -- Static (implement Singleton pattern) --
        public static string User = ConfigurationManager.AppSettings["SessionUser"];
        public static string Partner = ConfigurationManager.AppSettings["SessionPartner"];
        public static string UriInvoiceFolder = ConfigurationManager.AppSettings["UriInvoiceFolder"];
        public static string PhysicalInvoiceFolder = ConfigurationManager.AppSettings["PhysicalInvoiceFolder"];
        public static string AllowSigningNumberoOfInvoice = ConfigurationManager.AppSettings["AllowSigningNumberoOfInvoice"];
        public static string OutputInvoiceFolder = ConfigurationManager.AppSettings["OutputSignedInvoiceFolder"];

        // Hóa đơn nhiều trang
        public static string FirstPage = ConfigurationManager.AppSettings["FirstPage"];
        public static string NPage = ConfigurationManager.AppSettings["NPage"];
        public static string LastPage = ConfigurationManager.AppSettings["LastPage"];
        // End hóa đơn nhiều trang

        // Lấy thông tin doanh nghiệp
        public static string UriEnterpriseInfo = ConfigurationManager.AppSettings["UriEnterpriseInfo"];
        public static string UriEnterpriseInfoSearch = ConfigurationManager.AppSettings["UriEnterpriseInfoSearch"];
        // End lấy thông tin doanh nghiệp

        // Cấu hình đường dẫn Domain SPA_SAInvoice
        public static string UriAppAddress = ConfigurationManager.AppSettings["UriAppAddress"];

        //public static string Customer = ConfigurationManager.AppSettings["SessionCustomer"];
        /// <summary>
        /// The instance
        /// </summary>
        private static volatile ConfigHelper _instance;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private static object _syncRoot = new object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConfigHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        public string GetConnectionStringDS()
        {
            return ConfigurationManager.ConnectionStrings[Constants.CKEY_CONNECTIONDS].ConnectionString;
        }

        public ResultMessageBO WriteLog(string strTitle, Exception objEx, string strEvent, string strModuleName, string strUsername = "system", int locationId = 0)
        {
            SAB.Library.Data.FileLogger.LogAction(objEx);
            return MethodHelper.Instance.FillResultMessage(true, ErrorTypes.Others, strTitle, objEx.ToString());
        }
        public ResultMessageBO WriteLog(string strTitle, string strContent, string strEvent, string strModuleName, string strUsername = "system", int locationId = 0)
        {
            //WriteLog(strTitle, strContent, strEvent, strUsername, intStoreId, strModuleName);
            SAB.Library.Data.FileLogger.LogAction(strTitle, strContent, strEvent, strUsername, locationId);
            return MethodHelper.Instance.FillResultMessage(true, ErrorTypes.Others, strTitle, strContent);
        }
    }
}
