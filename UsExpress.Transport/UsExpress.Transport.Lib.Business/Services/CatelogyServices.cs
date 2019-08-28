using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class CatelogyServices : ICatelogyServices
    {
        public bool CheckExistCodeByID(int id, string code)
        {
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblCategories.Where(x => x.id != id && x.Code == code.Trim()).Count() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IPagedList<tblCategory> GetCatelogy(int pageIndex, int pageSize, string keyword)
        {
            List<tblCategory> ListRS = new List<tblCategory>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblCategories.Where(x => (keyword == null || x.CategoryName.Contains(keyword))).ToList();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public long InsertCatelogy(tblCategory catelogy)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblCategories.Add(catelogy);
                    db.SaveChanges();
                    result = catelogy.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

        public tblCategory SelectByID(int id)
        {
            tblCategory result = new tblCategory();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblCategories.FirstOrDefault(x => x.id == id);
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

        public long UpdateCatelogy(tblCategory catelogy)
        {
            long result = 0;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblCategories.Where(x => x.id == catelogy.id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(catelogy);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                        }

                    }
                    result = catelogy.id;
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
  