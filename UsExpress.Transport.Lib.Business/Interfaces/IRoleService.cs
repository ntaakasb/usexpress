using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IRoleService
    {
        List<tblRole> GetAllRole();
        tblUserRole Select(string UserName);

        long UpdateRoleUser(long UserID, int RoleID);
    }
}
