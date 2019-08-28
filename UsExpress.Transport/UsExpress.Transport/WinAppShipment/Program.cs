using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppShipment.DAL;
using WinAppShipment.Entity;

namespace WinAppShipment
{
    class Program
    {

        static string UHNWeight = System.Configuration.ConfigurationSettings.AppSettings.Get("UHNWeight");
        static string USGWeight = System.Configuration.ConfigurationSettings.AppSettings.Get("USGWeight");
        static string DestinationHNId = System.Configuration.ConfigurationSettings.AppSettings.Get("DestinationHNId");
        static string DestinationSGId = System.Configuration.ConfigurationSettings.AppSettings.Get("DestinationSGId");

        
        static void Main(string[] args)
        {
            Console.WriteLine("Begin!");
            //UHAN18082301
            var _ShipmentDAL = new ShipmentDAL();
            var lstShipmentHN = new List<tblShipment>();
            var lstShipmentSG = new List<tblShipment>();
            
            var AVG_Weight = 0;
            var MAX_Weight = 0;
            var startCode = "";
            int estimate = 0;
            try {
                //Lấy ra danh sách package trong ngày group theo kho bên mỹ và cảng hàng không VN cần chuyển order by theo orderId
                var lstPackage = _ShipmentDAL.GetListPackageOnday();
                Console.WriteLine("CO " + lstPackage.Count + " Group shipment can xu ly");
                //Duyệt tính toán vận đơn cho 1 kho tới VN
                for (int i = 0; i < lstPackage.Count; i++)
                {
                    var lstShipment = new List<tblShipment>();
                    if (lstPackage[i].Destination == int.Parse(DestinationHNId))
                    {
                        MAX_Weight = int.Parse(UHNWeight);
                        startCode = "UHAN";
                        Console.WriteLine("Dang xu ly WarehouseId " + lstPackage[i].WarehouseId + " Chuyen ve UHAN");
                        lstShipment = lstShipmentHN;

                    }
                    else if(lstPackage[i].Destination == int.Parse(DestinationSGId))                   
                    {
                        MAX_Weight = int.Parse(USGWeight);
                        startCode = "SGN";
                        Console.WriteLine("Dang xu ly WarehouseId " + lstPackage[i].WarehouseId + " Chuyen ve SGN");
                        lstShipment = lstShipmentSG;
                    }
                    

                    estimate = lstPackage[i].TotalPackage / MAX_Weight;
                    //Nếu tổng khối lượng vận chuyển có dư, estimate số lượng shipment tăng lên 1 để chia đều                  
                    if (lstPackage[i].TotalPackage % MAX_Weight != 0) estimate += 1;

                    AVG_Weight = MAX_Weight / estimate;

                    //Lấy danh sách package theo kho hàng và địa chỉ nhận hàng trong ngày
                    var lstPackageDetailt = _ShipmentDAL.getDetailPackageOnday(lstPackage[i].WarehouseId, lstPackage[i].Destination);                   
                    var shipment = new tblShipment();
                    string strPackageId = "";
                    for (int j = 0; j < lstPackageDetailt.Count; j++)
                    {
                        //Nếu chuyến hàng có khối lượng dưới khối lượng tối đa
                        var tmpWeight = lstPackageDetailt[j].Weight+ shipment.TotalWeight;
                        if (tmpWeight <= AVG_Weight)
                        {
                            Console.WriteLine("Dang add PackageId " + lstPackageDetailt[j].Id + " vao " + startCode + DateTime.Now.ToString("ddMMYY") + (lstShipment.Count + 1).ToString());
                            if (strPackageId != "") strPackageId += ",";
                            strPackageId += lstPackageDetailt[j].Id;
                            shipment.TotalWeight = tmpWeight;
                            //trường hợp 2 package vòng lặp cuối tạo luôn shipment
                            if(j== lstPackageDetailt.Count - 1)
                            {
                                //tạo shipment hiện tại
                                if (strPackageId != "") strPackageId += ",";
                                strPackageId += lstPackageDetailt[j].Id;
                                shipment.WarehouseId = lstPackageDetailt[j].WarehouseId;
                                shipment.Destination = lstPackageDetailt[j].Destination;
                                //shipment.ShipmentCode = startCode + DateTime.Now.ToString("ddMMyy") + (lstShipment.Count + 1).ToString();
                                lstShipment.Add(shipment);
                                if (lstPackage[i].Destination == int.Parse(DestinationHNId))
                                {
                                    lstShipmentHN= lstShipment;
                                }
                                else if (lstPackage[i].Destination == int.Parse(DestinationSGId))
                                {
                                    lstShipmentSG = lstShipment;                                   

                                }
                                shipment.ShipmentCode = startCode + DateTime.Now.ToString("ddMMyy") + (lstShipment.Count ).ToString();
                                _ShipmentDAL.CreateShipment(strPackageId, shipment);
                                Console.WriteLine("Tao xong shipment " + shipment.ShipmentCode);
                            }

                        }
                        else
                        {
                            //tạo shipment hiện tại
                            if (strPackageId != "") strPackageId += ",";
                            strPackageId += lstPackageDetailt[j].Id;
                            shipment.WarehouseId = lstPackageDetailt[j].WarehouseId;
                            shipment.Destination = lstPackageDetailt[j].Destination;                            
                            lstShipment.Add(shipment);
                            if (lstPackage[i].Destination == int.Parse(DestinationHNId))
                            {
                                lstShipmentHN = lstShipment;
                            }
                            else if (lstPackage[i].Destination == int.Parse(DestinationSGId))
                            {
                                lstShipmentSG = lstShipment;

                            }
                            shipment.ShipmentCode = startCode + DateTime.Now.ToString("ddMMyy") + (lstShipment.Count ).ToString();
                            _ShipmentDAL.CreateShipment(strPackageId, shipment);
                            Console.WriteLine("Tao xong shipment " + shipment.ShipmentCode);
                            //Chuyển qua add shipment mới
                            shipment = new tblShipment();
                            shipment.TotalWeight = lstPackageDetailt[j].Weight;
                            strPackageId = lstPackageDetailt[j].Id.ToString();
                        }

                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
