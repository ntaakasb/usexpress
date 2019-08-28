using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class MenuService : IMenuService
    {
        public List<tblMenu> GetLstMenuByLstRoleId(List<int> lstRoleId)
        {
            List<tblMenu> result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var lstmenuId = db.tblMenuPermissions.Where(x => lstRoleId.Contains(x.RoleId)).Select(x => x.MenuId).Distinct().ToList();
                    if (lstmenuId != null && lstmenuId.Any())
                    {
                        result = db.tblMenus.Where(x => lstmenuId.Contains(x.Id) && x.IsActive == true && !x.IsDeleted).Select(x => x).ToList();
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public int InsertMenu(tblMenu menu)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    db.tblMenus.Add(menu);
                    db.SaveChanges();
                    result = menu.Id;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public int UpdateMenu(tblMenu menu)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var m = db.tblMenus.FirstOrDefault(u => u.Id == menu.Id);
                    if (m != null)
                    {
                        db.Entry(m).CurrentValues.SetValues(menu);
                        if (db.SaveChanges() > 0)
                        {
                            result = m.Id;
                        }

                    }
                }
            }
            catch (Exception)
            {
            }

            return result;

        }
        public int MapMenuToRole(int roleId, List<int> lstMenuId)
        {
            int result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    if (lstMenuId == null || !lstMenuId.Any())
                    {
                        var lstRemove = db.tblMenuPermissions.Where(x => x.RoleId == roleId).Select(x => x);
                        if (lstRemove != null && lstRemove.Any())
                        {
                            db.tblMenuPermissions.RemoveRange(lstRemove);
                            result = db.SaveChanges();
                        }
                    }
                    else
                    {
                        var lstOldMenu = db.tblMenuPermissions.Where(x => x.RoleId == roleId).Select(x => x.MenuId).ToList();
                        // thêm role
                        if (lstOldMenu == null || !lstOldMenu.Any())
                        {
                            foreach (var r in lstMenuId)
                            {
                                db.tblMenuPermissions.Add(new tblMenuPermission { MenuId = r, RoleId = roleId });
                            }
                            result = db.SaveChanges();
                        }
                        // cập nhật, xóa role
                        else
                        {
                            bool bChange = false;
                            foreach (var o in lstOldMenu)
                            {
                                if (!lstMenuId.Contains(o))
                                {
                                    var r = db.tblMenuPermissions.FirstOrDefault(x => x.MenuId == o && x.RoleId == roleId);
                                    if (r != null)
                                    {
                                        bChange = true;
                                        db.tblMenuPermissions.Remove(r);
                                    }
                                }
                            }
                            foreach (var o in lstMenuId)
                            {
                                if (!lstOldMenu.Contains(o))
                                {
                                    bChange = true;
                                    db.tblMenuPermissions.Add(new tblMenuPermission { MenuId = o, RoleId = roleId });
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

        public List<tblMenu> GetAllMenu()
        {
            List<tblMenu> result = new List<tblMenu>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblMenus.Select(x => x).OrderBy(x => x.ParentId).OrderBy(x => x.DisplayOrder).ToList();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public Dictionary<int, List<int>> GetLstMenuByLstRoleId_Dic(List<int> lstRoleId)
        {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblMenuPermissions.Where(x => lstRoleId.Contains(x.RoleId))
                                                    .GroupBy(x => x.RoleId)
                                                    .ToDictionary(x => x.Key, (x => x.Select(m => m.MenuId).ToList()));


                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public tblMenu GetDetailMenuById(int id)
        {
            tblMenu result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblMenus.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception)
            {
                
            }
            return result;
        }
    }
}
