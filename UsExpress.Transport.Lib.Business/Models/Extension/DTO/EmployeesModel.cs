using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class EmployeesDTO
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? CityId { get; set; }

        public int? StateId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? WarehouseID { get; set; }

        public string Add1 { get; set; }

        public string WarehouseName { get; set; }
    }
}