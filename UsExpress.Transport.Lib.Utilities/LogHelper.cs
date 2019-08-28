using System;
using log4net;
using log4net.Core;

namespace UsExpress.Transport.Lib.Utilities
{
    public class LogHelper
    {
        public static void Configure()
        {
            // register log4net
            log4net.Config.XmlConfigurator.Configure();
        }

        public static Log GetLogger(System.Type type)
        {

            return new Log
            {
                _logImplementation =
                    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
            };
        }

        public class Log : ILog
        {
            public ILog _logImplementation;

            public ILogger Logger
            {
                get { return _logImplementation.Logger; }
            }

            public void Debug(object message)
            {
                Debug(message);
            }

            public void Debug(object message, Exception exception)
            {
                _logImplementation.Debug(message, exception);
            }

            public void DebugFormat(string format, params object[] args)
            {
                _logImplementation.DebugFormat(format, args);
            }

            public void DebugFormat(string format, object arg0)
            {
                _logImplementation.DebugFormat(format, arg0);
            }

            public void DebugFormat(string format, object arg0, object arg1)
            {
                _logImplementation.DebugFormat(format, arg0, arg1);
            }

            public void DebugFormat(string format, object arg0, object arg1, object arg2)
            {
                _logImplementation.DebugFormat(format, arg0, arg1, arg2);
            }

            public void DebugFormat(IFormatProvider provider, string format, params object[] args)
            {
                _logImplementation.DebugFormat(provider, format, args);
            }

            public void Info(object message)
            {
                _logImplementation.Info(message);
            }

            public void Info(object message, Exception exception)
            {
                _logImplementation.Info(message, exception);
            }

            public void InfoFormat(string format, params object[] args)
            {
                _logImplementation.InfoFormat(format, args);
            }

            public void InfoFormat(string format, object arg0)
            {
                _logImplementation.InfoFormat(format, arg0);
            }

            public void InfoFormat(string format, object arg0, object arg1)
            {
                _logImplementation.InfoFormat(format, arg0, arg1);
            }

            public void InfoFormat(string format, object arg0, object arg1, object arg2)
            {
                _logImplementation.InfoFormat(format, arg0, arg1, arg2);
            }

            public void InfoFormat(IFormatProvider provider, string format, params object[] args)
            {
                _logImplementation.InfoFormat(provider, format, args);
            }

            public void Warn(object message)
            {
                _logImplementation.Warn(message);
            }

            public void Warn(object message, Exception exception)
            {
                _logImplementation.Warn(message, exception);
            }

            public void WarnFormat(string format, params object[] args)
            {
                _logImplementation.WarnFormat(format, args);
            }

            public void WarnFormat(string format, object arg0)
            {
                _logImplementation.WarnFormat(format, arg0);
            }

            public void WarnFormat(string format, object arg0, object arg1)
            {
                _logImplementation.WarnFormat(format, arg0, arg1);
            }

            public void WarnFormat(string format, object arg0, object arg1, object arg2)
            {
                _logImplementation.WarnFormat(format, arg0, arg1, arg2);
            }

            public void WarnFormat(IFormatProvider provider, string format, params object[] args)
            {
                _logImplementation.WarnFormat(provider, format, args);
            }

            public void Error(object message)
            {
                _logImplementation.Error(message);
            }

            public void Error(object message, Exception exception)
            {
                _logImplementation.Error(message, exception);
            }

            public void ErrorFormat(string format, params object[] args)
            {
                _logImplementation.ErrorFormat(format, args);
            }

            public void ErrorFormat(string format, object arg0)
            {
                _logImplementation.ErrorFormat(format, arg0);
            }

            public void ErrorFormat(string format, object arg0, object arg1)
            {
                _logImplementation.ErrorFormat(format, arg0, arg1);
            }

            public void ErrorFormat(string format, object arg0, object arg1, object arg2)
            {
                _logImplementation.ErrorFormat(format, arg0, arg1, arg2);
            }

            public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
            {
                _logImplementation.ErrorFormat(provider, format, args);
            }

            public void Fatal(object message)
            {
                _logImplementation.Fatal(message);
            }

            public void Fatal(object message, Exception exception)
            {
                _logImplementation.Fatal(message, exception);
            }

            public void FatalFormat(string format, params object[] args)
            {
                _logImplementation.FatalFormat(format, args);
            }

            public void FatalFormat(string format, object arg0)
            {
                _logImplementation.FatalFormat(format, arg0);
            }

            public void FatalFormat(string format, object arg0, object arg1)
            {
                _logImplementation.FatalFormat(format, arg0, arg1);
            }

            public void FatalFormat(string format, object arg0, object arg1, object arg2)
            {
                _logImplementation.FatalFormat(format, arg0, arg1, arg2);
            }

            public void FatalFormat(IFormatProvider provider, string format, params object[] args)
            {
                _logImplementation.FatalFormat(provider, format, args);
            }

            public bool IsDebugEnabled
            {
                get { return _logImplementation.IsDebugEnabled; }
            }

            public bool IsInfoEnabled
            {
                get { return _logImplementation.IsInfoEnabled; }
            }

            public bool IsWarnEnabled
            {
                get { return _logImplementation.IsWarnEnabled; }
            }

            public bool IsErrorEnabled
            {
                get { return _logImplementation.IsErrorEnabled; }
            }

            public bool IsFatalEnabled
            {
                get { return _logImplementation.IsFatalEnabled; }
            }
        }
    }

   
}