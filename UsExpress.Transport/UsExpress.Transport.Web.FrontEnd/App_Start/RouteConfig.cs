using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UsExpress.Transport.Web.FrontEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "Login",
               url: "login",
               defaults: new { controller = "Home", action = "Login" }
           );
            routes.MapRoute(
              name: "Register",
              url: "dang-ky",
              defaults: new { controller = "Home", action = "Register" }
          );
            routes.MapRoute(
              name: "ChangePassword",
              url: "thay-doi-mat-khau",
              defaults: new { controller = "Home", action = "ChangePassword" }
          );
            routes.MapRoute(
             name: "Logout",
             url: "dang-xuat",
             defaults: new { controller = "Home", action = "Logout" }
         );

            routes.MapRoute(
           name: "ListStoreAccount",
           url: "list-store",
           defaults: new { controller = "Store", action = "Index" }
       );

            routes.MapRoute(
         name: "DetailStoreAccount",
         url: "detail-store/{id}",
         defaults: new { controller = "Store", action = "DetailStore", id = UrlParameter.Optional }
     );

            //

            routes.MapRoute(
     name: "updaterecipient",
     url: "Store/UpdateRecipient/{id}",
     defaults: new { controller = "Store", action = "CreateRecipient", id = UrlParameter.Optional }
 );
            //

            routes.MapRoute(
     name: "updatesender",
     url: "Store/UpdateSender/{id}",
     defaults: new { controller = "Store", action = "CreateSender", id = UrlParameter.Optional }
 );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
