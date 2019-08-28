using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class OrderService : IOrderService
    {
        public long Admin_CreateOrder(tblOrder model)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // check phone sender
                            tblSender s = db.tblSenders.FirstOrDefault(x => x.Phone == model.SenderShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (s == null)
                            {
                                s = new tblSender
                                {
                                    Phone = model.SenderShippingInfo.Phone,
                                    Add1 = model.SenderShippingInfo.AddressLine1,
                                    Add2 = model.SenderShippingInfo.AddressLine2,
                                    CityId = model.SenderShippingInfo.CityId,
                                    FullName = model.SenderShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblSenders.Add(s);
                                db.SaveChanges();
                            }
                            model.SenderId = s.Id;

                            tblRecipientsInfo r = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == model.RecipientShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (r == null)
                            {
                                r = new tblRecipientsInfo
                                {
                                    Phone = model.RecipientShippingInfo.Phone,
                                    Add1 = model.RecipientShippingInfo.AddressLine1,
                                    Add2 = model.RecipientShippingInfo.AddressLine2,
                                    CityId = model.RecipientShippingInfo.CityId,
                                    DistrictId = model.RecipientShippingInfo.DistrictId,
                                    FullName = model.RecipientShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblRecipientsInfoes.Add(r);
                                db.SaveChanges();
                            }
                            model.RecipientId = r.id;
                            model.NotifyToCustomer = 0;
                            model.CreatedDate = model.CreateDate.UnixTimeStampToDateTime();
                            model.IsActive = true;
                            db.tblOrders.Add(model);
                            db.SaveChanges();

                            model.RecipientShippingInfo.OrderId = model.id;
                            model.RecipientShippingInfo.TypeUser = (int)OrderUserInfo.Recipient;
                            db.tblShippingInfoes.Add(model.RecipientShippingInfo);
                            db.SaveChanges();
                            model.SenderShippingInfo.OrderId = model.id;
                            model.SenderShippingInfo.TypeUser = (int)OrderUserInfo.Sender;
                            db.tblShippingInfoes.Add(model.SenderShippingInfo);
                            db.SaveChanges();
                            transaction.Commit();
                            result = model.id;
                        }
                        catch (Exception ex)
                        {
                            SELog.WriteLog("OrderService => Admin_CreateOrder with rollback", ex);
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_CreateOrder", ex);
            }
            return result;
        }
        public long Admin_CreateOrderApi(tblOrder model)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // check phone sender
                            tblRecipientsInfo r = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == model.RecipientShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (r == null)
                            {
                                r = new tblRecipientsInfo
                                {
                                    Phone = model.RecipientShippingInfo.Phone,
                                    Add1 = model.RecipientShippingInfo.AddressLine1,
                                    Add2 = model.RecipientShippingInfo.AddressLine2,
                                    CityId = model.RecipientShippingInfo.CityId,
                                    DistrictId = model.RecipientShippingInfo.DistrictId,
                                    FullName = model.RecipientShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblRecipientsInfoes.Add(r);
                                db.SaveChanges();
                            }
                            model.RecipientId = r.id;
                            model.NotifyToCustomer = 0;
                            model.CreatedDate = model.CreateDate.UnixTimeStampToDateTime();
                            model.IsActive = true;
                            db.tblOrders.Add(model);
                            db.SaveChanges();
                            model.RecipientShippingInfo.OrderId = model.id;
                            model.RecipientShippingInfo.TypeUser = (int)OrderUserInfo.Recipient;
                            db.tblShippingInfoes.Add(model.RecipientShippingInfo);
                            db.SaveChanges();
                            var shipInfo = db.tblSenders.Where(x => x.Id == model.SenderId).FirstOrDefault();
                            model.SenderShippingInfo = new tblShippingInfo();
                            model.SenderShippingInfo.AddressLine1 = shipInfo.Add1;
                            model.SenderShippingInfo.AddressLine2 = shipInfo.Add2;
                            model.SenderShippingInfo.CityId = shipInfo.CityId;
                            model.SenderShippingInfo.DistrictId = shipInfo.DistrictId;
                            model.SenderShippingInfo.Phone = shipInfo.Phone;
                            model.SenderShippingInfo.OrderId = model.id;
                            model.SenderShippingInfo.TypeUser = (int)OrderUserInfo.Sender;
                            db.tblShippingInfoes.Add(model.SenderShippingInfo);
                            db.SaveChanges();
                            transaction.Commit();
                            result = model.id;
                        }
                        catch (Exception ex)
                        {
                            SELog.WriteLog("OrderService => Admin_CreateOrder with rollback", ex);
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_CreateOrder", ex);
            }
            return result;
        }

        public tblOrder Admin_GetDetailOrderById(int id)
        {
            tblOrder result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblOrders
                        .Include(x => x.tblPackageInfoes)
                        .FirstOrDefault(x => x.id == id);
                    result.SenderShippingInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == result.id && x.TypeUser == (int)OrderUserInfo.Sender);
                    result.RecipientShippingInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == result.id && x.TypeUser == (int)OrderUserInfo.Recipient);
                    //db.Entry(result).Collection(x => x.tblPackageInfoes).Load();
                    //result.tblPackageInfoes = db.tblPackageInfoes.Include(y => y.tblItemInPackages).Where(x => x.OrderId == result.id).Select(x => x).ToList();
                    if (result.tblPackageInfoes != null && result.tblPackageInfoes.Any())
                    {
                        foreach (var item in result.tblPackageInfoes)
                        {
                            item.tblItemInPackages = db.tblItemInPackages.Where(x => x.PackageId == item.id).Include(x => x.tblCategory).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_GetDetailOrderById", ex);
            }
            return result;
        }
        public long Admin_UpdateOrder(tblOrder model)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            #region check phone sender, recipient
                            // check phone sender
                            tblSender sender = db.tblSenders.FirstOrDefault(x => x.Phone == model.SenderShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (sender == null)
                            {
                                sender = new tblSender
                                {
                                    Phone = model.SenderShippingInfo.Phone,
                                    Add1 = model.SenderShippingInfo.AddressLine1,
                                    Add2 = model.SenderShippingInfo.AddressLine2,
                                    CityId = model.SenderShippingInfo.CityId,
                                    FullName = model.SenderShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblSenders.Add(sender);
                                db.SaveChanges();
                            }
                            model.SenderId = sender.Id;

                            tblRecipientsInfo reciver = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == model.RecipientShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (reciver == null)
                            {
                                reciver = new tblRecipientsInfo
                                {
                                    Phone = model.RecipientShippingInfo.Phone,
                                    Add1 = model.RecipientShippingInfo.AddressLine1,
                                    Add2 = model.RecipientShippingInfo.AddressLine2,
                                    CityId = model.RecipientShippingInfo.CityId,
                                    DistrictId = model.RecipientShippingInfo.DistrictId,
                                    FullName = model.RecipientShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblRecipientsInfoes.Add(reciver);
                                db.SaveChanges();
                            }
                            model.RecipientId = reciver.id;
                            #endregion

                            var order = db.tblOrders.Where(x => x.id == model.id).First();
                            model.CreateDate = order.CreateDate;
                            model.CreatedDate = order.CreatedDate;
                            model.NotifyToCustomer = order.NotifyToCustomer;
                            model.IsActive = true;
                            db.Entry(order).CurrentValues.SetValues(model);
                            db.SaveChanges();

                            #region update shipping info
                            tblShippingInfo s = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == model.id && x.TypeUser == (int)OrderUserInfo.Sender);
                            model.SenderShippingInfo.OrderId = model.id;
                            model.SenderShippingInfo.TypeUser = (int)OrderUserInfo.Sender;
                            if (s != null)
                            {
                                db.Entry(s).CurrentValues.SetValues(model.SenderShippingInfo);
                            }
                            else
                            {
                                db.tblShippingInfoes.Add(model.SenderShippingInfo);
                            }
                            db.SaveChanges();

                            tblShippingInfo r = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == model.id && x.TypeUser == (int)OrderUserInfo.Recipient);
                            model.RecipientShippingInfo.OrderId = model.id;
                            model.RecipientShippingInfo.TypeUser = (int)OrderUserInfo.Recipient;
                            if (r != null)
                            {
                                db.Entry(r).CurrentValues.SetValues(model.RecipientShippingInfo);
                            }
                            else
                            {
                                db.tblShippingInfoes.Add(model.RecipientShippingInfo);
                            }
                            db.SaveChanges();
                            #endregion

                            #region update/ delete package

                            var tblOldPackage = db.tblPackageInfoes.Where(x => x.OrderId == model.id).Include(y => y.tblItemInPackages).ToList();
                            if (tblOldPackage != null && tblOldPackage.Any())
                            {
                                foreach (var op in tblOldPackage.ToList())
                                {
                                    var opDelete = model.tblPackageInfoes.FirstOrDefault(x => x.id == op.id);
                                    if (opDelete == null)
                                    {
                                        db.tblItemInPackages.RemoveRange(op.tblItemInPackages);
                                        db.SaveChanges();
                                        db.tblPackageInfoes.Remove(op);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            foreach (var pa in model.tblPackageInfoes)
                            {
                                var pM = db.tblPackageInfoes.Include(x => x.tblItemInPackages).FirstOrDefault(x => x.id == pa.id);
                                if (pM != null)
                                {
                                    pM.Depth = pa.Depth;
                                    pM.Fee = pa.Fee;
                                    pM.Height = pa.Height;
                                    pM.Ordinal = pa.Ordinal;
                                    pM.TotalItem = pa.TotalItem;
                                    pM.TotalValue = pa.TotalValue;
                                    pM.Weight = pa.Weight;
                                    pM.Width = pa.Width;

                                    foreach (var it in pM.tblItemInPackages.ToList())
                                    {
                                        var itDelete = pa.tblItemInPackages.FirstOrDefault(x => x.id == it.id);
                                        if (itDelete == null)
                                        {
                                            db.tblItemInPackages.Remove(it);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    pa.CreateTime = model.CreateDate;
                                    pa.OrderId = model.id;
                                    db.tblPackageInfoes.Add(pa);
                                }
                                db.SaveChanges();
                                foreach (var i in pa.tblItemInPackages)
                                {
                                    var iM = db.tblItemInPackages.FirstOrDefault(x => x.id == i.id);
                                    if (iM != null)
                                    {
                                        i.PackageId = pa.id;
                                        db.Entry(iM).CurrentValues.SetValues(i);
                                    }
                                    else
                                    {
                                        i.PackageId = pa.id;
                                        db.tblItemInPackages.Add(i);
                                    }
                                    db.SaveChanges();
                                }

                            }
                            #endregion

                            //db.SaveChanges();
                            transaction.Commit();
                            result = model.id;
                        }
                        catch (Exception ex)
                        {
                            SELog.WriteLog("OrderService => Admin_UpdateOrder with rollback", ex);
                            transaction.Rollback();

                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateOrder", ex);
            }
            return result;
        }

        public List<OrderViewDTO> Admin_SearchOrder(PackageSearch model)
        {
            List<OrderViewDTO> result = new List<OrderViewDTO>();
            if (string.IsNullOrEmpty(model.Keyword))
            {
                model.Keyword = "";
            }
            model.Keyword = model.Keyword.ToLower().ConvertToUnSign3();
            try
            {
                List<tblOrder> lstOrder = null;
                List<tblStoreAccount> lstStoreAcount = null;
                List<tblShippingInfo> lstShippingInfo = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lstOrder = db.tblOrders
                                    .Where(x => model.GetUnixTimestampFromDate <= x.CreateDate && x.CreateDate < model.GetUnixTimestampToDate)
                                    .Select(x => x).ToList();
                    switch (model.TypeSearch)
                    {
                        case 0://search Id         
                            lstOrder = lstOrder.Where(x => x.Code.ToLower().Contains(model.Keyword)).Select(x => x).ToList();
                            break;
                        case 1: // search sender
                            lstShippingInfo = db.tblShippingInfoes.Where(x => x.TypeUser == (int)OrderUserInfo.Sender && x.FullName.Contains(model.Keyword)).ToList();
                            if (lstShippingInfo != null && lstShippingInfo.Any())
                            {
                                lstOrder = (from s in lstShippingInfo join o in lstOrder on s.OrderId equals o.id select o).ToList();
                            }
                            else
                            {
                                lstOrder = new List<tblOrder>();
                            }
                            break;
                    }
                }
                model.Total = lstOrder.Count;
                if (model.Total > 0)
                {
                    lstOrder = lstOrder.Take(model.PageSize).Skip(model.PageIndex * model.PageSize).ToList();
                    lstStoreAcount = Order_GetLstStoreAcountInfo(lstOrder.Select(x => x.StoreId ?? 0).Distinct().ToList());
                    lstShippingInfo = Order_GetLstShippingInfo((int)OrderUserInfo.Sender, lstOrder.Select(x => x.id).ToList());
                    foreach (var item in lstOrder)
                    {
                        var storeAccount = lstStoreAcount.FirstOrDefault(x => x.id == item.StoreId);
                        var customerSender = lstShippingInfo.FirstOrDefault(x => x.OrderId == item.id);
                        if (storeAccount != null)
                        {
                            result.Add(new OrderViewDTO
                            {
                                Id = item.id,
                                Code = item.Code,
                                //CreateDate = ((double)item.CreateDate).UnixTimeStampToDateTime().ToString("dd/MM/yyy"),
                                TotalPackage = item.TotalPackage ?? 0,
                                Tracking = item.Tracking,
                                Destination = Common.ServiceHelper.GetDestionationCodeById(item.Destination ?? 0),
                                FullName = customerSender?.FullName,// sender.FullName,
                                Weight = item.Weight ?? 0,
                                CreateTime = item.CreateDate,
                                ShippingTime = item.ShippingDate ?? 0,
                                PickupTime = item.PickupDate ?? 0,
                                ClearCustomTime = item.ClearCustomDate ?? 0,
                                DeliverTime = item.Deliver ?? 0,
                                StatusName = ((OrderStatusInfo)item.Status).ToString(),
                                SenderName = storeAccount.FullName
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_SearchOrder", ex);
            }
            return result;
        }

        public List<tblSender> Order_GetLstSenderByLstId(List<int> lstId)
        {
            List<tblSender> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblSenders.Where(x => lstId.Contains(x.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Order_GetLstSenderByLstId", ex);
            }
            return result;
        }
        public List<tblShippingInfo> Order_GetLstShippingInfo(int typeUser, List<long> lstOrderId)
        {
            List<tblShippingInfo> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblShippingInfoes.Where(x => x.TypeUser == typeUser && lstOrderId.Contains(x.OrderId ?? 0)).ToList();
                    if (result != null && result.Any())
                    {
                        var lstId = result.Select(x => x.CityId).ToList();
                        switch ((OrderUserInfo)typeUser)
                        {
                            case OrderUserInfo.Recipient:
                                var lstCity = db.tblStateProvices.Where(x => lstId.Contains(x.Id) && x.CountryId == (int)Common.CountrySupport.VietNam).ToDictionary(x => x.Id);
                                tblStateProvice city = null;
                                foreach (var item in result)
                                {
                                    if (!string.IsNullOrEmpty(item.CityId))
                                    {
                                        lstCity.TryGetValue(item.CityId, out city);
                                        item.CityName = city?.Name;
                                    }
                                }
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Order_GetLstShippingInfo", ex);
                throw;
            }
            return result;
        }

        public List<tblStoreAccount> Order_GetLstStoreAcountInfo(List<int> lstStoreId)
        {
            List<tblStoreAccount> result = null;
            var key = "Order-" + string.Join("-", lstStoreId);
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (List<tblStoreAccount>)HTML;
            }
            else
            {
                try
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        result = db.tblStoreAccounts.Where(x => lstStoreId.Contains(x.id)).ToList();
                    }
                    MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(60));
                }
                catch (Exception ex)
                {
                    SELog.WriteLog("OrderService => Order_GetLstStoreAcountInfo", ex);
                }
            }
            return result;
        }
        private bool checkSearchText(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                source = "";
            }
            if (string.IsNullOrEmpty(target))
            {
                target = "";
            }
            source = source.ToLower().ConvertToUnSign3();
            target = target.ToLower().ConvertToUnSign3();
            return source.Contains(target);
        }

        public List<PackageViewDTO> Admin_SearchPackage(PackageSearch model)
        {
            List<PackageViewDTO> result = new List<PackageViewDTO>();
            if (string.IsNullOrEmpty(model.Keyword))
            {
                model.Keyword = "";
            }
            model.Keyword = model.Keyword.ToLower().ConvertToUnSign3();
            List<int> lstStatusId = new List<int>();
            if (!string.IsNullOrEmpty(model.LstStatus))
            {
                var lstId = model.LstStatus.Split(',');
                foreach (var ids in lstId)
                {
                    if (!string.IsNullOrEmpty(ids))
                    {
                        lstStatusId.Add(int.Parse(ids));
                    }
                }
                // hủy search all
                model.StatusId = 0;
            }
            else
            {
                model.StatusId = -1;
            }
            List<int> lstDestinationId = new List<int>();
            if (!string.IsNullOrEmpty(model.LstDestination))
            {
                var lstId = model.LstDestination.Split(',');
                foreach (var ids in lstId)
                {
                    if (!string.IsNullOrEmpty(ids))
                    {
                        lstDestinationId.Add(int.Parse(ids));
                    }
                }
                // hủy search all
                model.Destination = 0;
            }
            else
            {
                model.Destination = -1;
            }
            var toDate = DateTime.Now;
            if (model != null)
            {
                toDate = model.DToDate.AddDays(1);
            }
            try
            {
                List<tblOrder> lstOrder = null;
                List<tblPackageInfo> lstPackage = new List<tblPackageInfo>();
                List<tblStoreAccount> lstStoreAcount = null;
                List<tblShippingInfo> lstShippingInfo = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lstPackage = db.tblPackageInfoes.Include(p => p.tblItemInPackages)
                                    .Where(x => DbFunctions.TruncateTime(model.DFromDate) <= DbFunctions.TruncateTime(x.CreateDateLocal) && DbFunctions.TruncateTime(x.CreateDateLocal) < DbFunctions.TruncateTime(toDate)
                                        //&& (lstStatusId.Contains(x.Status) || model.StatusId == -1)
                                        //&& (lstDestinationId.Contains(x.Destination ?? 0) || model.Destination == -1)
                                        && (x.StoreId == model.StoreId || model.StoreId == -1)
                                        && (x.WarehouseId == model.WarehouseId || model.WarehouseId == -1) && x.Status != -1
                                        )
                                    .ToList();
                    var sql = lstPackage.ToString();
                    switch (model.HasShipment)
                    {
                        case 0:
                            lstPackage = lstPackage.Where(x => x.ShipmentId.HasValue == false).ToList();
                            break;
                        case 1:
                            lstPackage = lstPackage.Where(x => x.ShipmentId.HasValue == true).ToList();
                            break;
                        default:
                            break;
                    }
                    switch (model.TypeSearch)
                    {
                        case 0://search Id         
                            lstPackage = lstPackage.Where(x => x.Code.ToLower().Contains(model.Keyword)).ToList();
                            break;
                        case 1: // search sender
                            lstShippingInfo = db.tblShippingInfoes.Where(x => x.TypeUser == (int)OrderUserInfo.Sender && x.FullName.Contains(model.Keyword)).ToList();
                            if (lstShippingInfo != null && lstShippingInfo.Any())
                            {
                                lstPackage = (from s in lstShippingInfo join p in lstPackage on s.OrderId equals p.OrderId select p).ToList();
                            }
                            else
                            {
                                lstPackage = new List<tblPackageInfo>();
                            }
                            break;
                    }
                }
                model.Total = lstPackage.Count;
                if (model.Total > 0)
                {
                    int i = 0;
                    lstPackage = lstPackage.OrderByDescending(x => x.id).Skip(model.PageIndex * model.PageSize).ToList();
                    lstOrder = Package_GetLstOrderByOrderIds(lstPackage.Select(x => x.OrderId).Distinct().ToList());
                    lstStoreAcount = Order_GetLstStoreAcountInfo(lstOrder.Select(x => x.StoreId ?? 0).Distinct().ToList());
                    lstShippingInfo = Order_GetLstShippingInfo((int)OrderUserInfo.Sender, lstOrder.Select(x => x.id).ToList());
                    var lstShippingRecipientInfo = Order_GetLstShippingInfo((int)OrderUserInfo.Recipient, lstOrder.Select(x => x.id).ToList());
                    var dicWarehouse = Shipment_GetLstWareHouse_Dic(lstPackage.Select(x => x.WarehouseId ?? 0).Distinct().ToList());
                    var warehouser = new tblWarehouse();
                    foreach (var item in lstPackage)
                    {
                        var orderDetail = lstOrder.FirstOrDefault(x => x.id == item.OrderId && x.IsActive == model.IsActive);
                        if (orderDetail != null)
                        {
                            var storeAccount = lstStoreAcount.FirstOrDefault(x => x.id == orderDetail.StoreId);
                            var customerSender = lstShippingInfo.FirstOrDefault(x => x.OrderId == orderDetail.id);
                            var recipientInfo = lstShippingRecipientInfo.FirstOrDefault(x => x.OrderId == orderDetail.id);
                            dicWarehouse.TryGetValue(item.WarehouseId ?? 0, out warehouser);
                            if (storeAccount != null)
                            {
                                result.Add(new PackageViewDTO
                                {
                                    Id = item.id,
                                    Ordinal = item.Ordinal,
                                    OrderId = item.OrderId,
                                    Code = item.Code,
                                    OrderCode = orderDetail.Code,
                                    TotalItems = item.TotalItem,
                                    Tracking = item.Tracking,
                                    DestinationName = GetDestionationCodeById(item.Destination ?? 0),
                                    FullName = customerSender?.FullName,// sender.FullName,
                                    Weight = item.Weight ?? 0,
                                    CreateTime = item.CreateTime,
                                    ShippingTime = item.ShippingTime ?? 0,
                                    PickupTime = item.PickupTime ?? 0,
                                    ClearCustomTime = item.ClearCustomTime ?? 0,
                                    DeliverTime = item.DeliverTime ?? 0,
                                    StatusName = ((OrderStatusInfo)item.Status).ToString(),
                                    StoreName = storeAccount.StoreName,
                                    WarehouseId = item.WarehouseId ?? 0,
                                    Destination = item.Destination ?? 0,
                                    WarehouseName = warehouser?.Warehouse,
                                    StatusId = item.Status,
                                    CreateDateLocalString = item.CreateDateLocal.ToString(DataTimeFormatHelper.FormatDate_DDMMYYYY_HHMMSS),
                                    RecipientCityName = recipientInfo?.CityName,
                                    IsActive = orderDetail.IsActive,
                                    ShipmentCode = item.ShipmentCode,
                                    Items = item.tblItemInPackages.AsEnumerable().Select(B => new ItemPackageViewDTO
                                    {
                                        Quantity = B.Quantity.Value,
                                        Description = B.Description
                                    }).ToList()
                                });
                            }
                            i++;
                        }
                        if (i == model.PageSize) break;
                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_SearchPackage", ex);
            }
            return result;
        }
        private List<tblOrder> Package_GetLstOrderByOrderIds(List<Int64> lstIds)
        {
            List<tblOrder> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblOrders.Where(x => lstIds.Contains(x.id)).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Package_GetLstOrderByOrderIds", ex);
            }
            return result;
        }
        private Dictionary<int, tblWarehouse> Shipment_GetLstWareHouse_Dic(List<int> lstWarehouseId)
        {
            Dictionary<int, tblWarehouse> lsResult = new Dictionary<int, tblWarehouse>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblWarehouses
                                .Where(x => lstWarehouseId.Contains(x.id))
                                .ToDictionary(x => x.id);
                }
                if (lsResult != null && lsResult.Any())
                {

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Shipment_GetLstWareHouse_Dic", ex);
            }
            return lsResult;
        }

        public PackageViewDTO Admin_GetDetailPackageById(int packageId)
        {
            PackageViewDTO result = null;
            try
            {
                tblOrder orderDetail = null;
                tblPackageInfo packageDetail = null;
                tblStoreAccount storeAccount = null;
                List<tblItemInPackage> lstItemPackage = null;
                List<tblCategory> lstCategory = null;
                tblShippingInfo senderInfo = null;
                tblShippingInfo recipientInfo = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.id == packageId);
                    orderDetail = db.tblOrders.FirstOrDefault(x => x.id == packageDetail.OrderId);
                    lstItemPackage = db.tblItemInPackages.Where(x => x.PackageId == packageDetail.id).ToList();
                    storeAccount = db.tblStoreAccounts.FirstOrDefault(x => x.id == orderDetail.StoreId);
                    senderInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == packageDetail.OrderId && x.TypeUser == (int)OrderUserInfo.Sender);
                    recipientInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == packageDetail.OrderId && x.TypeUser == (int)OrderUserInfo.Recipient);
                    var str = "";
                    if (recipientInfo != null)
                    {
                        var WardName = db.tblWards.Where(x => x.id == recipientInfo.WardId).FirstOrDefault()?.WardName;
                        str += (!String.IsNullOrEmpty(WardName + "") ? "," + WardName : "");
                        var StateName = db.tblDistrictStateProvices.Where(x => x.Id == recipientInfo.DistrictId).FirstOrDefault().Name;
                        str += (!String.IsNullOrEmpty(StateName + "") ? "," + StateName : "");
                        var CityName = db.tblStateProvices.Where(x => x.Id == recipientInfo.CityId).FirstOrDefault().Name;
                        str += (!String.IsNullOrEmpty(CityName + "") ? "," + CityName : "");
                        recipientInfo.AddressLine1 += str;
                    }
                }
                if (packageDetail != null && packageDetail.id > 0 && orderDetail != null)
                {
                    lstCategory = ItemPackage_GetLstCategoryByLstIds(lstItemPackage.Select(x => x.CategoryId ?? 0).Distinct().ToList());
                    result = new PackageViewDTO
                    {
                        Id = packageDetail.id,
                        Ordinal = packageDetail.Ordinal,
                        OrderId = packageDetail.OrderId,
                        Code = packageDetail.Code,
                        OrderCode = orderDetail.Code,
                        TotalItems = packageDetail.TotalItem,
                        Tracking = packageDetail.Tracking,
                        DestinationName = Common.ServiceHelper.GetDestionationCodeById(packageDetail.Destination ?? 0),
                        Weight = packageDetail.Weight ?? 0,
                        Height = packageDetail.Height ?? 0,
                        Width = packageDetail.Width ?? 0,
                        Depth = packageDetail.Depth ?? 0,
                        CreateTime = packageDetail.CreateTime,
                        ShippingTime = packageDetail.ShippingTime ?? 0,
                        PickupTime = packageDetail.PickupTime ?? 0,
                        ClearCustomTime = packageDetail.ClearCustomTime ?? 0,
                        DeliverTime = packageDetail.DeliverTime ?? 0,
                        StatusName = ((OrderStatusInfo)packageDetail.Status).ToString(),
                        StatusId = packageDetail.Status,
                        StoreName = storeAccount.FullName,
                        Items = new List<ItemPackageViewDTO>(),
                        TotalPackage = orderDetail.TotalPackage ?? 1,
                        SenderInfo = senderInfo.Map<Models.ShippingInfoDTO>(),
                        RecipientInfo = recipientInfo.Map<Models.ShippingInfoDTO>(),
                        Destination = packageDetail.Destination ?? 0
                    };
                    foreach (var item in lstItemPackage)
                    {
                        var categoryDetail = lstCategory.FirstOrDefault(x => x.id == item.CategoryId);
                        if (categoryDetail != null)
                        {
                            ItemPackageViewDTO itemDTO = item.Map<ItemPackageViewDTO>();
                            itemDTO.Id = item.id;
                            itemDTO.CategoryCode = item.Code;
                            itemDTO.CategoryName = categoryDetail.CategoryName;
                            result.Items.Add(itemDTO);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_GetDetailPackageById", ex);
            }
            return result;
        }
        public tblPackageInfo PrintInvoice(int packageId = 0)
        {
            tblPackageInfo _PackageInfo = new tblPackageInfo();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _PackageInfo = db.tblPackageInfoes.Include(y => y.tblItemInPackages).Where(x => x.id == packageId).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                SELog.WriteLog("OrderService => PrintInvoice", e);
                Libraries.Log.Write("PrintInvoice: " + e.Message);
            }
            return _PackageInfo;
        }
        public OrderCustomDTO getOrderByID(long id)
        {
            OrderCustomDTO _order = new OrderCustomDTO();
            if (Constant.APPSETTING == null)
            {
                Constant.APPSETTING = new Business.Models.AppSetting();
            }
            using (var conn = new SqlConnection(Constant.APPSETTING.Database.ConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT O.ID,O.Code,O.CreateDate,O.Status,SA.Email EmailStore,SA.FullName NameStore,SD.Email EmailSender,SD.FullName NameSender,SD.Add1 Address1,SD.Add2 Address2,SD.Zip,SD.Phone,RI.FullName NameRecipient,RI.Add1 Address1Recipient,RI.Phone PhoneRecipient  FROM tblOrder O,tblStoreAccount SA,tblSender SD,tblRecipientsInfo RI WHERE O.ID=" + id + " AND O.StoreId=SA.id AND O.SenderId=SD.Id AND O.RecipientId=RI.id", conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    sda.Fill(dt);
                    _order = (from dt1 in dt.AsEnumerable()
                              select new OrderCustomDTO
                              {
                                  ID = dt1.Field<long>("ID"),
                                  Code = dt1.Field<string>("Code").ToString(),
                                  CreateDate = dt1.Field<long>("CreateDate"),
                                  EmailStore = dt1.Field<string>("EmailStore"),
                                  NameStore = dt1.Field<string>("NameStore"),
                                  EmailSender = dt1.Field<string>("EmailSender"),
                                  NameSender = dt1.Field<string>("NameSender"),
                                  Address1 = dt1.Field<string>("Address1"),
                                  Address2 = dt1.Field<string>("Address2"),
                                  Zip = dt1.Field<string>("Zip"),
                                  Phone = dt1.Field<string>("Zip"),
                                  NameRecipient = dt1.Field<string>("NameRecipient"),
                                  Address1Recipient = dt1.Field<string>("Address1Recipient"),
                                  PhoneRecipient = dt1.Field<string>("PhoneRecipient"),
                                  Status = dt1.Field<int>("Status")
                              }
                            ).FirstOrDefault();
                }
                catch (SqlException se)
                {
                    SELog.WriteLog("OrderService => getOrderByID", se);
                    Libraries.Log.Write(se.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return _order;
        }
        #region App
        public List<PackageViewDTO> App_SearchPackage(PackageSearchFromApp model)
        {
            List<PackageViewDTO> result = new List<PackageViewDTO>();
            try
            {
                List<tblOrder> lstOrder = null;
                List<tblPackageInfo> lstPackage = new List<tblPackageInfo>();
                List<tblStoreAccount> lstStoreAcount = null;
                List<tblShippingInfo> lstShippingInfo = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lstPackage = db.tblPackageInfoes
                                    .Where(x => DbFunctions.TruncateTime(model.FromDate) <= DbFunctions.TruncateTime(x.CreateDateLocal) && DbFunctions.TruncateTime(x.CreateDateLocal) <= DbFunctions.TruncateTime(model.ToDate)
                                            && (x.Status == model.StatusId || model.StatusId == -1)
                                            && x.StoreId == model.StoreId)
                                    .OrderBy(x => x.CreateTime)
                                    .ToList();
                }
                model.Total = lstPackage.Count;
                if (model.Total > 0)
                {
                    lstPackage = lstPackage.Skip(model.PageIndex * model.PageSize).Take(model.PageSize).ToList();
                    lstOrder = Package_GetLstOrderByOrderIds(lstPackage.Select(x => x.OrderId).Distinct().ToList());
                    lstStoreAcount = Order_GetLstStoreAcountInfo(lstOrder.Select(x => x.StoreId ?? 0).Distinct().ToList());
                    lstShippingInfo = Order_GetLstShippingInfo((int)OrderUserInfo.Sender, lstOrder.Select(x => x.id).ToList());
                    foreach (var item in lstPackage)
                    {
                        var orderDetail = lstOrder.FirstOrDefault(x => x.id == item.OrderId);
                        if (orderDetail != null)
                        {
                            var storeAccount = lstStoreAcount.FirstOrDefault(x => x.id == orderDetail.StoreId);
                            var customerSender = lstShippingInfo.FirstOrDefault(x => x.OrderId == orderDetail.id);
                            if (storeAccount != null)
                            {
                                result.Add(new PackageViewDTO
                                {
                                    Id = item.id,
                                    Ordinal = item.Ordinal,
                                    OrderId = item.OrderId,
                                    Code = item.Code,
                                    OrderCode = orderDetail.Code,
                                    TotalItems = item.TotalItem,
                                    Tracking = item.Tracking,
                                    DestinationName = Common.ServiceHelper.GetDestionationCodeById(item.Destination ?? 0),
                                    FullName = customerSender?.FullName,// sender.FullName,
                                    Weight = item.Weight ?? 0,
                                    CreateTime = item.CreateTime,
                                    ShippingTime = item.ShippingTime ?? 0,
                                    PickupTime = item.PickupTime ?? 0,
                                    ClearCustomTime = item.ClearCustomTime ?? 0,
                                    DeliverTime = item.DeliverTime ?? 0,
                                    StatusName = ((OrderStatusInfo)item.Status).ToString(),
                                    StatusId = item.Status,
                                    StoreName = storeAccount.StoreName
                                });
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => App_SearchPackage", ex);
            }
            return result;
        }

        public PackageViewDTO App_GetDetailPackageHasItemByCode(string packageCode)
        {
            PackageViewDTO result = null;
            if (string.IsNullOrEmpty(packageCode))
            {
                return result;
            }
            try
            {
                tblOrder orderDetail = null;
                tblPackageInfo packageDetail = null;
                tblStoreAccount storeAccount = null;
                List<tblItemInPackage> lstItemPackage = null;
                List<tblCategory> lstCategory = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.Code.ToLower().Equals(packageCode.ToLower()));
                    orderDetail = db.tblOrders.FirstOrDefault(x => x.id == packageDetail.OrderId);
                    lstItemPackage = db.tblItemInPackages.Where(x => x.PackageId == packageDetail.id).ToList();
                    storeAccount = db.tblStoreAccounts.FirstOrDefault(x => x.id == orderDetail.StoreId);
                }
                if (packageDetail != null && packageDetail.id > 0 && orderDetail != null)
                {
                    lstCategory = ItemPackage_GetLstCategoryByLstIds(lstItemPackage.Select(x => x.CategoryId ?? 0).Distinct().ToList());
                    result = new PackageViewDTO
                    {
                        Id = packageDetail.id,
                        Ordinal = packageDetail.Ordinal,
                        OrderId = packageDetail.OrderId,
                        Code = packageDetail.Code,
                        OrderCode = orderDetail.Code,
                        TotalItems = packageDetail.TotalItem,
                        Tracking = packageDetail.Tracking,
                        DestinationName = packageDetail.Destination.HasValue ? (packageDetail.Destination.Value == 1 ? "HN" : "HCM") : "HCM",
                        Weight = packageDetail.Weight ?? 0,
                        CreateTime = packageDetail.CreateTime,
                        ShippingTime = packageDetail.ShippingTime ?? 0,
                        PickupTime = packageDetail.PickupTime ?? 0,
                        ClearCustomTime = packageDetail.ClearCustomTime ?? 0,
                        DeliverTime = packageDetail.DeliverTime ?? 0,
                        StatusName = ((OrderStatusInfo)packageDetail.Status).ToString(),
                        StatusId = packageDetail.Status,
                        StoreName = storeAccount.FullName,
                        Items = new List<ItemPackageViewDTO>()
                    };
                    foreach (var item in lstItemPackage)
                    {
                        var categoryDetail = lstCategory.FirstOrDefault(x => x.id == item.CategoryId);
                        if (categoryDetail != null)
                        {
                            ItemPackageViewDTO itemDTO = item.Map<ItemPackageViewDTO>();
                            itemDTO.Id = item.id;
                            itemDTO.CategoryCode = item.Code;
                            itemDTO.CategoryName = categoryDetail.CategoryName;
                            result.Items.Add(itemDTO);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => App_GetDetailPackageHasItemByCode", ex);
            }
            return result;
        }

        private List<tblCategory> ItemPackage_GetLstCategoryByLstIds(List<int> lstIds)
        {
            List<tblCategory> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblCategories.Where(x => lstIds.Contains(x.id)).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => ItemPackage_GetLstCategoryByLstIds", ex);
            }
            return result;
        }

        public CustomeResultDTO App_UpdateStatusPackage(int packageId, int statusUpdate)
        {
            CustomeResultDTO result = new Models.Extension.CustomeResultDTO();

            List<HistoryPackage> _lstHistoryPackage = new List<HistoryPackage>();
            HistoryPackage _historyPackage = new HistoryPackage();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.id == packageId);
                    if (packageDetail != null)
                    {
                        if (!string.IsNullOrEmpty(packageDetail.HistoryTransport))
                        {
                            _lstHistoryPackage = JsonConvert.DeserializeObject<List<HistoryPackage>>(packageDetail.HistoryTransport);
                        }

                        _lstHistoryPackage.Add(new HistoryPackage { Status = statusUpdate, CreateTime = DateTime.Now });
                        if (CheckFlowUpdateStatusFromApp(packageDetail.Status, statusUpdate))
                        {
                            packageDetail.Status = statusUpdate;
                            if (statusUpdate == (int)OrderStatusInfo.PickedUp)
                            {
                                packageDetail.PickupDateLocal = DateTime.Now;
                                packageDetail.PickupTime = DateTime.UtcNow.ToUnixTimestamp();
                            }
                            else if (statusUpdate == (int)OrderStatusInfo.SendToVN)
                            {
                                packageDetail.ShippingDateLocal = DateTime.Now;
                                packageDetail.ShippingTime = DateTime.UtcNow.ToUnixTimestamp();
                            }
                            else if (statusUpdate == (int)OrderStatusInfo.ClearCustom)
                            {
                                packageDetail.ClearCustomTime = DateTime.UtcNow.ToUnixTimestamp();
                            }
                            else if (statusUpdate == (int)OrderStatusInfo.Delivered)
                            {
                                packageDetail.DeliverTime = DateTime.UtcNow.ToUnixTimestamp();
                            }
                            packageDetail.HistoryTransport = JsonConvert.SerializeObject(_lstHistoryPackage);
                            packageDetail.NotifyToCustomer = 0;

                            db.SaveChanges();
                            result.Result = 1;
                            result.Message = "Package update success.";
                        }
                        else
                        {
                            result.Message = "Package's status current had changed. Please check again.";
                        }
                    }
                    else
                    {
                        result.Message = "Package not exist";
                    }
                }


            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => App_UpdateStatusPackage", ex);
                result.Message = ex.Message;
            }
            finally
            {
                _historyPackage = null;
            }
            return result;
        }

        public List<StoreWithPackageViewDTO> App_SearchStoreWithCountPackage(PackageSearchFromApp model)
        {
            List<StoreWithPackageViewDTO> result = new List<StoreWithPackageViewDTO>();
            if (string.IsNullOrEmpty(model.StoreName))
            {
                model.StoreName = "";
            }
            model.StoreName = model.StoreName.ToLower().ConvertToUnSign3();
            try
            {
                List<tblStoreAccount> lstStoreAcount = null;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var lstStoreId = db.tblStoreAccounts.Where(x => (x.WarehouseId == model.WarehouseId || model.WarehouseId == -1)
                                                                    && x.StoreName.Contains(model.StoreName))
                                                        .Select(x => x.id).Distinct().ToList();

                    result = db.tblPackageInfoes
                                    .Where(x => model.GetTimeFromDate <= x.CreateTime && x.CreateTime < model.GetTimeToDate
                                           && lstStoreId.Contains(x.StoreId)
                                        )
                                     .GroupBy(x => new { x.StoreId, x.WarehouseId })
                                     .Select(g => new StoreWithPackageViewDTO
                                     {
                                         WarehouseId = g.Key.WarehouseId ?? 0,
                                         StoreId = g.Key.StoreId,
                                         TotalPackageNew = g.Count(x => x.Status == (int)OrderStatusInfo.New),
                                         StatusNew = (int)OrderStatusInfo.New,
                                         TotalPackagePickedUp = g.Count(x => x.Status == (int)OrderStatusInfo.PickedUp),
                                         StatusPickedUp = (int)OrderStatusInfo.PickedUp,
                                         TotalPakkageSendToVn = g.Count(x => x.Status == (int)OrderStatusInfo.SendToVN),
                                         StatusSendToVn = (int)OrderStatusInfo.SendToVN
                                     })
                                    .ToList();
                }
                model.Total = result.Count;
                if (model.Total > 0)
                {
                    result = result.Skip(model.PageIndex * model.PageSize).Take(model.PageSize).ToList();
                    lstStoreAcount = Order_GetLstStoreAcountInfo(result.Select(x => x.StoreId).Distinct().ToList());
                    foreach (var item in result)
                    {
                        item.StoreName = lstStoreAcount.FirstOrDefault(x => x.id == item.StoreId)?.StoreName;
                    }
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => App_SearchStoreWithCountPackage", ex);
            }
            return result;
        }

        #endregion

        public string GetDestionationCodeById(int destinationId)
        {
            string result = "";
            switch ((Common.DestinationAirPort)destinationId)
            {
                case Common.DestinationAirPort.HN:
                    result = Common.DestinationAirPortCode.HN_AIRPORT;
                    break;
                default:
                    result = Common.DestinationAirPortCode.HCM_AIRPORT;
                    break;
            }
            return result;
        }

        private bool CheckFlowUpdateStatusFromApp(int statusCurrent, int statusNext)
        {
            bool result = false;
            switch ((OrderStatusInfo)statusCurrent)
            {
                case OrderStatusInfo.New:
                    switch ((OrderStatusInfo)statusNext)
                    {
                        case OrderStatusInfo.RejectPickup:
                        case OrderStatusInfo.PickedUp:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case OrderStatusInfo.PickedUp:
                    switch ((OrderStatusInfo)statusNext)
                    {
                        case OrderStatusInfo.WrongItem:
                        case OrderStatusInfo.ProhibitedItem:
                        case OrderStatusInfo.SendToVN:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case OrderStatusInfo.SendToVN:
                    switch ((OrderStatusInfo)statusNext)
                    {
                        case OrderStatusInfo.Damage:
                        case OrderStatusInfo.Good:
                        case OrderStatusInfo.Lost:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case OrderStatusInfo.Good:
                    switch ((OrderStatusInfo)statusNext)
                    {
                        case OrderStatusInfo.Delivering:
                        case OrderStatusInfo.Delivered:
                        case OrderStatusInfo.Failed:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public CustomeResultDTO Admin_UpdateStatusPackage(int packageId, int statusUpdate)
        {
            CustomeResultDTO result = new Models.Extension.CustomeResultDTO();

            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.id == packageId);
                    if (packageDetail != null)
                    {
                        packageDetail.Status = statusUpdate;
                        if (statusUpdate == (int)OrderStatusInfo.PickedUp)
                        {
                            packageDetail.PickupDateLocal = DateTime.Now;
                            packageDetail.PickupTime = DateTime.UtcNow.ToUnixTimestamp();
                        }
                        else if (statusUpdate == (int)OrderStatusInfo.SendToVN)
                        {
                            packageDetail.ShippingDateLocal = DateTime.Now;
                            packageDetail.ShippingTime = DateTime.UtcNow.ToUnixTimestamp();
                        }
                        else if (statusUpdate == (int)OrderStatusInfo.ClearCustom)
                        {
                            packageDetail.ClearCustomTime = DateTime.UtcNow.ToUnixTimestamp();
                        }
                        else if (statusUpdate == (int)OrderStatusInfo.Delivered)
                        {
                            packageDetail.DeliverTime = DateTime.UtcNow.ToUnixTimestamp();
                        }
                        db.SaveChanges();
                        result.Result = 1;
                        result.Message = "Package update success.";
                    }
                    else
                    {
                        result.Message = "Package not exist";
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateStatusPackage", ex);
                result.Message = ex.Message;
            }

            return result;
        }

        public CustomeResultDTO Admin_UpdateListStatusPackageSingle(string lstPackageId, int statusUpdate)
        {
            CustomeResultDTO result = new Models.Extension.CustomeResultDTO();

            var id = lstPackageId.Split(',').ToList();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (id.Count() > 0)
                    {
                        for (int i = 0; i < id.Count(); i++)
                        {
                            var _packageCode =Convert.ToInt32( id[i]);
                            var packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.id == _packageCode);
                            if (packageDetail != null)
                            {//update them parentID
                                packageDetail.Status = statusUpdate;
                                if (statusUpdate == (int)OrderStatusInfo.PickedUp)
                                {
                                    packageDetail.PickupDateLocal = DateTime.Now;
                                    packageDetail.PickupTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.SendToVN)
                                {
                                    packageDetail.ShippingDateLocal = DateTime.Now;
                                    packageDetail.ShippingTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.ClearCustom)
                                {
                                    packageDetail.ClearCustomTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.Delivered)
                                {
                                    packageDetail.DeliverTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                            }
                            else
                            {
                                result.Message = "Package not exist";
                            }
                        }

                        db.SaveChanges();
                        result.Result = 1;
                        result.Message = "Package update success.";
                    }
                }
                    
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateStatusPackage", ex);
                result.Message = ex.Message;
            }
            id = null;
            return result;
        }
        public CustomeResultDTO Admin_UpdateListStatusPackage(string lstPackageId, int packageId, int statusUpdate)
        {
            CustomeResultDTO result = new Models.Extension.CustomeResultDTO();

            var id = lstPackageId.Split(',').ToList();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _Code = db.tblPackageInfoes.FirstOrDefault(x => x.id == packageId)?.Code;
                    if (!String.IsNullOrEmpty(_Code))
                        id.Add(_Code + "");
                    if (id.Count() > 0)
                    {
                        for (int i = 0; i < id.Count(); i++)
                        {
                            var _packageCode = id[i];
                            var packageDetail = db.tblPackageInfoes.FirstOrDefault(x => x.Code == _packageCode);
                            if (packageDetail != null)
                            {//update them parentID
                                packageDetail.Status = statusUpdate;
                                if (statusUpdate == (int)OrderStatusInfo.PickedUp)
                                {
                                    packageDetail.PickupDateLocal = DateTime.Now;
                                    packageDetail.PickupTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.SendToVN)
                                {
                                    packageDetail.ShippingDateLocal = DateTime.Now;
                                    packageDetail.ShippingTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.ClearCustom)
                                {
                                    packageDetail.ClearCustomTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                else if (statusUpdate == (int)OrderStatusInfo.Delivered)
                                {
                                    packageDetail.DeliverTime = DateTime.UtcNow.ToUnixTimestamp();
                                }
                                if (_packageCode != _Code)
                                {
                                    packageDetail.ParentId = packageId;
                                }
                            }
                            else
                            {
                                result.Message = "Package not exist";
                            }
                        }

                        db.SaveChanges();
                        result.Result = 1;
                        result.Message = "Package update success.";
                    }

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateStatusPackage", ex);
                result.Message = ex.Message;
            }
            id = null;
            return result;
        }
        public List<ShipmentPropose> Admin_CheckPackageRecipient(int packageId)
        {

            List<ShipmentPropose> lstPackageId = new List<ShipmentPropose>();
            try
            {
                var xSql = " select PIF.id, PIF.Code, PIF.CreateDate, RI.FullName RecipientName,OD.Weight, OD.SenderId, OD.RecipientId from tblPackageInfo PIF left join tblOrder OD on PIF.OrderId = OD.id left join tblRecipientsInfo RI on OD.RecipientId = RI.id where RecipientId=(select RecipientId from tblPackageInfo PIF left join tblOrder OD on PIF.OrderId = OD.id where PIF.id=" + packageId + ") and OD.IsActive = 1 and OD.Status!=-1 and PIF.Status<2";
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lstPackageId = db.Database.SqlQuery<ShipmentPropose>(xSql).ToList();
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_CheckPackageRecipient", ex);
            }
            return lstPackageId;
        }
        public int CountOrderInDay(DateTime refDate, int storeId)
        {
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    DateTime startDate = new DateTime(refDate.Year, refDate.Month, refDate.Day, 0, 0, 0);
                    DateTime endDate = startDate.AddHours(24);

                    long startPoint = startDate.ToUnixTimestamp();
                    long endPoint = endDate.ToUnixTimestamp();
                    Libraries.Log.WriteInfo("startDate:" + startDate.ToString() + " endDate:" + endDate + " startPoint:" + startPoint + "endPoint:" + endPoint);
                    var total = db.tblOrders.Where(x => x.CreateDate >= startPoint && x.CreateDate <= endPoint && x.StoreId == storeId).Select(x => x.id).Count();
                    //var total = db.tblOrders.Where(x => ((x.CreateDate - startPoint) >= 0) && ((endPoint - x.CreateDate) >= 0) && x.StoreId == storeId).Select(x => x.id).Count();
                    Libraries.Log.WriteInfo(" total:" + total);
                    return total;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateStatusPackage", ex);
                return 0;
            }
        }

        public List<ReportOrderModel> getReportOrder(int storeId, int WarehouseId, DateTime fromdate, DateTime todate, int isActive, int pageSize, int pageIndex)
        {
            List<ReportOrderModel> lstResult = new List<ReportOrderModel>();
            DatabaseClass objData = new DatabaseClass();
            QueryString objQuery = new QueryString();

            long loFromDate = fromdate.ToUnixTimestamp();
            TimeSpan ts = new TimeSpan(23, 59, 59);
            long loToDate = (todate.Date + ts).ToUnixTimestamp();

            objQuery.strSPName = "SP_REPORT_ORDER";
            objQuery.AddInt("@StoreId", storeId);
            objQuery.AddInt("@WarehouseId", WarehouseId);
            objQuery.AddLong("@Fromdate", loFromDate);
            objQuery.AddLong("@Todate", loToDate);
            objQuery.AddInt("@IsActive", isActive);
            objQuery.AddInt("@PAGESIZE", pageSize);
            objQuery.AddInt("@PAGEINDEX", pageIndex);
            string strSqlStore = objQuery.GenSPString();
            LibCommon common = new LibCommon();
            try
            {
                var dt = objData.ExeDataset(strSqlStore).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    lstResult = objData.ExeDataset(strSqlStore).Tables[0].ToGenericList<ReportOrderModel>();
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => getReportOrder", ex);
                lstResult = null;
            }
            finally
            {
                objData.CloseData();
                objData = null;
                common = null;
            }
            return lstResult;
        }

        public bool SetActiveOrder(int idOrder)
        {
            bool isError = true;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var tblOrder = db.tblOrders.FirstOrDefault(s => s.id == idOrder);
                    if (tblOrder != null)
                    {
                        tblOrder.IsActive = !tblOrder.IsActive;
                        isError = db.SaveChanges() < 1;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("OrderService => Admin_UpdateStatusPackage", ex);
                isError = true;
            }
            return isError;
        }
    }
}
