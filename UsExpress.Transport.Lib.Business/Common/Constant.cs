using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Common
{
    public class Constant
    {
        public static Business.Models.AppSetting APPSETTING { get; set; }
        public const decimal PoundToKg = 0.454M;//0.453592M;

    }
    public enum CountrySupport
    {
        USA = 1,
        VietNam = 82
    }
    public enum DestinationAirPort
    {
        HN = 1,
        HCM = 2
    }

    public enum StatusChangePass
    {
        WrongPassword = -1,
        IsChange = 1,
        Faile = 0
    }
    public class DestinationAirPortCode
    {
        public const string HCM_AIRPORT = "SGN";
        public const string HN_AIRPORT = "HAN";
    }
}
