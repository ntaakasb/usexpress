using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface ICatelogyServices
    {
        IPagedList<tblCategory> GetCatelogy(int pageIndex, int pageSize, string keyword);
        long InsertCatelogy(tblCategory catelogy);
        long UpdateCatelogy(tblCategory catelogy);

        bool CheckExistCodeByID(int id, string code);
        tblCategory SelectByID(int id);
    }
} 
 