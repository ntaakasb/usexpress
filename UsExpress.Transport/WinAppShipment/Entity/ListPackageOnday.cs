using Entity.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace UsExpress.Transport.Lib.Business.Models.Extension.Shipment
{
    [Serializable]
    [DataContract]
    public class ListPackageOnday
    {
        [DataRowKey("Id")]
        [DataMember]
        public int Id { get; set; }
        [DataRowKey("TotalWeight")]
        [DataMember]
        public decimal TotalWeight { get; set; }
        [DataRowKey("TotalPackage")]
        [DataMember]
        public int TotalPackage { get; set; }
        [DataRowKey("Destination")]
        [DataMember]
        public int Destination { get; set; }
        [DataRowKey("WarehouseId")]
        [DataMember]
        public int WarehouseId { get; set; }
    }
}
