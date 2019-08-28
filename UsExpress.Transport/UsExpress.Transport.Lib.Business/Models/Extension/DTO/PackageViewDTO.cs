using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class PackageSearchFromApp
    {
        public PackageSearchFromApp()
        {
            StatusId = -1;
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            PageSize = 100;
            WarehouseId = 0;  
        }
        public string StoreName { get; set; }
        /// <summary>
        /// Null -> ToDate
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Null -> ToDate
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// -1 Search All
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// Begin = 0
        /// </summary>
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// Total record
        /// </summary>
        public int Total { get; set; }

        public Int64 GetTimeFromDate => (FromDate.HasValue ? FromDate.Value.Date.ToUnixTimestamp() : 0);
        public Int64 GetTimeToDate => (ToDate.HasValue ? ToDate.Value.Date.AddDays(1).ToUnixTimestamp() : DateTime.UtcNow.Date.AddDays(1).ToUnixTimestamp());
        /// <summary>
        /// -1: seach all
        /// </summary>
        public int WarehouseId { get; set; }
        public long UserId { get; set; }
        public int StoreId { get; set; }
    }
    public class PackageViewDTO
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int StatusId { get; set; }
        public string OrderCode { get; set; }
        public string FullName { get; set; }
        public int TotalItems { get; set; }
        public int Ordinal { get; set; }
        public string Code { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public string CreateDate { get; set; }
        public string PickupDate { get; set; }
        public string ShippingDate { get; set; }
        public string ClearCustomDate { get; set; }
        public string DeliverName { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// same AirPort in tblOrder
        /// </summary>
        public string DestinationName { get; set; }
        public int Destination { get; set; }
        public string Tracking { get; set; }
        public long CreateTime { get; set; }
        public long PickupTime { get; set; }
        public long ShippingTime { get; set; }
        public long ClearCustomTime { get; set; }
        public long DeliverTime { get; set; }

        public string StoreName { get; set; }

        public List<ItemPackageViewDTO> Items { get; set; }
        /// <summary>
        /// total package in order
        /// </summary>
        public int TotalPackage { get; set; }

        public ShippingInfoDTO SenderInfo { get; set; }
        public ShippingInfoDTO RecipientInfo { get; set; }

        public int ShipmentId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public List<object> LstStatusCanChange { get; set; }
        public string CreateDateLocalString { get; set; }        
        public string RecipientCityName { get; set; }

        public decimal WeightKg { get { return this.Weight * Common.Constant.PoundToKg; } }

        public bool IsActive { get; set; }
        public string ShipmentCode { get; internal set; }
    }

    public class CustomeResultDTO
    {
        /// <summary>
        /// 1: success, 0: fail
        /// </summary>
        public int Result { get; set; }
        public string Message { get; set; }
    }

    public class StoreWithPackageViewDTO
    {
        public int StoreId { get; set; }
        public int WarehouseId { get; set; }
        public string StoreName { get; set; }
        public int TotalPackageNew { get; set; }
        public int TotalPakkageSendToVn { get; set; }
        public int TotalPackagePickedUp { get; set; }
        public int StatusNew { get; set; }
        public int StatusPickedUp { get; set; }
        public int StatusSendToVn { get; set; }
    }
    public class HistoryPackage
    {
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
