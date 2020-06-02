using System;
using log4net;

namespace HnNetty.Tools
{
    public class LogHelper
    {
        private static ILog _log;

        static LogHelper()
        {
            _log = LogManager.GetLogger("loghelper", "hnlog");
        }

        public static void Debug(string msg)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(msg);
            }
        }

        public static void Info(string msg)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(msg);
            }
        }

        public static void Warn(string msg)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(msg);
            }
        }

        public static void Error(Exception ex)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(ex);
            }
        }

        public static void Error(string msg)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(msg);
            }
        }

        public static void Fatal(Exception ex)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(ex);
            }
        }

        public static void Fatal(string msg)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(msg);
            }
        }
    }
}