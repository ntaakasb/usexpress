using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class PackageGridDTO
    {
        public string OrderCode { get; set; }
        public tblPackageInfo PackageInfo { get; set; }
        public string SenderName { get; set; }
    }
}
