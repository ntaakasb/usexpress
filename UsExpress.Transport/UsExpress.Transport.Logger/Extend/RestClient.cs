using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace UsExpress.Transport.Logger
{
    public enum httpVerb
    {
        GET, POST, PUT, DELETE
    }
    public enum authenticationType
    {
        Basic,
        NTLM
    }
    public enum authenticationTechnique
    {
        RollYourOwn,
        NetworkCredential
    }
    public class RestClient
    {
        public string endPoint { get; set; }
        public httpVerb httpMethod { get; set; }
        public authenticationType authType { get; set; }
        public authenticationTechnique authTech { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string postJSON { get; set; }

        public RestClient(string _endPoint, httpVerb method = httpVerb.POST)
        {

            endPoint = _endPoint;
            httpMethod = method;
            //httpMethod = returnmethod(method);
            authType = authenticationType.Basic;
            //userName = "landingpage-atm";
            //userPassword = "123456";
            //username = Resources.Config.API_CHECK_USERNAME;
            //password = Resources.Config.AP_CHECK_PASSWORD;
        }


        public httpVerb returnmethod(string method)
        {
            switch (method)
            {
                case "POST":
                    return httpVerb.POST;
                case "GET":
                    return httpVerb.GET;
                case "PUT":
                    return httpVerb.PUT;
                case "DELETE":
                    return httpVerb.DELETE;
                default:
                    return httpVerb.GET;
            }
        }

        public string makeRequest()
        {
            string strResponseValue = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();

            String authHeader = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", authType.ToString() + " " + authHeader);


            if (request.Method == "POST" && postJSON != string.Empty)
            {
                request.ContentType = "application/json";

                using (StreamWriter swJsonPayload = new StreamWriter(request.GetRequestStream()))
                {
                    swJsonPayload.Write(postJSON);
                    swJsonPayload.Flush();
                    swJsonPayload.Close();
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Error code: " + response.StatusCode.ToString());
                }
                //Process the response
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponseValue = "{\"errorMessage\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }
            return strResponseValue;
        }


        public async Task<string> GetResponseAsStringAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();

            String authHeader = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", authType.ToString() + " " + authHeader);

            if (request.Method == "POST" && postJSON != string.Empty)
            {
                request.ContentType = "application/json";

                using (StreamWriter swJsonPayload = new StreamWriter(request.GetRequestStream()))
                {
                    swJsonPayload.Write(postJSON);
                    swJsonPayload.Flush();
                    swJsonPayload.Close();
                }

                using (Stream postStream = await request.GetRequestStreamAsync())
                {
                    byte[] postBytes = Encoding.ASCII.GetBytes(postJSON);
                    await postStream.WriteAsync(postBytes, 0, postBytes.Length);
                    await postStream.FlushAsync();
                }
            }


            try
            {
                Task<string> Response;
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream streamResponse = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(streamResponse))
                {
                    Response = streamReader.ReadToEndAsync();
                }

               

                return await Response;
            }

            catch (Exception ee)
            {
                //OCBLog.Error("RestClient => GetResponseAsStringAsync", ee);

                return null;
            }
        }
    }
}