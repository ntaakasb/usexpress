using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UsExpress.Transport.Lib.Business.Models.DBContext.UsTransport;

namespace UsExpress.Transport.Web.FrontEnd.Models
{
    public class MenuModel : tblMenu
    {
        public MenuModel()
        {

        }
        public List<MenuModel> LstMenuChild { get; set; }
    }
}