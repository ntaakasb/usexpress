using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Logger
{
    public static class DBLog
    {
        public static string EnvironmentName { get; set; }
        public static void WriteLog()
        {
            connectionString();
        }

        public static void connectionString()
        {
            JObject configObject = JObject.Parse(ConfigurationSettings.AppSettings["conn"]);
            EnvironmentName = configObject["Environment"] + "";
            var config = configObject[EnvironmentName];
           
            //string connectstring = "data source="+config.u+";initial catalog=[database name];integrated security=false;persist security info=True;User ID=[user];Password=[password]"
        }
    }
}
