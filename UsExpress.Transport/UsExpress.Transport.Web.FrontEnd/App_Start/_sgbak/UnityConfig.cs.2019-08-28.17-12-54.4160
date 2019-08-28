using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using UsExpress.Transport.Lib.Business;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Services;

namespace UsExpress.Transport.Web.FrontEnd
{
    public class UnityConfig
    {
        #region Unity Container
        private static System.Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion
        public static void RegisterComponents()
        {
            var container = GetConfiguredContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            //container.RegisterType<ITestService, TestService>();
            //container.RegisterType<IUserService, UserService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
        public static void RegisterTypes(IUnityContainer container)
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ITestService, TestService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IStoreServices, StoreServices>();
            container.RegisterType<ILocationServices, LocationServices>();
            container.RegisterType<ICatelogyService, CategoryService>();
            container.RegisterType<IOrderService, OrderService>();
        }
    }
    /// <summary>
    /// http://bradwilson.typepad.com/blog/2010/07/service-location-pt4-filters.html
    /// </summary>
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private IUnityContainer _container;

        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
                    ControllerContext controllerContext,
                    ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetControllerAttributes(controllerContext,
                                                          actionDescriptor);
            foreach (var attribute in attributes)
            {
                _container.BuildUp(attribute.GetType(), attribute);
            }

            return attributes;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(
                    ControllerContext controllerContext,
                    ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetActionAttributes(controllerContext,
                                                      actionDescriptor);
            foreach (var attribute in attributes)
            {
                _container.BuildUp(attribute.GetType(), attribute);
            }

            return attributes;
        }
    }
}
