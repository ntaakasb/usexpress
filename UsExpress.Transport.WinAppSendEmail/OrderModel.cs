using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.WinAppSendEmail
{
    public class OrderModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public long CreateDate { get; internal set; }
        public string EmailStore { get; internal set; }
        public string NameStore { get; internal set; }
        public string EmailSender { get; internal set; }
        public string NameSender { get; internal set; }
        public string NameRecipient { get; internal set; }
        public int Status { get; internal set; }
        public string Address1Recipient { get; internal set; }
        public string PhoneRecipient { get; internal set; }
        public string Address1 { get; internal set; }
        public string Address2 { get; internal set; }
        public string Zip { get; internal set; }
        public string Phone { get; internal set; }
        public string CityId { get; internal set; }
        public string StateId { get; internal set; }
    }
}
