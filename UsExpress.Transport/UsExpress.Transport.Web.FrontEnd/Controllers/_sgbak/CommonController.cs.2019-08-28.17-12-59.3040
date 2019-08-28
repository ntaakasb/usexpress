using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class CommonController : Controller
    {
        // GET: Common

        private readonly ILocationServices locationService;
        private readonly IRoleService roleService;
        private readonly IStoreServices _storeService;
        private readonly IUserService _userService;

        public CommonController(ILocationServices locationService, IRoleService roleService, IStoreServices storeService, IUserService userService)
        {
            this.locationService = locationService;
            this.roleService = roleService;
            _storeService = storeService;
            _userService = userService;
        }


        public PartialViewResult _HtmlSelectBoxCity(int Selected = -1)
        {
            var result = locationService.GetCity();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxDistrict(int CityID = -1, int Selected = -1)
        {
            var result = locationService.GetDistrictByCity(CityID);
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxState(int Selected = -1)
        {
            var result = locationService.GetState();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxWareHouse(int StateID = -1, int CityID = -1, int Selected = -1)
        {
            var result = locationService.GetWarehouse(CityID, StateID);
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        protected string RenderPartialViewToString(string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonResult LoadDistrictByCity(int CityID)
        {
            return Json(RenderPartialViewToString("_HtmlSelectBoxDistrict", locationService.GetDistrictByCity(CityID)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadWarehouse(int StateID, int CityID)
        {
            return Json(RenderPartialViewToString("_HtmlSelectBoxWareHouse", locationService.GetWarehouse(CityID, StateID)), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _HtmlSelectBoxRole(int Selected = -1)
        {
            var result = roleService.GetAllRole();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public string GetCityNameByID(int cityID = -1)
        {
            tblCity item = locationService.GetCityByID(cityID);
            return item != null ? item.CityName : "Not found";
        }

        public string GetStateNameByID(int stateID = -1)
        {
            tblState item = locationService.GetStateByID(stateID);
            return item != null ? item.StateName : "Not found";
        }
        [HttpPost]
        public ActionResult GetDistrictByCityId(string cityId)
        {
            var result = new CustomJsonResult();
            try
            {
                result.Result = locationService.GetLstDictrictByCityId(cityId).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult SearchPhone(string phone, int typeUser)
        {

            var result = new CustomJsonResult();
            try
            {
                // -1: sẽ lấy theo storeId của user
                result.Result = _userService.SearchInfoByPhone(20, phone, typeUser, -1);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
    }
}