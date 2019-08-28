using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Services;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Test
{
    [TestClass]
    public class KerryServiceTest
    {
      
        [TestMethod]
        public void PostNewOrder()
        {
            LogHelper.Configure();
            LogHelper.Log _Log = LogHelper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            // register log4net
            
            _Log.Info("tesst");
            IKerryService kerryService = new KerryService();
            kerryService.PostNewOrder(4);
        }
    }
}
