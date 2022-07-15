using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DS.SearchInvoice
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "review-index",
                url: "tracuu",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "search-code",
                url: "tracuu/search_by_code",
                defaults: new { controller = "Home", action = "SearchCode", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "convert-invoice",
                url: "tracuu/convert_invoice",
                defaults: new { controller = "Home", action = "ConvertInvoice", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "search-file",
                url: "tracuu/search_by_file",
                defaults: new { controller = "Home", action = "SearchFile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "download-file",
                url: "tracuu/downloadfile",
                defaults: new { controller = "Home", action = "Downloadfile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
