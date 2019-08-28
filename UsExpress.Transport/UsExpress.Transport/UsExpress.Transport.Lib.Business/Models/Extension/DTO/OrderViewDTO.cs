using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Lib.Business.Models.Extension
{
    public class OrderSearch
    {
        public string Keyword { get; set; }
        public string FromDate { get; set; }
        public int TypeSearch { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// kết quả trả về số dòng
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 0: search Id, 1: sender
        /// </summary>
        public int OptionId { get; set; }
        public DateTime DFromDate
        {
            get
            {
                if (string.IsNullOrEmpty(this.FromDate))
                {
                    return DateTime.Now.Date;
                }
                return DateTime.ParseExact(this.FromDate, "dd/MM/yyyy", null);
            }
        }
        public int GetUnixTimestampFromDate
        {
            get { return this.DFromDate.ToUnixTimestamp(); }
        }
    }
    public class OrderViewDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public int TotalPackage { get; set; }

        public decimal Weight { get; set; }
        public string CreateDate { get; set; }
        public string PickupDate { get; set; }
        public string ShippingDate { get; set; }
        public string ClearCustomDate { get; set; }
        public string DeliverName { get; set; }
        public string StartsName { get; set; }
        /// <summary>
        /// same AirPort in tblOrder
        /// </summary>
        public string Destination { get; set; }
        public string Tracking { get; set; }
    }
}
