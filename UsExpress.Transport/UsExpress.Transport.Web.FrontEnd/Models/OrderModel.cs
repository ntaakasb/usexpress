using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Web.FrontEnd.Models
{
    public class OrderModel
    {
        private const string FormatDateLocal = "dd/MM/yyyy HH:mm:ss";
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string Tracking { get; set; }
        public int StoreId { get; set; }
        public decimal Weight { get; set; }
        public int Status { get; set; }
        public decimal TotalCharge { get; set; }
        public bool Expedite { get; set; }
        public List<PackageModel> Packages { get; set; }

        public ShippingInfoModel SenderInfo { get; set; }
        public ShippingInfoModel RecipientInfo { get; set; }

        public long CreateTime { get; set; }
        public int Destination { get; set; }

        public string CreateDateLocalString { get; set; }
        public DateTime CreateDateLocal { get; set; }

        public long? TotalItems { get; set; }
        public decimal? TotalDeclareValues { get; set; }
        public decimal? ShippingFree { get; set; }


        public OrderModel()
        {
            SenderInfo = new ShippingInfoModel();
            RecipientInfo = new ShippingInfoModel();
        }

        public tblOrder ConvertToOrder()
        {
            var result = new tblOrder();
           
            result.Code = this.Code;
            result.CreateDate = DateTime.ParseExact(this.CreateDateLocalString, FormatDateLocal, null).ToUnixTimestamp();// (CreateTime == 0 ? DateTime.UtcNow.ToUnixTimestamp() : CreateTime);
            result.Status = this.Status;
            var lstPackage = GetPackageInfoFromModel();
            result.tblPackageInfoes = lstPackage;
            result.TotalCharge = lstPackage.Sum(x => x.Fee);
            result.Weight = lstPackage.Sum(x => x.Weight);
            result.TotalPackage = lstPackage.Count();
            result.SenderShippingInfo = this.SenderInfo.Map<tblShippingInfo>();
            result.SenderShippingInfo.Id = this.SenderInfo.Id;
            result.RecipientShippingInfo = this.RecipientInfo.Map<tblShippingInfo>();
            result.RecipientShippingInfo.Id = this.RecipientInfo.Id;
            result.Exedite = this.Expedite ? 1 : 0;
            result.StoreId = this.StoreId;
            result.Destination = this.Destination;
            result.TotalDeclareValues = TotalDeclareValues;
            result.TotalItems = TotalItems;
            result.ShippingFree = ShippingFree;

            return result;
        }
        public List<tblPackageInfo> GetPackageInfoFromModel()
        {
            var result = new List<tblPackageInfo>();
            if (this.Packages != null && this.Packages.Any())
            {
                var totalPackage = this.Packages.Count();
                foreach (var package in this.Packages)
                {
                    tblPackageInfo p = package.Map<tblPackageInfo>();
                    p.Expedite = package.Expedite ? 1 : 0;
                    p.id = package.Id;
                    p.Destination = this.Destination;
                    p.CreateDate = DateTime.ParseExact(this.CreateDateLocalString, FormatDateLocal, null);
                    p.Code = GenCodePackage(totalPackage, p.Ordinal);
                    p.CreateTime = DateTime.UtcNow.ToUnixTimestamp();
                    p.CreateDateLocal = DateTime.ParseExact(this.CreateDateLocalString, FormatDateLocal, null);
                    p.Status = this.Status;
                    p.StoreId = this.StoreId;
                    if (package.Items != null && package.Items.Any())
                    {
                        p.TotalItem = package.Items.Sum(x => x.Quantity);
                        p.TotalValue = package.Items.Sum(x => (x.Value * x.Quantity));
                        foreach (var item in package.Items)
                        {
                            if (item.CategoryId > 0)
                            {
                                tblItemInPackage ip = item.Map<tblItemInPackage>();
                                ip.id = item.Id;
                                p.tblItemInPackages.Add(ip);
                            }

                        }
                    }
                    result.Add(p);
                }
            }
            return result;
        }
        public string GenOrderCode(string codeStore)
        {
            return $"{codeStore}{DateTime.Now.ToString("yyMMddHHmmss")}";
        }
        public string GenCodePackage(int totalPackage, int ordinal)
        {
            string numberTotalPackage = totalPackage.ToString();
            string numberOrdinal = ordinal.ToString();
            if (totalPackage < 10)
            {
                numberTotalPackage = "" + totalPackage;
            }
            if (ordinal < 10)
            {
                numberOrdinal = "" + ordinal;
            }
            return $"{this.Code}-{numberTotalPackage}{numberOrdinal}";
        }
        public OrderModel(tblOrder model)
        {
            Id = model.id;
            StoreId = model.StoreId ?? 0;
            SenderInfo = model.SenderShippingInfo.Map<ShippingInfoModel>();
            SenderInfo.StoreId = this.StoreId;
            RecipientInfo = model.RecipientShippingInfo.Map<ShippingInfoModel>();
            RecipientInfo.StoreId = this.StoreId;
            this.Packages = new List<PackageModel>();
            DateTime createDateLocal = DateTime.Now;
            foreach (var pa in model.tblPackageInfoes)
            {
                PackageModel pM = new PackageModel
                {
                    Depth = pa.Depth ?? 0,
                    Weight = pa.Weight ?? 0,
                    Fee = pa.Fee ?? 0,
                    Height = pa.Height ?? 0,
                    Width = pa.Width ?? 0,
                    Expedite = pa.Expedite.HasValue ? (pa.Expedite.Value > 0 ? true : false) : false,
                    Ordinal = pa.Ordinal,
                    Id = pa.id,
                    Destination = pa.Destination ?? 0,
                    WarehouseId = pa.WarehouseId ?? 0,
                    Status = pa.Status,
                    CreateTime = model.CreateDate,
                    StoreId = pa.StoreId
                };
                pM.Items = new List<ItemPackageModel>();
                foreach (var i in pa.tblItemInPackages)
                {
                    ItemPackageModel iM = new ItemPackageModel {
                        CategoryId = i.CategoryId ?? 0,
                        Code = i.Code, Description = i.Description,
                        Id = i.id, Quantity = i.Quantity ?? 0,
                        Unit = i.Unit, Value = i.Value ?? 0,
                        NameCategory = i.tblCategory.CategoryName,
                        BarCode = i.BarCode

                    };
                    pM.Items.Add(iM);
                }
                this.Packages.Add(pM);
                createDateLocal = pa.CreateDateLocal;
            }
            this.Code = model.Code;
            this.CreateDate = (model.CreateDate).UnixTimeStampToDateTime();
            this.CreateTime = model.CreateDate;
            this.CreateDateLocalString = createDateLocal.ToString(FormatDateLocal);
            this.CreateDateLocal = createDateLocal;
            this.Expedite = model.Exedite.HasValue ? (model.Exedite.Value > 0 ? true : false) : false;
        }
    }
    public class PackageModel
    {
        public int Id { get; set; }
        public decimal Weight { get; set; }
        public decimal Fee { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public bool Expedite { get; set; }
        public int Ordinal { get; set; }
        public List<ItemPackageModel> Items { get; set; }
        public int WarehouseId { get; set; }
        public int Destination { get; set; }
        public int Status { get; set; }

        public long CreateTime { get; set; }
        public int StoreId { get; set; }

        public string CreateDateLocalString { get; set; }
        public DateTime CreateDateLocal { get; set; }
    }
    public class ItemPackageModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
        public string NameCategory { get; set; }
        public string BarCode { get; set; }
    }
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class ShippingInfoModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int CountryId { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public int OrderId { get; set; }
        public int TypeUser { get; set; }

        public string CityName { get; set; }
        public string StateName { get; set; }
        public int StoreId { get; set; }
    }

    public class SelectItemBase
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int AttributeId { get; set; }
        public string AttributeCode { get; set; }
    }
    public class ShipmentPositionModels
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ShipmentViewDTO ShipmentItem { get; set; }
    }

    public class ListShipmentModels
    {
        public ListShipmentModels()
        {
            ListResult = new List<ShipmentPositionModels>();
        }
        public List<ShipmentPositionModels> ListResult { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}