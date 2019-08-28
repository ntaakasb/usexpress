using Entity.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WinAppShipment.Entity
{
    [Serializable]
    [DataContract]
    public class tblShipment
    {
        [DataRowKey("Id")]
        [DataMember]
        public int Id { get; set; }
        [DataRowKey("ShipmentCode")]
        [DataMember]
        public string ShipmentCode { get; set; }
        [DataRowKey("Destination")]
        [DataMember]
        public int Destination { get; set; }
        [DataRowKey("WarehouseId")]
        [DataMember]
        public int WarehouseId { get; set; }
        [DataRowKey("TotalWeight")]
        [DataMember]
        public decimal TotalWeight { get; set; }

         
    }
}
