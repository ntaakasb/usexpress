using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface ILocationServices
    {
        List<tblCity> GetCity();
        List<tblDistrict> GetDistrictByCity(int CityID);
        List<tblState> GetState();
        List<tblWarehouse> GetWarehouse(int CityID, int StateID);

        tblCity GetCityByID(int id);
        tblState GetStateByID(int id);

        List<tblStateProvice> GetLstStateOfCountry(int countryId);

        List<tblDistrictStateProvice> GetLstDictrictByCityId(string cityId);
    }
}
