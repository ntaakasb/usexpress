using Entity.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension.Shipment
{
    [Serializable]
    [DataContract]
    public class DetailPackageOnday
    {
        [DataRowKey("Id")]
        [DataMember]
        public int Id { get; set; }
        [DataRowKey("Weight")]
        [DataMember]
        public decimal Weight { get; set; }
        [DataRowKey("OrderId")]
        [DataMember]
        public int OrderId { get; set; }
        [DataRowKey("Destination")]
        [DataMember]
        public int Destination { get; set; }
        [DataRowKey("WarehouseId")]
        [DataMember]
        public int WarehouseId { get; set; }
    }
}
