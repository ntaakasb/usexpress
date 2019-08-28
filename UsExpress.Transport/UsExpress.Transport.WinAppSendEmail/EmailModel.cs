using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.WinAppSendEmail
{
    class EmailModel
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
    }
}
