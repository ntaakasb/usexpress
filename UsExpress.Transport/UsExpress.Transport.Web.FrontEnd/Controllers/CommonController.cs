using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Services;
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
        private readonly ICategoryService _categoryService;

        public CommonController(ILocationServices locationService, IRoleService roleService, IStoreServices storeService, IUserService userService, ICategoryService categoryService)
        {
            this.locationService = locationService;
            this.roleService = roleService;
            _storeService = storeService;
            _userService = userService;
            _categoryService = categoryService;
        }


        public PartialViewResult _HtmlSelectBoxCity(string Selected)
        {
            var result = locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxDistrict(string CityID, string Selected)
        {
            var result = locationService.GetLstDictrictByCityId(CityID).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxWard(string DistrictID, string Selected)
        {
            var result = locationService.GetLstWardByDistrictId(DistrictID).Select(x => new SelectItemBase { Id = x.id, Name = x.WardName }).ToList();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxState(int Selected = -1)
        {
            var result = locationService.GetState();
            ViewBag.Selected = Selected;
            return PartialView(result);
        }

        public PartialViewResult _HtmlSelectBoxWareHouse(string Selected)
        {
            var result = locationService.GetWarehouse();
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

        public JsonResult LoadDistrictByCity(string CityID)
        {
            return Json(RenderPartialViewToString("_HtmlSelectBoxDistrict", locationService.GetLstDictrictByCityId(CityID).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadWardByDistrictID(string DistrictID)
        {
            return Json(RenderPartialViewToString("_HtmlSelectBoxWard", locationService.GetLstWardByDistrictId(DistrictID).Select(x => new SelectItemBase { Id = x.id, Name = x.WardName }).ToList()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadWarehouse(string StateID, string CityID)
        {
            return Json(RenderPartialViewToString("_HtmlSelectBoxWareHouse", locationService.GetWarehouse()), JsonRequestBehavior.AllowGet);
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
        public ActionResult SearchPhone(string phone, int typeUser, int storeId = -1)
        {

            var result = new CustomJsonResult();
            try
            {
                // -1: sẽ lấy theo storeId của user
                result.Result = _userService.SearchInfoByPhone(20, phone, typeUser, storeId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetWardByDistrictId(string districtId)
        {
            var result = new CustomJsonResult();
            try
            {
                result.Result = locationService.GetLstWardByDistrictId(districtId).Select(x => new SelectItemBase { Id = x.id, Name = x.WardName }).ToList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult SearchCategory(int top, string keyword)
        {
            var result = new CustomJsonResult();
            try
            {
                result.Result = _categoryService.GetAllCategory(1, top, keyword, 2).Select(x=> new SelectItemBase { Id = x.id.ToString(), Name = x.CategoryName, AttributeCode = x.Code }).ToList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetDetailCategoryById(int id)
        {
            var result = new CustomJsonResult();
            try
            {
                var detail = _categoryService.SelectByCatelogyID(id);
                if (detail != null)
                {
                    result.Result = new SelectItemBase { Id = detail.id.ToString(), Name = detail.CategoryName, AttributeCode = detail.Code };
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
    }
}