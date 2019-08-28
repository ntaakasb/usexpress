using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class CategoryService : ICategoryService
    {
        public bool CheckExistCodeByID(int id, string code)
        {
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblCategories.SingleOrDefault(x => (x.id != id && x.Code == code)) != null;
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => CheckExistCodeByID", ex);
                return false;
            }

        }

        public IPagedList<tblCategory> GetAllCategory(int pageIndex, int pageSize, string keyword = null, int searchType = -1)
        {
            List<tblCategory> result = new List<tblCategory>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblCategories.Where(x => (
                    (searchType == -1)
                    || (searchType == 2 && (keyword != null && x.CategoryName.Contains(keyword)))
                    || (searchType == 1 && (keyword != null && x.Code.Contains(keyword)))
                    )).ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => GetAllCategory", ex);
            }
            return result.ToPagedList(pageIndex, pageSize);
        }
        public List<tblCategory> GetAll()
        {
            List<tblCategory> result = new List<tblCategory>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("CategoryService => GetAllCategory", ex);
            }
            return result;
        }
        
        public IPagedList<tblTypeCategory> GetAllTypeCategory(int pageIndex, int pageSize)
        {
            List<tblTypeCategory> result = new List<tblTypeCategory>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblTypeCategories.ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => GetAllTypeCategory", ex);
            }
            return result.ToPagedList(pageIndex, pageSize);
        }

        public long InsertCatelogy(tblCategory category)
        {
            long result = 0;
            var key = "SelectByCatelogyID-" + category.id;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblCategories.Add(category);
                    db.SaveChanges();
                    MemoryCache.Default.Remove(key);
                    result = category.id;
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => InsertCatelogy", ex);
            }
            return result;
        }

        public tblCategory SelectByCatelogyID(int id)
        {
            tblCategory _item = new tblCategory();
            try
            {
                var key = "SelectByCatelogyID-" + id;
                object HTML = MemoryCache.Default.Get(key);
                if (HTML != null)
                {
                    return (tblCategory)HTML;
                }
                else
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        _item = db.tblCategories.Where(x => x.id == id).FirstOrDefault();

                    }
                    if (_item != null)
                        MemoryCache.Default.Add(key, _item, DateTimeOffset.UtcNow.AddMinutes(60));
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => SelectByCatelogyID", ex);
            }
            return _item;
        }

        public long UpdateCatelogy(tblCategory category)
        {
            long result = 0;
            var key = "SelectByCatelogyID-" + category.id;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblCategories.Where(x => x.id == category.id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(category);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                            MemoryCache.Default.Remove(key);
                        }

                    }
                    result = category.id;
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("CategoryService => UpdateCatelogy", ex);
            }
            return result;
        }
    }
}
