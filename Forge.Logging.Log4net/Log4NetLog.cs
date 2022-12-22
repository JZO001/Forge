using Forge.Logging.Abstraction;
using Forge.Shared;
using System;

namespace Forge.Logging.Log4net
{

    /// <summary>Log wrapper implementation</summary>
    /// <seealso cref="Abstraction.ILog" />
    public class Log4NetLog : ILog
    {

        private readonly log4net.ILog mLog = null;

        /// <summary>Initializes a new instance of the <see cref="Log4NetLog" /> class.</summary>
        /// <param name="log">The log.</param>
        public Log4NetLog(log4net.ILog log)
        {
            if (log == null) ThrowHelper.ThrowArgumentNullException("log");
            mLog = log;
        }

        /// <summary>Gets a value indicating whether this instance is fatal enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is fatal enabled; otherwise, <c>false</c>.</value>
        public bool IsFatalEnabled { get { return mLog.IsFatalEnabled; } }

        /// <summary>Gets a value indicating whether this instance is warn enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is warn enabled; otherwise, <c>false</c>.</value>
        public bool IsWarnEnabled { get { return mLog.IsWarnEnabled; } }

        /// <summary>Gets a value indicating whether this instance is information enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is information enabled; otherwise, <c>false</c>.</value>
        public bool IsInfoEnabled { get { return mLog.IsInfoEnabled; } }

        /// <summary>Gets a value indicating whether this instance is debug enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
        public bool IsDebugEnabled { get { return mLog.IsDebugEnabled; } }

        /// <summary>Gets a value indicating whether this instance is error enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is error enabled; otherwise, <c>false</c>.</value>
        public bool IsErrorEnabled { get { return mLog.IsErrorEnabled; } }

        /// <summary>Gets a value indicating whether this instance is trace enabled.</summary>
        /// <value>
        ///   <c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.</value>
        public bool IsTraceEnabled { get { return false; } }

        /// <summary>Logs the specified message as a debug entry</summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            mLog.Debug(message);
        }

        /// <summary>Logs the specified message as a debug entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            mLog.Debug(message, exception);
        }

        /// <summary>Logs the specified message as a error entry</summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            mLog.Error(message);
        }

        /// <summary>Logs the specified message as a error entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            mLog.Error(message, exception);
        }

        /// <summary>Logs the specified message as a fatal/critical entry</summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            mLog.Fatal(message);
        }

        /// <summary>Logs the specified message as a fatal/critical entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception)
        {
            mLog.Fatal(message, exception);
        }

        /// <summary>Logs the specified message as a info entry</summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            mLog.Info(message);
        }

        /// <summary>Logs the specified message as a info entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            mLog.Info(message, exception);
        }

        /// <summary>Logs the specified message as a trace entry</summary>
        /// <param name="message">The message.</param>
        public void Trace(string message)
        {
        }

        /// <summary>Logs the specified message as a trace entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Trace(string message, Exception exception)
        {
        }

        /// <summary>Logs the specified message as a warning entry</summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            mLog.Warn(message);
        }

        /// <summary>Logs the specified message as a warning entry</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            mLog.Warn(message, exception);
        }

    }

    /// <summary>Log wrapper implementation</summary>
    public class Log4NetLog<TLoggerType> : Log4NetLog, ILog<TLoggerType>
    {

        /// <summary>Initializes a new instance of the <see cref="Log4NetLog{TLoggerType}" /> class.</summary>
        /// <param name="log">The log.</param>
        public Log4NetLog(log4net.ILog log) : base(log)
        {
        }

    }

}
