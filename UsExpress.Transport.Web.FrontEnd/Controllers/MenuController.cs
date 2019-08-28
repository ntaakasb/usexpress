using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
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
            return View(GetLstMenu());
        }
        public List<MenuModel> GetLstMenu()
        {
            List<MenuModel> lst = new List<MenuModel>();

            try
            {
                var user = _userService.GetUserByUsername(GetUserNameFromSession());
                if (user != null && user.IsActive)
                {
                    var lstRoleId = _userService.GetLstRoleIdByUserId(user.Id);
                    List<MenuModel> lstMenu = new List<MenuModel>();
                    if (lstRoleId != null && lstRoleId.Any())
                    {
                        if (lstRoleId.Contains(Common.RoleEnum.Root))
                        {
                            lstMenu = _menuService.GetAllMenu().Select(x => x.Map<MenuModel>()).ToList();
                            lst = lstMenu.Where(x => x.ParentId == 0).ToList();
                        }
                        else
                        {
                            lstMenu = _menuService.GetLstMenuByLstRoleId(lstRoleId).Select(x => x.Map<MenuModel>()).ToList();
                            if (lstMenu != null && lstMenu.Any())
                            {
                                lst = lstMenu.Where(x => x.ParentId == 0 && x.IsShow == true).OrderBy(x => x.DisplayOrder).Select(x => x.Map<MenuModel>()).ToList();
                            }
                        }
                        foreach (var item in lst)
                        {
                            item.LstMenuChild = lstMenu.Where(x => x.ParentId == item.Id && x.IsShow == true && x.IsActive == true).OrderBy(x => x.DisplayOrder).Select(x => x).ToList();
                        }

                    }
                }


            }
            catch (Exception)
            {
            }
            return lst;
        }
        [ChildActionOnly]
        public ActionResult MenuMobile()
        {

            return View(GetLstMenu());
        }


    }
}