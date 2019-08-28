using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class ShipmentViewDTO
    {
        public int Id { get; set; }
        public string ShipmentCode { get; set; }
        public int Destination { get; set; }
        public string DestinationName { get; set; }
        public decimal TotalWeight { get; set; }
        public int WarehouseId { get; set; }
        public List<PackageViewDTO> LstPackage { get; set; }
        public int TotalPackage { get; set; }

        public long CreateTime { get; set; }
        public string WarehouseName { get; set; }

        public decimal TotalWeightKg { get { return Math.Round(this.TotalWeight * Common.Constant.PoundToKg,2); } }

    }

    public class ShipmentSearch
    {
        public string Keyword { get; set; }
        public int WarehouseId { get; set; }

        public double FromTime { get; set; }
        public double ToTime { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }
    public class ShipmentPropose
    {
        public int id { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalWeightKg { get { return Math.Round(this.Weight * Common.Constant.PoundToKg, 2); } }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Items { get; set; }
        public string AirPortCode { get; set; }
        public int PackageGroup { get; set; }
        public int PackageInGroup { get; set; }
    }
    public class LstShipmentPropose
    {
        public ShipmentPropose lstShipmentPropose { get; set; }
    }
}
