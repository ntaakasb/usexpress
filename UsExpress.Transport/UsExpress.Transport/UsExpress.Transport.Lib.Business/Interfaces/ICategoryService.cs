using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface ICategoryService
    {
        List<tblCategory> GetAllCategory(); 
    }
}
