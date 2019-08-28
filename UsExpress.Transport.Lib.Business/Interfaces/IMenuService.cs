using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Interfaces
{
    public interface IMenuService
    {
        List<tblMenu> GetLstMenuByLstRoleId(List<int> lstRoleId);
        int InsertMenu(tblMenu menu);
        int UpdateMenu(tblMenu menu);

        int MapMenuToRole(int roleId, List<int> lstMenuId);

        List<tblMenu> GetAllMenu();

        Dictionary<int, List<int>> GetLstMenuByLstRoleId_Dic(List<int> lstRoleId);

        tblMenu GetDetailMenuById(int id);
    }
}
