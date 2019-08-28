using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business;
using BSNC = UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Common;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class HomeController : BaseAdminController
    {

        private readonly ITestService testService;
        public HomeController(ITestService testService, IUserService userservice, IStoreServices storeServices) : base(storeServices, userservice)
        {
            this.testService = testService;
        }
        // GET: Home
        [Filters.Auth]
        public ActionResult Index()
        {
            StoreModel storeModel = new StoreModel();
            var storeInfo = GetStoreAccountInfo();
            if (storeInfo.id > 0)
            {
                return RedirectToAction("CreateOrder", "Order");
            }
            else
            {
                return RedirectToAction("Manager", "Shipment");
            }
        }

        #region User Area
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            var result = new Models.CustomJsonResult();
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var userId = _userService.Login(model.LoginUsername, model.LoginPassword);
                if (userId > 0)
                {
                    result.Result = userId;
                    Session[Constant.SessionUsername] = model.LoginUsername;

                }
                else
                {
                    result.Result = 0;
                    result.Message = "Password is correct or account is not active, please contact administrator!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Filters.Auth]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = new Models.CustomJsonResult();
            try
            {
                var userId = _userService.CreateUser(model.Username, model.Password, model.FullName, false);
                if (userId > 0)
                {
                    result.Result = userId;
                    _userService.MapRoleToUser(userId, new List<int> { RoleEnum.UserVN });
                    result.Message = LabelResources.Language.lb_RegisterSucceed;
                }
                else
                {
                    result.Result = 0;
                    result.Message = LabelResources.Language.lb_RegisterFailed ;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [Filters.Auth]
        public ActionResult ChangePassword()
        {
            if (Session[Constant.SessionUsername] != null)
            {
                return View();
            }
            else
            {
                return Redirect("/Home/Login");
            }
        }

        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    var result = new Models.CustomJsonResult();
        //    try
        //    {
        //        string username = GetUserNameFromSession();
        //        var checkUser = _userService.GetUserByUsername(username);
        //        if (checkUser != null && checkUser.Id > 0)
        //        {
        //            if(!Utils.HashMD5(model.CurrentPassword).Equals(checkUser.Password))
        //            {
        //                result.Result = 0;
        //                result.Message = "Mật khẩu hiện tại không đúng!";
        //            }
        //            else
        //            {
        //                var changeUer = _userService.ChangePass(checkUser.Id, model.CurrentPassword, model.Password);
        //                if (changeUer)
        //                {
        //                    result.Result = checkUser.Id;
        //                    result.Message = "Thay đổi mật khẩu thành công!";
        //                }
        //                else
        //                {
        //                    result.Result = 0;
        //                    result.Message = "Thay đổi mật khẩu thất bại!";
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var result = new Models.CustomJsonResult
            {
                Result = 0,
                Message = "Thay đổi mật khẩu thất bại!"
        };
            try
            {
                string username = GetUserNameFromSession();
                long userid = 0;
                var rs = _userService.ChangePassUser(username, model.CurrentPassword, model.Password, out userid);
                switch (rs)
                {
                    case (int)BSNC.StatusChangePass.Faile:
                        result.Result = 0;
                        result.Message = "Change password fail!";
                        break;
                    case (int)BSNC.StatusChangePass.WrongPassword:
                        result.Result = 0;
                        result.Message = "Curent password is correct!";
                        break;
                    case (int)BSNC.StatusChangePass.IsChange:
                        result.Result = userid;
                        result.Message = "Change password succesful!";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("index", "Home");
        }
        #endregion

        public PartialViewResult _HeaderPage()
        {
            return PartialView();
        }
        [Filters.Auth]
        public ActionResult ClearAll(string token = "")
        {
            if (token != "")
            {
                foreach (var item in MemoryCache.Default)
                {
                    MemoryCache.Default.Remove(item.Key);
                }
                Response.Write("Cache has been clear !!!");
            }
            else
            {
                Response.Write("Must be inputed token !");
            }
            return null;
        }


        public ActionResult ForgotPass()
        {
            return View();
        }

        public ActionResult SendLinks(string LoginUsername)
        {
            var url = Request.Url.Scheme + "://" + Request.Url.Host;
            var rs = _userService.SendLinks(LoginUsername, url);
            return Json(rs);
        }

        [HttpGet]
        public ActionResult ResetPass(string id)
        {
            if (!_userService.CheckKey(id))
            {
                return RedirectToAction("ForgotPass");
            }
            Session["Key"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPass(RetsetPassModel obj)
        {
            var key = Session["key"] + "";
            var rs = _userService.ResetPass(key, obj.Password);
            return Json(rs);
        }
    }
}