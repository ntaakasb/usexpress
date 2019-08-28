using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IOrderService _orderService;
        public ShipmentService(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public List<tblPackageInfo> listPackageByShipmentCode(string ShipmentCode)
        {
            List<tblPackageInfo> lsResult = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblPackageInfoes.Where(x => x.ShipmentCode == ShipmentCode).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => listPackageByShipmentCode", ex);
            }
            return lsResult;
        }

        public List<tblPackageInfo> listPackageInday()
        {
            List<tblPackageInfo> result = null;
            try
            {
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => listPackageInday", ex);
            }
            return null;
        }

        public List<ShipmentPropose> LstShipment()
        {
            List<ShipmentPropose> lsResult = null;
            var xSql = "select PIF.id, PIF.Code, PIF.CreateDate, SD.FullName SenderName, RI.FullName RecipientName, OD.Weight, OD.SenderId, OD.RecipientId,(CASE WHEN SP.AirPortCode IS NOT NULL THEN SP.AirPortCode ELSE '' END) AirPortCode, REPLACE(REPLACE(STUFF((SELECT B.Description FROM tblItemInPackage B(NOLOCK) WHERE PIF.id = B.PackageId FOR XML PATH('')), 1, 13, ''), '</Description><Description>', ', '), '</Description>', '') AS Items from tblPackageInfo PIF left join tblOrder OD on PIF.OrderId = OD.id left join tblSender SD on OD.SenderId = SD.Id left join tblRecipientsInfo RI on OD.RecipientId = RI.id left join blStateProvice SP on OD.RecipientId = SP.Id where PIF.ShipmentId is null and OD.IsActive = 1 order by OD.RecipientId, PIF.Weight desc";
            try
            {
                using (var dbContext = ContextFactory.UsTransportEntities())
                {
                    lsResult = dbContext.Database.SqlQuery<ShipmentPropose>(xSql).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => LstShipment", ex);
            }
            return lsResult;
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
                SELog.WriteLog("ShipmentService => Package_GetLstOrderByOrderIds", ex);
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
                SELog.WriteLog("ShipmentService => Order_GetLstShippingInfo", ex);
                throw;
            }
            return result;
        }
        public List<ShipmentViewDTO> Admin_SearchShipment(ShipmentSearch model = null)
        {
            if (model == null)
            {
                model = new ShipmentSearch
                {
                    WarehouseId = -1,
                    Keyword = ""
                };
            }
            List<ShipmentViewDTO> lsResult = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblShipments
                                .Where(x => !x.IsDeleted
                                && (x.WarehouseId == model.WarehouseId || model.WarehouseId == -1)
                                && (x.CreateTime <= model.ToTime && x.CreateTime >= model.FromTime)
                                )
                                .Select(x => new ShipmentViewDTO
                                {
                                    Destination = x.Destination ?? 0,
                                    Id = x.Id,
                                    ShipmentCode = x.ShipmentCode,
                                    TotalWeight = x.TotalWeight ?? 0,
                                    WarehouseId = x.WarehouseId ?? 0,
                                    CreateTime = x.CreateTime ?? 0
                                }).ToList();
                }
                if (lsResult != null && lsResult.Any())
                {
                    var dicPackage = Shipment_GetLstPackage_Dic(lsResult.Select(x => x.Id).Distinct().ToList());
                    var dicWarehouser = Shipment_GetLstWareHouse_Dic(lsResult.Select(x => x.WarehouseId).Distinct().ToList());
                    var lstPackage = new List<tblPackageInfo>();
                    var warehouse = new tblWarehouse();
                    foreach (var item in lsResult)
                    {
                        dicPackage.TryGetValue(item.Id, out lstPackage);
                        dicWarehouser.TryGetValue(item.WarehouseId, out warehouse);
                        item.WarehouseName = warehouse?.Warehouse;
                        if (lstPackage != null)
                        {
                            var lstOrder = Package_GetLstOrderByOrderIds(lstPackage.Select(x => x.OrderId).Distinct().ToList());
                            var lstShippingRecipientInfo = Order_GetLstShippingInfo((int)OrderUserInfo.Recipient, lstOrder.Select(x => x.id).ToList());
                            if (lstPackage != null && lstPackage.Any())
                            {
                                item.LstPackage = new List<PackageViewDTO>();
                                var dicItem = Package_GetLstItem_Dic(lstPackage.Select(x => x.id).Distinct().ToList());
                                var lstStoreAcount = _orderService.Order_GetLstStoreAcountInfo(lstPackage.Select(x => x.StoreId).Distinct().ToList());

                                foreach (var p in lstPackage)
                                {
                                    var recipientInfo = lstShippingRecipientInfo.FirstOrDefault(x => x.OrderId == p.OrderId);
                                    var op = p.Map<PackageViewDTO>();
                                    op.Id = p.id;
                                    op.StatusId = p.Status;
                                    op.Destination = p.Destination ?? 0;
                                    op.DestinationName = Common.ServiceHelper.GetDestionationCodeById(p.Destination ?? 0);
                                    op.StatusName = ((Models.Extension.Constant.OrderStatusInfo)p.Status).ToString();
                                    op.StoreName = lstStoreAcount.FirstOrDefault(x => x.id == p.StoreId)?.FullName;
                                    op.CreateTime = p.CreateTime;
                                    op.ShipmentId = p.ShipmentId ?? 0;
                                    op.WarehouseId = p.WarehouseId ?? 0;
                                    dicWarehouser.TryGetValue(op.WarehouseId, out warehouse);
                                    op.WarehouseName = warehouse?.Warehouse;
                                    op.RecipientCityName = recipientInfo?.CityName;
                                    var lstItem = new List<tblItemInPackage>();
                                    dicItem.TryGetValue(p.id, out lstItem);
                                    op.Items = new List<ItemPackageViewDTO>();
                                    if (lstItem != null && lstItem.Any())
                                    {
                                        foreach (var i in lstItem)
                                        {
                                            var idto = i.Map<ItemPackageViewDTO>();
                                            idto.CategoryCode = i.Code;
                                            op.Items.Add(idto);
                                        }
                                    }
                                    item.LstPackage.Add(op);
                                }
                                item.TotalPackage = item.LstPackage.Count();
                                item.DestinationName = Common.ServiceHelper.GetDestionationCodeById(item.Destination);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_SearchShipment", ex);
            }
            return lsResult;

        }
        private Dictionary<int, List<tblPackageInfo>> Shipment_GetLstPackage_Dic(List<int> lstId)
        {
            Dictionary<int, List<tblPackageInfo>> lsResult = new Dictionary<int, List<tblPackageInfo>>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblPackageInfoes
                                .Where(x => x.ShipmentId.HasValue && lstId.Contains(x.ShipmentId ?? 0))
                                .GroupBy(x => x.ShipmentId ?? 0)
                                .ToDictionary(g => g.Key, g => g.ToList());
                }
                if (lsResult != null && lsResult.Any())
                {

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Shipment_GetLstPackage_Dic", ex);
            }
            return lsResult;
        }
        private Dictionary<int, tblWarehouse> Shipment_GetLstWareHouse_Dic(List<int> lstWarehouseId)
        {
            Dictionary<int, tblWarehouse> lsResult = new Dictionary<int, tblWarehouse>();
            var key = "Shipment-" + string.Join("-", lstWarehouseId);
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (Dictionary<int, tblWarehouse>)HTML;
            }
            else
            {
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
                    MemoryCache.Default.Add(key, lsResult, DateTimeOffset.UtcNow.AddMinutes(60));
                }
                catch (Exception ex)
                {
                    SELog.WriteLog("ShipmentService => Shipment_GetLstWareHouse_Dic", ex);
                }
            }
            return lsResult;
        }
        public int Admin_CreateShipment(tblShipment model)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblShipments.Add(model);
                    db.SaveChanges();
                    result = model.Id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_CreateShipment", ex);
            }
            return result;
        }
        public int Admin_UpdateShipment(tblShipment model)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var curent = db.tblShipments.FirstOrDefault(x => x.Id == model.Id);
                    if (curent != null)
                    {
                        db.Entry(curent).CurrentValues.SetValues(model);
                        db.SaveChanges();
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_UpdateShipment", ex);
            }
            return result;
        }
        public int Admin_UpdateShipmentForPackage(List<long> lstPackagageId, int? shipmentId, string shipmentCode)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    foreach (var id in lstPackagageId)
                    {
                        var item = db.tblPackageInfoes.FirstOrDefault(x => x.id == id);
                        if (item != null)
                        {
                            item.ShipmentCode = shipmentCode;
                            item.ShipmentId = shipmentId;
                            //db.Entry(item).CurrentValues.SetValues(item);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_UpdateShipmentForPackage", ex);
            }
            return result;
        }

        public ShipmentViewDTO Admin_GeDetailShipmentHasPackageById(int shipmentId)
        {
            ShipmentViewDTO result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var detail = db.tblShipments.FirstOrDefault(x => x.Id == shipmentId);
                    if (detail != null)
                    {
                        result = new ShipmentViewDTO
                        {
                            Destination = detail.Destination ?? 0,
                            Id = detail.Id,
                            ShipmentCode = detail.ShipmentCode,
                            TotalWeight = detail.TotalWeight ?? 0,
                            WarehouseId = detail.WarehouseId ?? 0
                        };
                    }
                }
                if (result != null)
                {
                    var dicPackage = Shipment_GetLstPackage_Dic(new List<int> { result.Id });
                    var lstPackage = new List<tblPackageInfo>();
                    dicPackage.TryGetValue(result.Id, out lstPackage);
                    if (lstPackage != null && lstPackage.Any())
                    {
                        result.LstPackage = new List<PackageViewDTO>();
                        var lstStoreAcount = _orderService.Order_GetLstStoreAcountInfo(lstPackage.Select(x => x.StoreId).Distinct().ToList());
                        foreach (var p in lstPackage)
                        {
                            var op = p.Map<PackageViewDTO>();
                            op.Id = p.id;
                            op.StatusId = p.Status;
                            op.Destination = p.Destination ?? 0;
                            op.DestinationName = Common.ServiceHelper.GetDestionationCodeById(p.Destination ?? 0);
                            op.StatusName = ((Models.Extension.Constant.OrderStatusInfo)p.Status).ToString();
                            op.StoreName = lstStoreAcount.FirstOrDefault(x => x.id == p.StoreId)?.FullName;
                            op.CreateTime = p.CreateTime;
                            op.ShipmentId = p.ShipmentId ?? 0;
                            op.WarehouseId = p.WarehouseId ?? 0;
                            result.LstPackage.Add(op);
                        }
                        result.TotalPackage = result.LstPackage.Count();
                        result.DestinationName = Common.ServiceHelper.GetDestionationCodeById(result.Destination);
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_GeDetailShipmentHasPackageById", ex);
            }
            return result;
        }
        public int Admin_CountShipmentOnDateByDestination(int destinationId)
        {
            int result = 0;
            long startTime = DateTime.Now.Date.ToUnixTimestamp();
            long endTime = DateTime.Now.Date.AddDays(1).ToUnixTimestamp();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblShipments.Where(x => x.Destination == destinationId
                                                        && startTime <= x.CreateTime
                                                        && x.CreateTime < endTime).Count();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_CountShipmentOnDateByDestination", ex);
            }
            return result;
        }
        public int Admin_RemoveLstShipmentId(List<int> lstId)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in lstId)
                            {
                                var detail = db.tblShipments.FirstOrDefault(x => x.Id == item);
                                if (detail != null)
                                {
                                    detail.IsDeleted = true;
                                    db.Entry(detail).CurrentValues.SetValues(detail);
                                    db.SaveChanges();

                                    var lst = db.tblPackageInfoes.Where(x => x.ShipmentId == item).ToList();
                                    if (lst != null && lst.Any())
                                    {
                                        foreach (var p in lst)
                                        {
                                            p.ShipmentCode = string.Empty;
                                            p.ShipmentId = null;
                                            db.Entry(p).CurrentValues.SetValues(p);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            result = 1;

                        }
                        catch (Exception ex)
                        {
                            SELog.WriteLog("ShipmentService => Admin_RemoveLstShipmentId with rollback", ex);
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_RemoveLstShipmentId", ex);
            }
            return result;
        }
        public int Admin_RemovePackageInShipment(List<int> lstId)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in lstId)
                            {
                                var lst = db.tblPackageInfoes.Where(x => x.id == item).ToList();
                                if (lst != null && lst.Any())
                                {
                                    foreach (var p in lst)
                                    {
                                        p.ShipmentCode = string.Empty;
                                        p.ShipmentId = null;
                                        db.Entry(p).CurrentValues.SetValues(p);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            transaction.Commit();
                            result = 1;

                        }
                        catch (Exception ex)
                        {
                            SELog.WriteLog("ShipmentService => Admin_RemovePackageInShipment with rollback", ex);
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_RemovePackageInShipment", ex);
            }
            return result;
        }
        public List<ShipmentViewDTO> Admin_SearchShipmentByCondition(string keyword, DateTime FromDate, DateTime ToDate)
        {
            List<ShipmentViewDTO> lsResult = null;
            try
            {
                double douFromdate = FromDate.ToUnixTimestamp();
                double douTodate = ToDate.Date.AddDays(1).ToUnixTimestamp();
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblShipments
                                .Where(x => !x.IsDeleted
                                && (keyword == null || x.ShipmentCode.Contains(keyword))
                                && x.CreateTime >= douFromdate
                                && x.CreateTime <= douTodate
                                )
                                .Select(x => new ShipmentViewDTO
                                {
                                    Destination = x.Destination ?? 0,
                                    Id = x.Id,
                                    ShipmentCode = x.ShipmentCode,
                                    TotalWeight = x.TotalWeight ?? 0,
                                    WarehouseId = x.WarehouseId ?? 0,
                                    CreateTime = x.CreateTime ?? 0
                                }).ToList();
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_SearchShipmentByCondition", ex);
            }
            return lsResult;

        }

        public List<ShipmentViewDTO> Admin_ExportShipmentByLstId(List<int> lstId)
        {
            List<ShipmentViewDTO> lsResult = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblShipments
                                .Where(x => !x.IsDeleted
                                    && lstId.Contains(x.Id))
                                .Select(x => new ShipmentViewDTO
                                {
                                    Destination = x.Destination ?? 0,
                                    Id = x.Id,
                                    ShipmentCode = x.ShipmentCode,
                                    TotalWeight = x.TotalWeight ?? 0,
                                    WarehouseId = x.WarehouseId ?? 0,
                                    CreateTime = x.CreateTime ?? 0
                                }).ToList();
                }
                if (lsResult != null && lsResult.Any())
                {
                    var dicPackage = Shipment_GetLstPackage_Dic(lsResult.Select(x => x.Id).Distinct().ToList());
                    var lstPackage = new List<tblPackageInfo>();
                    foreach (var item in lsResult)
                    {
                        dicPackage.TryGetValue(item.Id, out lstPackage);
                        if (lstPackage != null && lstPackage.Any())
                        {
                            item.LstPackage = new List<PackageViewDTO>();
                            var lstStoreAcount = _orderService.Order_GetLstStoreAcountInfo(lstPackage.Select(x => x.StoreId).Distinct().ToList());
                            var dicItem = Package_GetLstItem_Dic(lstPackage.Select(x => x.id).Distinct().ToList());
                            foreach (var p in lstPackage)
                            {
                                var op = p.Map<PackageViewDTO>();
                                op.Id = p.id;
                                op.StatusId = p.Status;
                                op.Destination = p.Destination ?? 0;
                                op.DestinationName = Common.ServiceHelper.GetDestionationCodeById(p.Destination ?? 0);
                                op.StatusName = ((Models.Extension.Constant.OrderStatusInfo)p.Status).ToString();
                                op.StoreName = lstStoreAcount.FirstOrDefault(x => x.id == p.StoreId)?.FullName;
                                op.CreateTime = p.CreateTime;
                                op.ShipmentId = p.ShipmentId ?? 0;
                                op.WarehouseId = p.WarehouseId ?? 0;
                                var lstItem = new List<tblItemInPackage>();
                                dicItem.TryGetValue(p.id, out lstItem);
                                op.Items = new List<ItemPackageViewDTO>();

                                if (lstItem != null && lstItem.Any())
                                {
                                    foreach (var i in lstItem)
                                    {
                                        var idto = i.Map<ItemPackageViewDTO>();
                                        idto.CategoryCode = i.Code;
                                        idto.CategoryName = ContextFactory.UsTransportEntities().tblCategories.FirstOrDefault(s => s.id == idto.CategoryId)?.CategoryName;
                                        op.Items.Add(idto);
                                    }
                                }
                                item.LstPackage.Add(op);

                            }
                            item.TotalPackage = item.LstPackage.Count();
                            item.DestinationName = Common.ServiceHelper.GetDestionationCodeById(item.Destination);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Admin_ExportShipmentByLstId", ex);
            }
            return lsResult;
        }
        public Dictionary<int, List<tblItemInPackage>> Package_GetLstItem_Dic(List<int> lstId)
        {
            Dictionary<int, List<tblItemInPackage>> lsResult = new Dictionary<int, List<tblItemInPackage>>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblItemInPackages
                                .Where(x => x.PackageId.HasValue && lstId.Contains(x.PackageId ?? 0))
                                .GroupBy(x => x.PackageId ?? 0)
                                .ToDictionary(g => g.Key, g => g.ToList());
                }
                if (lsResult != null && lsResult.Any())
                {

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ShipmentService => Package_GetLstItem_Dic", ex);
            }
            return lsResult;
        }
    }
}
