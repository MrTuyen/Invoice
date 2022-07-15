using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SPA_Invoice
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

    
      routes.MapRoute(
        name: "register",
        url: "user-account/register",
        defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional }
      );
      routes.MapRoute(
        name: "inputphonenumber",
        url: "user-account/phonenumber",
        defaults: new { controller = "Account", action = "PhoneNumber", id = UrlParameter.Optional }
      );
      routes.MapRoute(
        name: "login",
        url: "user-account/login",
        defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
      );
      routes.MapRoute(
        name: "logout",
        url: "user-account/logout",
        defaults: new { controller = "Account", action = "Logout", id = UrlParameter.Optional }
      );
      routes.MapRoute(
        name: "quick-login",
        url: "user-account/quick-login",
        defaults: new { controller = "Account", action = "QuickLogin", id = UrlParameter.Optional }
      );
      routes.MapRoute(
        name: "forgot-password",
        url: "user-account/forgot-password",
        defaults: new { controller = "Account", action = "ForgotPassword", id = UrlParameter.Optional }
      );
      routes.MapRoute(
       name: "resend-email",
       url: "user-account/resend-email",
       defaults: new { controller = "Account", action = "ReSendEmail", id = UrlParameter.Optional }
     );
      routes.MapRoute(
        name: "change-password",
        url: "user-account/change-password",
        defaults: new { controller = "Account", action = "ChangePassword", id = UrlParameter.Optional }
      );

      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
      //routes.MapRoute(
      //  name: "Login",
      //  url: "Account/{action}",
      //  defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
      //);
    }
  }
}
