using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmazonProductAdvertising.Client
{
    /// <summary>
    /// Author:BinhNQ
    /// Created Date: 2017-11-30
    /// </summary>
    public class Client
    {
        public static string _Username { get; set; }
        public static string _Password { get; set; }
        public static string _Token { get; set; }
        public static string _UriApi { get; set; }
        public static void GetToken()
        {
            var error = Error.Get(Error.SUCCESS);
            try
            {
                if (!string.IsNullOrEmpty(_Token)) return;
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri(_UriApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", _Username),
                        new KeyValuePair<string, string>("password", _Password),
                    });

                    //send request
                    var responseMessage = client.PostAsync("token", formContent).Result;
                    //get access token from response body
                    var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                    var jObject = JObject.Parse(responseJson);
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        _Token = jObject.GetValue("access_token").ToString();
                    }
                    else
                    {
                        throw new Exception("Get Token failed!" + "\n" + responseJson);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                error = null;
            }
        }

      
        public static ErrorObject GetRequest(string apiUrl,string token)
        {
            var error = Error.Get(Error.SUCCESS);
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (string.IsNullOrEmpty(_Token)) throw new Exception("Token is empty");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    //make request
                    var response = client.GetAsync(apiUrl).Result;
                    var responseJson = response.Content.ReadAsStringAsync().Result;
                    //var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseJson);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    return JsonConvert.DeserializeObject<ErrorObject>(responseJson);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                error = null;
            }
        }

        public static ErrorObject GetRequest(string apiUrl)
        {
            try
            {
                return GetRequest(apiUrl,_Token);
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ErrorObject PostRequest(string apiUrl, List<KeyValuePair<string, string>> Params, string token = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_UriApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (string.IsNullOrEmpty(_Token)) throw new Exception("Token is empty");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _Token);
                    var formContent = new FormUrlEncodedContent(Params);
                    var responseMessage = client.PostAsync(apiUrl, formContent).Result;
                    var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    return JsonConvert.DeserializeObject<ErrorObject>(responseJson);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<ErrorObject> PostRequestAsync(string apiUrl, List<KeyValuePair<string, string>> Params, string token = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_UriApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (string.IsNullOrEmpty(_Token)) throw new Exception("Token is empty");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _Token);
                    var formContent = new FormUrlEncodedContent(Params);
                    var responseMessage = await client.PostAsync(apiUrl, formContent);
                    var responseJson = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    return await Task.FromResult(JsonConvert.DeserializeObject<ErrorObject>(responseJson));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ErrorObject PutRequest(string apiUrl, List<KeyValuePair<string, string>> Params, string token = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_UriApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (string.IsNullOrEmpty(_Token)) throw new Exception("Token is empty");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _Token);
                    var formContent = new FormUrlEncodedContent(Params);
                    var responseMessage = client.PutAsync(apiUrl, formContent).Result;
                    var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    return JsonConvert.DeserializeObject<ErrorObject>(responseJson);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ErrorObject DeleteRequest(string apiUrl, string token = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_UriApi);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (string.IsNullOrEmpty(_Token)) throw new Exception("Token is empty");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer "  +_Token);
                    var responseMessage = client.DeleteAsync(apiUrl).Result;
                    var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(responseJson);
                    }
                    return JsonConvert.DeserializeObject<ErrorObject>(responseJson);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
