using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Web.FrontEnd.Common
{
    public class Libs
    {
        public static bool IsAdmin()
        {
            return HttpContext.Current.Session[Constant.SessionIsAdmin] != null && bool.Parse(HttpContext.Current.Session[Constant.SessionIsAdmin].ToString());
        }
        public static bool IsStore()
        {
            return HttpContext.Current.Session[Constant.SessionStoreID] != null;
        }

        public static bool IsCSKH()
        {
            return HttpContext.Current.Session[Constant.SessionIsCSKH] != null && bool.Parse(HttpContext.Current.Session[Constant.SessionIsCSKH].ToString());
        }
        public static string UnicodeToPlain(string strEncode)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = strEncode.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string UnicodeToNoneMark(string iInput)
        {
            if (iInput == null)
                return string.Empty;
            if (iInput.Length <= 0)
                return string.Empty;
            iInput = iInput.ToLower();
            System.Text.StringBuilder strOutput = new System.Text.StringBuilder();
            string strMark = "";
            string strChar = null;
            for (int i = 0; i <= iInput.Length - 1; i++)
            {
                strChar = iInput.Substring(i, 1);
                switch (strChar)
                {
                    case "â":
                    case "ê":
                    case "ô":
                    case "đ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        break;
                    case "ấ":
                    case "ế":
                    case "ố":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "s";
                        break;
                    case "ầ":
                    case "ề":
                    case "ồ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "f";
                        break;
                    case "ẩ":
                    case "ể":
                    case "ổ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "r";
                        break;
                    case "ẫ":
                    case "ễ":
                    case "ỗ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "x";
                        break;
                    case "ậ":
                    case "ệ":
                    case "ộ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "j";
                        break;
                    case "ă":
                    case "ư":
                    case "ơ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        break;
                    case "ắ":
                    case "ứ":
                    case "ớ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "s";
                        break;
                    case "ằ":
                    case "ừ":
                    case "ờ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "f";
                        break;
                    case "ẳ":
                    case "ử":
                    case "ở":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "r";
                        break;
                    case "ẵ":
                    case "ữ":
                    case "ỡ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "x";
                        break;
                    case "ặ":
                    case "ự":
                    case "ợ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "j";
                        break;
                    case "á":
                    case "é":
                    case "í":
                    case "ó":
                    case "ú":
                    case "ý":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "s";
                        break;
                    case "à":
                    case "è":
                    case "ì":
                    case "ò":
                    case "ù":
                    case "ỳ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "f";
                        break;
                    case "ả":
                    case "ẻ":
                    case "ỉ":
                    case "ỏ":
                    case "ủ":
                    case "ỷ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "r";
                        break;
                    case "ã":
                    case "ẽ":
                    case "ĩ":
                    case "õ":
                    case "ũ":
                    case "ỹ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "x";
                        break;
                    case "ạ":
                    case "ẹ":
                    case "ị":
                    case "ọ":
                    case "ụ":
                    case "ỵ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "j";
                        break;
                    case "a":
                    case "b":
                    case "c":
                    case "d":
                    case "e":
                    case "f":
                    case "g":
                    case "h":
                    case "i":
                    case "j":
                    case "k":
                    case "l":
                    case "m":
                    case "n":
                    case "o":
                    case "p":
                    case "q":
                    case "r":
                    case "s":
                    case "t":
                    case "u":
                    case "v":
                    case "w":
                    case "x":
                    case "y":
                    case "z":
                        strOutput.Append(strChar);
                        break;
                    default:
                        strOutput.Append(strMark).Append(strChar);
                        strMark = "";
                        break;
                }
            }
            strOutput.Append(strMark);
            return strOutput.ToString();
        }

        // packageNumber = so luong package co trong 1 shipment, countShipment = so luong shipmentorder da trong ngay
        public static string AutoGenPackageID(DateTime refID,string storeCode, string packageNumber, int countShipment)
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