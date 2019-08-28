using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class RecipientInfoDTO
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

        public string CityName { get; set; }
        public string DistrictName { get; set; }

        public string WardName { get; set; }
    }
}
