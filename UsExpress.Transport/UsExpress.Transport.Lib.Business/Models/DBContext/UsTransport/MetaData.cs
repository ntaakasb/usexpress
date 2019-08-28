namespace UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport
{
    public partial class tblShippingInfo
    {
        public virtual tblDistrictStateProvice District { get; set; }
        public virtual tblStateProvice City { get; set; }
        public virtual tblWard Ward { get; set; }
    }
}