/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Invoker;
using Forge.Legacy;
using Forge.Logging.Abstraction;
using Forge.Net.Remoting.ConfigSection;
using Forge.Reflection;
using Forge.Shared;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Store registered channels.
    /// </summary>
    public sealed class ChannelServices : MBRBase
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger<ChannelServices>();

        private static readonly HashSet<Channel> mChannels = new HashSet<Channel>();

        private static readonly ChannelServices mChannelServicesSingleton = new ChannelServices();

        private static bool mInitialized = false;

        /// <summary>
        /// Occurs when [register channel event].
        /// </summary>
        public static event EventHandler<ChannelRegistrationEventArgs> RegisterChannelEvent;

        /// <summary>
        /// Occurs when [unregister channel event].
        /// </summary>
        public static event EventHandler<ChannelRegistrationEventArgs> UnregisterChannelEvent;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes the <see cref="ChannelServices"/> class.
        /// </summary>
        static ChannelServices()
        {
            bool autoLoad = false;
            if (ConfigurationAccessHelper.ParseBooleanValue(RemotingConfiguration.Settings.CategoryPropertyItems, "Settings/AutomaticallyLoadChannels", ref autoLoad) && autoLoad)
            {
                Initialize(RemotingConfiguration.Settings.CategoryPropertyItems);
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ChannelServices"/> class from being created.
        /// </summary>
        private ChannelServices()
        {
        }

        #endregion

        #region Public static method(s)

        /// <summary>Initializes this instance with the XML based configuration, if it is exist.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize()
        {
            if (RemotingConfiguration.Settings.CategoryPropertyItems.Count > 0)
            {
                Initialize(RemotingConfiguration.Settings.CategoryPropertyItems);
            }
        }

        /// <summary>Initializes this instance.</summary>
        /// <param name="configurationRoot">The configuration</param>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationException">Channel id has not been definied.
        /// or
        /// Channel class name has not been definied.</exception>
        /// <exception cref="Forge.Configuration.Shared.InvalidConfigurationValueException"></exception>
        /// <exception cref="Forge.Shared.InitializationException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(IPropertyItem configurationRoot)
        {
            if (configurationRoot is null)
            {
                throw new ArgumentNullException(nameof(configurationRoot));
            }

            if (!mInitialized)
            {
                if (LOGGER.IsInfoEnabled) LOGGER.Info("Initializing channel services.");
                IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(configurationRoot, "Channels");
                if (pi != null)
                {
                    try
                    {
                        IEnumerator<IPropertyItem> iterator = pi.Items.Values.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            pi = iterator.Current;
                            if (string.IsNullOrEmpty(pi.Id))
                            {
                                throw new InvalidConfigurationException("Channel id has not been definied.");
                            }
                            if (string.IsNullOrEmpty(pi.Value))
                            {
                                throw new InvalidConfigurationException("Channel class name has not been definied.");
                            }
                            Channel channel = null;
                            try
                            {
                                if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("Create channel instance from type '{0}'. ChannelId: '{1}'", pi.Value, pi.Id));
                                Type cls = TypeHelper.GetTypeFromString(pi.Value);
                                channel = (Channel)cls.GetConstructor(new Type[] { }).Invoke(null);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationValueException(string.Format("Unable to create instance of channel type: {0}", pi.Value), ex);
                            }
                            mChannels.Add(channel);
                            if (LOGGER.IsInfoEnabled) LOGGER.Info(string.Format("Initializing channel instance, type '{0}'. ChannelId: '{1}'", pi.Value, pi.Id));
                            channel.Initialize(pi);
                            //channel.startListening();
                        }
                    }
                    catch (Exception ex)
                    {
                        // system restoring
                        if (mChannels.Count > 0)
                        {
                            foreach (Channel c in mChannels)
                            {
                                c.StopListening();
                                c.Dispose();
                            }
                            mChannels.Clear();
                        }
                        throw new InitializationException(string.Format("Unable to initialize {0}.", typeof(ChannelServices).Name), ex);
                    }
                }

                mInitialized = true;
                if (LOGGER.IsInfoEnabled) LOGGER.Info("Channel services successfully initialized.");
            }
        }

        /// <summary>
        /// Starts the listening channels.
        /// </summary>
        /// <exception cref="Forge.Shared.InitializationException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void StartListeningChannels()
        {
            foreach (Channel channel in mChannels)
            {
                try
                {
                    channel.StartListening();
                }
                catch (Exception ex)
                {
                    StopListeningChannels();
                    throw new InitializationException(string.Format("Failed to start listening on a channel, ChannelId: {0}", channel.ChannelId), ex);
                }
            }
        }

        /// <summary>
        /// Stops the listening channels.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void StopListeningChannels()
        {
            foreach (Channel channel in mChannels)
            {
                try
                {
                    channel.StopListening();
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Failed to stop listening a channel. ChannelId: {0}", channel.ChannelId), ex);
                }
            }
        }

        /// <summary>
        /// Registers the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>True, if this channel was a new instance, otherwise False.</returns>
        /// <exception cref="System.ArgumentNullException">channel</exception>
        /// <exception cref="Forge.Shared.InitializationException">Channel has not been initialized.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool RegisterChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }
            if (!channel.IsInitialized)
            {
                throw new InitializationException("Channel has not been initialized.");
            }
            bool result = false;
            if (!mChannels.Contains(channel))
            {
                result = true;
                mChannels.Add(channel);
                Executor.Invoke(RegisterChannelEvent, mChannelServicesSingleton, new ChannelRegistrationEventArgs(channel));
            }
            return result;
        }

        /// <summary>
        /// Unregisters the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>True, if the channel unregistered, otherwise False</returns>
        /// <exception cref="System.ArgumentNullException">channel</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool UnregisterChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }
            Executor.Invoke(UnregisterChannelEvent, mChannelServicesSingleton, new ChannelRegistrationEventArgs(channel));
            return mChannels.Remove(channel);
        }

        /// <summary>
        /// Determines whether [is channel registered] [the specified channel].
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///   <c>true</c> if [is channel registered] [the specified channel]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">channel</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsChannelRegistered(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }
            return mChannels.Contains(channel);
        }

        /// <summary>
        /// Determines whether [is channel registered] [the specified channel id].
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <returns>
        ///   <c>true</c> if [is channel registered] [the specified channel id]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool IsChannelRegistered(string channelId)
        {
            return GetChannelById(channelId) == null ? false : true;
        }

        /// <summary>
        /// Gets the registered channels.
        /// </summary>
        /// <value>
        /// The registered channels.
        /// </value>
        /// <returns>Enumerator of channels</returns>
        public static IEnumerable<Channel> RegisteredChannels
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return new HashSet<Channel>(mChannels); }
        }

        /// <summary>
        /// Gets the channel by id.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <returns>Channel</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Channel GetChannelById(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }

            Channel result = null;
            foreach (Channel channel in mChannels)
            {
                if (channelId.Equals(channel.ChannelId))
                {
                    result = channel;
                    break;
                }
            }
            return result;
        }

        #endregion

    }

}
