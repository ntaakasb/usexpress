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
    
    public partial class tblKerryOrderProgress
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string StatusService { get; set; }
        public string StatusServiceDescription { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public string Cost { get; set; }
        public string Fee { get; set; }
        public string TimeStatus { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
