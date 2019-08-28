using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.DBContext;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Services
{
    public class OrderService : IOrderService
    {
        public long Admin_CreateOrder(tblOrder model)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // check phone sender
                            tblSender s = db.tblSenders.FirstOrDefault(x => x.Phone == model.SenderShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (s == null)
                            {
                                s = new tblSender
                                {
                                    Phone = model.SenderShippingInfo.Phone,
                                    Add1 = model.SenderShippingInfo.AddressLine1,
                                    Add2 = model.SenderShippingInfo.AddressLine2,
                                    CityId = model.SenderShippingInfo.CityId,
                                    FullName = model.SenderShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblSenders.Add(s);
                                db.SaveChanges();
                            }
                            model.SenderId = s.Id;

                            tblRecipientsInfo r = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == model.RecipientShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (r == null)
                            {
                                r = new tblRecipientsInfo
                                {
                                    Phone = model.RecipientShippingInfo.Phone,
                                    Add1 = model.RecipientShippingInfo.AddressLine1,
                                    Add2 = model.RecipientShippingInfo.AddressLine2,
                                    CityId = model.RecipientShippingInfo.CityId,
                                    DistrictId = model.RecipientShippingInfo.DistrictId,
                                    FullName = model.RecipientShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblRecipientsInfoes.Add(r);
                                db.SaveChanges();
                            }
                            model.RecipientId = r.id;
                            db.tblOrders.Add(model);
                            db.SaveChanges();

                            model.RecipientShippingInfo.OrderId = model.id;
                            model.RecipientShippingInfo.TypeUser = (int)OrderUserInfo.Recipient;
                            db.tblShippingInfoes.Add(model.RecipientShippingInfo);
                            db.SaveChanges();
                            model.SenderShippingInfo.OrderId = model.id;
                            model.SenderShippingInfo.TypeUser = (int)OrderUserInfo.Sender;
                            db.tblShippingInfoes.Add(model.SenderShippingInfo);
                            db.SaveChanges();
                            transaction.Commit();
                            result = model.id;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public tblOrder Admin_GetDetailOrderById(int id)
        {
            tblOrder result = null;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    result = db.tblOrders
                        .Include(x => x.tblPackageInfoes)
                        .FirstOrDefault(x => x.id == id);
                    result.SenderShippingInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == result.id && x.TypeUser == (int)OrderUserInfo.Sender);
                    result.RecipientShippingInfo = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == result.id && x.TypeUser == (int)OrderUserInfo.Recipient);
                    //db.Entry(result).Collection(x => x.tblPackageInfoes).Load();
                    //result.tblPackageInfoes = db.tblPackageInfoes.Include(y => y.tblItemInPackages).Where(x => x.OrderId == result.id).Select(x => x).ToList();
                    if (result.tblPackageInfoes != null && result.tblPackageInfoes.Any())
                    {
                        foreach (var item in result.tblPackageInfoes)
                        {
                            item.tblItemInPackages = db.tblItemInPackages.Where(x => x.PackageId == item.id).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public long Admin_UpdateOrder(tblOrder model)
        {
            long result = 0;
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            #region check phone sender, recipient
                            // check phone sender
                            tblSender sender = db.tblSenders.FirstOrDefault(x => x.Phone == model.SenderShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (sender == null)
                            {
                                sender = new tblSender
                                {
                                    Phone = model.SenderShippingInfo.Phone,
                                    Add1 = model.SenderShippingInfo.AddressLine1,
                                    Add2 = model.SenderShippingInfo.AddressLine2,
                                    CityId = model.SenderShippingInfo.CityId,
                                    FullName = model.SenderShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblSenders.Add(sender);
                                db.SaveChanges();
                            }
                            model.SenderId = sender.Id;

                            tblRecipientsInfo reciver = db.tblRecipientsInfoes.FirstOrDefault(x => x.Phone == model.RecipientShippingInfo.Phone && x.StoreId == model.StoreId);
                            if (reciver == null)
                            {
                                reciver = new tblRecipientsInfo
                                {
                                    Phone = model.RecipientShippingInfo.Phone,
                                    Add1 = model.RecipientShippingInfo.AddressLine1,
                                    Add2 = model.RecipientShippingInfo.AddressLine2,
                                    CityId = model.RecipientShippingInfo.CityId,
                                    DistrictId = model.RecipientShippingInfo.DistrictId,
                                    FullName = model.RecipientShippingInfo.FullName,
                                    StoreId = model.StoreId
                                };
                                db.tblRecipientsInfoes.Add(reciver);
                                db.SaveChanges();
                            }
                            model.RecipientId = reciver.id;
                            #endregion

                            var order = db.tblOrders.Where(x => x.id == model.id).First();
                            db.Entry(order).CurrentValues.SetValues(model);
                            db.SaveChanges();

                            #region update shipping info
                            tblShippingInfo s = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == model.id && x.TypeUser == (int)OrderUserInfo.Sender);
                            model.SenderShippingInfo.OrderId = model.id;
                            model.SenderShippingInfo.TypeUser = (int)OrderUserInfo.Sender;
                            if (s != null)
                            {
                                db.Entry(s).CurrentValues.SetValues(model.SenderShippingInfo);
                            }
                            else
                            {
                                db.tblShippingInfoes.Add(model.SenderShippingInfo);
                            }
                            db.SaveChanges();

                            tblShippingInfo r = db.tblShippingInfoes.FirstOrDefault(x => x.OrderId == model.id && x.TypeUser == (int)OrderUserInfo.Recipient);
                            model.RecipientShippingInfo.OrderId = model.id;
                            model.RecipientShippingInfo.TypeUser = (int)OrderUserInfo.Recipient;
                            if (r != null)
                            {
                                db.Entry(r).CurrentValues.SetValues(model.RecipientShippingInfo);
                            }
                            else
                            {
                                db.tblShippingInfoes.Add(model.RecipientShippingInfo);
                            }
                            db.SaveChanges();
                            #endregion

                            #region update/ delete package

                            var tblOldPackage = db.tblPackageInfoes.Where(x => x.OrderId == model.id).Include(y => y.tblItemInPackages).ToList();
                            if (tblOldPackage != null && tblOldPackage.Any())
                            {
                                foreach (var op in tblOldPackage.ToList())
                                {
                                    var opDelete = model.tblPackageInfoes.FirstOrDefault(x => x.id == op.id);
                                    if (opDelete == null)
                                    {
                                        db.tblItemInPackages.RemoveRange(op.tblItemInPackages);
                                        db.SaveChanges();
                                        db.tblPackageInfoes.Remove(op);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            foreach (var pa in model.tblPackageInfoes)
                            {
                                var pM = db.tblPackageInfoes.Include(x => x.tblItemInPackages).FirstOrDefault(x => x.id == pa.id);
                                if (pM != null)
                                {
                                    pM.Depth = pa.Depth;
                                    pM.Fee = pa.Fee;
                                    pM.Height = pa.Height;
                                    pM.Ordinal = pa.Ordinal;
                                    pM.TotalItem = pa.TotalItem;
                                    pM.TotalValue = pa.TotalValue;
                                    pM.Weight = pa.Weight;
                                    pM.Width = pa.Width;

                                    foreach (var it in pM.tblItemInPackages.ToList())
                                    {
                                        var itDelete = pa.tblItemInPackages.FirstOrDefault(x => x.id == it.id);
                                        if (itDelete == null)
                                        {
                                            db.tblItemInPackages.Remove(it);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    pa.OrderId = model.id;
                                    db.tblPackageInfoes.Add(pa);
                                }
                                db.SaveChanges();
                                foreach (var i in pa.tblItemInPackages)
                                {
                                    var iM = db.tblItemInPackages.FirstOrDefault(x => x.id == i.id);
                                    if (iM != null)
                                    {
                                        i.PackageId = pa.id;
                                        db.Entry(iM).CurrentValues.SetValues(i);
                                    }
                                    else
                                    {
                                        i.PackageId = pa.id;
                                        db.tblItemInPackages.Add(i);
                                    }
                                    db.SaveChanges();
                                }

                            }
                            #endregion

                            //db.SaveChanges();
                            transaction.Commit();
                            result = model.id;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public List<OrderViewDTO> Admin_SearchOrder(OrderSearch model)
        {
            List<OrderViewDTO> result = new List<OrderViewDTO>();
            if (string.IsNullOrEmpty(model.Keyword))
            {
                model.Keyword = "";
            }
            model.Keyword = model.Keyword.ToLower().ConvertToUnSign3();
            try
            {
                using (var db = ContextFactory.UsTransportEntities())
                {
                    var lstOrder = db.tblOrders
                                    .Where(x => x.CreateDate >= model.GetUnixTimestampFromDate)
                                    .Select(x => x).ToList();
                    switch (model.TypeSearch)
                    {
                        case 0://search Id         
                            lstOrder = lstOrder.Where(x => x.Code.ToLower().Contains(model.Keyword)).Select(x => x).ToList();
                            break;
                        case 1: // search sender

                            var lstSender = db.tblStoreAccounts.Where(x => x.FullName.Contains(model.Keyword)).Select(x => x.id).ToList();
                            if (lstSender != null && lstSender.Any())
                            {
                                lstOrder = lstOrder.Where(x => x.StoreId.HasValue && lstSender.Contains(x.StoreId.Value)).Select(x => x).ToList();
                            }
                            else
                            {
                                lstOrder = new List<tblOrder>();
                            }
                            break;
                    }
                    model.Total = lstOrder.Count;
                    lstOrder = lstOrder.Take(model.PageSize).Skip(model.PageIndex * model.PageSize).ToList();
                    foreach (var item in lstOrder)
                    {
                        var sender = db.tblStoreAccounts.FirstOrDefault(x => x.id == item.StoreId);
                        if (sender != null)
                        {
                            result.Add(new OrderViewDTO
                            {
                                Id = item.id,
                                Code = item.Code,
                                CreateDate = ((double)item.CreateDate).UnixTimeStampToDateTime().ToString("dd/MM/yyy"),
                                TotalPackage = item.TotalPackage ?? 0,
                                Tracking = item.Tracking,
                                Destination = item.Destination.HasValue ? (item.Destination.Value == 1 ? "HN" : "HCM") : "HCM",
                                FullName = sender.FullName,
                                Weight = item.Weight ?? 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        private bool checkSearchText(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                source = "";
            }
            if (string.IsNullOrEmpty(target))
            {
                target = "";
            }
            source = source.ToLower().ConvertToUnSign3();
            target = target.ToLower().ConvertToUnSign3();
            return source.Contains(target);
        }
    }
}
