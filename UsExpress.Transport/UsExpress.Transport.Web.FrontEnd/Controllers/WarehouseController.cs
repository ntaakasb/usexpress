using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class WarehouseController : BaseAdminController
    {
        private readonly IWareHouseServices _wareHouseServices;
        public WarehouseController(IStoreServices storeService, IUserService userService, IWareHouseServices wareHouseServices) : base(storeService, userService)
        {
            _wareHouseServices = wareHouseServices;
        }
        // GET: Warehouse
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
            var result = _wareHouseServices.GetListWarehouse(pageIndex, pageSize, null, -1);
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            return View(result);
        }

        public PartialViewResult _GridWarehouse(PagedList.IPagedList<tblWarehouse> model, int pageIndex)
        {
            ViewBag.Index = pageIndex;
            return PartialView(model);
        }

        public ActionResult DetailWareHouse(int id = -1)
        {
            tblWarehouse warehouse = new tblWarehouse();
            if (id != -1)
            {
                warehouse = _wareHouseServices.SelectByWarehouseID(id);
            }
            return View(warehouse);
        }

        [HttpPost]
        public JsonResult UpdateWarehouse(tblWarehouse model)
        {
            var result = new Models.CustomJsonResult();
            model.Status = bool.Parse(Request.Form["Status"]);
            try
            {
                if (model.id > 0)
                {
                    result.Result = _wareHouseServices.UpdateWarehouse(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Update Warehouse Succeed";
                    }
                }
                else
                {
                    result.Result = _wareHouseServices.InsertWarehouse(model);
                    if (int.Parse(result.Result.ToString()) > 0)
                    {
                        result.Message = "Create New Warehouse Succeed";

                    }

                }
                if (int.Parse(result.Result.ToString()) < 1)
                {
                    result.Message = "Had error, please try again!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AJXSearchWarehouse()
        {

            string keyword = Request.Form["SearchKeyword"];
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _wareHouseServices.GetListWarehouse(1, pageSize, keyword, searchType);
            return Json(RenderPartialViewToString("_GridWarehouse", result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IsExistsEmailUpdate(string Email, int id)
        {
            bool result = false;
            try
            {
                result = _wareHouseServices.CheckExistEmailWarehouseByUserID(Email.Trim(), id);


            }
            catch (Exception ex)
            {
                string ms = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}