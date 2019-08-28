using PagedList;
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
        IPagedList<tblCategory> GetAllCategory(int pageIndex, int pageSize, string keyword, int searchType);
        long InsertCatelogy(tblCategory category);
        long UpdateCatelogy(tblCategory category);
        tblCategory SelectByCatelogyID(int id);
        bool CheckExistCodeByID(int id, string code);
        List<tblCategory> GetAll();

        IPagedList<tblTypeCategory> GetAllTypeCategory(int pageIndex, int pageSize);
    }
}
