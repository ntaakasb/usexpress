using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class PackageSearch
    {
        public PackageSearch()
        {
            this.StatusId = -1;
            this.Destination = -1;
            this.HasShipment = -1;
            this.StoreId = -1;
            this.WarehouseId = -1;
            this.IsActive = true;
        }
        public string Keyword { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        /// <summary>
        /// 0: search Id, 1: sender, 2: warehouse name
        /// </summary>
        public int TypeSearch { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// kết quả trả về số dòng
        /// </summary>
        public int Total { get; set; }
       
        public DateTime DFromDate
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(this.FromDate))
                    {
                        return DateTime.Now.Date;
                    }
                    return DateTime.ParseExact(this.FromDate, "MMM dd yyyy", null);
                }
                catch (Exception)
                {
                }
                return DateTime.Now.Date;
            }
        }
        public DateTime DToDate
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.ToDate))
                    {
                        return DateTime.ParseExact(this.ToDate, "MMM dd yyyy", null);
                    }
                    return this.DFromDate;
                }
                catch (Exception)
                {
                }
                return DateTime.Now.Date;
            }
        }
        public long GetUnixTimestampFromDate
        {
            get { return this.DFromDate.ToUnixTimestamp(); }
        }
        public long GetUnixTimestampToDate
        {
            get { return this.DToDate.AddDays(1).ToUnixTimestamp(); }
        }
        /// <summary>
        /// -1: all
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// -1: all
        /// </summary>
        public int Destination { get; set; }
        /// <summary>
        /// các status cách nhau = ",", empty search all
        /// </summary>
        public string LstStatus { get; set; }
        public string LstDestination { get; set; }
        /// <summary>
        /// -1: All, 0: ko có shipment, 1: đã có shipment
        /// /// </summary>

        public int HasShipment { get; set; }
        /// <summary>
        /// -1: NV search, != -1 Store search
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// -1
        /// </summary>
        public int WarehouseId { get; set; }

        public bool IsActive { get; set; }
    }
    public class OrderViewDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public int TotalPackage { get; set; }

        public decimal Weight { get; set; }
        public string CreateDate { get; set; }
        public string PickupDate { get; set; }
        public string ShippingDate { get; set; }
        public string ClearCustomDate { get; set; }
        public string DeliverName { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// same AirPort in tblOrder
        /// </summary>
        public string Destination { get; set; }
        public string Tracking { get; set; }
        public long CreateTime { get; set; }
        public long PickupTime { get; set; }
        public long ShippingTime { get; set; }
        public long ClearCustomTime { get; set; }
        public long DeliverTime { get; set; }

        public string SenderName { get; set; }        
    }
    public class OrderCustomDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public long CreateDate { get; internal set; }
        public string EmailStore { get; internal set; }
        public string NameStore { get; internal set; }
        public string EmailSender { get; internal set; }
        public string NameSender { get; internal set; }
        public string NameRecipient { get; internal set; }
        public int Status { get; internal set; }
        public string Address1Recipient { get; internal set; }
        public string PhoneRecipient { get; internal set; }
        public string Address1 { get; internal set; }
        public string Address2 { get; internal set; }
        public string Zip { get; internal set; }
        public string Phone { get; internal set; }
    }
}
