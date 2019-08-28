using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json.Linq;
using UsExpress.Transport.Api.Models;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Api.DataController;

namespace UsExpress.Transport.Api.Controllers
{
    public class OrderController : ApiController
    {
        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IKerryService _iKerryServices;
        private readonly IOrderService _orderService;
        private readonly IStoreServices _storeService;
        public OrderController(IKerryService iKerryServices,IOrderService orderSerivce, IStoreServices storeService)
        {
            _iKerryServices = iKerryServices;
            _orderService = orderSerivce;
            _storeService = storeService;
        }

        [HttpPost]
        [Route("api/order/kerryupdate")]
        public IHttpActionResult KerryUpdateOrder(JObject jObject)
        {
            var response = new Response();
            try
            {
                _Log.Info("api/order/kerryupdate: " + jObject.ToJson());
                JToken jToken;
                if (!jObject.TryGetValue("OrderId", out jToken)) return Ok(response.Failed("OrderId invalid!"));
                var strPackageId = jToken.Value<string>();
                //
                if (!jObject.TryGetValue("StatusService", out jToken)) return Ok(response.Failed("StatusService invalid!"));
                var statusService = jToken.Value<string>();
                if (!jObject.TryGetValue("Weight", out jToken)) return Ok(response.Failed("Weight invalid!"));
                var weight = jToken.Value<string>();
                if (!jObject.TryGetValue("Dimension", out jToken)) return Ok(response.Failed("Dimension invalid!"));
                var dimension = jToken.Value<string>();
                if (!jObject.TryGetValue("Cost", out jToken)) return Ok(response.Failed("Cost invalid!"));
                var cost = jToken.Value<string>();
                if (!jObject.TryGetValue("Fee", out jToken)) return Ok(response.Failed("Fee invalid!"));
                var fee = jToken.Value<string>();
                if (!jObject.TryGetValue("TimeStatus", out jToken)) return Ok(response.Failed("TimeStatus invalid!"));
                var timeStatus = jToken.Value<string>();

                if (!jObject.TryGetValue("Note", out jToken)) return Ok(response.Failed("Note invalid!"));
                var note = jToken.Value<string>();

                //usexpress system
                //if (Regex.IsMatch(strPackageId, @"([a-zA-Z]{4}[\d]{12})")) //UsTransport system
                //{
                    var kerryOrderProgress = new tblKerryOrderProgress()
                    {
                        OrderCode = strPackageId,
                        StatusService = statusService,
                        Weight = weight,
                        Dimension = dimension,
                        Cost = cost,
                        Fee = fee,
                        TimeStatus = timeStatus,
                        Note = note,
                        CreatedDate = DateTime.Now
                    };
                    _iKerryServices.OrderProgress(kerryOrderProgress);
                   
                //}
                return Ok(response);
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message, ex);
                return Ok(response.System(ex));
            }
        }
        [HttpPost]
        [Route("api/order/CreateOder")]
        public IHttpActionResult SubmitOrder([FromBody]OrderModel model)
        {
            var response = new Response();
            try
            {
                if (model.Id == 0)
                {
                    // todo: thay bằng storeId

                    var storeid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StoreId"]);
                    var storeInfo = _storeService.SelectStoreByID(storeid);
                    if (storeInfo==null)
                    {
                        response.Message = "Invalid StoreId";
                        return Json(response);
                    }
                    if (model.Packages==null)
                    {
                        response.Message = "Packages must be not null";
                        return Json(response);
                    }
                    
                    model.Code = storeInfo.Code;
                    var orderDetail = model.ConvertToOrder();
                    var g = 0; foreach (var p in orderDetail.tblPackageInfoes)
                    {
                        g++;
                        p.Code = Libs.AutoGenPackageID(orderDetail.CreateDate.UnixTimeStampToDateTime(), storeInfo.Code, orderDetail.tblPackageInfoes.Count.ToString() + g.ToString(), _orderService.CountOrderInDay(orderDetail.CreateDate.UnixTimeStampToDateTime(), storeid));
                        p.WarehouseId = storeInfo.WarehouseId;
                        p.NotifyToCustomer = 0;
                    }
                    response.Data = _orderService.Admin_CreateOrderApi(orderDetail);
                    

                }
                else
                {
                    response.Message = "Not using update value";
                    //tblOrder detail = model.ConvertToOrder();
                    //detail.id = (int)model.Id;
                    //response.Data = _orderService.Admin_UpdateOrder(detail);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }
        
    }
}
