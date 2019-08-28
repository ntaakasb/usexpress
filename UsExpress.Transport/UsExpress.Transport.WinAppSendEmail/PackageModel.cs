using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.WinAppSendEmail
{
    class PackageModel: tblItemInPackage
    {
        public tblPackageInfo TPI { get; set; }
    }
}
