using System.Web.Http;
using Unity;
using Unity.WebApi;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Services;

namespace UsExpress.Transport.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ITestService, TestService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}