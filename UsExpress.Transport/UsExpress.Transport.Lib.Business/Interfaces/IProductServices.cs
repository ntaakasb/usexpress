using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Services
{
    public interface IProductServices
    {
        IPagedList<ProductDTO> GetAllProduct(int pageIndex, int pageSize, string keyword, int searchType);
        long InsertProduct(tblProduct product);
        long UpdateProduct(tblProduct product);
        tblProduct SelectByProductID(int id);

        string getScheduleBCodeByCategoryId(int categoryId);
    }
}
