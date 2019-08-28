using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    /// <summary>
    /// Author: BinhNQ
    /// </summary>
    public class KerryService : IKerryService
    {
        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void PostNewOrder(int PackageId)
        {
            try
            {
                tblOrder order;
                tblPackageInfo packageInfo;
                List<tblPackageInfo> packageInfoChildren;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    packageInfo = db.tblPackageInfoes.Where(x => x.id == PackageId).Include(x => x.tblItemInPackages).FirstOrDefault();
                    if (packageInfo == null) return;

                    var kerryNewPost = db.tblKerryPostNewOrders.FirstOrDefault(x => x.OrderNumber.Equals(packageInfo.Code));
                    if (kerryNewPost != null) return;

                    order = db.tblOrders.FirstOrDefault(x => x.id.Equals(packageInfo.OrderId));
                    if (order != null)
                    {
                        //db.Entry(order).Reference(x => x.tblShippingInfo).Load();
                        //db.Entry(order).Collection(x => x.tblPackageInfoes).Load();
                        //db.Entry(order.tblPackageInfoes).Collection(x => x.).Load();
                        //order.RecipientShippingInfo =
                        //    db.tblShippingInfoes.FirstOrDefault(x => x.Id == order.RecipientId);                            
                        order.RecipientInfo = db.tblRecipientsInfoes.FirstOrDefault(x => x.id == order.RecipientId);
                        //order.RecipientShippingInfo.City = db.tblStateProvices.FirstOrDefault(x => x.Id == order.RecipientShippingInfo.CityId);
                        //order.RecipientShippingInfo.District = db.tblDistrictStateProvices.FirstOrDefault(x => x.Id == order.RecipientShippingInfo.DistrictId);
                    }

                }

                if (order == null)
                {
                    return;
                }

                dynamic data = new ExpandoObject();
                data.token_key = Constant.APPSETTING.Kerry.Token;
                data.order_number = packageInfo.Code;
                data.waybill_number = packageInfo.Code;

                using (var db = ContextFactory.UsTransportEntities())
                {
                    packageInfoChildren = db.tblPackageInfoes.Where(x => x.ParentId == PackageId).Include(x => x.tblItemInPackages).ToList();
                    data.no_packs = (packageInfoChildren.Count() + 1) + "";
                }
                data.package_weight = (Math.Round((Double)(packageInfoChildren.Sum(x => x.Weight) * 0.45359237M), 1) + Math.Round((Double)(packageInfo.Weight * 0.45359237M), 1)).ToString();
                data.service_type = KerryExpress.Service.DichVuChuyenPhat48H;
                data.cod = string.Empty;
                data.order_note = string.Empty;

                data.receiver_address = new
                {
                    full_address = order.RecipientInfo?.Add1,
                    province_area_code = order.RecipientInfo?.CityId,
                    district_area_code = order.RecipientInfo?.DistrictId,
                    ward_area_code = string.Empty,
                    contact_phone = order.RecipientInfo?.Phone,
                    contact_name = order.RecipientInfo?.FullName
                };

                //sender address 
                data.sender_address = new
                {
                    full_address =
                    "Tầng 5 tòa nhà Diamond Flower – Số 1 Hoàng Đạo Thúy – Phường Nhân Chính – Q.Thanh Xuân – Hà Nội",
                    province_area_code = "01",
                    district_area_code = "009",
                    ward_area_code = "00343",
                    contact_phone = "02473006818",
                    contact_name = "Công ty cổ phần US EXPRESS",
                };

                data.orderItem = new List<dynamic>();
                foreach (var item in packageInfo.tblItemInPackages)
                {
                    data.orderItem.Add(new
                    {
                        product_name = item.Description,
                        package_weight = "0",
                        package_dimension = string.Empty
                    });
                }
                foreach (var itemPK in packageInfoChildren)
                {
                    foreach (var item in itemPK.tblItemInPackages)
                    {
                        data.orderItem.Add(new
                        {
                            product_name = item.Description,
                            package_weight = "0",
                            package_dimension = string.Empty
                        });
                    }
                }

                var jsonParams = JsonConvert.SerializeObject(data);
                //_Log.Info("jsonParams: " + jsonParams);
                Libraries.Log.WriteLogs("jsonParams: " + jsonParams);
                var result =
                    Client.Post<KerryExpress.PostNewOrderResponse>(Constant.APPSETTING.Kerry.ApiUri, jsonParams);
                //_Log.Info("result: " + JsonConvert.SerializeObject(result));
                Libraries.Log.WriteLogs("result: " + JsonConvert.SerializeObject(result));
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var kerryPostNewOrder =
                        db.tblKerryPostNewOrders.FirstOrDefault(x => x.OrderNumber.Equals(packageInfo.Code));
                    if (kerryPostNewOrder == null)
                    {
                        kerryPostNewOrder = new tblKerryPostNewOrder()
                        {
                            OrderId = packageInfo.id,
                            Status = result.status,
                            Message = result.message,
                            OrderNumber = result.order_number,
                            Cost = result.cost,
                            WoodBaleFee = result.wood_bale_fee,
                            RemoteDeliverFee = result.remote_deliver_fee,
                            DeliveryDate = result.delivery_date,
                            CreatedDate = DateTime.Now
                        };
                        db.tblKerryPostNewOrders.Add(kerryPostNewOrder);
                    }
                    else
                    {
                        kerryPostNewOrder.Status = result.status;
                        kerryPostNewOrder.Message = result.message;
                        kerryPostNewOrder.OrderNumber = result.order_number;
                        kerryPostNewOrder.Cost = result.cost;
                        kerryPostNewOrder.WoodBaleFee = result.wood_bale_fee;
                        kerryPostNewOrder.RemoteDeliverFee = result.remote_deliver_fee;
                        kerryPostNewOrder.DeliveryDate = result.delivery_date;
                        kerryPostNewOrder.CreatedDate = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Libraries.Log.Write("Lỗi: " + JsonConvert.SerializeObject(e.Message));
                // _Log.Error(e);
                // SELog.WriteLog("KerryService => PostNewOrder", e);
            }
            finally
            {

            }
        }


        public void OrderProgress(tblKerryOrderProgress kerryOrderProgress)
        {
            try
            {
                var packageId = 0;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var kerryStatusService = db.tblKerryStatusServices.FirstOrDefault(x => x.Id == kerryOrderProgress.StatusService);
                    var statusSeviceDesc = kerryStatusService?.Name;
                    kerryOrderProgress.StatusServiceDescription = statusSeviceDesc;
                    kerryOrderProgress.CreatedDate = DateTime.Now;
                    db.tblKerryOrderProgresses.Add(kerryOrderProgress);

                    var package = db.tblPackageInfoes.FirstOrDefault(x => x.Code.ToLower().Equals(kerryOrderProgress.OrderCode.ToLower()));
                    if (package != null)
                    {
                        packageId = package.id;
                    }
                    db.SaveChanges();
                }

                //kiểm tra trạng thái đang giao hàng
                if (kerryOrderProgress.StatusService.Equals("PUP"))
                {
                    // _Log.Info("Delivering");
                    Libraries.Log.WriteLogs("Delivering");
                    Task.Run(() => new OrderService().App_UpdateStatusPackage(packageId, (int)OrderStatusInfo.Delivering));
                }

                //kiểm tra trạng thái giao hàng thành công
                if (kerryOrderProgress.StatusService.Equals("POD"))
                {
                    Libraries.Log.WriteLogs("Completed");
                    //_Log.Info("Completed");
                    Task.Run(() => new OrderService().App_UpdateStatusPackage(packageId, (int)OrderStatusInfo.Delivered));
                }
            }
            catch (Exception ex)
            {
                //_Log.Error(ex);
                //SELog.WriteLog("KerryService => OrderProgress", ex);
                Libraries.Log.Write(ex.Message);
            }
        }

        private async Task PostRequestAsync(string apiUrl, string JsonParams)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var responseMessage = await client.PostAsync(apiUrl, new StringContent(JsonParams, Encoding.UTF8, "application/json"));
                    var responseJson = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    //return await Task.FromResult(JsonConvert.DeserializeObject<KerryExpress.PostNewOrderResponse>(responseJson));
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("KerryService => PostRequestAsync", ex);
                throw ex;
            }
        }
    }
}
