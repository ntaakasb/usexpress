using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Web.FrontEnd.Models;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Common;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class ReportController : BaseAdminController
    {
        private readonly IOrderService _orderServices;
        private readonly IWareHouseServices _wareHouseServices;
        public ReportController(IStoreServices storeService, IUserService userService, IOrderService oderServices, IWareHouseServices wareHouseServices) : base( storeService, userService)
        {
            _orderServices = oderServices;
            _wareHouseServices = wareHouseServices;
        }
        [Filters.Auth]
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        [Filters.Auth]
        public ActionResult ReportOrder()
        {
            //DateTime fromdate = DateTime.Now.AddDays(-30);
            //DateTime todate = DateTime.Now;
            //List<ReportOrderModel> lsResult = new List<ReportOrderModel>();
            //lsResult = _orderServices.getReportOrder(-1, -1, fromdate, todate, 10, 1);
            ViewBag.Warehouse = _wareHouseServices.GetListWarehouse(1, 9999, null, -1).Select(x => new SelectItemBase()
            {
                Id = x.id.ToString(),
                Name = x.Warehouse
            }).ToList();
            return View();
        }

        public ActionResult _gridReportOrder(int storeID = -1, int wareHouseId = -1, string fromdate = null, string todate = null, int pageIndex = 1, int isActive = 1)
        {
            DateTime dtmFromdate = DateTime.Now.AddDays(-30);
            DateTime dtmTodate = DateTime.Now;
            if (fromdate != null)
            {
                dtmFromdate = DateTime.Parse(fromdate);
            }
            else
            {
                dtmFromdate = DateTime.Now.AddDays(-30);
            }
            if(todate != null)
            {
                dtmTodate = DateTime.Parse(todate);
            }
            else
            {
                dtmTodate = DateTime.Now;
            }
            List<ReportOrderModel> lsResult = new List<ReportOrderModel>();
            pageIndex--;
            if(Libs.IsStore())
            {
                storeID = int.Parse(Session[Constant.SessionStoreID].ToString());
            }

            lsResult = _orderServices.getReportOrder(storeID, wareHouseId, dtmFromdate, dtmTodate, isActive, pageSize, pageIndex);
            if(lsResult!= null && lsResult.Any())
            {
                ViewBag.TotalRows = lsResult.Count > 0 ? lsResult[0].TOTALROWS : 0;
                ViewBag.Index = pageSize * (pageIndex - 1) + 1;
            }
          
            return PartialView(lsResult);
        }

        [Filters.Auth]
        public ActionResult ExportOrder(int storeID = -1, int wareHouseId = -1, string fromdate = null, string todate = null, int pageIndex = 1, int isActive = 1)
        {
            try
            {
                DateTime dtmFromdate = DateTime.Now.AddDays(-30);
                DateTime dtmTodate = DateTime.Now;
                if (fromdate != null)
                {
                    dtmFromdate = DateTime.Parse(fromdate);
                }
                else
                {
                    dtmFromdate = DateTime.Now.AddDays(-30);
                }
                if (todate != null)
                {
                    dtmTodate = DateTime.Parse(todate);
                }
                else
                {
                    dtmTodate = DateTime.Now;
                }
                if (Libs.IsStore())
                {
                    storeID = int.Parse(Session[Constant.SessionStoreID].ToString());
                }
                var lsResult = _orderServices.getReportOrder(storeID, wareHouseId, dtmFromdate, dtmTodate, isActive, -1, 0); // lấy all
                if (lsResult.Any())
                {
                    XLWorkbook wb = new XLWorkbook();
                        wb.Worksheets.Add(MapTable(lsResult), "CurrencyTermRate");

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename={DateTime.Now:yyyyMMdd}_ReportOrder.xlsx");
                    using (MemoryStream myMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(myMemoryStream);
                        myMemoryStream.WriteTo(Response.OutputStream);
                        Response.End();
                        return Content("Success");
                    }
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                _logHelper.Info(ex);
            }
            return RedirectToAction("ReportOrder");
        }

        private DataTable MapTable(List<ReportOrderModel> table)
        {
            //  GLinks is the new DataTable I want to construct and bind to a DataList, Repeater, ListView  
            DataTable gLinks = new DataTable();
            DataRow dr = null;
            gLinks.Columns.Add(new DataColumn("STT", typeof(long)));
            gLinks.Columns.Add(new DataColumn("Code", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Sender", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Sender phone", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Sender address", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Recipient", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Recipient phone", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Recipient address", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Weight", typeof(decimal)));
            gLinks.Columns.Add(new DataColumn("Fee", typeof(decimal)));
            gLinks.Columns.Add(new DataColumn("TotalFee", typeof(decimal)));
            gLinks.Columns.Add(new DataColumn("TotalValue", typeof(decimal)));
            gLinks.Columns.Add(new DataColumn("TotalNumberItems", typeof(long)));
            gLinks.Columns.Add(new DataColumn("Store", typeof(string)));
            gLinks.Columns.Add(new DataColumn("CreateDate", typeof(string)));
            gLinks.Columns.Add(new DataColumn("PickupDate", typeof(string)));
            gLinks.Columns.Add(new DataColumn("ShippingDate", typeof(string)));
            gLinks.Columns.Add(new DataColumn("ClearVNCustom", typeof(string)));
            gLinks.Columns.Add(new DataColumn("Delivery", typeof(long)));
            gLinks.Columns.Add(new DataColumn("RecipientCity", typeof(string)));

            foreach (var row in table)
            {

                dr = gLinks.NewRow();
                dr["STT"] = row.STT;
                dr["Code"] = row.Code;
                dr["Sender"] = row.Sender;
                dr["Sender phone"] = row.PhoneSender;
                dr["Sender address"] = row.AddressSender;
                dr["Recipient"] = row.Recipient;
                dr["Recipient phone"] = row.PhoneRecippient;
                dr["Recipient address"] = row.AddressRecippient;
                dr["Fee"] = row.Fee;
                dr["Weight"] = row.Totalweigh;
                dr["TotalFee"] = row.Totalfee;
                dr["TotalValue"] = row.Totalvalue;
                dr["TotalNumberItems"] = row.Totalpackage;
                dr["Store"] = row.StoreName;
                dr["CreateDate"] = row.CreateDate.UnixTimeStampToDateTime().CheckNullDateTime();
                dr["PickupDate"] = row.PickupDate.UnixTimeStampToDateTime().CheckNullDateTime();
                dr["ShippingDate"] = row.ShippingDate.UnixTimeStampToDateTime().CheckNullDateTime();
                dr["ClearVNCustom"] = row.ClearCustomDate.UnixTimeStampToDateTime().CheckNullDateTime();
                dr["Delivery"] = row.Deliver;
                dr["RecipientCity"] = row.Recipient;

                gLinks.Rows.Add(dr);
            }

            return gLinks;
        }
    }
}