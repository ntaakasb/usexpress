using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class CategoryService : ICatelogyService
    {
        public bool CheckExistCodeByID(int id, string code)
        {
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblCategories.Where(x => (x.id != id && x.Code == code)).ToList().Count > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public IPagedList<tblCategory> GetAllCategory(int pageIndex, int pageSize, string keyword = null)
        {
            List<tblCategory> result = new List<tblCategory>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblCategories.Where(x => (keyword == null || x.CategoryName.Contains(keyword))).ToList();
                }
            }
            catch (Exception)
            {
            }
            return result.ToPagedList(pageIndex, pageSize);
        }


        public long InsertCatelogy(tblCategory category)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblCategories.Add(category);
                    db.SaveChanges();
                    result = category.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

        public tblCategory SelectByCatelogyID(int id)
        {
            tblCategory _item = new tblCategory();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblCategories.Where(x => x.id == id).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return _item;
        }

        public long UpdateCatelogy(tblCategory category)
        {
            long result = 0;
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
                        }

                    }
                    result = category.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }
    }
}
