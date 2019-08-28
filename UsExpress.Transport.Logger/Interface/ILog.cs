using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Logger
{
    public interface ILogger
    {
        void Fatal(string message, Exception ex = null);
        void Error(string message, Exception ex = null);
        void Warn(string message, Exception ex = null);
        void Info(string message, Exception ex = null);
        void Debug(string message, Exception ex = null);
    }

   
}
