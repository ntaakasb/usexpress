using PagedList;
using System;
using System.Collections.Generic;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IUserService
    {
        long Login(string username, string password);
        tblUser GetUserByUsername(string username);
        tblUser GetUserById(long id);

        long CreateUser(string username, string password, string fullname, bool isactive);
        bool ChangePass(long userId, string oldPass, string newPass);
        long UpdateUser(tblUser user);

        long RegisUser(tblUser user);

        List<int> GetLstRoleIdByUserId(long userId);

        int MapRoleToUser(long userId, List<int> lstRoleId);

        bool CheckExistEmailByUserID(string email, int id);

        List<ShippingInfoDTO> SearchInfoByPhone(int top, string phone, int typeUser, int storeId = -1);

        IPagedList<EmployeesDTO> SearchUser(int pageIndex, int pageSize, string keyword, int searchType);


        /// <summary>
        /// Lib.Business.Models.Extension.Constant.RoleUser
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        bool CheckRoleUser(long userId, int role);

        int GetRoleByUserID(long id);
        int ChangePassUser(string username, string oldPass, string newPass, out long userId);
        CustomeResultDTO SendLinks(string username, string url);
        CustomeResultDTO ResetPass(string key, string pass);
        bool CheckKey(string key);
    }
}
