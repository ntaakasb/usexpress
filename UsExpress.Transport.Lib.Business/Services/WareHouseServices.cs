using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class WareHouseServices : IWareHouseServices
    {
        public IPagedList<tblWarehouse> GetListWarehouse(int pageIndex, int pageSize, string keyword, int searchType)
        {
            List<tblWarehouse> ListRS = new List<tblWarehouse>();
            try
            {
                keyword = string.IsNullOrEmpty(keyword) ? null : keyword;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblWarehouses.Where(x =>
                    (keyword == null
                    || (keyword != null &&
                    (
                        (searchType == 1 && keyword.Contains(x.id.ToString()))
                        || (searchType == 2 && x.FullName.Contains(keyword))
                        || (searchType == 3 && x.Warehouse.Contains(keyword))
                    ))
                    )).ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => GetListWarehouse", ex);
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public long InsertWarehouse(tblWarehouse warehouse)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    warehouse.CreateDate = DateTime.Now;
                    db.tblWarehouses.Add(warehouse);
                    db.SaveChanges();
                    result = warehouse.id;
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => InsertWarehouse", ex);
            }
            return result;
        }

        public tblWarehouse SelectByWarehouseID(int id)
        {
            tblWarehouse _item = new tblWarehouse();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblWarehouses.Where(x => x.id == id).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => SelectByWarehouseID", ex);
            }
            return _item;
        }

        public long UpdateWarehouse(tblWarehouse warehouse)
        {
            long result = 0;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblWarehouses.Where(x => x.id
                    == warehouse.id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(warehouse);
                        //if (db.SaveChanges() > 0)
                        //{
                        //    result = _temp.id;
                        //}
                        db.SaveChanges();
                        result = _temp.id;
                    }
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => UpdateWarehouse", ex);
            }
            return result;
        }

        public List<tblWarehouse> Shipment_GetLstWarehouse()
        {
            List<tblWarehouse> result = new List<tblWarehouse>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblWarehouses
                        .ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => Shipment_GetLstWarehouse", ex);
            }
            return result;
        }

        public bool CheckExistEmailWarehouseByUserID(string email, int id)
        {

            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblWarehouses.Where(u => u.Email.Equals(email) && (u.id != id)).Count() > 0;
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("WareHouseServices => CheckExistEmailWarehouseByUserID", ex);
                return false;
            }
        }
    }
}
