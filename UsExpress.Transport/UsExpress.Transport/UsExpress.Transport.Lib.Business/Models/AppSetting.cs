using System.Data.Entity.Core.EntityClient;
using System.Linq;

namespace UsExpress.Transport.Lib.Business.Models
{
    public class AppSetting
    {
        public string EnvironmentName { get; set; }
        public Database Database { get; set; }

        public Kerry Kerry { get; set; }
         
        public AppSetting()
        {
            //get path appsetting.json
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            var configPath = System.IO.Path.Combine(basePath, string.Empty, "AppSetting.json");
            //get config properties
            dynamic configObject = JsonConfig.Config.ApplyJsonFromPath(configPath);

            EnvironmentName = configObject.Environment;
            dynamic config = configObject[EnvironmentName];

            Database = new Database(config.Database);
            Kerry = new Kerry(config.Kerry);
        }
    }

    public class Kerry
    {
        public string  Token { get; set; }
        public string  ApiUri { get; set; }

        public Kerry(dynamic kerry)
        {
            Token = kerry.Token;
            ApiUri = kerry.ApiUri;
        }

    }

    public class Database
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string ConnectionString { get; set; }
        public string EntityConnectionString { get; set; }
        public string MetaData { get; set; }

        public Database(dynamic database)
        {

            ConnectionString = database.ConnectionString;
            MetaData = database.Metadata;
            var dicConnection = ConnectionString.Replace(" ",string.Empty).Split(';').Select(s => s.Split('=')).ToDictionary(f => f.First().Trim(), f => f.Last().Trim());
            DataSource = dicConnection["DataSource"];
            InitialCatalog = dicConnection["InitialCatalog"];

            var entityString = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = MetaData,
                ProviderConnectionString = ConnectionString
            };

            EntityConnectionString = entityString.ConnectionString;

        }

       
    }
}