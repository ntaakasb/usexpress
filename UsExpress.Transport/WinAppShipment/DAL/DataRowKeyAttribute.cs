using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace Entity.Extend
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
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
}
