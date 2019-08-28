using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models
{
    public class ProductDTO
    {
        public int id { get; set; }

        public string Description { get; set; }

        public string BarCode { get; set; }

        public string ScheduleBCode { get; set; }

        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
