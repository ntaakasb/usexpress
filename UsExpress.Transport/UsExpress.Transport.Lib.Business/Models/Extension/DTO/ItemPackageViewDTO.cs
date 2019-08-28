using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class ItemPackageViewDTO
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
