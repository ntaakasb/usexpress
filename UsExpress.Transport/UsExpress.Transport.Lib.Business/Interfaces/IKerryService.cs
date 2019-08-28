using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IKerryService
    {
        void PostNewOrder(int PackageId);
        void OrderProgress(tblKerryOrderProgress kerryOrderProgress);
    }
}