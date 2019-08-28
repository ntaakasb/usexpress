using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Services;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class ProductController : BaseAdminController
    {
        // GET: Product
        private readonly IProductServices _productServices;
        public ProductController(IProductServices productServices) : base(null, null)
        {
            _productServices = productServices;
        }
        [Filters.Auth]
        public ActionResult Index()
        {
            return View("Manage");
        }
        [Filters.Auth]
        public ActionResult Manage()
        {
          
            return View();
        }
        public PartialViewResult _ProductsGrid(int? page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int pageIndex = page != null ? int.Parse(page.ToString()) : 1;
            var result = _productServices.GetAllProduct(pageIndex, pageSize, null, -1);
            return PartialView(result);
        }

        public JsonResult AJXSearch()
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            int searchType = Request.Form["SearchType"] != null ? int.Parse(Request.Form["SearchType"]) : -1;
            var result = _productServices.GetAllProduct(1, pageSize, Request.Form["txtkeyword"], searchType);
            return Json(RenderPartialViewToString("_ProductsGrid", result), JsonRequestBehavior.AllowGet);
        }
        [Filters.Auth]
        public ActionResult InsertProduct(ProductDTO model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model != null)
                {
                    var productModel = model.Map<tblProduct>();
                    result.Result = _productServices.InsertProduct(productModel);
                    result.Message = int.Parse(result.Result.ToString()) > 0 ? "Create new product success!" : "Action had error or barcode is exists, please try again!";
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
        [Filters.Auth]
        public ActionResult UpdateProduct(ProductDTO model)
        {
            var result = new Models.CustomJsonResult();
            try
            {
                if (model != null)
                {
                    var productModel = model.Map<tblProduct>();
                    if (model.id > 0)
                    {
                        result.Result = _productServices.UpdateProduct(productModel);
                        result.Message = int.Parse(result.Result.ToString()) > 0 ? "Update product success!" : "Action had error, please try again!";
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

        [HttpPost]
        public JsonResult AJXUpdateForm()
        {
            int id = Request.Form["updateID"] != null ? int.Parse(Request.Form["updateID"]) : -1;
            tblProduct item = _productServices.SelectByProductID(id);
            return Json(RenderPartialViewToString("_ProductDetailForm", item.Map<ProductDTO>()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadCodeByCategoryId(int? categoryId)
        {
            string code = string.Empty;
            if(categoryId != null)
            {
                int intCategoryID = int.Parse(categoryId.ToString());
                code = _productServices.getScheduleBCodeByCategoryId(intCategoryID);
            }
            return Json(code, JsonRequestBehavior.AllowGet);
        }
    }
}