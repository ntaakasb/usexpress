using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAL
{
    public class DatabaseClass
    {
        public SqlConnection dataConn;
        public SqlCommand dataComm;
        public SqlDataReader dataRead;
        public SqlTransaction dataTran;
        public SqlDataAdapter dataAdap;

        public string strDataString = "";

        //
        public DatabaseClass()
        {
            this.strDataString = System.Configuration.ConfigurationSettings.AppSettings.Get("ConnectionStringModel");
        }
        //'Khởi tạo Object
        public DatabaseClass(string ConnectString)
        {
            this.strDataString = ConnectString;
        }

        public void ConnectData()
        {
            if (dataConn == null) dataConn = new SqlConnection();
            if (dataConn.State == System.Data.ConnectionState.Closed)
            {
                dataConn.ConnectionString = strDataString;
                dataConn.Open();
            }
            if (dataComm == null) dataComm = new SqlCommand();
            if (dataAdap == null) dataAdap = new SqlDataAdapter();
            dataComm.Connection = dataConn;
            dataComm.CommandTimeout = 120;
            dataComm.CommandType = System.Data.CommandType.Text;

        }

        //Đóng kết nối dữ liệu
        public void CloseData()
        {
            if (dataRead != null) dataRead.Close();
            if (dataConn != null)
            {
                if (dataConn.State == System.Data.ConnectionState.Open) dataConn.Close();
            }
            dataRead = null;
            dataConn = null;
        }

        //Chạy câu lệnh ExecuteNonQuery (không trả về dữ liệu)
        public void ExeNonQuery(string strQuery)
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataComm.CommandText = strQuery;
            dataComm.ExecuteNonQuery();
        }
        public int ExecuteNonQuery(string strQuery)
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataComm.CommandText = strQuery;
            return dataComm.ExecuteNonQuery();
        }


        //Chạy câu lệnh ExecuteReader (có hoặc không trả về dữ liệu - lưu ý đóng dữ liệu)
        public void ExeReader(string strQuery)
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataComm.CommandText = strQuery;
            dataRead = dataComm.ExecuteReader();
        }

        public DataSet ExeDataset(string strQuery)
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataComm.CommandText = strQuery;
            dataAdap.SelectCommand = dataComm;
            DataSet ds = new DataSet();
            dataAdap.Fill(ds);
            return ds;
        }

        public object ExeScalar(string strQuery)
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataComm.CommandText = strQuery;
            return dataComm.ExecuteScalar();
        }

        //Bắt đầu Transaction
        public void BeginTrans()
        {
            if (dataConn == null) ConnectData();
            if (dataRead != null) dataRead.Close();
            dataTran = dataConn.BeginTransaction();
            dataComm.Transaction = dataTran;
        }

        //Kết thúc Transaction
        public void CommitTrans()
        {
            if (dataRead != null) dataRead.Close();
            dataTran.Commit();
        }

        //Quay lại các lệnh cho dữ liệu
        public void RollBackTrans()
        {
            if (dataRead != null) dataRead.Close();
            dataTran.Rollback();
        }

        public SqlCommand GetCommand(string sql, SqlParameter[] pa, bool hasStore)
        {
            dataConn = new SqlConnection(strDataString);
            if (dataConn.State != ConnectionState.Open)
            {
                dataConn.Open();
            }
            dataComm = new SqlCommand(sql, dataConn);
            if (hasStore)
            {
                dataComm.CommandType = CommandType.StoredProcedure;
            }
            AddParameter(dataComm, pa);
            return dataComm;
        }
        public SqlCommand GetCommandNonParameter(string sql, bool hasStore)
        {
            dataConn = new SqlConnection(strDataString);
            if (dataConn.State != ConnectionState.Open)
            {
                dataConn.Open();
            }
            dataComm = new SqlCommand(sql, dataConn);
            if (hasStore)
            {
                dataComm.CommandType = CommandType.StoredProcedure;
            }

            return dataComm;
        }

        private void AddParameter(SqlCommand command, SqlParameter[] listPa)
        {
            foreach (SqlParameter pa in listPa)
            {
                command.Parameters.Add(pa);
            }
        }

        // kiểm tra sqldatareader có column muốn kiểm tra ko
        public static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

    }


    //public static class DataTableExtensions
    //{
    //    public static DataTable ToDataTable<T>(this IList<T> data)
    //    {
    //        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
    //        DataTable dt = new DataTable();
    //        for (int i = 0; i < properties.Count; i++)
    //        {
    //            PropertyDescriptor property = properties[i];
    //            dt.Columns.Add(property.Name, property.PropertyType);
    //        }
    //        object[] values = new object[properties.Count];
    //        foreach (T item in data)
    //        {
    //            for (int i = 0; i < values.Length; i++)
    //            {
    //                values[i] = properties[i].GetValue(item);
    //            }
    //            dt.Rows.Add(values);
    //        }
    //        return dt;
    //    }

    //    public static List<T> ToGenericList<T>(this DataTable datatable) where T : new()
    //    {
    //        return (from row in datatable.AsEnumerable()
    //                select Convert<T>(row)).ToList();
    //    }

    //    private static T Convert<T>(DataRow row) where T : new()
    //    {
    //        var result = new T();

    //        var type = result.GetType();

    //        foreach (var fieldInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
    //        {
    //            var dataRowKeyAttribute = fieldInfo.GetCustomAttributes(typeof(DataRowKeyAttribute), true).FirstOrDefault() as DataRowKeyAttribute;
    //            if (dataRowKeyAttribute != null)
    //            {
    //                if (row.Table.Columns.Contains(dataRowKeyAttribute.Key))
    //                {
    //                    var value = row[dataRowKeyAttribute.Key];
    //                    if (value != DBNull.Value)
    //                    {
    //                        fieldInfo.SetValue(result, value, null);
    //                    }
    //                }
    //            }
    //        }

    //        return result;
    //    }

    //}

    //public class DataRowKeyAttribute : Attribute
    //{
    //    private readonly string _Key;

    //    public string Key
    //    {
    //        get { return _Key; }
    //    }

    //    public DataRowKeyAttribute(string key)
    //    {
    //        _Key = key;
    //    }
    //}

    public abstract class BaseRepository
    {
        protected T XmlNodeToModel<T>(XmlNode xmlNode, params string[] includePropeties)
            where T : class, new()
        {
            return (T)XmlNodeToModel(xmlNode, typeof(T), includePropeties);
        }

        protected object XmlNodeToModel(XmlNode xmlNode, Type type, params string[] includePropeties)
        {
            if (string.Compare(xmlNode.Name, "root", 0) == 0 && xmlNode.ChildNodes.Count == 1)
                xmlNode = xmlNode.ChildNodes[0];

            if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
            {
                if (xmlNode.Attributes.Count > 0)
                    return GetValue(xmlNode.Attributes[0].Value, type);
                return GetValue(xmlNode.InnerText, type);
            }

            var model = Activator.CreateInstance(type);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanWrite && (includePropeties.Length == 0 || includePropeties.Contains(property.Name)))
                {
                    if (xmlNode.Attributes[property.Name] != null)
                    {
                        var value = GetValue(xmlNode.Attributes[property.Name].Value, property.PropertyType);
                        if (value != null)
                        {
                            property.SetValue(model, value, null);
                        }
                    }
                    else
                    {
                        var childNode = xmlNode.SelectSingleNode(property.Name);
                        if (childNode != null)
                        {
                            if (property.PropertyType.IsGenericType && property.PropertyType.GetInterface("IList") != null)
                            {
                                if (childNode.ChildNodes.Count > 0)
                                {
                                    var value = (IList)Activator.CreateInstance(property.PropertyType);
                                    var itemType = property.PropertyType.GetGenericArguments()[0];
                                    foreach (XmlNode item in childNode.ChildNodes)
                                    {
                                        value.Add(XmlNodeToModel(item, itemType));
                                    }
                                    property.SetValue(model, value, null);
                                }
                            }
                            else if (childNode.ChildNodes.Count == 1)
                            {

                                var value = XmlNodeToModel(childNode.FirstChild, property.PropertyType);
                                if (value != null)
                                    property.SetValue(model, value, null);
                            }
                        }
                    }
                }
            }

            return model;
        }

        protected object GetValue(string value, Type type, params string[] properties)
        {
            if (type.IsGenericType && type.GetInterface("IList") != null)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(value);
                    if (doc.FirstChild.ChildNodes.Count > 0)
                    {
                        var result = (IList)Activator.CreateInstance(type);
                        var itemType = type.GetGenericArguments()[0];
                        foreach (XmlNode item in doc.FirstChild.ChildNodes)
                        {
                            result.Add(XmlNodeToModel(item, itemType, properties));
                        }
                        return result;
                    }
                    else
                        return null;
                }
                catch
                {
#if DEBUG
                    throw;
#endif
                    return null;
                }
            }

            if (TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string)))
            {
                try
                {
                    if (type.FullName == typeof(bool).FullName)
                    {
                        if (value == "1" || string.Compare(value, "True", true) == 0)
                            return true;
                        else
                            return false;
                    }

                    return CommonHelperDAL.To(value, type);
                }
                catch
                {
                    //#if DEBUG
                    //                    throw;
                    //#endif
                    return null;
                }
            }

            if (type.IsClass)
            {
                var doc = new XmlDocument();
                doc.LoadXml(value);
                return XmlNodeToModel(doc.FirstChild, type, properties);
            }

            return null;
        }
        /// <summary>
        /// tra ve object tu column xml trong table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected T GetValue<T>(string value, params string[] properties)
            where T : new()
        {
            var type = typeof(T);
            var result = GetValue(value, type, properties);

            if (result != null)
                return (T)result;

            return default(T);
        }
        /// <summary>
        /// map data into class from table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public T BindData<T>(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

            // Get all columns' name
            List<string> columns = GetListColumnFromDataTable(dt);
            
            // Create object
            var ob = Activator.CreateInstance<T>();

            // Get all properties
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var itemType = propertyInfo.PropertyType.GetGenericArguments();
                if (columns.Contains(propertyInfo.Name))
                {
                    try
                    {
                        if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.FullName.StartsWith("System."))
                        {
                            // Fill the data into the property
                            propertyInfo.SetValue(ob, dr[propertyInfo.Name]);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            }

            return ob;
        }
        /// <summary>
        /// tra ve object tuong ung voi table select
        /// neu co column return xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public T BindData<T>(DataRow dr, List<string> columns)
        {            
            // Create object
            var ob = Activator.CreateInstance<T>();
           
            // Get all properties
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var itemType = propertyInfo.PropertyType.GetGenericArguments();
                if (columns.Contains(propertyInfo.Name))
                {
                    try
                    {
                        if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.FullName.StartsWith("System."))
                        {
                            // Fill the data into the property
                            propertyInfo.SetValue(ob, dr[propertyInfo.Name]);
                        }                        
                    }
                    catch (Exception)
                    {
                    }

                }
            }

            return ob;
        }
        public int ExtractInt(object data)
        {
            if (data.GetType() == typeof(int))
            {
                return (int)data;
            }
            else
            {
                int i = 0;
                int.TryParse(data + "", out i);
                return i;
            }
        }
        /// <summary>
        /// tra ve list object tu datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> BindDataList<T>(DataTable dt)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }

            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                var ob = Activator.CreateInstance<T>();

                foreach (var propertyInfo in properties)
                {
                    if (columns.Contains(propertyInfo.Name))
                    {
                        try
                        {
                            if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.FullName.StartsWith("System."))
                            {
                                // Fill the data into the property
                                propertyInfo.SetValue(ob, dr[propertyInfo.Name]);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                lst.Add(ob);
            }

            return lst;
        }

        public List<string> GetListColumnFromDataTable(DataTable dt)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }
            return columns;
        }
    }
    public class CommonHelperDAL
    {
        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                Type sourceType = value.GetType();

                TypeConverter destinationConverter = GetCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }
        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            //if (type == typeof(ShippingOption))
            //    return new ShippingOptionTypeConverter();

            return TypeDescriptor.GetConverter(type);
        }
    }
    public class GenericListTypeConverter<T> : TypeConverter
    {
        protected readonly TypeConverter _typeConverter;

        public GenericListTypeConverter()
        {
            _typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (_typeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
        }

        protected virtual string[] GetStringArray(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                string[] result = input.Split(',');
                Array.ForEach(result, s => s.Trim());
                return result;
            }
            else
                return new string[0];
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                string[] items = GetStringArray(sourceType.ToString());
                return (items.Count() > 0);
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] items = GetStringArray((string)value);
                var result = new List<T>();
                Array.ForEach(items, s =>
                {
                    object item = _typeConverter.ConvertFromInvariantString(s);
                    if (item != null)
                    {
                        result.Add((T)item);
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = string.Empty;
                if (((IList<T>)value) != null)
                {
                    //we don't use string.Join() because it doesn't support invariant culture
                    for (int i = 0; i < ((IList<T>)value).Count; i++)
                    {
                        var str1 = Convert.ToString(((IList<T>)value)[i], CultureInfo.InvariantCulture);
                        result += str1;
                        //don't add comma after the last element
                        if (i != ((IList<T>)value).Count - 1)
                            result += ",";
                    }
                }
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
