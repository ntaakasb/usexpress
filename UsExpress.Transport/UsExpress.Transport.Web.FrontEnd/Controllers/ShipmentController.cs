
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Web.FrontEnd.Models;

namespace UsExpress.Transport.Web.FrontEnd.Controllers
{
    public class ShipmentController : BaseAdminController
    {
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWareHouseServices _warehouseService;
        public ShipmentController(IOrderService orderService, IStoreServices storeService,
            IUserService userService, IShipmentService shipmentService,
            IWareHouseServices warehouseService)
            : base(storeService, userService)
        {
            _orderService = orderService;
            _shipmentService = shipmentService;
            _warehouseService = warehouseService;
        }
        // GET: Shipment
        [Filters.Auth]
        public ActionResult Index()
        {
            return RedirectToAction("Manager");
        }
        [Filters.Auth]
        public ActionResult Manager()
        {
            ShipmentSearch model = new ShipmentSearch
            {
                WarehouseId = -1,
                Keyword = "",
                ToTime = DateTime.Now.ToUnixTimestamp(),
                FromTime = DateTime.Now.AddDays(-30).ToUnixTimestamp()
            };

            List<ShipmentViewDTO> lsShipment = _shipmentService.Admin_SearchShipment(model);
            ListShipmentModels lsResult = new ListShipmentModels()
            {
                FromDate = DateTime.Now.AddDays(-30),
                ToDate = DateTime.Now
            };
            if (lsShipment != null && lsShipment.Any())
            {
                var todate = DateTime.Now;
                var fromdate = DateTime.Now.AddDays(-30);//lsShipment.Min(x => x.CreateTime).UnixTimeStampToDateTime();
                var result = GetPositionWithModel(lsShipment, fromdate, todate);
                lsResult = new ListShipmentModels()
                {
                    ListResult = result,
                    FromDate = fromdate,
                    ToDate = todate
                };

            }
            return View(lsResult);
        }


        //public PartialViewResult _GridOrder()
        //{
        //    PackageSearch orderSearch = new PackageSearch()
        //    {
        //        Keyword = null,
        //        PageSize = 20,
        //        PageIndex = 0
        //    };
        //    ListOrderModels lsResult = new ListOrderModels();
        //    var lsOrder = _orderService.Admin_SearchOrder(orderSearch);
        //    var todate = DateTime.Now;
        //    var fromdate = todate.AddDays(-14);
        //    lsResult.FromDate = fromdate;
        //    lsResult.ToDate = todate;
        //    lsResult.ListResult = GetPositionWithModel(lsOrder, fromdate, todate);
        //    return PartialView(lsResult);
        //}


        public List<ShipmentPositionModels> GetPositionWithModel(List<ShipmentViewDTO> model, DateTime dtmFromdate, DateTime dtmToDate)
        {

            List<ShipmentPositionModels> lsResult = new List<ShipmentPositionModels>();
            //model = model.Where(x => DateTime.Compare(dtmFromdate, DateTime.Parse(x.CreateDate)) <= 0 
            //                        && DateTime.Compare(DateTime.Parse(x.CreateDate), dtmToDate) <= 0).ToList();

            int k = 0;
            for (DateTime i = dtmFromdate; i <= dtmToDate; i = i.AddDays(1))
            {
                var colItem = model.Where(x => x.CreateTime.UnixTimeStampToDateTime().Date == i.Date);
                if (colItem.Count() > 0)
                {
                    int j = 0;
                    foreach (var _colitem in colItem)
                    {
                        ShipmentPositionModels item = new ShipmentPositionModels();
                        item.X = k;
                        item.Y = j;
                        item.ShipmentItem = _colitem;
                        lsResult.Add(item);
                        j++;
                    }
                }
                k++;
            }
            return lsResult;
        }

        //public PartialViewResult _GridPackage(List<PackageGridDTO> model)
        //{
        //    return PartialView();
        //}

        public JsonResult AJXLoadPackageOrderDetail(string ShipmentCode)
        {
            int OrderId = Request.Form["OrderId"] != null ? int.Parse(Request.Form["OrderId"].ToString()) : -1;
            var result = _shipmentService.listPackageByShipmentCode(ShipmentCode);
            if (result != null && result.Count > 0)
            {
                return Json(RenderPartialViewToString("_GridPackage", result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Data could not be found!");
            }
        }
        [Filters.Auth]
        public ActionResult CreateShipment()
        {
            var result = _warehouseService.Shipment_GetLstWarehouse().Select(x => new SelectItemBase { AttributeId = x.id, Name = x.Warehouse }).ToList();
            result.Insert(0, new SelectItemBase { AttributeId = -1, Name = "--All--" });
            ViewBag.LstWarehouse = result;
            return View();
        }
        //public ActionResult CreateShipmentPropose()
        //{
        //    var LstSGNPackage = new List<List<ShipmentPropose>>();
        //    var _LstShipment = _shipmentService.LstShipment();
        //    //var _ListSGN = _LstShipment.Where(x=>x.AirPortCode=="SGN");
        //    //var _ListHAN = _LstShipment.Where(x => x.AirPortCode == "HAN");
        //    //var groupedShipmentList = _LstShipment.Where(x => x.AirPortCode == "SGN").GroupBy(u => u.RecipientId).Select(c => new {PackageGroup=0,PackageInGroup=1, Key = c.Key, total = c.Count(), lst = c.ToList(),sumLst=c.Sum(x=>x.Weight) }).ToList();
        //    var groupedShipmentList = _LstShipment.Where(x => x.AirPortCode == "SGN").OrderByDescending(x=>x.TotalWeightKg).ToList();
        //    var numPackageSGN = Math.Ceiling(groupedShipmentList.Sum(x=>x.TotalWeightKg) /80);
        //    //var numPackageHAN = Math.Ceiling(_ListHAN.Sum(x => x.Weight) / 80);
        //    //var _groupedCouple = groupedCustomerList.Where(x=>x.total>1).ToList();
        //    //var _groupedSingle = groupedCustomerList.Where(x => x.total == 1).ToList();
        //    for (int i = 1; i <= numPackageSGN; i++)
        //    {
        //        groupedShipmentList[i].PackageGroup = i;
        //    }
        //    moveInGroup(groupedShipmentList, numPackageSGN);
        //    return View();
        //}
        //public void moveInGroup(List<ShipmentPropose> lstShip, Decimal numPackageSGN)
        //{
        //    if (lstShip.Count==0)
        //    {
        //        return;
        //    }
        //    for (int i = 1; i <= lstShip.Count; i++)
        //    {
        //        if (lstShip[i].PackageGroup==0)
        //        {
        //            //uu tien cung recipient
        //            var l = lstShip.Where(x => x.RecipientId == lstShip[i].RecipientId).ToList();
        //            var _coupleRecipientSum = _coupleRecipient.Sum(x => x.RecipientId);
        //            if (_coupleRecipientSum > 1)
        //            {
        //                for (int k = 0; k < _coupleRecipientSum; k++)
        //                {

        //                }
        //            }
        //            else
        //            {
        //                for (int j = 1; j <= numPackageSGN; j++)
        //                {
        //                    //uu tien cung recipient

        //                    if (lstShip.Where(x => x.PackageGroup == j).Sum(x => x.TotalWeightKg) + lstShip[i].TotalWeightKg < 80)
        //                    {
        //                        lstShip[i].PackageGroup = j;
        //                        lstShip[i].PackageInGroup = i;
        //                        break;
        //                    }
        //                }
        //            }
                    
        //        }
        //    }
        //}
        [HttpPost]
        public ActionResult SearchPackage(PackageSearch model)
        {
            CustomJsonResult result = new CustomJsonResult();
            model.HasShipment = 0;
            model.FromDate = new DateTime(2018, 07, 01).ToString("MMM dd yyyy");
            model.ToDate = DateTime.Now.ToString("MMM dd yyyy");
            result.Result = _orderService.Admin_SearchPackage(model);
            result.Optional = model.Total;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchShipment(ShipmentSearch model)
        {
            CustomJsonResult result = new CustomJsonResult();
            if (model.ToTime == 0)
            {
                model.ToTime = DateTime.Now.ToUnixTimestamp();
            }
            result.Result = _shipmentService.Admin_SearchShipment(model);
            //result.Optional = model.Total;
            return Json(result);
        }
        public PartialViewResult _GridShipment(ListShipmentModels lsResult)
        {
            //Grid shipment

            return PartialView(lsResult);
        }

        [HttpPost]
        public ActionResult SaveShipment(ShipmentViewDTO model)
        {
            CustomJsonResult result = new CustomJsonResult();
            if (model.Id < 1)
            {
                // create shipment
                if (model.LstPackage != null && model.LstPackage.Any())
                {
                    // lấy destination của 1 package đầu tiên
                    var packageFirst = model.LstPackage[0];
                    tblShipment shipment = new tblShipment();
                    shipment.WarehouseId = packageFirst.WarehouseId;
                    //shipment.Destination = packageFirst.Destination;
                    shipment.Destination = model.Destination;
                    if (string.IsNullOrEmpty(model.ShipmentCode))
                    {
                        string startCode = Lib.Business.Common.ServiceHelper.GetDestionationCodeById(packageFirst.Destination);
                        model.ShipmentCode = $"{startCode}{DateTime.Now.ToString("ddMMyy")}{_shipmentService.Admin_CountShipmentOnDateByDestination(packageFirst.Destination) + 1}";
                    }
                    shipment.ShipmentCode = model.ShipmentCode;
                    shipment.TotalWeight = Math.Round(model.LstPackage.Sum(x => x.Weight),2);
                    shipment.CreateTime = (int)model.CreateTime;
                    shipment.Id = _shipmentService.Admin_CreateShipment(shipment);
                    if (shipment.Id < 1)
                    {
                        result.Message = "Có lỗi xảy ra khi tạo shipment";
                        result.Result = -1;
                    }
                    else
                    {
                        _shipmentService.Admin_UpdateShipmentForPackage(model.LstPackage.Select(x => x.Id).ToList(), shipment.Id, shipment.ShipmentCode);
                    }
                }
                else
                {
                    result.Message = "Thông tin tạo shipment không hợp lệ!";
                    result.Result = -1;
                }
            }
            else
            {
                // update shipment
                tblShipment shipment = model.BindToModel<tblShipment>();
                if (model.LstPackage.Count>0)
                {
                    shipment.TotalWeight = Math.Round(model.LstPackage.Sum(x => x.Weight), 2);
                }                
                if (_shipmentService.Admin_UpdateShipment(shipment) > 0)
                {
                    var shipmentOld = _shipmentService.Admin_GeDetailShipmentHasPackageById(model.Id);
                    if (shipmentOld.LstPackage == null)
                    {
                        shipmentOld.LstPackage = new List<PackageViewDTO>();
                    }
                    if (model.LstPackage != null && model.LstPackage.Any())
                    {
                        // lấy các shipment remove
                        List<long> lstPackageIdRemove = new List<long>();
                        foreach (var item in shipmentOld.LstPackage)
                        {
                            var o = model.LstPackage.FirstOrDefault(x => x.Id == item.Id);
                            if (o == null)
                            {
                                lstPackageIdRemove.Add(item.Id);
                            }
                        }
                        if (lstPackageIdRemove.Any())
                        {
                            _shipmentService.Admin_UpdateShipmentForPackage(lstPackageIdRemove, null, null);
                        }
                        // các shipment thêm mới
                        var lstPackagenew = model.LstPackage.Where(x => x.ShipmentId < 1).ToList();
                        if (lstPackagenew != null && lstPackagenew.Any())
                        {
                            _shipmentService.Admin_UpdateShipmentForPackage(lstPackagenew.Select(x => x.Id).ToList(), shipmentOld.Id, shipmentOld.ShipmentCode);
                        }
                    }
                    else
                    {
                        // xóa hết các package hiện có của shipment này
                        _shipmentService.Admin_UpdateShipmentForPackage(shipmentOld.LstPackage.Select(x => x.Id).ToList(), null, null);
                    }
                }
                else
                {
                    result.Message = "Có lỗi xảy ra khi cập nhật shipment";
                    result.Result = -1;
                }
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddNewShipment(int destinationId, int warehouseId)
        {
            string startCode = Lib.Business.Common.ServiceHelper.GetDestionationCodeById(destinationId);
            ShipmentViewDTO model = new ShipmentViewDTO
            {
                LstPackage = new List<PackageViewDTO>(),
                Destination = destinationId,
                ShipmentCode = $"{startCode}{DateTime.Now.ToString("yyMMdd")}{(_shipmentService.Admin_CountShipmentOnDateByDestination(destinationId) + 1)}",
                TotalWeight = 0,
                WarehouseId = warehouseId,
                WarehouseName = _warehouseService.SelectByWarehouseID(warehouseId)?.Warehouse
            };
            CustomJsonResult result = new CustomJsonResult();
            result.Result = model;
            return Json(result);
        }

        [HttpPost]
        public ActionResult RemoveItemInShipment(List<int> lstId)
        {
            CustomJsonResult result = new CustomJsonResult();
            try
            {
                if (lstId != null && lstId.Any())
                {
                    result.Result = _shipmentService.Admin_RemovePackageInShipment(lstId);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult RemoveLstShipment(List<int> lstId)
        {
            CustomJsonResult result = new CustomJsonResult();
            try
            {
                if (lstId != null && lstId.Any())
                {
                    result.Result = _shipmentService.Admin_RemoveLstShipmentId(lstId);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }


        public ActionResult ExportShipmentInvoice(int typeId, string ids)
        {
            try
            {
                //_logHelper.Info($"request {ids}");
                string fileName = "";
                List<int> lstShipmentId = new List<int>();
                if (!string.IsNullOrEmpty(ids))
                {
                    var lstId = ids.Split(',');
                    int id = 0;
                    foreach (var i in lstId)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            int.TryParse(i, out id);
                            if (id != 0)
                            {
                                lstShipmentId.Add(id);
                            }
                        }
                    }
                    if (lstShipmentId.Any())
                    {
                        fileName = string.Format("{0} - {1} - Invoice shipment.xlsx", DateTime.Today.ToString("yyyyMMdd"), fileName);
                        var lstShipment = _shipmentService.Admin_ExportShipmentByLstId(lstShipmentId);
                        MemoryStream ms = null;
                        switch (typeId)
                        {
                            case 1: // USA
                                ms = ExportTemplateUSA(lstShipment);
                                break;
                            case 2: // VN
                                ms = ExportTemplateVN(lstShipment);
                                break;
                        }
                        ms.WriteTo(Response.OutputStream);
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                        ms.Close();
                        return new EmptyResult();
                    }

                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                _logHelper.Info(ex);
            }
            return RedirectToAction("Manager");
        }

        public JsonResult AJXSearchShipment()
        {
            string keyword = !string.IsNullOrEmpty(Request.Form["SearchKeyword"]) ? Request.Form["SearchKeyword"] : null;
            string fromdate = Request.Form["fromdate"];
            string todate = Request.Form["todate"];
            DateTime dtmFromdate = !string.IsNullOrEmpty(fromdate) ? DateTime.Parse(fromdate) : DateTime.Now.AddDays(-30);
            DateTime dtmTodate = !string.IsNullOrEmpty(todate) ? DateTime.Parse(todate) : DateTime.Now;
            var lsShipment = _shipmentService.Admin_SearchShipmentByCondition(keyword, dtmFromdate, dtmTodate);
            var result = GetPositionWithModel(lsShipment, dtmFromdate, dtmTodate);
            ListShipmentModels lsResult = new ListShipmentModels()
            {
                ListResult = result,
                FromDate = dtmFromdate,
                ToDate = dtmTodate
            };

            return Json(RenderPartialViewToString("_GridShipment", lsResult), JsonRequestBehavior.AllowGet);
        }
        private MemoryStream ExportTemplateUSA(List<ShipmentViewDTO> lstShipment)
        {
            MemoryStream Result = new MemoryStream();
            int iRow = 2;

            string fileName = "Template_Invoice_USA.xlsx";
            string filePath = Server.MapPath("~/Content/TemplateExport/" + fileName);
            FileInfo template = new FileInfo(filePath);
            ExcelPackage package;
            ExcelWorksheet ws;// = package.Workbook.ExcelWorksheets();
            if (template != null)
            {
                package = new ExcelPackage(template);
                ws = package.Workbook.Worksheets.First();
                ws.Name = "Invoice";
                ws.Select();
                ws.Cells[3, 2].Value = DateTime.Now.ToString("MM/dd/yyyy");

                var sheetFedex = package.Workbook.Worksheets.Last();
                sheetFedex.Name = "Invoice Fedex";

                Dictionary<int, ExcelWorksheet> dicWs = new Dictionary<int, ExcelWorksheet>();
                for (int i = 0; i < lstShipment.Count; i++)
                {
                    var wsi = ExcelCopyWorkSheet(package.Workbook, ws.Name, lstShipment[i].ShipmentCode);
                    wsi.Cells[3, 2].Value = DateTime.Now.ToString("MM/dd/yyyy");
                    if (!dicWs.ContainsKey(lstShipment[i].Id))
                    {
                        dicWs.Add(lstShipment[i].Id, wsi);
                    }
                }

                int rowTotal = 12;
                int cellTotal = 4;
                iRow = 15;
                int stt = 1;
                int totalQuantity = 0;
                decimal totalValue = 0;
                int totalQuantiyInShip = 0;
                decimal totalValueInShip = 0;
                int iRowInShip = 15;
                int sttInShip = 1;
                var listItemFedex = new List<ItemPackageViewDTO>();

                foreach (var item in lstShipment)
                {
                    totalQuantiyInShip = 0;
                    totalValueInShip = 0;
                    iRowInShip = 15;
                    sttInShip = 1;
                    var wsItem = dicWs[item.Id];
                    foreach (var p in item.LstPackage)
                    {
                        foreach (var pi in p.Items)
                        {
                            var index = listItemFedex.FindIndex(s => s.CategoryCode == pi.CategoryCode);
                            if (index > -1)
                            {
                                listItemFedex[index].TotalPrice += (pi.Quantity * pi.Value);
                                listItemFedex[index].Quantity += pi.Quantity;
                            }
                            else
                            {
                                var itemPackageViewDto = new ItemPackageViewDTO
                                {
                                    CategoryCode = pi.CategoryCode,
                                    TotalPrice = pi.Quantity * pi.Value,
                                    Quantity = pi.Quantity,
                                    Unit = "PCS",
                                    CategoryName = pi.CategoryName
                                };
                                listItemFedex.Add(itemPackageViewDto);
                            }

                            ws.Cells[iRow, 1].Value = stt++;
                            ws.Cells[iRow, 2].Value = pi.Description;
                            ws.Cells[iRow, 3].Value = pi.CategoryCode;
                            ws.Cells[iRow, 4].Value = pi.Quantity;
                            ws.Cells[iRow, 5].Value = "PCS";
                            ws.Cells[iRow, 6].Value = pi.Value;
                            ws.Cells[iRow, 7].Value = pi.Quantity * pi.Value;
                            ws.Cells[iRow, 8].Value = pi.CategoryName;

                            /// ws item
                            wsItem.Cells[iRowInShip, 1].Value = sttInShip++;
                            wsItem.Cells[iRowInShip, 2].Value = pi.Description;
                            wsItem.Cells[iRowInShip, 3].Value = pi.CategoryCode;
                            wsItem.Cells[iRowInShip, 4].Value = pi.Quantity;
                            wsItem.Cells[iRowInShip, 5].Value = "PCS";
                            wsItem.Cells[iRowInShip, 6].Value = pi.Value;
                            wsItem.Cells[iRowInShip, 7].Value = pi.Quantity * pi.Value;
                            wsItem.Cells[iRowInShip, 8].Value = pi.CategoryName;

                            totalQuantiyInShip += pi.Quantity;
                            totalValueInShip += (pi.Quantity * pi.Value);
                            iRow++;
                            iRowInShip++;
                            if (stt != 1)
                            {
                                ws.InsertRow(iRow, 1, iRow - 1);
                            }
                            if (sttInShip != 1)
                            {
                                wsItem.InsertRow(iRowInShip, 1, iRowInShip - 1);
                            }
                        }
                    }
                    totalQuantity += totalQuantiyInShip;
                    totalValue += totalValueInShip;
                    wsItem.Cells[rowTotal, cellTotal].Value = totalQuantiyInShip;
                    wsItem.Cells[rowTotal, cellTotal + 3].Value = totalValueInShip;
                }
                ws.Cells[rowTotal, cellTotal].Value = totalQuantity;
                ws.Cells[rowTotal, cellTotal + 3].Value = totalValue;



                totalQuantity = 0;
                totalValue = 0;
                sheetFedex.Select();
                sheetFedex.Cells[2, 2].Value = DateTime.Now.ToString("MM/dd/yyyy");
                stt = 1;
                var row = 14;
                foreach (var item in listItemFedex)
                {
                    sheetFedex.Cells[row, 1].Value = stt++;
                    sheetFedex.Cells[row, 2].Value = "";
                    sheetFedex.Cells[row, 3].Value = item.CategoryCode;
                    sheetFedex.Cells[row, 4].Value = item.Quantity;
                    sheetFedex.Cells[row, 5].Value = "PCS";
                    sheetFedex.Cells[row, 6].Value = "";
                    sheetFedex.Cells[row, 7].Value = item.TotalPrice / item.Quantity;
                    sheetFedex.Cells[row, 8].Value = item.TotalPrice;
                    sheetFedex.Cells[row, 9].Value = item.CategoryName;
                    row++;
                    if (stt != 1)
                    {
                        sheetFedex.InsertRow(row, 1, row - 1);
                    }
                    totalQuantity += item.Quantity;
                    totalValue += item.TotalPrice;
                    
                }

                sheetFedex.Cells[rowTotal - 1, cellTotal].Value = totalQuantity;
                sheetFedex.Cells[rowTotal - 1, cellTotal + 4].Value = totalValue;
                package.SaveAs(Result);
            }


            return Result;
        }
        private MemoryStream ExportTemplateVN(List<ShipmentViewDTO> lstShipment)
        {
            MemoryStream Result = new MemoryStream();
            int iRow = 2;

            string fileName = "Template_Invoice_VN.xlsx";
            string filePath = Server.MapPath("~/Content/TemplateExport/" + fileName);
            FileInfo template = new FileInfo(filePath);
            ExcelPackage package;
            ExcelWorksheet ws, ws1;// = package.Workbook.ExcelWorksheets();
            if (template != null)
            {
                package = new ExcelPackage(template);
                ws = package.Workbook.Worksheets[1];
                ws.Name = "Vận đơn chính";
                //ws1 = package.Workbook.Worksheets[2];
                //ws1.Name = lstShipment[0].ShipmentCode;
                //ws1.Cells[1, 1].Value = $"Danh sách vận đơn 1";
                //ws1.Cells[2, 1].Value = $"Lô hàng: {lstShipment[0].ShipmentCode}";
                ws.Select();
                ws.Cells[5, 1].Value = $"Lô hàng: {lstShipment[0].ShipmentCode}";
                //Dictionary<int, ExcelWorksheet> dicWs = new Dictionary<int, ExcelWorksheet>();
                //dicWs.Add(lstShipment[0].Id, ws1);
                //for (int i = 1; i < lstShipment.Count; i++)
                //{
                //    var wsi = ExcelCopyWorkSheet(package.Workbook, package.Workbook.Worksheets[2].Name, lstShipment[i].ShipmentCode);
                //    if (!dicWs.ContainsKey(lstShipment[i].Id))
                //    {
                //        wsi.Cells[1, 1].Value = $"Danh sách vận đơn {(i + 1)}";
                //        wsi.Cells[2, 1].Value = $"Lô hàng: {lstShipment[i].ShipmentCode}";
                //        dicWs.Add(lstShipment[i].Id, wsi);
                //    }
                //}

                iRow = 13;
                int stt = 1;
                string descriptionPackage = string.Empty;
                decimal totalValue = 0;
                decimal totalValueInShip = 0;
                int sttInShip = 1;
                int iRowInShip = 6;
                foreach (var item in lstShipment)
                {
                    totalValueInShip = 0;
                    sttInShip = 1;
                    iRowInShip = 6;
                    //var wsInShip = dicWs[item.Id];
                    foreach (var p in item.LstPackage)
                    {
                        ws.Cells[iRow, 1].Value = stt++;
                        ws.Cells[iRow, 2].Value = p.Code;
                        descriptionPackage = string.Empty;
                        foreach (var pi in p.Items)
                        {
                            descriptionPackage += $"{pi.Quantity} {pi.Description}; ";
                        }
                        ws.Cells[iRow, 3].Value = descriptionPackage;
                        ws.Cells[iRow, 4].Value = p.WeightKg;
                        ws.Cells[iRow, 5].Value = "";

                        //wsInShip.Cells[iRowInShip, 1].Value = sttInShip++;
                        //wsInShip.Cells[iRowInShip, 2].Value = p.Code;
                        //wsInShip.Cells[iRowInShip, 3].Value = descriptionPackage;
                        //wsInShip.Cells[iRowInShip, 4].Value = p.WeightKg;
                        //wsInShip.Cells[iRowInShip, 5].Value = "";
                        totalValueInShip += p.WeightKg;


                        iRow++;
                        iRowInShip++;
                        if (stt != 1)
                        {
                            ws.InsertRow(iRow, 1, iRow - 1);
                        }
                        //if (sttInShip != 1)
                        //{
                        //    wsInShip.InsertRow(iRowInShip, 1, iRowInShip - 1);
                        //}
                    }
                    totalValue += totalValueInShip;
                    //wsInShip.Cells[iRowInShip, 3].Value = "Tổng khối lượng";
                    //wsInShip.Cells[iRowInShip, 4].Value = totalValueInShip;
                }
                ws.Cells[iRow, 3].Value = "Tổng khối lượng";
                ws.Cells[iRow, 4].Value = totalValue;

                package.SaveAs(Result);
            }


            return Result;
        }

        private ExcelWorksheet ExcelCopyWorkSheet(ExcelWorkbook workbook, string existingWorksheetName, string newWorksheetName)
        {
            ExcelWorksheet worksheet = workbook.Worksheets.Copy(existingWorksheetName, newWorksheetName);
            return worksheet;
        }
    }
}