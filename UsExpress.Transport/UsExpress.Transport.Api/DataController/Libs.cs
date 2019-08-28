using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Api.DataController
{
    public class Libs
    {
        public static string AutoGenPackageID(DateTime refID, string storeCode, string packageNumber, int countShipment)
        {
            try
            {
                Dictionary<int, string> months = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, { 3, "C" }, { 4, "D" }, { 5, "E" }, { 6, "F" }, { 7, "G" }, { 8, "H" }, { 9, "K" }, { 10, "L" }, { 11, "M" }, { 12, "N" } };
                //Dictionary<int, string> types = new Dictionary<int, string> { { 1, "AM" }, { 2, "CC" }, { 3, "BB" } };
                DateTime current = refID, startDate = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0);
                //int orderNumber = new Random().Next(100, 999);
                //try { orderNumber = DB.Orders.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= current).Select(x => x.Id).ToList().Count(); } catch { }
                string orderCode = ((current.Day * 10) + (countShipment + 1)).ToString();

                return storeCode + "-" // cụm 1
                        + current.Year.ToString().Substring(3, 1) + months[current.Month] + (current.Day.ToString().Length == 1 ? "0" + current.Day : current.Day.ToString()) // cụm ngày tháng
                        + (orderCode.Length == 2 ? ("0" + orderCode) : orderCode) + "-" + packageNumber; // cụm id
            }
            catch (Exception ex)
            {
                Libraries.Log.Write(System.Web.HttpContext.Current.Server.MapPath("~") + "\r\n AutoGenOrderID(string storeCode, int packageNumber, int countShipment) \r\n" + ex.ToString());
                return storeCode + "-" + DateTime.Now.TimeOfDay.TotalSeconds.ToString() + "-" + packageNumber;
            }
        }
    }
}