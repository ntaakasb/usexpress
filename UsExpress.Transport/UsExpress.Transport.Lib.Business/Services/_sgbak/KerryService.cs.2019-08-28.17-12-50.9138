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
using Newtonsoft.Json;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Services
{
    /// <summary>
    /// Author: BinhNQ
    /// </summary>
    public class KerryService :IKerryService
    {
        public void PostNewOrder(string OrderCode)
        {
            try
            {
                tblOrder order;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var kerryNewPost = db.tblKerryPostNewOrders.FirstOrDefault(x => x.OrderNumber.Equals(OrderCode));
                    if (kerryNewPost != null) //neu da co
                    {
                        return;
                    }
                    order = db.tblOrders.Where(x => x.Code.Equals(OrderCode))
                        .Include(x => x.tblPackageInfoes)
                        .Include("tblPackageInfoes.tblItemInPackages").First();
                    if (order != null)
                    {
                        //db.Entry(order).Reference(x => x.tblShippingInfo).Load();
                        //db.Entry(order).Collection(x => x.tblPackageInfoes).Load();
                        //db.Entry(order.tblPackageInfoes).Collection(x => x.).Load();
                        order.RecipientShippingInfo =
                            db.tblShippingInfoes.FirstOrDefault(x => x.Id == order.RecipientId);
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
                data.order_number = order.Code;
                data.waybill_number = order.Code;
                data.no_packs = "1";
                data.package_weight = "0";
                data.service_type = KerryExpress.Service.DichVuChuyenPhatNhanh;
                data.cod = string.Empty;
                data.order_note = string.Empty;

                data.receiver_address = new
                {
                    full_address = order.RecipientShippingInfo.AddressLine1,
                    province_area_code = order.RecipientShippingInfo.CityId,
                    district_area_code = order.RecipientShippingInfo.DistrictId,
                    ward_area_code = string.Empty,
                    contact_phone = order.RecipientShippingInfo.Phone,
                    contact_name = order.RecipientShippingInfo.FullName
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

                foreach (var orderTblPackageInfoe in order.tblPackageInfoes)
                {
                    foreach (var item in orderTblPackageInfoe.tblItemInPackages)
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
                var result =
                    Client.Post<KerryExpress.PostNewOrderResponse>(Constant.APPSETTING.Kerry.ApiUri, jsonParams);
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var kerryPostNewOrder =
                        db.tblKerryPostNewOrders.FirstOrDefault(x => x.OrderNumber.Equals(OrderCode));
                    if (kerryPostNewOrder == null)
                    {
                        kerryPostNewOrder = new tblKerryPostNewOrder()
                        {
                            OrderId = order.id,
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

            }
            finally
            {
                
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
                throw ex;
            }
        }
    }
}
