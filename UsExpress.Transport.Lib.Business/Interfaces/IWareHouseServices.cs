
using PagedList;
using System.Collections.Generic;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IWareHouseServices
    {
        tblWarehouse SelectByWarehouseID(int id);
        IPagedList<tblWarehouse> GetListWarehouse(int pageIndex, int pageSize, string keyword, int searchType);
        long InsertWarehouse(tblWarehouse warehouse);
        long UpdateWarehouse(tblWarehouse warehouse);

        List<tblWarehouse> Shipment_GetLstWarehouse();
        bool CheckExistEmailWarehouseByUserID(string email, int id);
    }
}
