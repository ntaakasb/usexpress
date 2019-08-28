namespace UsExpress.Transport.Lib.Business.Models
{
    public class KerryExpress
    {
        public class Service
        {
            public static string DichVuChuyenPhatNhanh { get; set; } = "0201";
            public static string DichVuChuyenPhat48H { get; set; } = "0202";
            public static string DichVuChuyenPhatHoaToc { get; set; } = "0203";
            public static string DichVuChuyenPhatGiaRe { get; set; } = "0204";
            public static string DichVuVanChuyenDuongBo { get; set; } = "0205";
        }

        public class PostNewOrderResponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public string order_number { get; set; }
            public string cost { get; set; }
            public string wood_bale_fee { get; set; }
            public string remote_deliver_fee { get; set; }
            public string delivery_date { get; set; }
        }
    }
}