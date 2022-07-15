using DS.BusinessObject.Account;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TaxReport
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                string strUrl = HttpContext.Current.Request.Url.ToString();
                string rawUrl = HttpContext.Current.Request.RawUrl;
                string strUserAgent = HttpContext.Current.Request.UserAgent;


                if (!string.IsNullOrEmpty(strUserAgent))
                {
                    if (Session[ConfigHelper.User] != null)
                    {
                        AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
                        //Do anything

                    }
                    else if (!strUrl.Contains("Account"))
                    {
                        Response.Redirect("~/Account/Login");
                    }
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Application_AcquireRequestState", ex, "Application_AcquireRequestState", "Global.asax");
            }
        }
    }
}
