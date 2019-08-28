using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class OrderController : BaseAdminController
    {
        // GET: Order
        private readonly ICatelogyService _categoryService;
        private readonly ILocationServices _locationService;
        private readonly IOrderService _orderService;
        public OrderController(ICatelogyService categoryService, ILocationServices locationService, IOrderService orderSerivce, IStoreServices storeService, IUserService userService) : base(storeService, userService)
        {
            _categoryService = categoryService;
            _locationService = locationService;
            _orderService = orderSerivce;
        }
        public ActionResult Index()
        {
            return RedirectToAction("Manage");
        }
        public ActionResult CreateOrder()
        {
            //OrderModel od = new OrderModel();
            //od.Code = od.GenOrderCode("AM");
            //od.CreateDate = DateTime.Now;
            //ViewBag.OrderDetail = od;
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult SubmitOrder(OrderModel model)
        {
            var result = new CustomJsonResult();
            try
            {
                if (model.Id == 0)
                {
                    // todo: thay bằng storeId
                    var storeInfo = GetStoreAccountInfo();
                    if (storeInfo.id < 1)
                    {
                        result.Message = "Bạn không có quyền tạo Order";
                    }
                    else
                    {
                        var orderDetail = model.ConvertToOrder();
                        foreach (var p in orderDetail.tblPackageInfoes)
                        {
                            p.WarehouseId = storeInfo.WarehouseId;
                        }
                        result.Result = _orderService.Admin_CreateOrder(orderDetail);
                    }

                }
                else
                {
                    tblOrder detail = model.ConvertToOrder();
                    detail.id = (int)model.Id;
                    result.Result = _orderService.Admin_UpdateOrder(detail);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        public ActionResult DetailOrder(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult GetDetailOrderById(int id)
        {
            var result = new CustomJsonResult();
            OrderModel od = new OrderModel();
            if (id > 0)
            {
                var detail = _orderService.Admin_GetDetailOrderById(id);
                od = new OrderModel(detail);
            }
            else
            {
                if (id == 0)
                {
                    var storeInfo = GetStoreAccountInfo();
                    if (storeInfo.id < 1)
                    {
                        result.Message = "Bạn không được phép tạo Order";
                    }
                    else
                    {
                        od.Code = od.GenOrderCode(storeInfo.Code);
                        od.CreateDate = DateTime.Now;
                        od.StoreId = storeInfo.id;
                    }
                }


            }
            result.Result = od;
            return Json(result);
        }

        public ActionResult Manage()
        {
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult SearchOrder(OrderSearch model)
        {
            var result = new CustomJsonResult();
            result.Result = _orderService.Admin_SearchOrder(model);
            result.Optional = model.Total;
            return Json(result);
        }
    }
}