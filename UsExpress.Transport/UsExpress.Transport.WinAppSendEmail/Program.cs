using AmazonProductAdvertising.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using System.Data.Entity;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.WinAppSendEmail
{
    public class Program
    {
        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Api _Api { get; set; }
        public static string _EmailBCC { get; set; }
        public static string _EmailCC { get; set; }
        static List<EmailModel> _emailTemplates = new List<EmailModel>();
        static List<EmailModel> _emailTemplateAmazon = new List<EmailModel>();
        static List<OrderModel> lstOrder = new List<OrderModel>();
        static List<tblPackageInfo> lstPackageInfo = new List<tblPackageInfo>();
        private static string _htmlHeader;
        private static string _htmlFooter;
        private static string _htmlOrderInfo;
        private static string _htmlProductDetail;
        //Email Launching event
        public static string _htmlRemind;
        public static string _htmlFeedback1;
        public static string _htmlFeedback2;

        public static string _htmlMaintenance;
        static void Main(string[] args)
        {

            Console.WriteLine("Send mail start");
            Init();
            Thread t1 = new Thread(() =>
            {
                while (true)
                {

                    try
                    {
                        ProcessEmail();
                    }
                    catch (Exception ex)
                    {
                        Libraries.WriteLog(ex.Message);
                    }
                    Thread.Sleep(Int32.Parse(ConfigurationSettings.AppSettings["TIME_SLEEP"]));
                }
            });
            t1.IsBackground = false;
            t1.Start();

        }
        private static void Init()
        {
            try
            {
                //init api
                _EmailBCC = ConfigurationSettings.AppSettings["EMAIL_BCC"];
                _EmailCC = ConfigurationSettings.AppSettings["EMAIL_CC"];
                var apiUrl = ConfigurationSettings.AppSettings["API_URL"];
                var apiUser = ConfigurationSettings.AppSettings["API_USER"];
                var apiPass = ConfigurationSettings.AppSettings["API_PASS"];
                _Api = new Api(apiUrl, apiUser, apiPass);
                //init data
                getEmailTemplate();
                InitTemplateBody(_emailTemplates, "EmailTemplate");

            }
            catch (Exception e)
            {
                Libraries.WriteLog(e.Message);
            }
        }
        private static void getEmailTemplate()
        {
            var lstEmail = new List<EmailModel>();
            using (var conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn2"]))
            {
                SqlDataAdapter sda = new SqlDataAdapter("select * from EmailTemplate", conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    sda.Fill(dt);
                    _emailTemplates = (from dt1 in dt.AsEnumerable()
                                       select new EmailModel
                                       {
                                           Id = dt1.Field<int>("Id"),
                                           Body = dt1.Field<string>("Body").ToString(),
                                           Subject = dt1.Field<string>("Subject"),
                                           Description = dt1.Field<string>("Description")

                                       }
                            ).ToList();
                }
                catch (SqlException se)
                {
                    Libraries.WriteLog(se.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private static void getOrder()
        {
            using (var conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn"]))
            {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT O.ID,O.Code,O.CreateDate,O.Status,SA.Email EmailStore,SA.FullName NameStore,SD.Email EmailSender,SD.FullName NameSender,SD.Add1 Address1,SD.Add2 Address2,SD.Zip,SD.Phone,RI.FullName NameRecipient,RI.Add1 Address1Recipient,RI.Phone PhoneRecipient  FROM tblOrder O,tblStoreAccount SA,tblSender SD,tblRecipientsInfo RI WHERE O.NotifyToCustomer=0 AND O.StoreId=SA.id AND O.SenderId=SD.Id AND O.RecipientId=RI.id", conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    sda.Fill(dt);
                    lstOrder = (from dt1 in dt.AsEnumerable()
                                select new OrderModel
                                {
                                    ID = dt1.Field<long>("ID"),
                                    Code = dt1.Field<string>("Code").ToString(),
                                    CreateDate = dt1.Field<long>("CreateDate"),
                                    EmailStore = dt1.Field<string>("EmailStore"),
                                    NameStore = dt1.Field<string>("NameStore"),
                                    EmailSender = dt1.Field<string>("EmailSender"),
                                    NameSender = dt1.Field<string>("NameSender"),
                                    Address1 = dt1.Field<string>("Address1"),
                                    Address2 = dt1.Field<string>("Address2"),
                                    Zip = dt1.Field<string>("Zip"),
                                    Phone = dt1.Field<string>("Zip"),
                                    NameRecipient = dt1.Field<string>("NameRecipient"),
                                    Address1Recipient = dt1.Field<string>("Address1Recipient"),
                                    PhoneRecipient = dt1.Field<string>("PhoneRecipient"),
                                    Status = dt1.Field<int>("Status")
                                }
                            ).ToList();
                }
                catch (SqlException se)
                {
                    Libraries.WriteLog(se.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private static OrderModel getOrderByID(long id)
        {
            var _order = new OrderModel();
            using (var conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn2"]))
            {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT O.ID,O.Code,O.CreateDate,O.Status,SA.Email EmailStore,SA.FullName NameStore,SD.Email EmailSender,"+
                                                        "SD.FullName NameSender, SD.Add1 Address1, SD.Add2 Address2,SD.CityId,SD.StateId, SD.Zip, SD.Phone, RI.FullName NameRecipient," +
                                                        "(RI.Add1 + (CASE WHEN WA.WardName IS NOT NULL" +
                                                           " THEN(',' + WA.WardName)" +
                                                           " ELSE '' END) + (CASE WHEN DT.Name IS NOT NULL" +
                                                           " THEN(',' + DT.Name)" +
                                                           " ELSE '' END)+(CASE WHEN CI.Name IS NOT NULL" +
                                                           " THEN(',' + CI.Name)" +
                                                           " ELSE '' END)) Address1Recipient,RI.Phone PhoneRecipient" +
                                                        " FROM tblOrder O" +
                                                        " LEFT JOIN tblStoreAccount SA ON O.StoreId = SA.id" +
                                                        " LEFT JOIN tblSender SD ON O.SenderId = SD.Id" +
                                                        " LEFT JOIN tblRecipientsInfo RI ON O.RecipientId = RI.id" +
                                                        " LEFT JOIN tblWard WA on RI.WardId = WA.id" +
                                                        " left join tblDistrictStateProvice DT on RI.DistrictId = DT.id" +
                                                        " left join tblStateProvice CI on RI.CityId = CI.id" +
                                                        " WHERE O.ID = "+ id, conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    sda.Fill(dt);
                    _order = (from dt1 in dt.AsEnumerable()
                              select new OrderModel
                              {
                                  ID = dt1.Field<long>("ID"),
                                  Code = dt1.Field<string>("Code").ToString(),
                                  CreateDate = dt1.Field<long>("CreateDate"),
                                  EmailStore = dt1.Field<string>("EmailStore"),
                                  NameStore = dt1.Field<string>("NameStore"),
                                  EmailSender = dt1.Field<string>("EmailSender"),
                                  NameSender = dt1.Field<string>("NameSender"),
                                  Address1 = dt1.Field<string>("Address1"),
                                  Address2 = dt1.Field<string>("Address2"),
                                  CityId = dt1.Field<string>("CityId"),
                                  StateId = dt1.Field<string>("StateId"),
                                  Zip = dt1.Field<string>("Zip"),
                                  Phone = dt1.Field<string>("Phone"),
                                  NameRecipient = dt1.Field<string>("NameRecipient"),
                                  Address1Recipient = dt1.Field<string>("Address1Recipient"),
                                  PhoneRecipient = dt1.Field<string>("PhoneRecipient"),
                                  Status = dt1.Field<int>("Status")
                              }
                            ).FirstOrDefault();
                }
                catch (SqlException se)
                {
                    Libraries.WriteLog(se.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return _order;
        }
        //private static void getPackage(long orderId)
        //{
        //    using (var conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn2"]))
        //    {
        //        SqlDataAdapter sda = new SqlDataAdapter("select PIF.id,PIF.Weight,PIF.Width,PIF.Depth,PIF.Height,PIF.Fee,PIF.ShipmentCode,IIP.* from tblPackageInfo PIF,tblItemInPackage IIP where PIF.OrderId="+ orderId + " AND IIP.PackageId=PIF.id", conn);
        //        DataTable dt = new DataTable();
        //        try
        //        {
        //            conn.Open();
        //            sda.Fill(dt);
        //            lstPackageInfo = (from dt1 in dt.AsEnumerable()
        //                        select new PackageModel
        //                        {
        //                            id = dt1.Field<int>("id"),
        //                            PackageId= dt1.Field<int?>("PackageId"),
        //                            Description = dt1.Field<string>("Description"),
        //                            Code = dt1.Field<string>("Code"),
        //                            Quantity = dt1.Field<int?>("Quantity"),
        //                            Value = dt1.Field<decimal?>("Value"),
        //                            Unit = dt1.Field<string>("Unit"),
        //                            TPI = new tblPackageInfo()
        //                            {
        //                                id = dt1.Field<int>("id"),
        //                                Weight = dt1.Field<decimal?>("Weight"),
        //                                Depth = dt1.Field<decimal?>("Depth"),
        //                                Height = dt1.Field<decimal?>("Height"),
        //                                Fee = dt1.Field<decimal?>("Fee"),
        //                                ShipmentCode = dt1.Field<string>("ShipmentCode"),

        //                            }

        //                        }
        //                    ).ToList();
        //        }
        //        catch (SqlException se)
        //        {
        //            Libraries.WriteLog(se.Message);
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //}
        private static void InitTemplateBody(List<EmailModel> _emailTemplatesA, string folderTemplate)
        {
            foreach (var emailTemplate in _emailTemplatesA)
            {
                var fileEmail = AppDomain.CurrentDomain.BaseDirectory + "\\" + folderTemplate + "\\EmailTemplate_" + emailTemplate.Id + ".html";
                if (File.Exists(fileEmail))
                {
                    var model = new EmailModel();
                    model.Body = System.IO.File.ReadAllText(fileEmail);
                    model.Description = emailTemplate.Description;
                    model.Id = emailTemplate.Id;
                    model.Subject = emailTemplate.Subject;
                    if (folderTemplate == "EmailTemplate")
                    {
                        _emailTemplateAmazon.Add(model);
                    }
                }
            }
        }
        //private static void ProcessEmail()
        //{
        //    try
        //    {
        //        Console.WriteLine("Thread start: " + DateTime.Now);
        //        getOrder();

        //        if (lstOrder.Any())
        //        {
        //            foreach (var order in lstOrder)
        //            {
        //                if (order != null)
        //                {
        //                    //getPackage(order.ID);
        //                    using (var db = ContextFactory.UsTransportEntities())
        //                    {
        //                        lstPackageInfo = db.tblPackageInfoes.Include(y => y.tblItemInPackages).Where(x => x.OrderId == order.ID).Select(x => x).ToList();
        //                    }
        //                    var _status = order.Status + 1;
        //                    var emailTemplate = _emailTemplateAmazon.First(x => x.Id == _status);

        //                    var email = new EmailModel()
        //                    {
        //                        Subject = emailTemplate.Subject,
        //                        Body = BuildContentEmail(lstPackageInfo, order, emailTemplate.Body)
        //                    };
        //                    Console.WriteLine("Send mail to: " + order.EmailStore);
        //                    var result = _Api._EmailOperation.Send(order.EmailStore, email.Subject, email.Body, _EmailCC, _EmailBCC);
        //                    //var result = _Api._EmailOperation.SendWithBcc("voduchiendhcn@hotmail.com", email.Subject, email.Body, "");
        //                    if (result.Code == 0)
        //                    {
        //                        Console.WriteLine("Success");
        //                        using (SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn2"]))
        //                        {
        //                            conn.Open();
        //                            SqlCommand command = new SqlCommand("UPDATE tblOrder SET NotifyToCustomer=1 WHERE id = @id", conn);
        //                            command.Parameters.AddWithValue("@id", order.ID);
        //                            command.ExecuteNonQuery();
        //                        }
        //                    }
        //                    //else
        //                    //{

        //                    //}


        //                }

        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);

        //    }
        //}
        private static void ProcessEmail()
        {
            try
            {
                Console.WriteLine("Thread start: " + DateTime.Now.ToShortTimeString());

                //getPackage(order.ID);
                using (var db = ContextFactory.UsTransportEntities())
                {
                    lstPackageInfo = db.tblPackageInfoes.Include(y => y.tblItemInPackages).Where(x => x.NotifyToCustomer == 0).ToList();
                    var _order = new OrderModel();
                    foreach (var item in lstPackageInfo)
                    {
                        _order = getOrderByID(item.OrderId);
                        if (_order != null)
                        {
                            var _status = _order.Status + 1;
                            var emailTemplate = _emailTemplateAmazon.First(x => x.Id == _status);

                            var email = new EmailModel()
                            {
                                Subject = emailTemplate.Subject.Replace("${Code}", item.Code),
                                Body = BuildContentEmail2(item, _order, emailTemplate.Body)
                            };
                            Console.WriteLine("Send mail to: " + _order.EmailStore);
                            var result = _Api._EmailOperation.Send(_order.EmailStore, email.Subject, email.Body, _EmailCC, _EmailBCC);
                            //var result = _Api._EmailOperation.SendWithBcc("voduchiendhcn@hotmail.com", email.Subject, email.Body, "");
                            if (result.Code == 0)
                            {
                                Console.WriteLine("Success");
                                using (SqlConnection conn = new SqlConnection(ConfigurationSettings.AppSettings["Conn2"]))
                                {
                                    conn.Open();
                                    SqlCommand command = new SqlCommand("UPDATE tblPackageInfo SET NotifyToCustomer=1 WHERE id = @id", conn);
                                    command.Parameters.AddWithValue("@id", item.id);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            Libraries.WriteLog("ProcessEmail Error - Không có orderID " + item.OrderId);
                        }

                    }
                }

                //else
                //{

                //}


            }
            catch (Exception e)
            {
                Libraries.WriteLog(e.Message);

            }
        }
        private static string BuildContentEmail(List<tblPackageInfo> Lstorder, OrderModel orderItem, string body)
        {
            var str = "";
            foreach (var order in Lstorder)
            {
                Dictionary<string, string> dicContentKey = new Dictionary<string, string>();
                //header
                dicContentKey.Add("${code}", order.Code);
                //footer
                dicContentKey.Add("${createdate}", order.CreateDate.ToString());
                //orderInfo
                dicContentKey.Add("${sender}", "Sender: " + orderItem.NameSender + "<br/> Address: " + orderItem.Address1 ?? orderItem.Address2 + "</br> Zip: " + orderItem.Zip + "</br> Phone: " + orderItem.Phone);
                dicContentKey.Add("${recipient}", "Recipient: " + orderItem.NameRecipient + "<br/> Address: " + orderItem.Address1Recipient + "</br> Phone: " + orderItem.PhoneRecipient);
                dicContentKey.Add("${weight}", order.Weight + "");
                dicContentKey.Add("${size}", order.Height + " x " + order.Width + " x " + order.Depth);
                dicContentKey.Add("${fee}", order.Fee + "");
                dicContentKey.Add("${datenow}", DateTime.Now + "");


                var statusDesc = string.Empty;
                switch (orderItem.Status + 1)
                {
                    case 1:
                        statusDesc = "Đơn hàng mới";
                        break;
                    case 2:
                        statusDesc = "Giao hàng thành công";
                        break;

                }

                //Build HTML Product
                var itemInPackage = "";
                foreach (var item in order.tblItemInPackages)
                {
                    itemInPackage += "<tr style='margin:0;padding:0'><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Description + "</td><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Quantity + "</td><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Value + "</td></tr>";
                }
                dicContentKey.Add("${content}", itemInPackage);

                //Replace Value
                foreach (KeyValuePair<string, string> pair in dicContentKey)
                {
                    string key = pair.Key;
                    string value = pair.Value;

                    body = body.Replace(key, value);
                }
                str += body;

            }

            return str;
        }
        private static string BuildContentEmail2(tblPackageInfo _tblPackageInfo, OrderModel orderItem, string body)
        {
            var str = "";

            Dictionary<string, string> dicContentKey = new Dictionary<string, string>();
            //header
            dicContentKey.Add("${code}", _tblPackageInfo.Code);
            //footer
            dicContentKey.Add("${createdate}", _tblPackageInfo.CreateDate.ToString());
            //orderInfo
            dicContentKey.Add("${sender}", "Sender: " + orderItem.NameSender + "<br/> Address: " + (orderItem.Address1 ?? orderItem.Address2) + ", " + (orderItem.CityId)+"<br/> State: " + (orderItem.StateId)  + "</br> Zip: " + (orderItem.Zip ?? "") + "</br> Phone: " + (orderItem.Phone ?? ""));
            dicContentKey.Add("${recipient}", "Recipient: " + orderItem.NameRecipient + "<br/> Address: " + orderItem.Address1Recipient + "</br> Phone: " + (orderItem.PhoneRecipient ?? ""));
            dicContentKey.Add("${weight}", _tblPackageInfo.Weight + "");
            dicContentKey.Add("${size}", _tblPackageInfo.Height + " x " + _tblPackageInfo.Width + " x " + _tblPackageInfo.Depth);
            dicContentKey.Add("${fee}", Math.Ceiling(_tblPackageInfo.Weight.Value) * Convert.ToDecimal(_tblPackageInfo.Fee) + "");
            dicContentKey.Add("${datenow}", DateTime.Now + "");


            var statusDesc = string.Empty;
            switch (orderItem.Status + 1)
            {
                case 1:
                    statusDesc = "Đơn hàng mới";
                    break;
                case 2:
                    statusDesc = "Giao hàng thành công";
                    break;

            }

            //Build HTML Product
            var itemInPackage = "";
            foreach (var item in _tblPackageInfo.tblItemInPackages)
            {
                itemInPackage += "<tr style='margin:0;padding:0'><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Description + "</td><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Quantity + "</td><td style='border: 1px solid #d3d3d3;padding:5px'>" + item.Value + "</td></tr>";
            }
            dicContentKey.Add("${content}", itemInPackage);

            //Replace Value
            foreach (KeyValuePair<string, string> pair in dicContentKey)
            {
                string key = pair.Key;
                string value = pair.Value;

                body = body.Replace(key, value);
            }
            str += body;


            return str;
        }
    }
}
