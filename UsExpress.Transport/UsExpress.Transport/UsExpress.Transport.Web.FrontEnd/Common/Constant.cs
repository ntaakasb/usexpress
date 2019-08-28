using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsExpress.Transport.Web.FrontEnd.Common
{
    public class Constant
    {
        public const string SessionUsername = "UserName";
        public const string SessionPreviousUrl = "PreviousUrl";
        public const string SessionStoreID = "-1";
        public const string SessionStoreAccountInfo = "StoreAccountInfo";
    }
    public class RoleEnum
    {
        public const int AllRoles = -1;
        public const int Root = 1;
        public const int Admin = 2;
        public const int Manager = 3;
        public const int User = 4;
        public const int SupplierManager = 5;
        public const int SupplierEmployee = 6;
    }
}