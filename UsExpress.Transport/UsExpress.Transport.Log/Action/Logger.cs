using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Log
{
    public static class GhhLog
    {
        private static ILogger _logger;

        public static void Instance()
        {
            _logger = new Log();
        }

        public static void Fatal(string message, Exception ex = null) => _logger?.Fatal(message, ex);

        public static void Error(string message, Exception ex = null) => _logger.Error(message, ex);

        public static void Warn(string message, Exception ex = null) => _logger.Warn(message, ex);

        public static void Info(string message, Exception ex = null) => _logger.Info(message, ex);

        public static void Debug(string message, Exception ex = null) => _logger.Debug(message, ex);
    }
    
    public static class GhhDBLog
    {
        private static readonly string strConnectionstring = null;

        public static void WriteLog(string mesage, string ip, string userName)
        {
            RunStoredProc(mesage, ip, userName);
        }
        private static void RunStoredProc(string mesage, string ip, string userName)
        {
            SqlConnection conn = null;
            try
            {

                using (conn = new SqlConnection(strConnectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_tblLog_INSERT", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userName;
                        cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = mesage;
                        cmd.Parameters.Add("@ip", SqlDbType.VarChar).Value = ip;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }
    }
}
