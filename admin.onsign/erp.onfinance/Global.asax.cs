using DS.BusinessObject.Account;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace erp.onfinance
{
    public class MvcApplication : HttpApplication
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
                string strUserAgent = HttpContext.Current.Request.UserAgent;


                if (!string.IsNullOrEmpty(strUserAgent))
                {
                    if (Session[ConfigHelper.User] != null)
                    {
                        AccountBO objUser = (AccountBO)Session[ConfigHelper.User];
                    }
                    else if (!strUrl.Contains("Account") && !strUrl.Contains("Login") && !strUrl.Contains("DeleteCurrentUser") &&
                             !strUrl.Contains("RefreshSession") && !strUrl.Contains("AddErrorLog") && !strUrl.Contains("UnauthorisedRequest") && !strUrl.Contains("Contact"))
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

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var exception = Server.GetLastError();
        //    var httpContext = ((HttpApplication)sender).Context;
        //    httpContext.Response.Clear();
        //    httpContext.ClearError();

        //    if (new HttpRequestWrapper(httpContext.Request).IsAjaxRequest())
        //    {
        //        return;
        //    }

        //    ExecuteErrorController(httpContext, exception as HttpException);
        //}

        //private void ExecuteErrorController(HttpContext httpContext, HttpException exception)
        //{
        //    var routeData = new RouteData();
        //    routeData.Values["controller"] = "Error";

        //    if (exception != null && exception.GetHttpCode() == (int)HttpStatusCode.NotFound)
        //    {
        //        routeData.Values["action"] = "NotFound";
        //    }
        //    else
        //    {
        //        routeData.Values["action"] = "InternalServerError";
        //    }

        //    using (Controller controller = new ErrorController())
        //    {
        //        ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        //    }
        //}
    }
}
