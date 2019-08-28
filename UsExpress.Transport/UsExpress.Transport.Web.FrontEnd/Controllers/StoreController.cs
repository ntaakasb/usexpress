using System;
using System.Collections.Generic;
using System.Configuration;
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
        [Filters.Auth]
        public ActionResult Index()
        {
            return RedirectToAction("Manager");
        }
        [Filters.Auth]
        public ActionResult Manager(int? page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            var result = _storeService.GetListStore(pageIndex, pageSize, null);
            return View(result);
        }

        public PartialViewResult _StoreGrid(PagedList.IPagedList<tblStoreAccount> model, int pageIndex)
        {
            ViewBag.Index = pageIndex;
            return PartialView(model);
        }

        public JsonResult AJXSearchStore()
        {
            string keyword = Request.Form["SearchKeyword"];
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _storeService.GetListStore(1, pageSize, keyword);
            return Json(RenderPartialViewToString("_StoreGrid", result), JsonRequestBehavior.AllowGet);
        }
        [Filters.Auth]
        public ActionResult DetailStore(int id = -1)
        {
            StoreModel storeModel = new StoreModel();
            int userID = -1;
            if (int.TryParse(Session[Constant.SessionUserID].ToString(), out userID))
            {
                var lstRoleId = _userService.GetLstRoleIdByUserId(int.Parse(Session[Constant.SessionUserID].ToString()));
                if (lstRoleId != null && lstRoleId.Any())
                {
                    if (lstRoleId.Count == 1 && lstRoleId.Contains(RoleEnum.Store))
                    {
                        tblStoreAccount storeInfo = (tblStoreAccount)Session[Constant.SessionStoreAccountInfo];
                        if (storeInfo != null)
                        {
                            ViewBag.UpdateYourStore = true;
                            storeModel.StoreAccount = _storeService.SelectStoreByID(storeInfo.id); ;
                            storeModel.UserRole = roleService.Select(storeModel.StoreAccount.Email);
                        }
                    }
                    if (lstRoleId.Contains(RoleEnum.Admin))
                    {
                        ViewBag.IsAdmin = true;
                    }
                }
            }
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
                string notencodepassword = storeModel.StoreAccount.Password;

                if (storeModel.StoreAccount.id > 0)
                {
                    storeModel.StoreAccount.AliasFullName = Libs.UnicodeToNoneMark(storeModel.StoreAccount.StoreName);
                    result.Result = _storeService.UpdateStore(storeModel.StoreAccount);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Update Store successly";
                        string userName = Request.Form["hUserName"];
                        tblUser _item = _userService.GetUserByUsername(userName);
                        if (_item != null)
                        {
                            if (storeModel.StoreAccount.Status > 0)
                            {
                                _item.IsActive = true;
                                _userService.UpdateUser(_item);
                            }
                        }

                        //int roleID = Request.Form["RoleID"] != null ? int.Parse(Request.Form["RoleID"].ToString()) : -1;

                        //tblUser _item = _userService.GetUserByUsername(userName);
                        ////If user had ready => update role for this user else add new user and update role
                        //if (_item != null)
                        //{
                        //    roleService.UpdateRoleUser(_item.Id, roleID);
                        //    if (storeModel.StoreAccount.Status > 0)
                        //    {
                        //        _item.IsActive = true;
                        //        _userService.UpdateUser(_item);
                        //    }
                        //}
                        //else
                        //{
                        //    long _addnewUserID = userService.CreateUser(storeModel.StoreAccount.Email, storeModel.StoreAccount.Password, storeModel.StoreAccount.FullName);
                        //}
                    }
                }
                else
                {
                    int userID = -1;
                    bool isActiveAccount = false;
                    storeModel.StoreAccount.AliasFullName = Libs.UnicodeToNoneMark(storeModel.StoreAccount.StoreName);
                    storeModel.StoreAccount.Password = Utils.HashMD5(storeModel.StoreAccount.Password);
                    if (int.TryParse(Session[Constant.SessionUserID].ToString(), out userID))
                    {
                        var lstRoleId = _userService.GetLstRoleIdByUserId(int.Parse(Session[Constant.SessionUserID].ToString()));
                        if (lstRoleId != null && lstRoleId.Any())
                        {
                            if (lstRoleId.Contains(RoleEnum.Admin))
                            {
                                storeModel.StoreAccount.Status = 1;
                                isActiveAccount = true;
                            }
                        }
                    }



                    result.Result = _storeService.InsertStore(storeModel.StoreAccount);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        long UserID = _userService.CreateUser(storeModel.StoreAccount.Email, notencodepassword, storeModel.StoreAccount.FullName, isActiveAccount);
                        if (UserID > 0)
                        {
                            roleService.UpdateRoleUser(UserID, RoleEnum.Store);
                            result.Message = "Create New Store Succeed";
                        }
                        else
                            result.Result = UserID;

                    }

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


        #endregion


        [Filters.Auth]
        public ActionResult CreateSender(int id = -1)
        {
            bool IsHadStore = true;
            if (Session[Constant.SessionStoreID] == null)
            {
                IsHadStore = false;
            }
            ViewBag.StoreID = Session[Constant.SessionStoreID];
            tblSender sender = new tblSender();
            sender = _storeService.SelectSenderByID(id);
            ViewBag.HadStore = IsHadStore;
            return View(sender);
        }

        public ActionResult SetActiveSender(int senderId) => Json(_storeService.SetActiveSender(senderId));
        [Filters.Auth]
        public ActionResult CreateRecipient(int id = -1)
        {
            bool IsHadStore = true;
            if (Session[Constant.SessionStoreID] == null)
            {
                IsHadStore = false;
            }
            ViewBag.HadStore = IsHadStore;
            ViewBag.StoreID = Session[Constant.SessionStoreID];
            tblRecipientsInfo reciver = new tblRecipientsInfo();
            reciver = _storeService.SelectReciverByID(id);
            return View(reciver);
        }
        [Filters.Auth]
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
            PagedList.IPagedList lsSender = null;
            if (Libs.IsAdmin())
            {
                lsSender = _storeService.GetListSenderByStoreID(pageIndex, pageSize, -1, null, -1);
            }
            else
            {
                lsSender = _storeService.GetListSenderByStoreID(pageIndex, pageSize, storeID, null, -1);
            }
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            return PartialView(lsSender);
        }

        public PartialViewResult _RecieverGrid(int? page)
        {

            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            PagedList.IPagedList lsReciever = null;
            if (Libs.IsAdmin())
            {
                lsReciever = _storeService.GetListRecieverByStoreID(pageIndex, pageSize, -1, null, -1);
            }
            else
            {
                lsReciever = _storeService.GetListRecieverByStoreID(pageIndex, pageSize, storeID, null, -1);
            }
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            return PartialView(lsReciever);
        }



        [HttpPost]
        public JsonResult AJXUpdateSender(tblSender model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model.Id > 0)
                {
                    result.Result = _storeService.UpdateSender(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Sender Updated Succesful";
                    }
                }
                else
                {
                    result.Result = _storeService.InsertSender(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Order Created Succesful";

                    }

                }
                if (int.Parse(result.Result.ToString()) < 1)
                {
                    result.Message = "Action has error, please try again!";
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
                        result.Message = "Update Receiver Succeed";
                    }
                }
                else
                {
                    result.Result = _storeService.InsertReciever(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Create Receiver Succeed";

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

        public ActionResult SetActiveRecieverInfo(int recieverId) => Json(_storeService.SetActiveRecieverInfo(recieverId));

        public JsonResult AJXSearchSender()
        {
            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            string keyword = Request.Form["SearchKeyword"].ToString().Length < 1 ? null : Request.Form["SearchKeyword"];
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            var isActive = Request.Form["IsActive"] == "on";
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

            PagedList.IPagedList lsSender = null;
            int ddlStore = -1;
            if (!String.IsNullOrEmpty(Request.Form["ddlStore"]) && !Libs.IsStore())
            {
                ddlStore = Request.Form["ddlStore"] != null ? int.Parse(Request.Form["ddlStore"]) : -1;
            }
            if (Libs.IsStore())
            {
                ddlStore = storeID;
            }
            if (Libs.IsAdmin())
            {
                lsSender = _storeService.GetListSenderByStoreID(1, pageSize, ddlStore, keyword, searchType, isActive);

            }
            else
            {
                lsSender = _storeService.GetListSenderByStoreID(1, pageSize, ddlStore, keyword, searchType, isActive);
            }

            return Json(RenderPartialViewToString("_SenderGrid", lsSender), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AJXSearchRecipients()
        {
            int storeID = Session[Constant.SessionStoreID] != null ? int.Parse(Session[Constant.SessionStoreID].ToString()) : -1;
            string keyword = Request.Form["SearchKeyword"].ToString().Length < 1 ? null : Request.Form["SearchKeyword"];
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            var isActive = Request.Form["IsActive"] == "on";
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            PagedList.IPagedList result = null;
            int ddlStore = -1;
            if (!String.IsNullOrEmpty(Request.Form["ddlStore"]) && !Libs.IsStore())
            {
                ddlStore = Request.Form["ddlStore"] != null ? int.Parse(Request.Form["ddlStore"]) : -1;
            }
            if (Libs.IsStore())
            {
                ddlStore = storeID;
            }
            if (Libs.IsAdmin())
            {
                result = _storeService.GetListRecieverByStoreID(1, pageSize, ddlStore, keyword, searchType, isActive);

            }
            else
            {
                result = _storeService.GetListRecieverByStoreID(1, pageSize, ddlStore, keyword, searchType, isActive);
            }
            return Json(RenderPartialViewToString("_RecieverGrid", result), JsonRequestBehavior.AllowGet);
        }
        [Filters.Auth]
        public ActionResult SenderList()
        {
            return View();
        }
        [Filters.Auth]
        public ActionResult RecipientList()
        {
            return View();
        }

        public JsonResult IsExistStoreCode(int storeID, string code)
        {
            return Json(_storeService.CheckExistsStoreCode(storeID, code));
        }

        public PartialViewResult _HtmlSelectBoxStore(string selected)
        {
            ViewBag.Selected = selected;
            return PartialView(_storeService.GetListStore(1, 9999, null).Select(x => new SelectItemBase { Id = x.id.ToString(), Name = x.StoreName }).ToList());
        }

        public ContentResult GetCity(string cityID)
        {
            var result = locationService.GetLstStateOfCountry((int)UsExpress.Transport.Lib.Business.Common.CountrySupport.VietNam).FirstOrDefault(x => x.Id == cityID);
            string name = result != null ? result.Name : "";
            return Content(name);
        }
        public ContentResult GetDistrict(string DistrictID, string cityID)
        {
            var result = locationService.GetLstDictrictByCityId(cityID).FirstOrDefault(x => x.Id == DistrictID);
            string name = result != null ? result.Name : "";
            return Content(name);
        }

        public ContentResult GetWard(string wardID, string DistrictID)
        {
            var result = locationService.GetLstWardByDistrictId(DistrictID).FirstOrDefault(x => x.id == wardID);
            string name = result != null ? result.WardName : "";
            return Content(name);
        }

        public ActionResult SetActiveStore(int[] arrId)
        {
            return Json(_storeService.SetActiveStore(arrId));
        }
    }
}
