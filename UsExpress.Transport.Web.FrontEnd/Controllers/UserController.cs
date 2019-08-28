using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Web.FrontEnd.Common;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleServices;
        public UserController(IStoreServices storeService, IUserService userService, IRoleService roleServices) : base(storeService, userService)
        {
            _userService = userService;
            _roleServices = roleServices;
        }
        // GET: User
        [Filters.Auth]
        public ActionResult Index()
        {
            return RedirectToAction("Manage");
        }
        [Filters.Auth]
        public ActionResult Manage(int? page)
        {
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _userService.SearchUser(pageIndex, pageSize, null, -1);
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            return View(result);
        }
        [Filters.Auth]
        public ActionResult CreateUser(int? id)
        {
            tblUser user = new tblUser();
            if (id != null)
            {
                user = _userService.GetUserById(long.Parse(id.ToString()));
                ViewBag.RoleID = _userService.GetRoleByUserID(long.Parse(id.ToString()));
            }
            return View(user);
        }
        [Filters.Auth]
        public ActionResult MyAccount()
        {
            return View();
        }

        public PartialViewResult _GridEmployees(PagedList.IPagedList<EmployeesDTO> lsModel, int pageIndex)
        {
            ViewBag.Index = pageIndex;
            return PartialView(lsModel);
        }

        public JsonResult AJXCreateUser(tblUser model)
        {
            var result = new Models.CustomJsonResult();
            model.IsActive = Request.Form["IsActive"].ToString().Equals("1") ? true : false;
            if (model.Id > 0)
            {
                result.Result = _userService.UpdateUser(model);
                if (int.Parse(result.Result.ToString()) > 0)
                {
                    result.Message = "Update user success!";
                    if (Request.Form["slRole"] != null)
                    {
                        _roleServices.UpdateRoleUser(long.Parse(result.Result.ToString()), int.Parse(Request.Form["slRole"]));
                    }
                }

            }
            else
            {
                result.Result = _userService.RegisUser(model);
                if (int.Parse(result.Result.ToString()) > 0)
                {
                    result.Message = "Create user success!";
                    if (Request.Form["slRole"] != null)
                    {
                        _roleServices.UpdateRoleUser(long.Parse(result.Result.ToString()), int.Parse(Request.Form["slRole"]));
                    }
                }
            }
            if(int.Parse(result.Result.ToString()) == -1)
            {
                result.Message = "This account already exists on system";
            }
            else if (int.Parse(result.Result.ToString()) < 1)
            {
                result.Message = "Had error, please try again!";
            }
            return Json(result);
        }

        public JsonResult AJXSearchUser()
        {
            string keyword = Request.Form["SearchKeyword"];
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _userService.SearchUser(1, pageSize, keyword, searchType);
            return Json(RenderPartialViewToString("_GridEmployees", result), JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetRoleUser(int userID)
        {
            string role = string.Empty;
            var result = _userService.GetLstRoleIdByUserId(userID);
            if (result != null && result.Any())
            {
                int i = 0;
                foreach(var item in result)
                {
                   switch(item)
                    {
                        case 1:
                            role += " Root";
                            break;
                        case 2:
                            role += " Admin";
                            break;
                        case 3:
                            role += " Store";
                            break;
                        case 4:
                            role += " User VN";
                            break;
                        case 5:
                            role += " User USA";
                            break;
                        case 6:
                            role += " SupplierEmployee";
                            break;
                            
                    }
                }
            }
            return Content(role);
        }

        public static string GetDescription(RoleEnum role)
        {
            System.Reflection.FieldInfo oFieldInfo = role.GetType().GetField(role.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])oFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return role.ToString();
            }
        }

    }
}