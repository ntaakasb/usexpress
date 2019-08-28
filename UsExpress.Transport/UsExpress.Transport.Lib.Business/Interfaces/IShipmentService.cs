using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IShipmentService
    {
        List<tblPackageInfo> listPackageInday();
        List<tblPackageInfo> listPackageByShipmentCode(string ShipmentCode);

        List<ShipmentViewDTO> Admin_SearchShipment(ShipmentSearch model = null);
        List<ShipmentPropose> LstShipment();

        int Admin_CreateShipment(tblShipment model);
        int Admin_UpdateShipment(tblShipment model);

        int Admin_UpdateShipmentForPackage(List<long> lstPackagageId, int? shipmentId, string shipmentCode);

        ShipmentViewDTO Admin_GeDetailShipmentHasPackageById(int shipmentId);

        int Admin_CountShipmentOnDateByDestination(int destinationId);

        int Admin_RemoveLstShipmentId(List<int> lstId);
        int Admin_RemovePackageInShipment(List<int> lstId);
        List<ShipmentViewDTO> Admin_SearchShipmentByCondition(string keyword, DateTime FromDate, DateTime ToDate);
        List<ShipmentViewDTO> Admin_ExportShipmentByLstId(List<int> lstId);
    }
}
