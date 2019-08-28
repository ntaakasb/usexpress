using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension.DTO
{
    public class RecieverDTO
    {
        public int id { get; set; }
        public string FullName { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string Phone { get; set; }
        public int? StoreId { get; set; }
        public string WardId { get; set; }
        public long? CreateDate { get; set; }
        public long? UpdateDate { get; set; }
        public string FullAddress { get; set; }
        public bool IsActive { get; set; }
        public string StoreName { get; set; }
    }
}
