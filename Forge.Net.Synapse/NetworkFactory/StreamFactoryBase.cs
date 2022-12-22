/* *********************************************************************
 * Date: 14 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Legacy;
using Forge.Net.Synapse.NetworkServices;
using Forge.Net.Synapse.Options;
using Forge.Shared;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Base class for stream factories. Contains helper methods for cofiguration parsing.
    /// </summary>
    public abstract class StreamFactoryBase : MBRBase, IStreamFactory
    {

        #region Field(s)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mReceiveBufferSize = 8192;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSendBufferSize = 8192;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mReceiveTimeout = Timeout.Infinite;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSendTimeout = 120000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFactoryBase"/> class.
        /// </summary>
        protected StreamFactoryBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFactoryBase"/> class.
        /// </summary>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        protected StreamFactoryBase(int receiveBufferSize, int sendBufferSize)
        {
            if (receiveBufferSize >= 1024)
            {
                mReceiveBufferSize = receiveBufferSize;
            }
            if (sendBufferSize >= 1024)
            {
                mSendBufferSize = sendBufferSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFactoryBase"/> class.
        /// </summary>
        /// <param name="receiveBufferSize">Size of the receive buffer.</param>
        /// <param name="sendBufferSize">Size of the send buffer.</param>
        /// <param name="noDelay">if set to <c>true</c> [no delay].</param>
        protected StreamFactoryBase(int receiveBufferSize, int sendBufferSize, bool noDelay)
            : this(receiveBufferSize, sendBufferSize)
        {
            NoDelay = noDelay;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool IsInitialized
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        [DebuggerHidden]
        public int ReceiveBufferSize
        {
            get { return mReceiveBufferSize; }
            set
            {
                if (value >= 1024)
                {
                    mReceiveBufferSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the send buffer.
        /// </summary>
        /// <value>
        /// The size of the send buffer.
        /// </value>
        [DebuggerHidden]
        public int SendBufferSize
        {
            get { return mSendBufferSize; }
            set
            {
                if (value >= 1024)
                {
                    mSendBufferSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value of no delay
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no delay]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public bool NoDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        [DebuggerHidden]
        public int ReceiveTimeout
        {
            get { return mReceiveTimeout; }
            set
            {
                if (value >= Timeout.Infinite)
                {
                    mReceiveTimeout = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>
        /// The send timeout.
        /// </value>
        [DebuggerHidden]
        public int SendTimeout
        {
            get { return mSendTimeout; }
            set
            {
                if (value >= Timeout.Infinite)
                {
                    mSendTimeout = value;
                }
            }
        }

        #endregion

#region Public method(s)

#if NETCOREAPP3_1_OR_GREATER
        /// <summary>
        /// Creates the network stream asynhronously.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        public async Task<NetworkStream> CreateNetworkStreamAsync(ITcpClient tcpClient)
        {
            if (tcpClient == null) ThrowHelper.ThrowArgumentNullException("tcpClient");
            return await Task.Run(() => CreateNetworkStream(tcpClient));
        }
#endif

        /// <summary>
        /// Creates the network stream.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Network Stream instance</returns>
        public abstract NetworkStream CreateNetworkStream(ITcpClient tcpClient);

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        public virtual void Initialize(IPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }

            int value = 8192;
            if (ParseIntValue(configItem, "ReceiveBufferSize", 1024, 65536, ref value))
            {
                mReceiveBufferSize = value;
            }

            value = 8192;
            if (ParseIntValue(configItem, "SendBufferSize", 1024, 65536, ref value))
            {
                mSendBufferSize = value;
            }

            value = Timeout.Infinite;
            if (ParseIntValue(configItem, "ReceiveTimeout", Timeout.Infinite, int.MaxValue, ref value))
            {
                mReceiveTimeout = value;
            }

            value = 120000;
            if (ParseIntValue(configItem, "SendTimeout", Timeout.Infinite, int.MaxValue, ref value))
            {
                mSendTimeout = value;
            }

            NoDelay = ParseBooleanValue(configItem, "NoDelay");
        }

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="options">The pptions.</param>
        public virtual void Initialize(StreamFactoryOptions options)
        {
            if (options == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }

            mReceiveBufferSize = options.ReceiveBufferSize;
            mSendBufferSize = options.SendBufferSize;
            mReceiveTimeout = options.ReceiveTimeout;
            mSendTimeout = options.SendTimeout;
            NoDelay = options.NoDelay;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Does the initialize check.
        /// </summary>
        protected void DoInitializeCheck()
        {
            if (!IsInitialized)
            {
                throw new InitializationException();
            }
        }

        /// <summary>
        /// Parses the int value.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="value">The value.</param>
        /// <returns>True, if the parse method was succeeded, otherwise False.</returns>
        protected static bool ParseIntValue(IPropertyItem root, string entryId, int minValue, int maxValue, ref int value)
        {
            bool result = false;
            IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(root, entryId);
            if (pi != null)
            {
                try
                {
                    value = int.Parse(pi.Value);
                    result = true;
                }
                catch (FormatException ex)
                {
                    throw new InvalidConfigurationException(string.Format("Invalid value for item: {0}", entryId), ex);
                }
                if (value < minValue)
                {
                    throw new InvalidConfigurationException(string.Format("Minimum value ({0}) is out of range ({1}) for item: {2}", minValue, result, entryId));
                }
                if (value > maxValue)
                {
                    throw new InvalidConfigurationException(string.Format("Maximum value ({0}) is out of range ({1}) for item: {2}", maxValue, result, entryId));
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the boolean value.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="entryId">The entry id.</param>
        /// <returns>True, if the parse method was succeeded, otherwise False.</returns>
        protected static bool ParseBooleanValue(IPropertyItem root, string entryId)
        {
            bool result = false;
            IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(root, entryId);
            if (pi != null)
            {
                bool.TryParse(pi.Value, out result);
            }
            return result;
        }

        /// <summary>
        /// Parses the string value.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>True, if the parse method was succeeded, otherwise False.</returns>
        protected static string ParseStringValue(IPropertyItem root, string entryId, string defaultValue)
        {
            string result = defaultValue;
            IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(root, entryId);
            if (pi != null)
            {
                result = pi.Value;
            }
            return result;
        }

        /// <summary>
        /// Parses the enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="root">The root.</param>
        /// <param name="entryId">The entry unique identifier.</param>
        /// <returns></returns>
        protected static TEnum ParseEnumValue<TEnum>(IPropertyItem root, string entryId) where TEnum : struct
        {
            TEnum result = default(TEnum);
            IPropertyItem pi = ConfigurationAccessHelper.GetPropertyByPath(root, entryId);
            if (pi != null)
            {
                try
                {
                    result = (TEnum)Enum.Parse(typeof(TEnum), pi.Value, true);
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

#endregion

    }

}
