using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class RoleService : IRoleService
    {
        public List<tblRole> GetAllRole()
        {
            List<tblRole> result = new List<tblRole>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblRoles.Select(x => x).ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("RoleService => GetAllRole", ex);
            }
            return result;
        }

        public tblUserRole Select(string UserName)
        {
            tblUserRole result = new tblUserRole();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    long userID = db.tblUsers.FirstOrDefault(x => x.Username.Contains(UserName))?.Id ?? -1;
                    result = db.tblUserRoles.SingleOrDefault(x => x.UserId == userID);
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("RoleService => Select", ex);
            }
            return result;
        }

        public long UpdateRoleUser(long UserID, int RoleID)
        {
            long result = 0;
            try
            {
                tblUserRole userRole = new tblUserRole { UserId = UserID, RoleId = RoleID };
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var updaterole = db.tblUserRoles.FirstOrDefault(u => u.UserId == UserID);
                    if (updaterole != null)
                    {
                        updaterole.RoleId = RoleID;
                        if (db.SaveChanges() > 0)
                        {
                            result = updaterole.Id;
                        }

                    }
                    else
                    {
                        db.tblUserRoles.Add(userRole);
                        db.SaveChanges();
                        result = userRole.Id;
                    }
                  
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("RoleService => UpdateRoleUser", ex);
            }

            return result;
        }
    }
}
