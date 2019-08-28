using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class MenuController : BaseAdminController
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService, IUserService userService, IStoreServices storeService) : base(storeService, userService)
        {
            _menuService = menuService;
        }
        // GET: Menu
        [ChildActionOnly]
        public ActionResult MenuTop()
        {
            List<MenuModel> lst = new List<MenuModel>();
            try
            {
                var user = _userService.GetUserByUsername(GetUserNameFromSession());
                if (user != null && user.IsActive)
                {
                    var lstRoleId = _userService.GetLstRoleIdByUserId(user.Id);
                    if (lstRoleId != null && lstRoleId.Any())
                    {
                        if (lstRoleId.Contains(Common.RoleEnum.Root))
                        {
                            lst = _menuService.GetAllMenu().Where(x => x.ParentId == 0).Select(x => x.Map<MenuModel>()).ToList();
                        }
                        else
                        {
                            var lstMenu = _menuService.GetLstMenuByLstRoleId(lstRoleId);
                            if (lstMenu != null && lstMenu.Any())
                            {
                                lst = lstMenu.Where(x => x.ParentId == 0).Select(x => x.Map<MenuModel>()).ToList();
                            }

                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return View(lst);
        }

    }
}