using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business;
using UsExpress.Transport.Lib.Business.Interfaces;
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
        public ActionResult Index()
        {
            return View(testService.GetAll());
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
                    Lib.Business.Models.DBContext.UsTransport.tblStoreAccount storeOfUser = _storeService.SelectStoreByUserName(model.LoginUsername);
                    Session[Constant.SessionStoreID] = storeOfUser.id;
                }
                else
                {
                    result.Result = 0;
                    result.Message = "Tài khoản không đúng hoặc chưa được kích hoạt, vui lòng liên hệ với administrator!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
                var userId = _userService.CreateUser(model.Username, model.Password, model.FullName);
                if (userId > 0)
                {
                    result.Result = userId;
                    _userService.MapRoleToUser(userId, new List<int> { RoleEnum.User });
                    result.Message = "Đăng ký thành công!";
                }
                else
                {
                    result.Result = 0;
                    result.Message = "Đăng ký thất bại!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }

        public ActionResult ChangePassword()
        {
            if (Session[Constant.SessionUsername] != null)
            {
                return View();
            }
            else
            {
                return Redirect("/dang-nhap");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = new Models.CustomJsonResult();
            try
            {
                string username = GetUserNameFromSession();
                var checkUser = _userService.GetUserByUsername(username);
                if (checkUser != null && checkUser.Id > 0)
                {
                    var changeUer = _userService.ChangePass(checkUser.Id, model.CurrentPassword, model.Password);
                    if (changeUer)
                    {
                        result.Result = checkUser.Id;
                        result.Message = "Thay đổi mật khẩu thành công!";
                    }
                    else
                    {
                        result.Result = 0;
                        result.Message = "Thay đổi mật khẩu thất bại!";
                    }
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


    }
}