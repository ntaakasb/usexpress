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
    public class CatelogyController : BaseAdminController
    {

        // GET: Catelogy
        ICatelogyService _catelogyServices;

        public CatelogyController(ICatelogyService catelogyServices) : base(null, null)
        {
            _catelogyServices = catelogyServices;
        }



        public ActionResult Index()
        {
            return RedirectToAction("Manager");
        }

        public ActionResult Manager()
        {
            return View();
        }

        public PartialViewResult _CatelogyGrid(int? page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            var result = _catelogyServices.GetAllCategory(pageIndex, pageSize, null);
            return PartialView(result);
        }

        public JsonResult AJXUpdateForm()
        {
            int id = Request.Form["updateID"] != null ? int.Parse(Request.Form["updateID"]) : -1;
            tblCategory item = _catelogyServices.SelectByCatelogyID(id);
            return Json(RenderPartialViewToString("_CatelogyDetailForm", item), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AJXSearch()
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            var result = _catelogyServices.GetAllCategory(1, pageSize, Request.Form["txtkeyword"]);
            return Json(RenderPartialViewToString("_CatelogyGrid", result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertUpdateCatelogy(tblCategory model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.CategoryName))
                {
                    if (model.id > 0)
                    {
                        result.Result = _catelogyServices.UpdateCatelogy(model);
                        result.Message = int.Parse(result.Result.ToString()) > 0 ? "Update catelogy success!" : "Action had error, please try again!";
                    }
                    else
                    {
                        result.Result = _catelogyServices.InsertCatelogy(model);
                        result.Message = int.Parse(result.Result.ToString()) > 0 ? "Create new catelogy success!" : "Action had error, please try again!";
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
            return Json(_catelogyServices.CheckExistCodeByID(id, code), JsonRequestBehavior.AllowGet);
        }
    }
}