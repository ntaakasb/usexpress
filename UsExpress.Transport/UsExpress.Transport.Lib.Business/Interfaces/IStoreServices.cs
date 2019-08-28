using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension.DTO;

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

        IPagedList<SenderDTO> GetListSenderByStoreID(int pageIndex, int pageSize, int storeID, string keyword, int searchType, bool isActive = true);
        tblSender SelectSenderByID(int id);
        long InsertSender(tblSender sender);
        long UpdateSender(tblSender sender);
        bool SetActiveSender(int senderId);

        IPagedList<RecieverDTO> GetListRecieverByStoreID(int pageIndex, int pageSize, int storeID, string keyword, int searchType, bool isActive = true);
        tblRecipientsInfo SelectReciverByID(int id);
        long InsertReciever(tblRecipientsInfo reciver);
        long UpdateReciever(tblRecipientsInfo reciver);
        bool SetActiveRecieverInfo(int recieverId);
        bool Admin_CheckPhoneUserInfoOfStore(int storeId, int typeUser, string phone);

        bool CheckExistsStoreCode(int storeID, string strCode);
        List<tblStoreAccount> LoadListStore();
        bool SetActiveStore(int[] arrId);
    }
}
