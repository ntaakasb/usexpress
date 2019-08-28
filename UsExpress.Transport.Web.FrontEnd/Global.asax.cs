using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Web.FrontEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            var oldProvider = FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider);
            FilterProviders.Providers.Remove(oldProvider);

            var container = UnityConfig.GetConfiguredContainer();
            //var provider = new UnityFilterAttributeFilterProvider(container);
            //container.RegisterInstance<IFilterProvider>(provider, "attributes");
            var provider = new UnityFilterAttributeFilterProvider(container);
            FilterProviders.Providers.Add(provider);
            UnityConfig.RegisterComponents();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception ex = Server.GetLastError();
                Libraries.Log.Write(ex.Message);
            }
            catch { }
        }
    }
}
