using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Common;

namespace UsExpress.Transport.Lib.Business.Models
{
    public class ReportOrderModel
    {
        [DataRowKey("id")]
        [DataMember]
        public long id { get; set; }

        [DataRowKey("Code")]
        [DataMember]
        public string Code { get; set; }

        [DataRowKey("Sender")]
        [DataMember]
        public string Sender { get; set; }

        [DataRowKey("Recipient")]
        [DataMember]
        public string Recipient { get; set; }

        [DataRowKey("Totalweigh")]
        [DataMember]
        public decimal Totalweigh { get; set; }

        [DataRowKey("Totalpackage")]
        [DataMember]
        public int Totalpackage { get; set; }

        [DataRowKey("Totalvalue")]
        [DataMember]
        public decimal Totalvalue { get; set; }

        [DataRowKey("Totalfee")]
        [DataMember]
        public decimal Totalfee { get; set; }

        [DataRowKey("StoreName")]
        [DataMember]
        public string StoreName { get; set; }

        [DataRowKey("CreateDate")]
        [DataMember]
        public long CreateDate { get; set; }

        [DataRowKey("PickupDate")]
        [DataMember]
        public long PickupDate { get; set; }

        [DataRowKey("ShippingDate")]
        [DataMember]
        public long ShippingDate { get; set; }

        [DataRowKey("ClearCustomDate")]
        [DataMember]
        public long ClearCustomDate { get; set; }

        [DataRowKey("Deliver")]
        [DataMember]
        public long Deliver { get; set; }

        [DataRowKey("CityName")]
        [DataMember]
        public string CityName { get; set; }

        [DataRowKey("AddressSender")]
        [DataMember]
        public string AddressSender { get; set; }

        [DataRowKey("PhoneSender")]
        [DataMember]
        public string PhoneSender { get; set; }

        [DataRowKey("AddressRecippient")]
        [DataMember]
        public string AddressRecippient { get; set; }

        [DataRowKey("PhoneRecippient")]
        [DataMember]
        public string PhoneRecippient { get; set; }

        [DataRowKey("Fee")]
        [DataMember]
        public decimal Fee { get; set; }

        [DataRowKey("STT")]
        [DataMember]
        public long STT { get; set; }

        [DataRowKey("TOTALROWS")]
        [DataMember]
        public long TOTALROWS { get; set; }

      
    }
}
