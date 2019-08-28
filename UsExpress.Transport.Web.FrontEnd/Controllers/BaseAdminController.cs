using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Web.FrontEnd.Common;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    
    public class BaseAdminController : Controller
    {
        protected readonly IStoreServices _storeService;
        protected readonly IUserService _userService;
        protected readonly Lib.Utilities.LogHelper.Log _logHelper;
        public int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        public BaseAdminController(IStoreServices storeService, IUserService userService)
        {
            this._storeService = storeService;
            this._userService = userService;
            //Lib.Utilities.LogHelper.Configure();
            //_logHelper = Lib.Utilities.LogHelper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        // GET: BaseAdmin
        public string GetUserNameFromSession()
        {
            if (Session[Constant.SessionUsername] != null)
            {
                return Session[Constant.SessionUsername].ToString();
            }
            return string.Empty;
        }

        public Lib.Business.Models.DBContext.UsTransport.tblStoreAccount GetStoreAccountInfo()
        {
            if (Session[Constant.SessionStoreAccountInfo] == null)
            {
                Lib.Business.Models.DBContext.UsTransport.tblStoreAccount storeInfo = _storeService.SelectStoreByUserName(GetUserNameFromSession());
                Session[Constant.SessionStoreAccountInfo] = storeInfo;
            }
            var info = (Lib.Business.Models.DBContext.UsTransport.tblStoreAccount)Session[Constant.SessionStoreAccountInfo];
            if (info == null)
            {
                info = new Lib.Business.Models.DBContext.UsTransport.tblStoreAccount();
            }
            return info;
        }


        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        protected string RenderPartialViewToString(string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/

            // tham khảo cách viêt: http://blog.janjonas.net/2011-06-18/aspnet-mvc3-controller-extension-method-render-partial-view-string
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}