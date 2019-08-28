using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Web.FrontEnd.Common;

namespace UsExpress.Transport.Web.FrontEnd.Filters
{
    public class AuthAttribute : AuthorizeAttribute
    {
        [Unity.Attributes.Dependency]
        public IUserService _userService { get; set; }
        [Unity.Attributes.Dependency]
        public IMenuService _menuService { get; set; }
        [Unity.Attributes.Dependency]
        public IStoreServices _storeServices { get; set; }

        private List<string> _lstActionNoneCheck = new List<string> { "login", "logout", "register", "changepassword" };
        private List<string> _lstControllerNoneCheck = new List<string> { "common" };
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                string action = filterContext.RouteData.Values["action"].ToString().ToLower();
                string returnUrl = filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri;


                if (!((controller.Equals("home") && _lstActionNoneCheck.Contains(action)) || _lstControllerNoneCheck.Contains(controller)))
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
                        if (user != null && user.IsActive)
                        {

                            var isAllowAccess = false;
                            bool IsStore = false; //Kiem tra accout login co phai store ko
                            bool IsAdmin = false;
                            bool IsCSKH = false;
                            var lstRoleId = _userService.GetLstRoleIdByUserId(user.Id);
                            HttpContext.Current.Session[Constant.SessionUserRole] = string.Join(",", lstRoleId);

                            if (lstRoleId != null && lstRoleId.Any())
                            {
                                if (lstRoleId.Contains(RoleEnum.Admin))
                                {
                                    IsAdmin = true;
                                    isAllowAccess = true;
                                }
                                if (lstRoleId.Contains(RoleEnum.Store))
                                {
                                    IsStore = true;
                                      
                                }
                                if (lstRoleId.Contains(RoleEnum.SupplierEmployee))
                                {
                                    IsCSKH = true;
                                }
                                //
                                if (lstRoleId.Contains(RoleEnum.Root))
                                {
                                    isAllowAccess = true;
                                    IsAdmin = true;
                                }

                                else
                                {
                                    var lstMenu = _menuService.GetLstMenuByLstRoleId(lstRoleId);
                                    if (lstMenu != null && lstMenu.Any())
                                    {
                                        Lib.Business.Models.DBContext.UsTransport.tblMenu menu = null;
                                        if (action == "index")
                                        {
                                            menu = lstMenu.FirstOrDefault(x => (!string.IsNullOrEmpty(x.Controller)
                                                                                      && x.Controller.ToLower().Equals(controller)));
                                        }
                                        else
                                        {
                                            menu = lstMenu.FirstOrDefault(x => (!string.IsNullOrEmpty(x.Controller) && !string.IsNullOrEmpty(x.Action)
                                                                                      && x.Controller.ToLower().Equals(controller)
                                                                                      && x.Action.ToLower().Equals(action)));
                                        }
                                        if (menu != null && menu.IsActive)
                                        {
                                            isAllowAccess = true;
                                        }

                                    }
                                }

                            }
                            if (isAllowAccess)
                            {
                                HttpContext.Current.Session[Constant.SessionUsername] = username;
                                HttpContext.Current.Session[Constant.SessionUserID] = user.Id;
                                if (IsStore)
                                {
                                    Lib.Business.Models.DBContext.UsTransport.tblStoreAccount storeOfUser = _storeServices.SelectStoreByUserName(username.ToString());
                                    if (storeOfUser != null)
                                    {
                                        HttpContext.Current.Session[Constant.SessionStoreID] = storeOfUser.id;
                                        HttpContext.Current.Session[Constant.SessionStoreName] = storeOfUser.StoreName;
                                    }
                                }
                                HttpContext.Current.Session[Constant.SessionIsAdmin] = IsAdmin;
                                HttpContext.Current.Session[Constant.SessionIsCSKH] = IsCSKH;
                            }
                            else
                            {
                                filterContext.Result = new HttpStatusCodeResult(403);
                                throw new HttpException(403, "Access Denied");
                            }
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult("~/Home/Login");
                        }
                    }
                }
            }
        }
    }
}