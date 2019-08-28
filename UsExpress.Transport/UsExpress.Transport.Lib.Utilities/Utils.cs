using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;


namespace UsExpress.Transport.Lib.Utilities
{
    public class Utils
    {
        private static readonly Random Rd = new Random();
        public static string HashMD5(string InputText)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            if (String.IsNullOrEmpty(InputText.Trim()))
                return "";
            byte[] arrInput = null;
            arrInput = UnicodeEncoding.UTF8.GetBytes(InputText);
            byte[] arrOutput = null;
            arrOutput = MD5.ComputeHash(arrInput);
            return Convert.ToBase64String(arrOutput);
        }

        public static string GetComplexString(int length)
        {
            string[] strArray = new string[62]
            {
                "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "a", "b",
                "c", "d", "e", "f", "g", "h",
                "i", "j", "k", "l", "m", "n",
                "o", "p", "q", "r", "s", "t",
                "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F",
                "G", "H", "I", "J", "K", "L",
                "M", "N", "O", "P", "Q", "R",
                "S", "T", "U", "V", "W", "X",
                "Y", "Z"            };
            string str = "";
            for (int index1 = 0; index1 < length; ++index1)
            {
                int index2 = Rd.Next(0, strArray.Length - 1);
                str += strArray[index2];
            }
            return str;
        }

    }
    public static class DataHelper
    {
        /// <summary>
        /// 2 class same about property
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination Map<TDestination>(this object source)
        {
            var r = Activator.CreateInstance<TDestination>();
            r.PopulateWith(source);
            return r;
        }
        public static T BindToModel<T>(this object model)
        {
            /* Begin: Auto's fields loader */
            var adapter = new RouteValueDictionary(model);
            var obj = Activator.CreateInstance<T>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    //prop.SetValue(obj, Convert.ChangeType(adapter[prop.Name]??null, prop.PropertyType), null);                    
                    prop.SetValue(obj, ChangeType(adapter[prop.Name], prop.PropertyType), null);
                }
                catch (Exception e) { }
            }
            /* End: Auto's fields loader */

            return (T)obj;

        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
        public static string ConvertToUnSign3(this string s)
        {
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\p{IsCombiningDiacriticalMarks}+");
                    string temp = s.Normalize(NormalizationForm.FormD);
                    return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                }
                return s;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Converts a given DateTime into a Unix timestamp
        /// </summary>
        /// <param name="value">Any DateTime</param>
        /// <returns>The given DateTime in Unix timestamp format</returns>
        public static long ToUnixTimestamp(this DateTime value)
        {
            return (long)Math.Truncate((value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc))).TotalSeconds);
        }
        /// <summary>
        /// Time conver to UTC
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        public static string CheckNullDateTime(this DateTime DateTime)
        {
            if (DateTime.Year < 2018)
                return "";
            else
                return DateTime.ToString();


        }
    }
}
