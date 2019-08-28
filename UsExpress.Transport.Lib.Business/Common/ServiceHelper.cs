using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Common
{
    public class DataRowKeyAttribute : Attribute
    {
        private readonly string _Key;

        public string Key
        {
            get { return _Key; }
        }

        public DataRowKeyAttribute(string key)
        {
            _Key = key;
        }
    }
    public static class ServiceHelper
    {
        public static string GetDestionationCodeById(int destinationId)
        {
            string result = "";
            switch ((Common.DestinationAirPort)destinationId)
            {
                case Common.DestinationAirPort.HN:
                    result = Common.DestinationAirPortCode.HN_AIRPORT;
                    break;
                default:
                    result = Common.DestinationAirPortCode.HCM_AIRPORT;
                    break;
            }
            return result;
        }
        public static List<T> ToGenericList<T>(this DataTable datatable) where T : new()
        {
            return (from row in datatable.AsEnumerable()
                    select Convert<T>(row)).ToList();
        }
        private static T Convert<T>(DataRow row) where T : new()
        {
            var result = new T();

            var type = result.GetType();

            foreach (var fieldInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var dataRowKeyAttribute = fieldInfo.GetCustomAttributes(typeof(DataRowKeyAttribute), true).FirstOrDefault() as DataRowKeyAttribute;
                if (dataRowKeyAttribute != null)
                {
                    if (row.Table.Columns.Contains(dataRowKeyAttribute.Key))
                    {
                        var value = row[dataRowKeyAttribute.Key];
                        if (value != DBNull.Value)
                        {
                            fieldInfo.SetValue(result, value, null);
                        }
                    }
                }
            }

            return result;
        }
    }
}
