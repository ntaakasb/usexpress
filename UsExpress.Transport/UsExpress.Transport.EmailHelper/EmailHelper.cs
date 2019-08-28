using AmazonProductAdvertising.Client;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace UsExpress.Transport.EmailLib
{
    public class EmailHelper
    {

        private static string _smtpServer = "";
        private static int _smtpPort = 587;
        private static string _siteEmail = "";
        private static string _siteEmailPass = "";
        private static MailAddress _mailAddress;

        public EmailHelper()
        {
        }
        public EmailHelper(string smtpServer, int smptPort, string siteEmail, string siteEmailPass)
        {
            _mailAddress = new MailAddress(siteEmail, siteEmail);
            _siteEmail = siteEmail;
            _siteEmailPass = siteEmailPass;
            _smtpServer = smtpServer;
            _smtpPort = smptPort;
        }

        public bool SendMail(string to, string bcc, string cc, string subject, string body, bool usingHtml)
        {
            try
            {
                MailMessage message = new MailMessage()
                {
                    From = _mailAddress
                };
                message.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(bcc))
                    message.Bcc.Add(new MailAddress(bcc));
                if (!string.IsNullOrEmpty(cc))
                    message.CC.Add(new MailAddress(cc));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.Normal;
                SmtpClient smtpClient = new SmtpClient();
                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    return this.SendMail(to, cc, subject, body, usingHtml);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendMail(string toEmail, string ccEmail, string title, string body, bool usingHTML)
        {
            try
            {
                MailAddress from = new MailAddress(_siteEmail, _siteEmail);
                MailAddress to = new MailAddress(toEmail, toEmail);
                SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_siteEmail, _siteEmailPass),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };
                MailMessage message = new MailMessage(from, to);
                if (ccEmail.Trim() != "")
                    message.CC.Add(ccEmail);
                message.Subject = title;
                message.Body = body;
                message.IsBodyHtml = usingHTML;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ErrorObject SendMail(string toEmail, string title, string body)
        {
            var apiUrl = ConfigurationSettings.AppSettings["API_URL_MAIL"];
            var apiUser = ConfigurationSettings.AppSettings["API_USER_MAIL"];
            var apiPass = ConfigurationSettings.AppSettings["API_PASS_MAIL"];
            var api = new Api(apiUrl, apiUser, apiPass);

            var result = api._EmailOperation.Send(toEmail, title, body);
            return result;
        }
    }
}
