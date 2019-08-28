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
    public class StoreServices : IStoreServices
    {
        #region Store Services

        public IPagedList<tblStoreAccount> GetListStore(int pageIndex, int pageSize, string keyword = null)
        {
            List<tblStoreAccount> ListRS = new List<tblStoreAccount>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblStoreAccounts.Where(x => (keyword == null || x.FullName.Contains(keyword) || x.StoreName.Contains(keyword))).ToList();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }
        public long InsertStore(tblStoreAccount storeaccount)
        {
            long result = 0;
            try
            {
                storeaccount.CreatedDate = DateTime.Now;
                storeaccount.Status = 0;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblStoreAccounts.Add(storeaccount);
                    db.SaveChanges();
                    result = storeaccount.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }


        public tblStoreAccount SelectStoreByID(int id)
        {
            tblStoreAccount _item = new tblStoreAccount();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblStoreAccounts.Where(x => x.id == id).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return _item;
        }

        public tblStoreAccount SelectStoreByUserName(string UserName)
        {
            tblStoreAccount _item = new tblStoreAccount();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblStoreAccounts.Where(x => x.Email == UserName.Trim()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return _item;
        }


        public long UpdateStore(tblStoreAccount storeaccount)
        {
            long result = 0;
            try
            {
                storeaccount.CreatedDate = DateTime.Now;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblStoreAccounts.Where(x => x.id == storeaccount.id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(storeaccount);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                        }

                    }
                    result = storeaccount.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }
       
        #endregion

        #region Sender Services
        public tblSender SelectSenderByID(int id)
        {
            tblSender _item = new tblSender();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblSenders.Where(x => x.Id == id).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return _item;
        }
        public long InsertSender(tblSender sender)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblSenders.Add(sender);
                    db.SaveChanges();
                    result = sender.Id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }




        public long UpdateSender(tblSender sender)
        {
            long result = 0;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblSenders.Where(x => x.Id == sender.Id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(sender);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.Id;
                        }

                    }
                    result = sender.Id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

        #endregion

        #region Reciver Services
        public long InsertReciever(tblRecipientsInfo reciver)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblRecipientsInfoes.Add(reciver);
                    db.SaveChanges();
                    result = reciver.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

        public long UpdateReciever(tblRecipientsInfo reciver)
        {
            long result = 0;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    var _temp = db.tblRecipientsInfoes.Where(x => x.id
                    == reciver.id).FirstOrDefault();
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(reciver);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                        }

                    }
                    result = reciver.id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return result;
        }

  

        public IPagedList<tblSender> GetListSenderByStoreID(int pageIndex, int pageSize, int storeID, string keyword = null)
        {
            List<tblSender> ListRS = new List<tblSender>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblSenders.Where(x => x.StoreId == storeID && (keyword == null || x.FullName.Contains(keyword))).ToList();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public IPagedList<tblRecipientsInfo> GetListRecieverByStoreID(int pageIndex, int pageSize, int storeID, string keyword = null)
        {
            List<tblRecipientsInfo> ListRS = new List<tblRecipientsInfo>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblRecipientsInfoes.Where(x => x.StoreId == storeID && (keyword == null || x.FullName.Contains(keyword))).ToList();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

      

        public tblRecipientsInfo SelectReciverByID(int id)
        {
            tblRecipientsInfo _item = new tblRecipientsInfo();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    _item = db.tblRecipientsInfoes.Where(x => x.id == id).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return _item;
        }

      
        #endregion
    }
}
