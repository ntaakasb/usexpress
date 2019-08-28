using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Web.FrontEnd.Models
{
    public class StoreModel
    {
        public tblStoreAccount StoreAccount { get; set; }
        public tblCity City { get; set; }
        public tblState State { get; set; }
        public tblUserRole UserRole { get; set; }
    }
    public class DeliveryModel
    {
        
    }
}