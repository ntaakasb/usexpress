using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Utilities
{
    public class Utils
    {
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
        public static int ToUnixTimestamp(this DateTime value)
        {
            return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
