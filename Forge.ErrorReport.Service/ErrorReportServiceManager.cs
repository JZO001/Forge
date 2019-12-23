/* *********************************************************************
 * Date: 15 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.CompilerServices;
using Forge.Configuration.Shared;
using Forge.ErrorReport.Contracts;
using Forge.Management;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Service;
using Forge.Net.Services;
using Forge.Net.Services.ConfigSection;
using Forge.Net.Services.Services;

namespace Forge.ErrorReport.Service
{

    /// <summary>
    /// Error Report Service Manager
    /// </summary>
    public sealed class ErrorReportServiceManager : RemoteServiceBase<IErrorReportSendContract, ErrorReportServiceImpl, ErrorReportServiceManager>
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportServiceManager"/> class.
        /// </summary>
        public ErrorReportServiceManager()
            : base(Consts.SERVICE_ID)
        {
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        /// <returns>Manager State</returns>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Start(long priority, IServiceDescriptor serviceDescriptor)
        {
            if (this.ManagerState != ManagerStateEnum.Started)
            {
                if (priority < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException("priority");
                }

                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, initializing service...", LOG_PREFIX));

                OnStart(ManagerEventStateEnum.Before);

                try
                {
                    ChannelServices.Initialize();

                    ChannelId = ConfigurationAccessHelper.GetValueByPath(NetworkServiceConfiguration.Settings.CategoryPropertyItems, string.Format("Services/{0}", Id));
                    if (string.IsNullOrEmpty(this.ChannelId))
                    {
                        this.ChannelId = Id;
                    }

                    Channel channel = LookUpChannel();

                    if (channel.ServerEndpoints.Count == 0)
                    {
                        throw new InvalidConfigurationException(string.Format("Channel '{0}' has not got listening server endpoint(s).", channel.ChannelId));
                    }

                    ServiceBaseServices.Initialize();
                    if (!ServiceBaseServices.IsContractRegistered(typeof(IErrorReportSendContract)))
                    {
                        ServiceBaseServices.RegisterContract(typeof(IErrorReportSendContract), typeof(ErrorReportServiceImpl));
                    }

                    this.mPriority = priority;
                    this.mServiceDescriptor = serviceDescriptor;

                    RegisterToPeerContext(channel, priority, serviceDescriptor);

                    this.ManagerState = ManagerStateEnum.Started;

                    if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("{0}, service successfully initialized.", LOG_PREFIX));
                }
                catch (Exception)
                {
                    this.ManagerState = ManagerStateEnum.Fault;
                    throw;
                }

                OnStart(ManagerEventStateEnum.After);
            }
            return this.ManagerState;
        }

        #endregion

    }

}
