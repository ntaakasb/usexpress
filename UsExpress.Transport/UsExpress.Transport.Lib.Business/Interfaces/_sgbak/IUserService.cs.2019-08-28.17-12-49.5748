using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IUserService
    {
        long Login(string username, string password);
        tblUser GetUserByUsername(string username);
        tblUser GetUserById(long id);

        long CreateUser(string username, string password, string fullname);
        bool ChangePass(long userId, string oldPass, string newPass);
        long UpdateUser(tblUser user);

        List<int> GetLstRoleIdByUserId(long userId);

        int MapRoleToUser(long userId, List<int> lstRoleId);

        bool CheckExistEmailByUserID(string email, int id);

        List<ShippingInfoDTO> SearchInfoByPhone(int top, string phone, int typeUser, int storeId = -1);
    }
}
