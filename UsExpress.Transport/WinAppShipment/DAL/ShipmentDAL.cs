using DAL;
using Entity.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.Extension.Shipment;
using WinAppShipment.Entity;

namespace WinAppShipment.DAL
{
    class ShipmentDAL
    {
        public List<ListPackageOnday> GetListPackageOnday()
        {
            List<ListPackageOnday> lstResult = new List<ListPackageOnday>();
            DatabaseClass objData = new DatabaseClass();
            QueryString objQuery = new QueryString();
            objQuery.strSPName = "getTotalPakageinWarehouseOnday";            
            string strSqlStore = objQuery.GenSPString();
            Common common = new Common();
            try
            { 
                    lstResult = objData.ExeDataset(objQuery.strSPName).Tables[0].ToGenericList<ListPackageOnday>();
                 
            }
            catch (Exception ex)
            {
                common.WriteLog(Environment.CurrentDirectory, "WinAppShipment - GetListPackageOnday: " + ex.StackTrace);
                lstResult = null;
            }
            finally
            {
                objData.CloseData();
                objData = null;
                common = null;
            }
            return lstResult;
        }

        public List<DetailPackageOnday> getDetailPackageOnday(int WarehouseId,int Destination)
        {
            List<DetailPackageOnday> lstResult = new List<DetailPackageOnday>();
            DatabaseClass objData = new DatabaseClass();
            QueryString objQuery = new QueryString();
            objQuery.strSPName = "getDetailPackageOnday";
            objQuery.AddInt("@WarehouseId", WarehouseId);
            objQuery.AddInt("@Destination", Destination);
            string strSqlStore = objQuery.GenSPString();
            Common common = new Common();
            try
            {
                var dt = objData.ExeDataset(strSqlStore).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    lstResult = objData.ExeDataset(strSqlStore).Tables[0].ToGenericList<DetailPackageOnday>();
                }

            }
            catch (Exception ex)
            {
                common.WriteLog(Environment.CurrentDirectory, "WinAppShipment - getDetailPackageOnday: " + ex.StackTrace);
                lstResult = null;
            }
            finally
            {
                objData.CloseData();
                objData = null;
                common = null;
            }
            return lstResult;
        }


        public bool CreateShipment(string PackageId, tblShipment _tblShipment)
        {
            List<DetailPackageOnday> lstResult = new List<DetailPackageOnday>();
            DatabaseClass objData = new DatabaseClass();
            QueryString objQuery = new QueryString();
            objQuery.strSPName = "CreateShipment";
            objQuery.AddString("@ShipmentCode", _tblShipment.ShipmentCode);
            objQuery.AddInt("@Destination", _tblShipment.Destination);
            objQuery.AddInt("@WarehouseId", _tblShipment.WarehouseId);
            objQuery.AddDecimal("@TotalWeight", _tblShipment.TotalWeight);
            objQuery.AddString("@PackageId", PackageId);
            string strSqlStore = objQuery.GenSPString();
            Common common = new Common();
            try
            {
                objData.ExecuteNonQuery(strSqlStore);

                return true;
            }
            catch (Exception ex)
            {
                common.WriteLog(Environment.CurrentDirectory, "WinAppShipment - getDetailPackageOnday: " + ex.StackTrace);
                lstResult = null;

                return false;

            }
            finally
            {
                objData.CloseData();
                objData = null;
                common = null;
            }
        }
    }
}
