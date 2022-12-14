using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UsExpress.Transport.Log
{
    public class Log : ILogger
    {
        private static ILog _logger;

        public Log()
        {
            var fi = new FileInfo(HttpContext.Current.Server.MapPath("~/Web.config"));
            XmlConfigurator.Configure(fi);
            _logger = LogManager.GetLogger("SystemLogger");
        }
        public void Fatal(string message, Exception ex = null)
        {
            if (_logger.IsFatalEnabled)
            {
                if (ex == null)
                    _logger.Fatal(message);
                else
                    _logger.Fatal(message, ex);
            }
        }

        public void Error(string message, Exception ex = null)
        {
            if (_logger.IsErrorEnabled)
            {
                if (ex == null)
                    _logger.Error(message);
                else
                    _logger.Error(message, ex);
            }
        }

        public void Warn(string message, Exception ex = null)
        {
            if (_logger.IsWarnEnabled)
            {
                if (ex == null)
                    _logger.Warn(message);
                else
                    _logger.Warn(message, ex);
            }
        }

        public void Info(string message, Exception ex = null)
        {
            if (_logger.IsInfoEnabled)
            {
                if (ex == null)
                    _logger.Info(message);
                else
                    _logger.Info(message, ex);
            }
        }

        public void Debug(string message, Exception ex = null)
        {
            if (_logger.IsDebugEnabled)
            {
                if (ex == null)
                    _logger.Debug(message);
                else
                    _logger.Debug(message, ex);
            }
        }
    }


}
