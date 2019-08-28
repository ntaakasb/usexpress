using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.DTO;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

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
                    ListRS = db.tblStoreAccounts.Where(x => (keyword == null || x.FullName.Contains(keyword) || x.StoreName.Contains(keyword)) && x.IsActive).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => GetListStore", ex);
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public List<tblStoreAccount> LoadListStore()
        {
            List<tblStoreAccount> listStore;
            try
            {
                listStore = ContextFactory.UsTransportEntities().tblStoreAccounts.ToList();
            }
            catch (Exception ex)
            {
                listStore = new List<tblStoreAccount>();
                SELog.WriteLog("StoreServices => LoadListStore", ex);
            }
            return listStore;
        }

        public long InsertStore(tblStoreAccount storeaccount)
        {
            long result = 0;
            try
            {
                storeaccount.CreatedDate = DateTime.Now;
                storeaccount.IsActive = true;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (db.tblStoreAccounts.FirstOrDefault(s => s.Email.ToLower() == storeaccount.Email.ToLower()) != null)
                        return -1;
                    db.tblStoreAccounts.Add(storeaccount);
                    db.SaveChanges();
                    result = storeaccount.id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => InsertStore", ex);
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
                SELog.WriteLog("StoreServices => SelectStoreByID", ex);
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
                SELog.WriteLog("StoreServices => SelectStoreByUserName", ex);
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
                    var _temp = db.tblStoreAccounts.FirstOrDefault(x => x.id == storeaccount.id);
                    if (storeaccount.Status == null)
                    {
                        storeaccount.Status = _temp.Status;
                        storeaccount.Password = _temp.Password;
                    }
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
                SELog.WriteLog("StoreServices => UpdateStore", ex);
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
                    _item = db.tblSenders.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => SelectSenderByID", ex);
            }
            return _item;
        }
        public long InsertSender(tblSender sender)
        {
            long result = 0;
            try
            {
                sender.CreateDate = DateTime.Now.ToFileTimeUtc();
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblSenders.Add(sender);
                    db.SaveChanges();
                    result = sender.Id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => InsertSender", ex);
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
                    var _temp = db.tblSenders.FirstOrDefault(x => x.Id == sender.Id);
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
                SELog.WriteLog("StoreServices => UpdateSender", ex);
            }
            return result;
        }

        public bool SetActiveSender(int senderId)
        {
            var isError = true;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var sender = db.tblSenders.FirstOrDefault(s => s.Id == senderId);
                    if (sender != null)
                    {
                        sender.IsActive = !sender.IsActive;
                        isError = db.SaveChanges() < 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => SetActiveSender", ex);
            }
            return isError;
        }

        #endregion

        #region Reciver Services
        public long InsertReciever(tblRecipientsInfo reciver)
        {
            long result = 0;
            try
            {
                reciver.CreateDate = DateTime.Now.ToUnixTimestamp();
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblRecipientsInfoes.Add(reciver);
                    db.SaveChanges();
                    result = reciver.id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => InsertReciever", ex);
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
                    var _temp = db.tblRecipientsInfoes.FirstOrDefault(x => x.id == reciver.id);
                    if (_temp != null)
                    {
                        db.Entry(_temp).CurrentValues.SetValues(reciver);
                        if (db.SaveChanges() > 0)
                        {
                            result = _temp.id;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => UpdateReciever", ex);
            }
            return result;
        }




        public IPagedList<SenderDTO> GetListSenderByStoreID(int pageIndex, int pageSize, int storeID, string keyword, int searchType, bool isActive = true)
        {
            List<SenderDTO> ListRS = new List<SenderDTO>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblSenders.Where(x => (storeID == -1 || x.StoreId == storeID)
                            && (searchType == -1
                            || (searchType == 1 && (x.Phone.Contains(keyword) || keyword == null))
                            || (searchType == 2 && (x.FullName.Contains(keyword) || keyword == null)))
                            && x.IsActive == isActive
                    ).Join(db.tblStoreAccounts.Select(s => s), sd => sd.StoreId, st => st.id, (sd, st) => new { sd, st.StoreName })
                    .OrderBy(x => x.sd.FullName).Select(s => new SenderDTO
                    {
                        Add1 = s.sd.Add1,
                        Add2 = s.sd.Add2,
                        CityId = s.sd.CityId,
                        CreateDate = s.sd.CreateDate,
                        DistrictId = s.sd.DistrictId,
                        Email = s.sd.Email,
                        FullAddress = s.sd.FullAddress,
                        FullName = s.sd.FullName,
                        Id = s.sd.Id,
                        Phone = s.sd.Phone,
                        StateId = s.sd.StateId,
                        StoreId = s.sd.StoreId,
                        StoreName = s.StoreName,
                        UpdateDate = s.sd.UpdateDate,
                        Zip = s.sd.Zip,
                        IsActive = s.sd.IsActive
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => GetListSenderByStoreID", ex);
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public bool SetActiveRecieverInfo(int recieverId)
        {
            var isError = true;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var reciever = db.tblRecipientsInfoes.FirstOrDefault(s => s.id == recieverId);
                    if (reciever != null)
                    {
                        reciever.IsActive = !reciever.IsActive;
                        isError = db.SaveChanges() < 0;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => SetActiveRecieverInfo", ex);
            }
            return isError;
        }


        public IPagedList<RecieverDTO> GetListRecieverByStoreID(int pageIndex, int pageSize, int storeID, string keyword, int searchType, bool isActive = true)
        {
            List<RecieverDTO> ListRS = new List<RecieverDTO>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblRecipientsInfoes.Where(x => (storeID == -1 || x.StoreId == storeID)
                                                          && (searchType == -1
                                                          || (searchType == 1 && (x.Phone.Contains(keyword) || keyword == null))
                                                          || (searchType == 2 && (x.FullName.Contains(keyword) || keyword == null)))
                                                          && x.IsActive == isActive)
                                                   .Join(db.tblStoreAccounts.Select(s => s),
                                                         rc => rc.StoreId,
                                                         st => st.id,
                                                         (rc, st) => new { rc, st.StoreName })
                                                   .OrderBy(x => x.rc.FullName)
                                                   .Select(s => new RecieverDTO
                                                   {
                                                       Add1 = s.rc.Add1,
                                                       Add2 = s.rc.Add2,
                                                       CityId = s.rc.CityId,
                                                       CreateDate = s.rc.CreateDate,
                                                       DistrictId = s.rc.DistrictId,
                                                       FullAddress = s.rc.FullAddress,
                                                       FullName = s.rc.FullName,
                                                       Phone = s.rc.Phone,
                                                       StoreId = s.rc.StoreId,
                                                       StoreName = s.StoreName,
                                                       UpdateDate = s.rc.UpdateDate,
                                                       id = s.rc.id,
                                                       WardId = s.rc.WardId,
                                                       IsActive = s.rc.IsActive
                                                   }).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => GetListRecieverByStoreID", ex);
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
                SELog.WriteLog("StoreServices => SelectReciverByID", ex);
            }
            return _item;
        }


        #endregion

        public bool Admin_CheckPhoneUserInfoOfStore(int storeId, int typeUser, string phone)
        {
            bool result = false;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    switch ((Models.Extension.Constant.OrderUserInfo)typeUser)
                    {
                        case Models.Extension.Constant.OrderUserInfo.Sender:
                            var sender = db.tblSenders.FirstOrDefault(x => x.Phone == phone && x.StoreId == storeId);
                            if (sender != null && sender.Id > 0)
                            {
                                result = true;
                            }
                            break;
                        case Models.Extension.Constant.OrderUserInfo.Recipient:
                            var receipient = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == phone && x.StoreId == storeId);
                            if (receipient != null && receipient.id > 0)
                            {
                                result = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => Admin_CheckPhoneUserInfoOfStore", ex);
            }
            return result;
        }

        public bool CheckExistsStoreCode(int storeID, string strCode)
        {
            bool result = false;
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    var store = db.tblStoreAccounts.FirstOrDefault(x => x.id != storeID && x.Code == strCode.Trim());
                    if (store != null && store.id > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("StoreServices => CheckExistsStoreCode", ex);
            }
            return result;
        }

        public bool SetActiveStore(int[] arrId)
        {
            var rs = false;
            try
            {
                if (arrId.Length == 0) rs = false;
                else
                {
                    using(var db = ContextFactory.UsTransportEntities())
                    {
                        foreach (var item in arrId)
                        {
                            var st = db.tblStoreAccounts.FirstOrDefault(s => s.id == item);
                            if (st != null) st.IsActive = !st.IsActive;
                        }
                        rs  = db.SaveChanges() > 0;
                    }
                }
            }
            catch (Exception)
            {
                rs = false;
            }
            return rs;
        }
    }
}
