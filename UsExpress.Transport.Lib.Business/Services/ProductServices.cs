using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Logger;
using System.Runtime.Caching;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class ProductServices : IProductServices
    {

        public IPagedList<ProductDTO> GetAllProduct(int pageIndex, int pageSize, string keyword, int searchType)
        {
            List<ProductDTO> result = new List<ProductDTO>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var query = db.tblProducts.Join(db.tblCategories,
                            pro => pro.CategoryID,
                            cat => cat.id,
                            (pro, cat) => new { Pro = pro, Cat = cat }
                        ).Where(x => (
                    (searchType == -1)
                    || (searchType == 1 && (keyword != null && x.Pro.BarCode.Contains(keyword)))
                    || (searchType == 2 && (keyword != null && x.Pro.Description.Contains(keyword)))
                     || (searchType == 2 && (keyword != null && x.Cat.CategoryName.Contains(keyword)))
                    )).ToList();
                    result = query.Select(x => new ProductDTO()
                    {
                        BarCode = x.Pro.BarCode,
                        CategoryID = x.Pro.CategoryID,
                        CategoryName = x.Cat.CategoryName,
                        Description = x.Pro.Description,
                        id = x.Pro.id,
                        ScheduleBCode = x.Pro.ScheduleBCode
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                SELog.WriteLog("ProductServices => GetAllProduct", ex);

            }
            return result.ToPagedList(pageIndex, pageSize);
        }

        public long InsertProduct(tblProduct product)
        {
            long result = 0;
            string key = "usexpress_productlist";
            tblProduct _item = new tblProduct();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (db.tblProducts.Any(x => x.BarCode == product.BarCode))
                    {
                        return -1;
                    }
                    db.tblProducts.Add(product);
                    db.SaveChanges();
                    MemoryCache.Default.Remove(key);
                    result = product.id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ProductServices => InsertProduct", ex);
            }
            return result;
        }

        public long UpdateProduct(tblProduct product)
        {
            long result = 0;
            var key = "usexpress_select" + product.id;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblProducts.FirstOrDefault(x => x.id == product.id);
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(product);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                            MemoryCache.Default.Remove(key);
                        }

                    }
                    result = product.id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ProductServices => UpdateProduct", ex);
            }
            return result;
        }

        public tblProduct SelectByProductID(int id)
        {
            tblProduct _item = new tblProduct();
            try
            {
                var key = "usexpress_select" + id;
                object HTML = MemoryCache.Default.Get(key);
                if (HTML != null)
                {
                    return (tblProduct)HTML;
                }
                else
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        _item = db.tblProducts.FirstOrDefault(x => x.id == id);

                    }
                    if (_item != null)
                        MemoryCache.Default.Add(key, _item, DateTimeOffset.UtcNow.AddMinutes(60));
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ProductServices => SelectByProductID", ex);
            }
            return _item;
        }

        public string getScheduleBCodeByCategoryId(int categoryId)
        {

            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var result = db.tblCategories.FirstOrDefault(x => x.id == categoryId)?.Code ?? "";
                    return result;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("ProductServices => getScheduleBCodeByCategoryId", ex);
            }
            return string.Empty;
        }
    }
}
