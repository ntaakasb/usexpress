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
    
    public partial class tblSender
    {
        public int Id { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string FullName { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string StateId { get; set; }
        public string Phone { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public Nullable<long> CreateDate { get; set; }
        public Nullable<long> UpdateDate { get; set; }
        public string FullAddress { get; set; }
        public bool IsActive { get; set; }
    
        public virtual tblStoreAccount tblStoreAccount { get; set; }
    }
}
