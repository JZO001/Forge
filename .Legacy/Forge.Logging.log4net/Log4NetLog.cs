using System;

namespace Forge.Logging.Log4net
{

    /// <summary>Log wrapper implementation</summary>
    /// <seealso cref="Forge.Logging.ILog" />
    public class Log4NetLog : ILog
    {

        private log4net.ILog mLog = null;

        public Log4NetLog(log4net.ILog log)
        {
            if (log == null) ThrowHelper.ThrowArgumentNullException("log");
            mLog = log;
        }

        public bool IsFatalEnabled { get { return mLog.IsFatalEnabled; } }

        public bool IsWarnEnabled { get { return mLog.IsWarnEnabled; } }

        public bool IsInfoEnabled { get { return mLog.IsInfoEnabled; } }

        public bool IsDebugEnabled { get { return mLog.IsDebugEnabled; } }

        public bool IsErrorEnabled { get { return mLog.IsErrorEnabled; } }

        public void Debug(object message)
        {
            mLog.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            mLog.Debug(message, exception);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            mLog.DebugFormat(provider, format, args);
        }

        public void DebugFormat(string format, params object[] args)
        {
            mLog.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            mLog.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            mLog.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            mLog.DebugFormat(format, arg0, arg1);
        }

        public void Error(object message)
        {
            mLog.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            mLog.Error(message, exception);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            mLog.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            mLog.ErrorFormat(provider, format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            mLog.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            mLog.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            mLog.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            mLog.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            mLog.Fatal(message, exception);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            mLog.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(string format, object arg0)
        {
            mLog.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            mLog.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            mLog.FatalFormat(provider, format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            mLog.FatalFormat(format, arg0, arg1);
        }

        public void Info(object message, Exception exception)
        {
            mLog.Info(message, exception);
        }

        public void Info(object message)
        {
            mLog.Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            mLog.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            mLog.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            mLog.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            mLog.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            mLog.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            mLog.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            mLog.Warn(message, exception);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            mLog.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            mLog.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            mLog.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            mLog.WarnFormat(provider, format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            mLog.WarnFormat(format, arg0, arg1, arg2);
        }
    }

}
