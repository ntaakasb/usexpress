using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IOrderService
    {
        long Admin_CreateOrder(tblOrder model);

        tblOrder Admin_GetDetailOrderById(int id);

        long Admin_UpdateOrder(tblOrder model);

        List<OrderViewDTO> Admin_SearchOrder(OrderSearch model);
    }
}
