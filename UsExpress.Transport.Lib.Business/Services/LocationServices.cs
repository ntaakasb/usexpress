using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class LocationServices : ILocationServices
    {

        public List<tblCity> GetCity()
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblCities.ToList();
            }
        }

        public tblCity GetCityByID(int id)
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblCities.FirstOrDefault(x => x.id == id);
            }
        }

        public List<tblDistrict> GetDistrictByCity(int CityID)
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblDistricts.Where(x => x.CityId == CityID).ToList();
            }
        }

        public List<tblState> GetState()
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblStates.ToList();
            }
        }

        public tblState GetStateByID(int id)
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblStates.FirstOrDefault(x => x.id == id);
            }
        }

        public List<tblWarehouse> GetWarehouse()
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblWarehouses.Where(x => x.Status == true).ToList();
            }
        }

        public List<tblStateProvice> GetLstStateOfCountry(int countryId)
        {
            List<tblStateProvice> result = new List<tblStateProvice>();
            var key = "GetLstStateOfCountry-" + countryId;
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (List<tblStateProvice>)HTML;
            }
            else
            {
                try
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        result = db.tblStateProvices.Where(x => x.CountryId == countryId).Select(x => x).ToList();
                    }
                    if (result != null)
                        MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(30));
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public List<tblDistrictStateProvice> GetLstDictrictByCityId(string cityId)
        {
            List<tblDistrictStateProvice> result = new List<tblDistrictStateProvice>();
            var key = "GetLstDictrictByCityId-" + cityId;
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (List<tblDistrictStateProvice>)HTML;
            }
            else
            {
                try
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        result = db.tblDistrictStateProvices.Where(x => x.StateProvinceID.Equals(cityId, StringComparison.OrdinalIgnoreCase)).Select(x => x).ToList();
                    }
                    if (result != null)
                        MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(30));
                }
                catch (Exception ex)
                {
                    SELog.WriteLog("LocationServices => GetLstDictrictByCityId", ex);
                }
            }
            return result;
        }

        public List<tblWard> GetLstWardByDistrictId(string districtId)
        {
            List<tblWard> result = new List<tblWard>();
            var key = "GetLstWardByDistrictId-" + districtId;
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (List<tblWard>)HTML;
            }
            else
            {
                try
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        result = db.tblWards.Where(x => x.DistrictId.Equals(districtId, StringComparison.OrdinalIgnoreCase)).Select(x => x).ToList();
                    }
                    if (result != null)
                        MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(30));
                }
                catch (Exception ex)
                {
                    SELog.WriteLog("LocationServices => GetLstWardByDistrictId", ex);
                }
            }
            return result;
        }
    }
}
