using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Common;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class StoreNoLoginController : Controller
    {
        // GET: StoreNoLogin
        private readonly IUserService userService;
        private readonly IStoreServices storeService;
        private readonly ILocationServices locationService;
        private readonly IRoleService roleService;

        public StoreNoLoginController(
            IUserService userService,
            IStoreServices storeService,
            ILocationServices locationService,
            IRoleService roleService)
        {
            this.userService = userService;
            this.storeService = storeService;
            this.locationService = locationService;
            this.roleService = roleService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RegisterStore()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertStore(StoreModel storeModel)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                string notencodepassword = storeModel.StoreAccount.Password;
                storeModel.StoreAccount.Password = Utils.HashMD5(storeModel.StoreAccount.Password);
                storeModel.StoreAccount.AliasFullName = Libs.UnicodeToNoneMark(storeModel.StoreAccount.StoreName);
                result.Result = storeService.InsertStore(storeModel.StoreAccount);
                if (int.Parse(result.Result.ToString()) > 0)
                {
                    long UserID = userService.CreateUser(storeModel.StoreAccount.Email, notencodepassword, storeModel.StoreAccount.FullName, false);
                    if (UserID > 0)
                    {
                        roleService.UpdateRoleUser(UserID, RoleEnum.Store);
                        result.Message = "Create New Store Succeed";
                    }
                    else
                        result.Result = UserID;

                }

                if (int.Parse(result.Result.ToString()) == -1)
                {
                    result.Message = "This store account already exists on system";
                }
                else if (int.Parse(result.Result.ToString()) < 1)
                {
                    result.Message = "Has error, please try again!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult IsExistsUserName(string UserName, int id)
        {
            bool result = false;
            try
            {
                result = userService.CheckExistEmailByUserID(UserName.Trim(), id);


            }
            catch (Exception ex)
            {
                string ms = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}