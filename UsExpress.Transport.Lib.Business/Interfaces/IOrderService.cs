using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IOrderService
    {
        long Admin_CreateOrder(tblOrder model);
        long Admin_CreateOrderApi(tblOrder model);
        tblOrder Admin_GetDetailOrderById(int id);

        long Admin_UpdateOrder(tblOrder model);

        List<OrderViewDTO> Admin_SearchOrder(PackageSearch model);
        List<PackageViewDTO> Admin_SearchPackage(PackageSearch model);

        PackageViewDTO Admin_GetDetailPackageById(int packageId);
        tblPackageInfo PrintInvoice(int packageId);
        OrderCustomDTO getOrderByID(long id);

        #region App
        List<PackageViewDTO> App_SearchPackage(PackageSearchFromApp model);

        PackageViewDTO App_GetDetailPackageHasItemByCode(string packageCode);

        CustomeResultDTO App_UpdateStatusPackage(int packageId, int statusUpdate);

        List<StoreWithPackageViewDTO> App_SearchStoreWithCountPackage(PackageSearchFromApp model);

        #endregion

        string GetDestionationCodeById(int destinationId);
        List<tblSender> Order_GetLstSenderByLstId(List<int> lstId);
        List<tblShippingInfo> Order_GetLstShippingInfo(int typeUser, List<long> lstOrderId);
        List<tblStoreAccount> Order_GetLstStoreAcountInfo(List<int> lstStoreId);


        CustomeResultDTO Admin_UpdateStatusPackage(int packageId, int statusUpdate);
        CustomeResultDTO Admin_UpdateListStatusPackage(string lstPackageId, int packageId, int statusUpdate);
        List<ShipmentPropose> Admin_CheckPackageRecipient(int packageId);
        int CountOrderInDay(DateTime refDate,int storeId);

        List<ReportOrderModel> getReportOrder(int WarehouseId, int storeId, DateTime fromdate, DateTime todate, int isActive, int pageSize, int pageIndex);
        bool SetActiveOrder(int idOrder);
        CustomeResultDTO Admin_UpdateListStatusPackageSingle(string lstId, int statusUpdate);
    }
}
