using DS.BusinessLogic.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace erp.onfinance.Common
{
    public class SABAuthorize : AuthorizeAttribute
    {
        public int Key { get; set; }
        public SABAuthorize(int key)
        {
            Key = key;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!IsPermission(Key))
            {
                filterContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(
                           new
                           {
                               controller = "Common",
                               action = "NotRight"
                           })
                       );
            }
        }

        public static bool IsPermission(int key)
        {
            //AccountModel objUser = AccountModel.Current.CurrentUser();
            //AccountBLL objAccountBLL = new AccountBLL();
            //bool isAuth = objAccountBLL.IsPermission(objUser.USERNAME, key);
            //return isAuth;
            return true;
        }
    }
}