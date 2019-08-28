using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Common
{
    public class EntitytModel
    {
        public string Environment { get; set; }
        [JsonProperty(PropertyName = "Dev")]
        public ConnectModel Dev { get; set; }
        [JsonProperty(PropertyName = "Prod")]
        public ConnectModel Prod { get; set; }
    }

    public class ConnectModel
    {
        public DBModel Database { get; set; }
    }

    public class DBModel
    {
        [JsonProperty(PropertyName = "ConnectionString")]
        public string ConnectionString { get; set; }
        [JsonProperty(PropertyName = "Metadata")]
        public string Metadata { get; set; }
    }
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
            this.strDataString = System.Configuration.ConfigurationSettings.AppSettings.Get("conn");
            if(!string.IsNullOrEmpty(this.strDataString))
            {
                EntitytModel model = JsonConvert.DeserializeObject<EntitytModel>(this.strDataString);
                if (model.Environment.Equals("Prod"))
                {
                    this.strDataString = model.Prod.Database.ConnectionString;
                }
                else
                {
                    this.strDataString = model.Dev.Database.ConnectionString;
                }
            }
           
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
}
