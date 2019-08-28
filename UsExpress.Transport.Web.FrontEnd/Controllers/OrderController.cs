using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Business.Services;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Common;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class OrderController : BaseAdminController
    {
        // GET: Order
        private readonly ICategoryService _categoryService;
        private readonly ILocationServices _locationService;
        private readonly IOrderService _orderService;
        private readonly IKerryService _iKerryService;
        private readonly IProductServices _productServices;
        IKerryService kerryService = new KerryService();
        public OrderController(ICategoryService categoryService,
                            ILocationServices locationService,
                            IOrderService orderSerivce,
                            IStoreServices storeService,
                            IUserService userService,
                            IKerryService kerryService,
                            IProductServices productServices) : base(storeService, userService)
        {
            _categoryService = categoryService;
            _locationService = locationService;
            _orderService = orderSerivce;
            _iKerryService = kerryService;
            _productServices = productServices;
        }
        [Filters.Auth]
        public ActionResult Index()
        {
            return RedirectToAction("Manage");
        }
        [Filters.Auth]
        public ActionResult CreateOrder()
        {
            //OrderModel od = new OrderModel();
            //od.Code = od.GenOrderCode("AM");
            //od.CreateDate = DateTime.Now;
            //ViewBag.OrderDetail = od;
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null, -1).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            var storeInfo = GetStoreAccountInfo();

            ViewBag.ProductList = _productServices.GetAllProduct(1, 9999, null, -1).ToList();

            ViewBag.StoreName = storeInfo.id > 0 ? storeInfo.StoreName : "";
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
                        var g = 0;  foreach (var p in orderDetail.tblPackageInfoes)
                        {
                            g++;
                            p.Code = Common.Libs.AutoGenPackageID(orderDetail.CreateDate.UnixTimeStampToDateTime(),storeInfo.Code, orderDetail.tblPackageInfoes.Count.ToString() + g.ToString(), _orderService.CountOrderInDay(orderDetail.CreateDate.UnixTimeStampToDateTime(),model.StoreId));
                            p.WarehouseId = storeInfo.WarehouseId;
                            p.NotifyToCustomer = 0;
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
        [Filters.Auth]
        public ActionResult DetailOrder(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null, -1).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            var storeInfo = GetStoreAccountInfo();
            ViewBag.StoreName = storeInfo.id > 0 ? storeInfo.FullName : "";
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
                        od.Code = storeInfo.Code;//od.GenOrderCode(storeInfo.Code);
                        od.CreateDate = DateTime.UtcNow;
                        od.StoreId = storeInfo.id;
                        od.SenderInfo.StoreId = od.StoreId;
                        od.SenderInfo.TypeUser = (int)OrderUserInfo.Sender;
                        od.RecipientInfo.StoreId = od.StoreId;
                        od.RecipientInfo.TypeUser = (int)OrderUserInfo.Recipient;
                    }
                }


            }
            result.Result = od;
            return Json(result);
        }
        [Filters.Auth]
        public ActionResult Manage()
        {
            ViewBag.Category = _categoryService.GetAllCategory(1, 9999, null, -1).Select(x => new CategoryModel { Id = x.id, Name = x.CategoryName, Code = x.Code }).ToList();
            ViewBag.CitySender = _locationService.GetLstStateOfCountry((int)CountrySupport.USA).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name }).ToList();
            ViewBag.CityRecipient = _locationService.GetLstStateOfCountry((int)CountrySupport.VietNam).Select(x => new SelectItemBase { Id = x.Id, Name = x.Name, AttributeId = x.AirPort ?? 0 }).ToList();
            var storeInfo = GetStoreAccountInfo();
            ViewBag.StoreName = storeInfo.id > 0 ? storeInfo.FullName : "";
            ViewBag.StoreId = storeInfo.id;

            return View();
        }
        [HttpPost]
        public ActionResult SearchOrder(PackageSearch model)
        {
            var result = new CustomJsonResult();
            if (model.StoreId == 0)
            {
                model.StoreId = -1;
            }
            if(Libs.IsStore())
            {
                int tempInt = 1;

                model.StoreId = int.TryParse(Session[Common.Constant.SessionStoreID].ToString(), out tempInt) ? int.Parse(Session[Common.Constant.SessionStoreID].ToString()) : -1;
            }
            var lstResult = _orderService.Admin_SearchPackage(model);// _orderService.Admin_SearchOrder(model);
            lstResult.ForEach(x => { x.LstStatusCanChange = Compute_FilterOrderStatus(x); });
            result.Result = lstResult;
            result.Optional = model.Total;
            return Json(result);
        }
        private List<object> Compute_FilterOrderStatus(PackageViewDTO model)
        {
            var lst = new List<object>();

            lst.Add(new
            {
                Id = -1,
                Name = string.Format("{0}", model.StatusName)
            });

            switch ((OrderStatusInfo)model.StatusId)
            {
                case OrderStatusInfo.New:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.PickedUp,
                        Name = OrderStatusInfo.PickedUp.ToString()
                    });

                    break;

                case OrderStatusInfo.PickedUp:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.SendToVN,
                        Name = OrderStatusInfo.SendToVN.ToString()
                    });

                    break;
                case OrderStatusInfo.ProhibitedItem:
                case OrderStatusInfo.RejectPickup:
                case OrderStatusInfo.RejectSendToVN:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.PickedUp,
                        Name = OrderStatusInfo.PickedUp.ToString()
                    });
                    break;
                case OrderStatusInfo.Damage:
                case OrderStatusInfo.Lost:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.Good,
                        Name = OrderStatusInfo.Good.ToString()
                    });
                    break;
                case OrderStatusInfo.Good:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.Delivering,
                        Name = OrderStatusInfo.Delivering.ToString()
                    });
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.Delivered,
                        Name = OrderStatusInfo.Delivered.ToString()
                    });
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.Failed,
                        Name = OrderStatusInfo.Failed.ToString()
                    });
                    break;
                case OrderStatusInfo.Delivering:
                    lst.Add(new
                    {
                        Id = (int)OrderStatusInfo.Delivered,
                        Name = OrderStatusInfo.Delivered.ToString()
                    });
                    break;
                case OrderStatusInfo.SendToVN:
                    lst.Add(new {
                        Id = (int)OrderStatusInfo.ClearCustom,
                        Name = OrderStatusInfo.ClearCustom.ToString()
                    });
                    break;
                case OrderStatusInfo.ClearCustom:
                    lst.Add(new {
                        Id = (int)OrderStatusInfo.Delivered,
                        Name = OrderStatusInfo.Delivered.ToString()
                    });
                    break;
            }

            return lst;
        }
        [Filters.Auth]
        public ActionResult Tracking()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateShippingInfo(ShippingInfoModel model)
        {
            /// -2: có lỗi xảy ra
            /// -1: đã tồn tại số phone, không update
            /// 0: insert bị lỗi
            /// > 0: insert thành công.
            var result = new CustomJsonResult() { Result = -2 };
            try
            {
                /// check user info by store
                if (model.StoreId == 0)
                {
                    var storeInfo = GetStoreAccountInfo();
                    if (storeInfo != null && storeInfo.id > 0)
                    {
                        model.StoreId = storeInfo.id;
                    }
                }
                if (model.StoreId > 0)
                {
                    bool hasExists = _storeService.Admin_CheckPhoneUserInfoOfStore(model.StoreId, model.TypeUser, model.Phone);
                    // init
                    tblSender _s = new tblSender();
                    tblRecipientsInfo _r = new tblRecipientsInfo();

                    switch ((OrderUserInfo)model.TypeUser)
                    {
                        case OrderUserInfo.Recipient:
                            _r = new tblRecipientsInfo
                            {
                                Phone = model.Phone,
                                Add1 = model.AddressLine1,
                                Add2 = model.AddressLine2,
                                CityId = model.CityId,
                                DistrictId = model.DistrictId,
                                FullName = model.FullName,
                                StoreId = model.StoreId,
                                WardId = model.WardId
                            };                            
                            break;
                        default:
                        case OrderUserInfo.Sender:
                            _s = new tblSender
                            {
                                Phone = model.Phone,
                                Add1 = model.AddressLine1,
                                Add2 = model.AddressLine2,
                                CityId = model.CityId,
                                FullName = model.FullName,
                                StoreId = model.StoreId,
                                StateId = model.StateId,
                                Zip = model.Zip,
                                Email = model.Email
                            };                            
                            break;
                    }
                    if (!hasExists)
                    {
                        switch ((OrderUserInfo)model.TypeUser)
                        {
                            case OrderUserInfo.Recipient:
                                result.Result = _storeService.InsertReciever(_r);
                                break;
                            case OrderUserInfo.Sender:
                                result.Result = _storeService.InsertSender(_s);
                                break;
                        }
                    }
                    else
                    {
                        switch ((OrderUserInfo)model.TypeUser)
                        {
                            case OrderUserInfo.Recipient:
                                _r.id = model.Id;
                                result.Result = _storeService.UpdateReciever(_r);
                                break;
                            case OrderUserInfo.Sender:
                                _s.Id = model.Id;
                                result.Result = _storeService.UpdateSender(_s);
                                break;
                        }
                        //result.Result = -1;
                    }
                }
                else
                {
                    result.Message = "You dont have store info!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                Libraries.Log.Write(ex.Message);
            }
            return Json(result);
        }

        public ActionResult PrintLabel(int id)
        {
            var detail = _orderService.Admin_GetDetailPackageById(id);
            return View(detail);
        }
        public ActionResult PrintInVoice(int id)
        {
            tblPackageInfo detail = new tblPackageInfo();
            OrderCustomDTO _order = new OrderCustomDTO();
            try
            {
                detail = _orderService.PrintInvoice(id);
                if (detail!=null)
                {
                    _order = _orderService.getOrderByID(detail.OrderId);
                }
                ViewBag.Order = _order;
            }
            catch (Exception ex)
            {
                Libraries.Log.Write("PrintInVoice: "+ex.Message);
            }
            
            return View(detail);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateStatusPackage(int id, int statusUpdate)
        {
            var result = new CustomeResultDTO();
            try
            {
                result = _orderService.Admin_UpdateStatusPackage(id, statusUpdate);
                if (result.Result == 1 && statusUpdate == (int)OrderStatusInfo.SendToVN)
                {
                    //new KerryService().PostNewOrder(id);
                  await Task.Run(() => _iKerryService.PostNewOrder(id));
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateListStatusPackage(string lstId, int id, int statusUpdate)
        {
            var result = new CustomeResultDTO();
            try
            {
                result = _orderService.Admin_UpdateListStatusPackage(lstId,id, statusUpdate);
                if (result.Result == 1 && statusUpdate == (int)OrderStatusInfo.SendToVN)
                {
                    await Task.Run(() => _iKerryService.PostNewOrder(id));
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateListStatusPackageSingle(string lstId, int statusUpdate)
        {
            var result = new CustomeResultDTO();
            try
            {
                result = _orderService.Admin_UpdateListStatusPackageSingle(lstId,  statusUpdate);
                if (result.Result == 1 && statusUpdate == (int)OrderStatusInfo.SendToVN)
                {
                    var arr = lstId.Split(',').ToList();
                    for (int i = 0; i < arr.Count; i++)
                    {
                        await Task.Run(() => _iKerryService.PostNewOrder(Convert.ToInt32(arr[i])));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult CheckMultiRecipient(int id)
        {
            List<ShipmentPropose> result = new List<ShipmentPropose>();
            try
            {
                result = _orderService.Admin_CheckPackageRecipient(id);
            }
            catch (Exception ex)
            {
                
            }
            return Json(result);
        }

        public ActionResult LoadListStore()
        {
            List<tblStoreAccount> lstStoreAccount;
            if (Libs.IsAdmin() || Libs.IsCSKH())
                lstStoreAccount = _storeService.LoadListStore();
            else
            {
                var storeAccount = GetStoreAccountInfo();
                lstStoreAccount = new List<tblStoreAccount>
                {
                    storeAccount
                };
            }
            return Json(lstStoreAccount);
        }

        public ActionResult SetActiveOrder(int idOrder)
        {
            return Json(_orderService.SetActiveOrder(idOrder));        }
    }
}
