using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.WinAppSendEmail
{
    class tblOrderModel
    {
        public long id { get; set; }
        public string Code { get; set; }
        public Nullable<int> SenderId { get; set; }
        public Nullable<int> RecipientId { get; set; }
        public Nullable<int> TotalPackage { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> TotalCharge { get; set; }
        public Nullable<int> Exedite { get; set; }
        public long CreateDate { get; set; }
        public Nullable<long> PickupDate { get; set; }
        public Nullable<long> ShippingDate { get; set; }
        public Nullable<long> ClearCustomDate { get; set; }
        public Nullable<long> Deliver { get; set; }
        public Nullable<int> Status { get; set; }
        public string Tracking { get; set; }
        public Nullable<int> Destination { get; set; }
        public string Note { get; set; }
        public Nullable<int> StoreId { get; set; }
    }
}
