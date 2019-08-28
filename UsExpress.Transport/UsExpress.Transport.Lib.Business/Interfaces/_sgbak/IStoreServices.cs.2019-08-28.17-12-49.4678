using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    //Create by: AnhNT - 23/09/2018
    public interface IStoreServices
    {
        long InsertStore(tblStoreAccount storeaccount);
        long UpdateStore(tblStoreAccount storeaccount);
        IPagedList<tblStoreAccount> GetListStore(int pageIndex, int pageSize, string keyword);
        tblStoreAccount SelectStoreByID(int id);
        tblStoreAccount SelectStoreByUserName(string UserName);

        IPagedList<tblSender> GetListSenderByStoreID(int pageIndex, int pageSize, int storeID, string keyword);
        tblSender SelectSenderByID(int id);
        long InsertSender(tblSender sender);
        long UpdateSender(tblSender sender);

        IPagedList<tblRecipientsInfo> GetListRecieverByStoreID(int pageIndex, int pageSize, int storeID, string keyword);
        tblRecipientsInfo SelectReciverByID(int id);
        long InsertReciever(tblRecipientsInfo reciver);
        long UpdateReciever(tblRecipientsInfo reciver);
    }
}
