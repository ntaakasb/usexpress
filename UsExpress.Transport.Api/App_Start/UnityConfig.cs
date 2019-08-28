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
            container.RegisterType<IStoreServices, StoreServices>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IAppService, AppService>();
            container.RegisterType<IKerryService, KerryService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}