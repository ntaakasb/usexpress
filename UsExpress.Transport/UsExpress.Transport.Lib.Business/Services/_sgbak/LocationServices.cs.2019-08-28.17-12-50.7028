using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

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

        public List<tblWarehouse> GetWarehouse(int CityID, int StateID = -1)
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.tblWarehouses.Where(x => x.StateId == StateID && CityID == x.CityId).ToList();
            }
        }

        public List<tblStateProvice> GetLstStateOfCountry(int countryId)
        {
            List<tblStateProvice> result = new List<tblStateProvice>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblStateProvices.Where(x => x.CountryId == countryId).Select(x => x).ToList();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public List<tblDistrictStateProvice> GetLstDictrictByCityId(string cityId)
        {
            List<tblDistrictStateProvice> result = new List<tblDistrictStateProvice>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblDistrictStateProvices.Where(x => x.StateProvinceID.Equals(cityId, StringComparison.OrdinalIgnoreCase)).Select(x => x).ToList();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
