using System.Collections.Generic;
using System.Linq;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using ContextFactory = UsExpress.Transport.Lib.Business.Models.DBContext.ContextFactory;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class TestService :ITestService
    {
        public IEnumerable<Test> GetAll()
        {
            using (var db = ContextFactory.UsTransportEntities())
            {
                return db.Tests.ToList();
            }
        }
    }
}