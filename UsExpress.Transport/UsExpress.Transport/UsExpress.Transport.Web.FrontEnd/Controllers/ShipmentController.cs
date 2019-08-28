using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.DTO;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class ShipmentController : BaseAdminController
    {
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        public ShipmentController(IOrderService orderService, IStoreServices storeService, IUserService userService, IShipmentService shipmentService) : base(storeService, userService)
        {
            _orderService = orderService;
            _shipmentService = shipmentService;
        }
        // GET: Shipment
        public ActionResult Index()
        {
            
            return View();
        }

       

        public PartialViewResult _GridOrder()
        {
            OrderSearch orderSearch = new OrderSearch()
            {
                Keyword = null,
                PageSize = 20,
                PageIndex = 0
            };
            ListOrderModels lsResult = new ListOrderModels();
            var lsOrder = _orderService.Admin_SearchOrder(orderSearch);
            var todate = DateTime.Now;
            var fromdate = todate.AddDays(-14);
            lsResult.FromDate = fromdate;
            lsResult.ToDate = todate;
            lsResult.ListResult = GetPositionWithModel(lsOrder, fromdate, todate);
            return PartialView(lsResult);
        }

      
        public List<OrderPositionModels> GetPositionWithModel(List<OrderViewDTO> model, DateTime dtmFromdate, DateTime dtmToDate)
        {

            List<OrderPositionModels> lsResult = new List<OrderPositionModels>();  
            model = model.Where(x => DateTime.Compare(dtmFromdate, DateTime.Parse(x.CreateDate)) <= 0 
                                    && DateTime.Compare(DateTime.Parse(x.CreateDate), dtmToDate) <= 0).ToList();

            int datediff = (dtmToDate.Date - dtmFromdate.Date).Days;

            for (int i = 0; i <= datediff; i++)
            {
                var colItem = model.Where(x => DateTime.Parse(x.CreateDate).Day == i);
                if(colItem.Count() > 0)
                {
                    int j = 0;
                    foreach(var _colitem in colItem)
                    {
                        OrderPositionModels item = new OrderPositionModels();    
                        item.X = i;
                        item.Y = j;
                        item.OrderItem = _colitem;
                        lsResult.Add(item);
                        j++;
                    }
                }
            }
            return lsResult;
        }

        public PartialViewResult _GridPackage(List<PackageGridDTO> model)
        {
            return PartialView();
        }

        public JsonResult AJXLoadPackageOrderDetail()
        {
            int OrderId = Request.Form["OrderId"] != null ? int.Parse(Request.Form["OrderId"].ToString()) : -1;
            var result = _shipmentService.listPackageByOrder(OrderId);
            if(result!= null && result.Count > 0)
            {
                return Json(RenderPartialViewToString("_GridPackage", result), JsonRequestBehavior.AllowGet);
            }
           else
            {
                return Json("Data could not be found!");
            }
        }
    }
}