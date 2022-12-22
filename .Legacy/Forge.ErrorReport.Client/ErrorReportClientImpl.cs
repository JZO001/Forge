/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.ErrorReport.Client
{

    /// <summary>
    /// Error Report client proxy
    /// </summary>
    public class ErrorReportClientImpl : Forge.Net.Remoting.Proxy.ProxyBase, Forge.ErrorReport.Contracts.IErrorReportSendContract
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportClientImpl"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sessionId">The session id.</param>
        public ErrorReportClientImpl(Forge.Net.Remoting.Channels.Channel channel, System.String sessionId) : base(channel, sessionId) { }

        /// <summary>
        /// Sends the error report.
        /// </summary>
        /// <param name="_p0">The _P0.</param>
        /// <exception cref="Forge.Net.Remoting.RemoteMethodInvocationException">Unable to call remote method. See inner exception for details.</exception>
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public void SendErrorReport(Forge.ErrorReport.ReportPackage _p0)
        {
            DoDisposeCheck();
            try
            {
                Forge.Net.Remoting.Messaging.MethodParameter[] _mps = null;

                Forge.Net.Remoting.Messaging.MethodParameter _mp0 = new Forge.Net.Remoting.Messaging.MethodParameter(0, typeof(Forge.ErrorReport.ReportPackage).FullName + ", " + new System.Reflection.AssemblyName(typeof(Forge.ErrorReport.ReportPackage).Assembly.FullName).Name, _p0);
                _mps = new Forge.Net.Remoting.Messaging.MethodParameter[] { _mp0 };

                Forge.Net.Remoting.Messaging.RequestMessage _message = new Forge.Net.Remoting.Messaging.RequestMessage(System.Guid.NewGuid().ToString(), Forge.Net.Remoting.MessageTypeEnum.Datagram, Forge.Net.Remoting.MessageInvokeModeEnum.RequestService, typeof(Forge.ErrorReport.Contracts.IErrorReportSendContract), "SendErrorReport", _mps, true);
                _message.Context.Add(Forge.Net.Remoting.Proxy.ProxyBase.PROXY_ID, this.ProxyId);

                long _timeout = GetTimeoutByMethod(typeof(Forge.ErrorReport.Contracts.IErrorReportSendContract), "SendErrorReport", _mps, Forge.Net.Remoting.Proxy.MethodTimeoutEnum.CallTimeout);

                this.mChannel.SendMessage(this.mSessionId, _message, _timeout);
            }
            catch (System.Exception ex)
            {
                throw new Forge.Net.Remoting.RemoteMethodInvocationException("Unable to call remote method. See inner exception for details.", ex);
            }
        }

    }

}
