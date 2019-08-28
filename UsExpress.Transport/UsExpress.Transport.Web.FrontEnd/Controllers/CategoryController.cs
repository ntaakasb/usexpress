using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Services;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class CategoryController : BaseAdminController
    {

        // GET: Category
        ICategoryService _categoryServices;

        public CategoryController(ICategoryService categoryServices) : base(null, null)
        {
            _categoryServices = categoryServices;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Manager");
        }

        public ActionResult Manager()
        {
            return View();
        }

        public PartialViewResult _CategoryGrid(int? page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            var result = _categoryServices.GetAllCategory(pageIndex, pageSize, null, -1);
            ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            return PartialView(result);
        }

        [HttpPost]
        public JsonResult AJXUpdateForm()
        {
            int id = Request.Form["updateID"] != null ? int.Parse(Request.Form["updateID"]) : -1;
            tblCategory item = _categoryServices.SelectByCatelogyID(id);
            return Json(RenderPartialViewToString("_CategoryDetailForm", item), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AJXSearch()
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            var result = _categoryServices.GetAllCategory(1, pageSize, Request.Form["txtkeyword"], searchType);
            return Json(RenderPartialViewToString("_CategoryGrid", result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertUpdateCategory(tblCategory model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.CategoryName))
                {
                    if (model.id > 0)
                    {
                        result.Result = _categoryServices.UpdateCatelogy(model);
                        result.Message = int.Parse(result.Result.ToString()) > 0 ? "Catelogy update succesful!" : "Action had error, please try again!";
                    }
                    else
                    {
                        result.Result = _categoryServices.InsertCatelogy(model);
                        result.Message = int.Parse(result.Result.ToString()) > 0 ? "Catelogy created succesful!" : "Action had error, please try again!";
                    }
                }
                else
                {
                    result.Result = 0;
                    result.Message = "Action had error, please try again!";
                }
            }
            catch (Exception ex)
            { }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckExistCode(int id, string code)
        {
            return Json(_categoryServices.CheckExistCodeByID(id, code), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _OptionGetAllTypeCategory(int? selectedValue)
        {
            var result = _categoryServices.GetAllTypeCategory(1, 9999);
            ViewBag.Selected = selectedValue;
            return PartialView(result);
        }

        public ActionResult _OptionGetAllCategory(int? selectedValue)
        {
            var result = _categoryServices.GetAllCategory(1, 9999, null, -1).Select(x => new SelectListItem() {
                Value = x.id.ToString(),
                Text = x.CategoryName,
                Selected = selectedValue != null && x.id  == selectedValue
            });
            return Content(RenderPartialViewToString("/Views/Common/_HtmlSelectBox.cshtml", result));
        }
    }
}