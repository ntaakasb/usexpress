using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Web.FrontEnd.Common;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{

    public class StoreController : BaseAdminController
    {
        // GET: Store
        private readonly ILocationServices locationService;
        private readonly IRoleService roleService;


        public StoreController(
            IUserService userService,
            IStoreServices storeService,
            ILocationServices locationService,
            IRoleService roleService) : base(storeService, userService)
        {
            this.locationService = locationService;
            this.roleService = roleService;
        }

        #region Store Areas
        public ActionResult Index(int? page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            var result = _storeService.GetListStore(pageIndex, pageSize, null);
            return View(result);
        }

        public PartialViewResult _StoreGrid(PagedList.IPagedList<tblStoreAccount> model)
        {
            return PartialView(model);
        }

        public JsonResult AJXSearchStore()
        {
            string keyword = Request.Form["SearchKeyword"];
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _storeService.GetListStore(1, pageSize, keyword);
            return Json(RenderPartialViewToString("_StoreGrid", result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailStore(int id = -1)
        {
            StoreModel storeModel = new StoreModel();
            if (id != -1)
            {

                storeModel.StoreAccount = _storeService.SelectStoreByID(id);
                storeModel.UserRole = roleService.Select(storeModel.StoreAccount.Email);
            }
            return View(storeModel);
        }

        [HttpPost]
        public ActionResult UpdateStore(StoreModel storeModel)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (storeModel.StoreAccount.id > 0)
                {
                    result.Result = _storeService.UpdateStore(storeModel.StoreAccount);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Update Store successly";
                        int roleID = Request.Form["RoleID"] != null ? int.Parse(Request.Form["RoleID"].ToString()) : -1;
                        string userName = Request.Form["hUserName"];
                        tblUser _item = _userService.GetUserByUsername(userName);
                        //If user had ready => update role for this user else add new user and update role
                        if (_item != null)
                        {
                            roleService.UpdateRoleUser(_item.Id, roleID);
                            if (storeModel.StoreAccount.Status > 0)
                            {
                                _item.IsActive = true;
                                _userService.UpdateUser(_item);
                            }
                        }
                        //else
                        //{
                        //    long _addnewUserID = userService.CreateUser(storeModel.StoreAccount.Email, storeModel.StoreAccount.Password, storeModel.StoreAccount.FullName);
                        //}
                    }
                }
                else
                {
                    result.Result = _storeService.InsertStore(storeModel.StoreAccount);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        long UserID = _userService.CreateUser(storeModel.StoreAccount.Email, storeModel.StoreAccount.Password, storeModel.StoreAccount.FullName);
                        if (UserID > 0)
                        {
                            roleService.UpdateRoleUser(UserID, 4);
                            result.Message = "Create new Store successly";
                        }

                    }

                }
                if (int.Parse(result.Result.ToString()) < 1)
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


        #endregion

        public ActionResult SenderManager(int id = -1)
        {
            if (Session[Constant.SessionStoreID] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.StoreID = Session[Constant.SessionStoreID];
            tblSender sender = new tblSender();
            sender = _storeService.SelectSenderByID(id);
            return View(sender);
        }
        public ActionResult RecieverManager(int id = -1)
        {
            if (Session[Constant.SessionStoreID] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.StoreID = Session[Constant.SessionStoreID];
            tblRecipientsInfo reciver = new tblRecipientsInfo();
            reciver = _storeService.SelectReciverByID(id);
            return View(reciver);
        }

        public ActionResult DeliveryManager()
        {
            ViewBag.StoreID = Session[Constant.SessionStoreID];
            return View();
        }
        public PartialViewResult _SenderGrid(int? page)
        {

            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;

            var lsSender = _storeService.GetListSenderByStoreID(pageIndex, pageSize, storeID, null);
            return PartialView(lsSender);
        }
        public PartialViewResult _RecieverGrid(int? page)
        {

            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;

            var lsReciever = _storeService.GetListRecieverByStoreID(pageIndex, pageSize, storeID, null);
            return PartialView(lsReciever);
        }
        [HttpPost]
        public JsonResult UpdateSender(tblSender model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model.Id > 0)
                {
                    result.Result = _storeService.UpdateSender(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Update sender successly";
                    }
                }
                else
                {
                    result.Result = _storeService.InsertSender(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Create new sender successly";

                    }

                }
                if (int.Parse(result.Result.ToString()) < 1)
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
        public JsonResult UpdateReciever(tblRecipientsInfo model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model.id > 0)
                {
                    result.Result = _storeService.UpdateReciever(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Update reciver successly";
                    }
                }
                else
                {
                    result.Result = _storeService.InsertReciever(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Create new reciver successly";

                    }

                }
                if (int.Parse(result.Result.ToString()) < 1)
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
        public JsonResult AJXSearchSender()
        {
            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            string keyword = Request.Form["SearchKeyword"];
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _storeService.GetListSenderByStoreID(1, pageSize, storeID, keyword);
            return Json(RenderPartialViewToString("_TableGridSender", result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AJXSearchRecipients()
        {
            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            string keyword = Request.Form["SearchKeyword"];
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _storeService.GetListRecieverByStoreID(1, pageSize, storeID, keyword);
            return Json(RenderPartialViewToString("_TableGridRecipients", result), JsonRequestBehavior.AllowGet);
        }

    }


}