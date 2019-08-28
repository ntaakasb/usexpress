using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class SystemController : BaseAdminController
    {
        // GET: System
        private readonly IRoleService _roleSerivce;
        private readonly IMenuService _menuService;
        public SystemController(IUserService userSerivce, IRoleService roleSerivce, IMenuService menuSerivce) : base(null, userSerivce)
        {
            _roleSerivce = roleSerivce;
            _menuService = menuSerivce;
        }
        public ActionResult Index()
        {
            return RedirectToAction("ListMenu");
        }
        #region UserRole
        public ActionResult UserRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetLstUser(int[] SearchCustomerRoleIds, string username)
        {
            var result = new CustomJsonResult();

            return Json(result);
        }

        public ActionResult EditUserRole(long id)
        {
            var user = _userService.GetUserById(id);
            if (user != null)
            {
                ViewBag.User = user;

                ViewBag.LstRole = _userService.GetLstRoleIdByUserId(id);
            }
            return View();
        }
        [HttpPost]
        public ActionResult UpdateRoleForUser(long userId, List<int> lstRole)
        {
            var result = new CustomJsonResult();
            int i = _userService.MapRoleToUser(userId, lstRole);
            if (i == 0)
            {
                result.Message = "Cập nhật role không thành công!";
            }
            return Json(result);
        }
        #endregion

        #region Permission
        public ActionResult Permission()
        {
            var model = new MenuPermissionModel();
            var lstRole = _roleSerivce.GetAllRole();
            foreach (var r in lstRole)
            {
                model.Roles.Add(new BaseItemModel { Id = r.Id, Name = r.Name });
            }
            var lstMenu = _menuService.GetAllMenu();
            var dicMenuPemission = _menuService.GetLstMenuByLstRoleId_Dic(lstRole.Select(x => x.Id).ToList());
            bool allowed = false;
            foreach (var m in lstMenu)
            {
                foreach (var r in lstRole)
                {
                    allowed = false;
                    if (dicMenuPemission.ContainsKey(r.Id))
                    {
                        allowed = dicMenuPemission[r.Id].Contains(m.Id);
                    }
                    if (!model.Allowed.ContainsKey(r.Id))
                    {
                        //model.Allowed.Add(r.Id, new Dictionary<int, bool>());
                        model.Allowed[r.Id] = new Dictionary<int, bool>();
                    }
                    model.Allowed[r.Id][m.Id] = allowed;
                }
                model.Menus.Add(new BaseItemModel { Id = m.Id, Name = $"{m.Controller} / { m.Action }" });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Permission(FormCollection form)
        {
            var result = new CustomJsonResult();
            try
            {
                var lstRole = _roleSerivce.GetAllRole();
                List<int> lstMenuId = new List<int>();
                foreach (var r in lstRole)
                {
                    var formKey = "allow_" + r.Id;
                    var menuPermission = !string.IsNullOrEmpty(form[formKey])
                        ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>();
                    lstMenuId = new List<int>();
                    if (menuPermission.Any())
                    {
                        foreach (var m in menuPermission)
                        {
                            lstMenuId.Add(int.Parse(m));
                        }
                    }
                    _menuService.MapMenuToRole(r.Id, lstMenuId);

                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion

        #region Menu
        public ActionResult ListMenu()
        {
            var lstMenu = _menuService.GetAllMenu().OrderBy(x => x.ParentId).ToList();
            List<MenuModel> lst = new List<MenuModel>();
            for (int i = 0; i < lstMenu.Count; i++)
            {
                if (lstMenu[i].ParentId == 0)
                {
                    MenuModel c = lstMenu[i].Map<MenuModel>();
                    c.LstMenuChild = new List<MenuModel>();
                    for (int j = i + 1; j < lstMenu.Count; j++)
                    {
                        if (lstMenu[j].ParentId == c.Id && lstMenu[j].ParentId != 0)
                        {
                            c.LstMenuChild.Add(lstMenu[j].Map<MenuModel>());
                        }
                    }
                    lst.Add(c);
                }

            }

            return View(lst);
        }
        [HttpPost]
        public ActionResult ManageMenu(MenuModel model)
        {
            var result = new CustomJsonResult();
            try
            {
                if (model.Id > 0)
                {
                    _menuService.UpdateMenu(model.Map<tblMenu>());
                }
                else
                {
                    _menuService.InsertMenu(model.Map<tblMenu>());
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetDetailMenu(int id)
        {
            var result = new CustomJsonResult();
            try
            {
                var lstMenu = _menuService.GetAllMenu().OrderBy(x => x.ParentId).ToList();
                MenuModel model = new MenuModel();
                if (id > 0)
                {
                    var menu = _menuService.GetDetailMenuById(id);
                    if (menu != null)
                    {
                        model = menu.Map<MenuModel>();
                    }
                }
                ViewBag.LstMenuParent = lstMenu;
                result.Result = this.RenderPartialViewToString("_DetailMenuModel", model);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion
    }
}