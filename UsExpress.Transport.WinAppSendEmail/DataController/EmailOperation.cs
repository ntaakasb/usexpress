using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonProductAdvertising.Client
{
    public class EmailOperation
    {
        public string _UriEmailSend { get; set; } = "api/email/send";



        public ErrorObject Send(string ToEmail, string Subject,string Body)
        {
            return Send(ToEmail, Subject, Body, string.Empty, string.Empty);
        }

        public ErrorObject SendWithCc(string ToEmail, string Subject, string Body, string CC)
        {
            return Send(ToEmail, Subject, Body, CC: CC, BCC: string.Empty);
        }

        public ErrorObject SendWithBcc(string ToEmail, string Subject, string Body, string BCC)
        {
            return Send(ToEmail, Subject, Body, CC: string.Empty, BCC: BCC);
        }

        public ErrorObject Send(string ToEmail, string Subject, string Body,string CC,string BCC)
        {
            try
            {
                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ToEmail",ToEmail),
                    new KeyValuePair<string, string>("Subject", Subject),
                    new KeyValuePair<string, string>("Body", Body),
                    new KeyValuePair<string, string>("CC", CC),
                    new KeyValuePair<string, string>("BCC", BCC)
                };
                return Client.PostRequest(_UriEmailSend, data);
            }
            catch (Exception ex)
            {
                return new ErrorObject(Error.SYSTEM, ex);
            }
        }


        public async Task<ErrorObject> SendAsync(string ToEmail, string Subject, string Body)
        {
            return await SendAsync(ToEmail, Subject, Body, string.Empty, string.Empty);
        }

        public async Task<ErrorObject> SendWithCcAsync(string ToEmail, string Subject, string Body, string CC)
        {
            return await SendAsync(ToEmail, Subject, Body, CC: CC, BCC: string.Empty);
        }

        public async Task<ErrorObject> SendWithBccAsync(string ToEmail, string Subject, string Body, string BCC)
        {
            return await SendAsync(ToEmail, Subject, Body, CC: string.Empty, BCC: BCC);
        }

        public async Task<ErrorObject> SendAsync(string ToEmail, string Subject, string Body,string CC,string BCC)
        {
            try
            {
                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ToEmail",ToEmail),
                    new KeyValuePair<string, string>("Subject", Subject),
                    new KeyValuePair<string, string>("Body", Body),
                    new KeyValuePair<string, string>("CC", CC),
                    new KeyValuePair<string, string>("BCC", BCC)
                };
                return await Client.PostRequestAsync(_UriEmailSend, data);
            }
            catch (Exception ex)
            {
                return new ErrorObject(Error.SYSTEM, ex);
            }
        }
    }
}