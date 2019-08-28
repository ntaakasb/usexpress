using System;
using System.Security.AccessControl;

namespace AmazonProductAdvertising.Client
{
    public class Api
    {
        public EmailOperation _EmailOperation { get; set; }
        public string _ApiUrl { get; set; }
        public string _ApiUsername { get; set; }
        public string _ApiPassword { get; set; }
        public string _ApiToken { get; set; }
        
        public Api(string ApiUrl, string Username, string Password)
        {
            Init( ApiUrl,  Username,  Password);
        }

        public void Init(string ApiUrl, string Username, string Password)
        {
            _ApiUrl = Client._UriApi = ApiUrl;
            _ApiUsername = Client._Username = Username;
            _ApiPassword = Client._Password = Password;
            if (string.IsNullOrEmpty(ApiUrl) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                throw new Exception("Parameter input invalid!");
            }
            Client.GetToken();
            _ApiToken = Client._Token;
            _EmailOperation = new EmailOperation();
        }
      }

}