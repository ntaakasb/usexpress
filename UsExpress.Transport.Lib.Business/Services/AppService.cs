using System;
using System.Collections.Generic;
using System.Linq;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Utilities;
using UsExpress.Transport.Logger;
using UsExpress.Transport.Logger.Action;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class AppService :IAppService
    {
        public tblUser Login(string username, string password)
        {
            try
            {
                password = Utils.HashMD5(password);
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblUsers.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()) && x.Password.Equals(password) && !x.IsDeleted);
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("AppService => Login", ex);
                throw ex;
            }
        }

        public List<tblAppUserRoleMenu> GetAppUserRoleMenu(int UserId)
        {
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblAppUserRoleMenus.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception ex)
            {
               SELog.WriteLog("AppService => GetAppUserRoleMenu", ex);
                throw ex;
            }
        }
    }
}