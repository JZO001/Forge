using System;

namespace Forge.Logging
{

    /// <summary>The ILog interface is use by application to log messages into the underlying logger framework.</summary>
    public interface ILog
    {

        bool IsFatalEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(IFormatProvider provider, string format, params object[] args);

        void DebugFormat(string format, params object[] args);

        void DebugFormat(string format, object arg0);

        void DebugFormat(string format, object arg0, object arg1, object arg2);

        void DebugFormat(string format, object arg0, object arg1);

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, object arg0, object arg1, object arg2);

        void ErrorFormat(IFormatProvider provider, string format, params object[] args);

        void ErrorFormat(string format, object arg0, object arg1);

        void ErrorFormat(string format, object arg0);

        void ErrorFormat(string format, params object[] args);

        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void FatalFormat(string format, object arg0, object arg1, object arg2);

        void FatalFormat(string format, object arg0);

        void FatalFormat(string format, params object[] args);

        void FatalFormat(IFormatProvider provider, string format, params object[] args);

        void FatalFormat(string format, object arg0, object arg1);

        void Info(object message, Exception exception);

        void Info(object message);

        void InfoFormat(string format, object arg0, object arg1, object arg2);

        void InfoFormat(string format, object arg0, object arg1);

        void InfoFormat(string format, object arg0);

        void InfoFormat(string format, params object[] args);

        void InfoFormat(IFormatProvider provider, string format, params object[] args);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string format, object arg0, object arg1);

        void WarnFormat(string format, object arg0);

        void WarnFormat(string format, params object[] args);

        void WarnFormat(IFormatProvider provider, string format, params object[] args);

        void WarnFormat(string format, object arg0, object arg1, object arg2);

    }

}
