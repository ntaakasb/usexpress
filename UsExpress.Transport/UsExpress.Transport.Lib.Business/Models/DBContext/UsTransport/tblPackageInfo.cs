//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblPackageInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPackageInfo()
        {
            this.tblItemInPackages = new HashSet<tblItemInPackage>();
        }
    
        public int id { get; set; }
        public long OrderId { get; set; }
        public int Ordinal { get; set; }
        public Nullable<int> Expedite { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Depth { get; set; }
        public Nullable<decimal> Fee { get; set; }
        public int TotalItem { get; set; }
        public Nullable<decimal> TotalValue { get; set; }
        public string PackName { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<int> Destination { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ShipmentCode { get; set; }
        public Nullable<int> ShipmentId { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public long CreateTime { get; set; }
        public string Tracking { get; set; }
        public Nullable<long> PickupTime { get; set; }
        public Nullable<long> ShippingTime { get; set; }
        public Nullable<long> ClearCustomTime { get; set; }
        public Nullable<long> DeliverTime { get; set; }
        public int StoreId { get; set; }
        public System.DateTime CreateDateLocal { get; set; }
        public Nullable<System.DateTime> PickupDateLocal { get; set; }
        public Nullable<System.DateTime> ShippingDateLocal { get; set; }
        public Nullable<int> NotifyToCustomer { get; set; }
        public string HistoryTransport { get; set; }
        public int ParentId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblItemInPackage> tblItemInPackages { get; set; }
        public virtual tblOrder tblOrder { get; set; }
    }
}
