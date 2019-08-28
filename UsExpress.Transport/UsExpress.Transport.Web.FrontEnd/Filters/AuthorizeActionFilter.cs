using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Web.FrontEnd.Common;

namespace UsExpress.Transport.Web.FrontEnd.Filters
{

    public class AuthorizeActionFilter : ActionFilterAttribute
    {
        private readonly long[] _roles;
        [Unity.Attributes.Dependency]
        public IUserService _userService { get; set; }
        public AuthorizeActionFilter(params long[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                
                var username = HttpContext.Current.Session[Constant.SessionUsername];
                if (username == null)
                {
                    // Redirect to Login Page
                    FormsAuthentication.SignOut();

                    HttpContext.Current.Session[Constant.SessionPreviousUrl] = filterContext.HttpContext.Request.Url;

                    filterContext.Result = new RedirectResult("~/Home/Login");

                }
                else //nếu đang còn session
                {
                    var user = _userService.GetUserByUsername(username.ToString());
                    var isAllowAccess = true;
                    var action = Action();
                    var controller = Controller();

                    //if (_roles != null && _roles.Length > 0)
                    //{
                    //    isAllowAccess = _roles.Contains(username.RoleId);
                    //}

                    //var menuAll =
                    //    MvcApplication.AllMenuGlobal.FirstOrDefault(
                    //        m => action.Equals(m.Action) && controller.Equals(m.Controller));

                    //var menuP =
                    //    MvcApplication.MenuGlobal.FirstOrDefault(
                    //        m => action.Equals(m.Action) && controller.Equals(m.Controller));

                    //if (menuP == null && menuAll != null)
                    //{
                    //    filterContext.Result = new HttpStatusCodeResult(403);
                    //    throw new HttpException(403, "Access Denied");
                    //}

                    //if (isAllowAccess)
                    //{
                    //    HttpContext.Current.Session[Constant.USER] = username.Id;
                    //}
                    //else
                    //{
                    //    filterContext.Result = new HttpStatusCodeResult(403);
                    //    throw new HttpException(403, "Access Denied");
                    //}
                }
            }
        }

        public static string Controller()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }


    }
}