using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class ShipmentService : IShipmentService
    {
        public List<tblPackageInfo> listPackageByOrder(int orderID)
        {
            List<tblPackageInfo> lsResult = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lsResult = db.tblPackageInfoes.Where(x => x.OrderId == orderID).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return lsResult;
        }

        public List<tblPackageInfo> listPackageInday()
        {
            List<tblPackageInfo> result = null;
            try
            {
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
