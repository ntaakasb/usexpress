using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace UsExpress.Transport.Web.FrontEnd.Common
{
    public class Constant
    {
        public const string SessionUsername = "UserName";
        public const string SessionPreviousUrl = "PreviousUrl";
        public const string SessionStoreID = "StoreID";
        public const string SessionStoreAccountInfo = "StoreAccountInfo";
        public const string SessionUserID = "-1";
        public const string SessionIsAdmin = "IsAdmin";
        public const string SessionStoreName = "StoreName";
        public const string SessionUserRole = "UserRole";
        public const string SessionIsCSKH = "IsCSKH";
    }
    public class RoleEnum
    {
        public const int AllRoles = -1;
        [Description("Root")]
        public const int Root = 1;
        [Description("Admin")]
        public const int Admin = 2;
        [Description("Store")]
        public const int Store = 3;
        [Description("User VN")]
        public const int UserVN = 4;
        [Description("User USA")]
        public const int UserUSA = 5;
        [Description("SupplierEmployee")]
        public const int SupplierEmployee = 6;
    }

   
}