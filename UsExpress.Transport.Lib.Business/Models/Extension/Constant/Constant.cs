using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension.Constant
{
    public enum OrderUserInfo
    {
        Sender = 1,
        Recipient = 2
    }
    public enum OrderStatusInfo
    {
        New = 0,
        PickedUp = 1,
        RejectPickup = 6,

        SendToVN = 2,
        WrongItem = 7,
        ProhibitedItem = 8,
        RejectSendToVN = 9,

        Good = 10,
        Damage = 11,
        Lost = 12,

        Delivering = 3,
        Delivered = 4,
        Failed = 5,    

        ClearCustom = 13
    }
    public class RoleUser
    {
        public const int AllRoles = -1;
        public const int Root = 1;
        public const int Admin = 2;
        /// <summary>
        /// Store
        /// </summary>
        public const int Store = 3;
        /// <summary>
        /// Nhan vien cham soc khac hang
        /// </summary>
        public const int UserVN = 4;
        public const int UserUSA = 5;
        /// <summary>
        /// Nhan vien kho
        /// </summary>
        public const int SupplierEmployee = 6;
    }
    public class DataTimeFormatHelper
    {
        public const string FormatDate_DDMMYYYY_HHMMSS = "dd/MM/yyyy HH:mm:ss";
    }
}
