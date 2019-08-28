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
    
    public partial class tblShippingInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FullAddress1 { get; set; }
        public string FullAddress2 { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<int> TypeUser { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public Nullable<System.DateTime> ClearCustomDateLocal { get; set; }
        public Nullable<System.DateTime> DeliverDateLocal { get; set; }
    }
}
