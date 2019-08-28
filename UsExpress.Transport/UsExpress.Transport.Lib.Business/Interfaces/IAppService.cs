using System.Collections.Generic;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IAppService
    {
        tblUser Login(string username, string password);
        List<tblAppUserRoleMenu> GetAppUserRoleMenu(int UserId);
    }
}