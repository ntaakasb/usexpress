using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class UserService : IUserService
    {
        public long CreateUser(string username, string password, string fullname)
        {
            long result = 0;
            try
            {
                tblUser user = new tblUser { Username = username, Password = Utils.HashMD5(password), IsActive = false, IsDeleted = false, CreatedOn = DateTime.Now, FullName = fullname };
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblUsers.Add(user);
                    db.SaveChanges();
                    result = user.Id;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
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
            catch (Exception)
            {
            }
            return result;
        }

        public tblUser GetUserByUsername(string username)
        {
            tblUser result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblUsers.FirstOrDefault(u => u.Username.Equals(username));
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public long Login(string username, string password)
        {
            long result = 0;
            try
            {
                password = Utils.HashMD5(password);
                var acc = GetUserByUsername(username);
                if (acc != null && acc.Password.Equals(password) && acc.IsActive && !acc.IsDeleted)
                {
                    result = acc.Id;
                }
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
            }
            return result;
        }

        public long UpdateUser(tblUser user)
        {
            long result = 0;
            try
            {
                if (user.Id > 0)
                {
                    using (var db = ContextFactory.UsTransportEntities())
                    {
                        var acc = db.tblUsers.FirstOrDefault(u => u.Id == user.Id);
                        if (acc != null)
                        {
                            db.Entry(acc).CurrentValues.SetValues(user);
                            if (db.SaveChanges() > 0)
                            {
                                result = acc.Id;
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public List<int> GetLstRoleIdByUserId(long userId)
        {
            List<int> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
                }
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch
            {
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
                                                        Id = x.Id
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
                                                        DistrictId = x.DistrictId
                                                    }).ToList();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
