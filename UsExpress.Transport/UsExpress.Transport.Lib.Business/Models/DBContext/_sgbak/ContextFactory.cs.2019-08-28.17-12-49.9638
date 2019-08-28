using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Models.DBContext
{
    public class ContextFactory
    {
       
        public ContextFactory()
        {
            Constant.APPSETTING = new Business.Models.AppSetting();
        }


        public static UsTransportEntities UsTransportEntities()
        {
            if (Constant.APPSETTING == null)
            {
                Constant.APPSETTING = new Business.Models.AppSetting();
            }
            var db = new UsTransportEntities(Constant.APPSETTING.Database.EntityConnectionString);
            db.Configuration.LazyLoadingEnabled = false;
            return db;
        }
    }
}