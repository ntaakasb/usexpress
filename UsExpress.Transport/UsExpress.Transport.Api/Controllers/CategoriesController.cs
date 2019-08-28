using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UsExpress.Transport.Api.Models;
using UsExpress.Transport.Lib.Business.Interfaces;

namespace UsExpress.Transport.Api.Controllers
{
    public class CategoriesController : ApiController
    {
        public readonly ICategoryService _categoryServices;
        public CategoriesController(ICategoryService categoryServices)
        {
            _categoryServices = categoryServices;
        }
        [HttpPost]
        [Route("api/categories/getall")]
        public IHttpActionResult GetAllCategories()
        {
            var response = new Response();
            response.Data=_categoryServices.GetAll();
            return Json(response);
        }
    }
}
