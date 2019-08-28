using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UsExpress.Transport.Logger.Action
{
    public class LogsAttributes
    {
        public string appname { set; get; }
        public string logtype { set; get; }
        public string path { set; get; }
        public string note { set; get; }
        public string action { set; get; }
        public DateTime createTime { set; get; }
        public string data { set; get; }

    }
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(".issued")]
        public string issued { get; set; }
        [JsonProperty(".expires")]
        public string expires { get; set; }
    }
    public static class SELog
    {
        static string apiUrl = ConfigurationSettings.AppSettings["API_URL"];
        static string apiUser = ConfigurationSettings.AppSettings["API_USER"];
        static string apiPass = ConfigurationSettings.AppSettings["API_PASS"];
        public static void WriteLog(string path, Exception excep)
        {
          try
            {
                LogsAttributes logdata = new LogsAttributes()
                {
                    appname = "frontend",
                    note = HttpContext.Current.Session["UserName"] != null ? HttpContext.Current.Session["UserName"].ToString() : "",
                    action = "",
                    logtype = "ghhlog",
                    path = path,
                    createTime = DateTime.Now,
                    data = excep.Message
                };

                var token = GetToken(apiUrl, apiUser, apiPass);
                var t = JsonConvert.DeserializeObject<Token>(token);
                string json = JsonConvert.SerializeObject(logdata);
                string url = apiUrl + "api/es/insert";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.Authorization] = "bearer " + t.access_token;
                    wc.QueryString.Add("indexname", "logs");
                    wc.QueryString.Add("typename", "docs");
                    wc.QueryString.Add("docsinsert", json);
                    var respone1 = wc.UploadValues(url, "POST", wc.QueryString);
                    var responseString = UnicodeEncoding.UTF8.GetString(respone1);
                    //var myob = JsonConvert.DeserializeObject<List<LogsAttributes>>(responseString);
                    //var datasend = myob;

                }
            }
            catch(Exception ex)
            {
                WriteLog(ex.Message);
            }
          
        }
        private static string GetToken(string url, string userName, string password)
        {

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(apiUrl + "Token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static void WriteLog(string str)
        {
            try
            {
                bool exists = System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/error/");

                if (!exists)
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/error/");
                using (var writer = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/error/" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "error.log", true))
                {
                    StringBuilder err = new StringBuilder();
                    err.Append("\n--------------------------------------------\nDate: " + DateTime.Now.ToString("dd/MM/yyyy HH:ss") + "\n");
                    err.Append("\nError Message:" + str);
                    writer.WriteLine(err);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
