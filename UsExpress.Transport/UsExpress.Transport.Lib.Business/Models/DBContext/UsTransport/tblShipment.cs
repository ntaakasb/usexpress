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
    
    public partial class tblShipment
    {
        public int Id { get; set; }
        public string ShipmentCode { get; set; }
        public Nullable<int> Destination { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public int StatusId { get; set; }
        public Nullable<long> CreateTime { get; set; }
        public bool IsDeleted { get; set; }
        public string ExtraData { get; set; }
        public string LogChange { get; set; }
    }
}
