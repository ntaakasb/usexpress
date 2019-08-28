using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport
{
    public partial class UsTransportEntities
    {
        public void SetOffLazyLoading(bool enable)
        {
            this.Configuration.LazyLoadingEnabled = enable;
        }
    }

    public partial class tblOrder
    {
        public tblShippingInfo SenderShippingInfo { get; set; }
        public tblShippingInfo RecipientShippingInfo { get; set; }

        public tblSender SenderInfo { get; set; }
        public tblRecipientsInfo RecipientInfo { get; set; }
    }
}