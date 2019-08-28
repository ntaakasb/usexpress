using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;
using System.Runtime.Caching;
using UsExpress.Transport.Logger.Action;
using UsExpress.Transport.Lib.Business.Common;
using UsExpress.Transport.EmailLib;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class UserService : IUserService
    {
        public long CreateUser(string username, string password, string fullname, bool isactive = false)
        {
            long result = 0;
            try
            {
                tblUser user = new tblUser { Username = username, Password = Utils.HashMD5(password), IsActive = isactive, IsDeleted = false, CreatedOn = DateTime.Now, FullName = fullname };
                user.CreatedOn = DateTime.Now;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (db.tblUsers.FirstOrDefault(s => s.Username.ToLower() == user.Username.ToLower()) != null)
                        return -1;
                    db.tblUsers.Add(user);
                    db.SaveChanges();
                    result = user.Id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => CreateUser", ex);
            }
            return result;

        }

        public tblUser GetUserById(long id)
        {
            tblUser result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblUsers.FirstOrDefault(u => u.Id == id);
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => GetUserById", ex);
            }
            return result;
        }

        public int GetRoleByUserID(long id)
        {
            int roleID = -1;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    roleID = db.tblUserRoles.FirstOrDefault(u => u.UserId == id)?.RoleId ?? -1;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => GetRoleByUserID", ex);
            }
            return roleID;
        }

        public int ChangePassUser(string username, string oldPass, string newPass, out long userId)
        {
            var isChange = StatusChangePass.Faile;
            userId = 0;
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        var _oldPass = Utils.HashMD5(oldPass);
                        var _newPass = Utils.HashMD5(newPass);
                        var user = db.tblUsers.FirstOrDefault(s => s.Username.ToUpper() == username.ToUpper());
                        if (user != null)
                        {
                            userId = user.Id;
                            if (user.Password == _oldPass)
                            {
                                user.Password = _newPass;
                                if (db.SaveChanges() > 0)
                                {
                                    isChange = StatusChangePass.IsChange;
                                    var key = "User-" + username;
                                    MemoryCache.Default.Remove(key);
                                }
                            }
                            else
                                isChange = StatusChangePass.WrongPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => ChangePassUser", ex);
            }

            return (int)isChange;
        }

        public tblUser GetUserByUsername(string username)
        {
            tblUser result = null;
            if (!String.IsNullOrEmpty(username))
            {
                var key = "User-" + username;
                object HTML = MemoryCache.Default.Get(key);
                if (HTML != null)
                {
                    return (tblUser)HTML;
                }
                else
                {
                    try
                    {
                        using (var db = ContextFactory.UsTransportEntities())
                        {
                            result = db.tblUsers.FirstOrDefault(u => u.Username.Equals(username));
                        }
                        if (result != null)
                            MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(60));
                    }
                    catch (Exception ex)
                    {
                        SELog.WriteLog("UserService => GetUserByUsername", ex);
                    }
                }
            }

            return result;
        }

        public long Login(string username, string password)
        {
            long result = 0;
            try
            {
                //DBLog.connectionString(); 
                password = Utils.HashMD5(password);
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblUsers.FirstOrDefault(s => s.Username.ToLower() == username.ToLower() && s.Password == password && s.IsActive && !s.IsDeleted)?.Id ?? 0;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => Login", ex);
            }

            return result;

        }

        public bool ChangePass(long userId, string oldPass, string newPass)
        {
            bool result = false;
            try
            {
                var acc = GetUserById(userId);
                oldPass = Utils.HashMD5(oldPass);
                if (acc != null && acc.Password.Equals(oldPass))
                {
                    acc.Password = Utils.HashMD5(newPass);
                    if (UpdateUser(acc) > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => ChangePass", ex);
            }
            return result;
        }

        public long UpdateUser(tblUser user)
        {

            long result = 0;
            try
            {
                var key = "User-" + user.Id;
                if (user.Id > 0)
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        var acc = db.tblUsers.FirstOrDefault(u => u.Id == user.Id);
                        if (string.IsNullOrEmpty(user.Password))
                        {
                            user.Password = acc.Password;
                        }
                        user.Username = user.Email != null ? user.Email : user.Username;
                        user.CreatedOn = acc.CreatedOn;
                        if (acc != null)
                        {
                            db.Entry(acc).CurrentValues.SetValues(user);
                            //if (db.SaveChanges() > 0)
                            //{
                            //    result = acc.Id;
                            //}
                            db.SaveChanges();
                            result = acc.Id;
                            MemoryCache.Default.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => UpdateUser", ex);
            }

            return result;
        }

        public List<int> GetLstRoleIdByUserId(long userId)
        {
            List<int> result = null;
            var key = "User-" + userId;
            object HTML = MemoryCache.Default.Get(key);
            if (HTML != null)
            {
                return (List<int>)HTML;
            }
            else
            {
                try
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        result = db.tblUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
                    }

                    MemoryCache.Default.Add(key, result, DateTimeOffset.UtcNow.AddMinutes(60));
                }
                catch (Exception ex)
                {
                    SELog.WriteLog("UserService => GetLstRoleIdByUserId", ex);
                }
            }

            return result;
        }

        public int MapRoleToUser(long userId, List<int> lstRoleId)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (lstRoleId == null || !lstRoleId.Any())
                    {
                        var lstRemove = db.tblUserRoles.Where(x => x.UserId == userId).Select(x => x);
                        if (lstRemove != null && lstRemove.Any())
                        {
                            db.tblUserRoles.RemoveRange(lstRemove);
                            result = db.SaveChanges();
                        }
                    }
                    else
                    {
                        var lstOldRole = db.tblUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
                        // thêm role
                        if (lstOldRole == null)
                        {
                            foreach (var r in lstRoleId)
                            {
                                db.tblUserRoles.Add(new tblUserRole { UserId = userId, RoleId = r });
                            }
                            result = db.SaveChanges();
                        }
                        // cập nhật, xóa role
                        else
                        {
                            bool bChange = false;
                            foreach (var o in lstOldRole)
                            {
                                if (!lstRoleId.Contains(o))
                                {
                                    var r = db.tblUserRoles.FirstOrDefault(x => x.UserId == userId && x.RoleId == o);
                                    if (r != null)
                                    {
                                        bChange = true;
                                        db.tblUserRoles.Remove(r);
                                    }
                                }
                            }
                            foreach (var o in lstRoleId)
                            {
                                if (!lstOldRole.Contains(o))
                                {
                                    bChange = true;
                                    db.tblUserRoles.Add(new tblUserRole { UserId = userId, RoleId = o });
                                }
                            }
                            if (bChange)
                            {
                                result = db.SaveChanges();
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => MapRoleToUser", ex);
            }

            return result;

        }

        public bool CheckExistEmailByUserID(string email, int id)
        {

            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    return db.tblUsers.Where(u => u.Username.Equals(email) && (u.Id != id)).Count() > 0;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => CheckExistEmailByUserID", ex);
                return false;
            }
        }

        public List<ShippingInfoDTO> SearchInfoByPhone(int top, string phone, int typeUser, int storeId = -1)
        {
            List<ShippingInfoDTO> result = new List<ShippingInfoDTO>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    switch ((OrderUserInfo)typeUser)
                    {
                        case OrderUserInfo.Sender:
                            result = db.tblSenders.Where(x => x.Phone.Contains(phone) && (x.StoreId == storeId || storeId == -1))
                                                    .Take(top)
                                                    .Select(x => new ShippingInfoDTO
                                                    {
                                                        Phone = x.Phone,
                                                        FullName = x.FullName,
                                                        AddressLine1 = x.Add1,
                                                        AddressLine2 = x.Add2,
                                                        Id = x.Id,
                                                        StoreId = x.StoreId ?? 0,
                                                        TypeUser = typeUser,
                                                        CityId = x.CityId,
                                                        CityName = x.CityId,
                                                        StateId = x.StateId,
                                                        StateName = x.StateId,
                                                        Zip = x.Zip
                                                    }).ToList();
                            break;
                        case OrderUserInfo.Recipient:
                            result = db.tblRecipientsInfoes.Where(x => x.Phone.Contains(phone) && (x.StoreId == storeId || storeId == -1))
                                                    .Take(top)
                                                    .Select(x => new ShippingInfoDTO
                                                    {
                                                        Phone = x.Phone,
                                                        FullName = x.FullName,
                                                        AddressLine1 = x.Add1,
                                                        AddressLine2 = x.Add2,
                                                        Id = x.id,
                                                        CityId = x.CityId,
                                                        DistrictId = x.DistrictId,
                                                        StoreId = x.StoreId ?? 0,
                                                        TypeUser = typeUser,
                                                        WardId = x.WardId
                                                    }).ToList();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => SearchInfoByPhone", ex);
            }
            return result;
        }

        public long RegisUser(tblUser user)
        {
            long result = 0;
            try
            {
                user.Password = Utils.HashMD5(user.Password);
                user.CreatedOn = DateTime.Now;
                user.Username = user.Email;
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (db.tblUsers.FirstOrDefault(s => s.Username.ToLower() == user.Username.ToLower()) != null)
                        return -1;
                    db.tblUsers.Add(user);
                    db.SaveChanges();
                    result = user.Id;
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => RegisUser", ex);
            }
            return result;
        }

        public IPagedList<EmployeesDTO> SearchUser(int pageIndex, int pageSize, string keyword, int searchType = -1)
        {
            List<EmployeesDTO> ListRS = new List<EmployeesDTO>();
            try
            {

                using (var db = ContextFactory.UsTransportEntities())
                {
                    ListRS = db.tblUsers.GroupJoin(db.tblWarehouses,
                        user => user.WarehouseID,
                        warehouse => warehouse.id,
                        (user, warehouse) => new { U = user, W = warehouse.FirstOrDefault() })
                        .Where(x => (searchType == -1
                        || (searchType == 1 && x.U.Phone.Contains(keyword))
                        || (searchType == 2 && x.U.FullName.Contains(keyword))
                         || (searchType == 3 && x.W.Warehouse.Contains(keyword))
                        )).Select(x => new EmployeesDTO
                        {
                            Id = x.U.Id,
                            FullName = x.U.FullName,
                            Phone = x.U.Phone,
                            Add1 = x.U.Add1,
                            Email = x.U.Email,
                            WarehouseName = x.W.Warehouse,
                            IsActive = x.U.IsActive
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => SearchUser", ex);
            }
            return ListRS.ToPagedList(pageIndex, pageSize);
        }

        public bool CheckRoleUser(long userId, int role)
        {
            bool result = false;
            try
            {
                var lstRole = GetLstRoleIdByUserId(userId);
                if (lstRole != null && lstRole.Any())
                    result = lstRole.Contains(role);
            }
            catch (Exception ex)
            {
                SELog.WriteLog("UserService => CheckRoleUser", ex);
            }
            return result;
        }

        public CustomeResultDTO SendLinks(string username, string url)
        {
            var custom = new CustomeResultDTO
            {
                Message = "Email do not exists on out system",
                Result = 0
            };

            using (var db = ContextFactory.UsTransportEntities())
            {
                var acc = db.tblUsers.FirstOrDefault(s => s.Username.ToLower() == username.ToLower());
                if (acc != null)
                {
                    var keyChange = Utils.GetComplexString(30);
                    acc.KeyChange = keyChange;
                    acc.CreateDateKey = DateTime.Now;

                    var rs = db.SaveChanges();
                    if (rs > 0)
                    {
                        var email = new EmailHelper();
                        var result = email.SendMail(username, "Reset Password", $"Please click on link if you sent request change your password: <br /> <br />" +
                            $"<a target='_blank' href='{url}/Home/ResetPass/{keyChange}'>{url}/Home/ResetPass/{keyChange}</a>");

                        if (result.Code != 0)
                        {
                            custom.Message = "Reset pass failed";
                            custom.Result = 0;
                        }
                        else
                        {
                            custom.Message = "A link to reset password sent for email: " + username;
                            custom.Result = 1;
                        }
                    }
                }
            }
            return custom;
        }

        public CustomeResultDTO ResetPass(string key, string pass)
        {
            var custom = new CustomeResultDTO
            {
                Message = "Reset pass faile",
                Result = 0
            };

            using (var db = ContextFactory.UsTransportEntities())
            {
                var isHasKey = !string.IsNullOrEmpty(key);
                var acc = db.tblUsers.FirstOrDefault(s => s.KeyChange == key && isHasKey);
                if (acc != null)
                {
                    acc.KeyChange = "";
                    acc.Password = Utils.HashMD5(pass);
                    var rs = db.SaveChanges();

                    if (rs > 0)
                    {
                        custom.Message = "Change password successfully";
                        custom.Result = 1;
                    }
                }
            }

            return custom;
        }

        public bool CheckKey(string key)
        {
            bool keyForUser = false;
            using (var db = ContextFactory.UsTransportEntities())
            {
                keyForUser = db.tblUsers.FirstOrDefault(s => s.KeyChange == key) != null;
            }
            return !string.IsNullOrEmpty(key) && keyForUser;
        }
    }
}
