/* *********************************************************************
 * Date: 14 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Forge.Configuration.Shared;

namespace Forge.Net.Synapse.NetworkFactory
{

    /// <summary>
    /// Base class for stream factories. Contains helper methods for cofiguration parsing.
    /// </summary>
    public abstract class StreamFactoryBase : MBRBase
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
                this.mReceiveBufferSize = receiveBufferSize;
            }
            if (sendBufferSize >= 1024)
            {
                this.mSendBufferSize = sendBufferSize;
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

        /// <summary>
        /// Finalizes an instance of the <see cref="StreamFactoryBase"/> class.
        /// </summary>
        ~StreamFactoryBase()
        {
            Dispose(false);
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

        /// <summary>
        /// Initializes the specified config item.
        /// </summary>
        /// <param name="configItem">The config item.</param>
        public virtual void Initialize(CategoryPropertyItem configItem)
        {
            if (configItem == null)
            {
                ThrowHelper.ThrowArgumentNullException("configItem");
            }

            int value = 8192;
            if (ParseIntValue(configItem, "ReceiveBufferSize", 1024, 65536, ref value))
            {
                this.mReceiveBufferSize = value;
            }

            value = 8192;
            if (ParseIntValue(configItem, "SendBufferSize", 1024, 65536, ref value))
            {
                this.mSendBufferSize = value;
            }

            value = Timeout.Infinite;
            if (ParseIntValue(configItem, "ReceiveTimeout", Timeout.Infinite, int.MaxValue, ref value))
            {
                this.mReceiveTimeout = value;
            }

            value = Timeout.Infinite;
            if (ParseIntValue(configItem, "SendTimeout", Timeout.Infinite, int.MaxValue, ref value))
            {
                this.mSendTimeout = value;
            }

            NoDelay = ParseBooleanValue(configItem, "NoDelay");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
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
        protected static bool ParseIntValue(CategoryPropertyItem root, String entryId, int minValue, int maxValue, ref int value)
        {
            bool result = false;
            CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, entryId);
            if (pi != null)
            {
                try
                {
                    value = int.Parse(pi.EntryValue);
                    result = true;
                }
                catch (FormatException ex)
                {
                    throw new InvalidConfigurationException(String.Format("Invalid value for item: {0}", entryId), ex);
                }
                if (value < minValue)
                {
                    throw new InvalidConfigurationException(String.Format("Minimum value ({0}) is out of range ({1}) for item: {2}", minValue, result, entryId));
                }
                if (value > maxValue)
                {
                    throw new InvalidConfigurationException(String.Format("Maximum value ({0}) is out of range ({1}) for item: {2}", maxValue, result, entryId));
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
        protected static bool ParseBooleanValue(CategoryPropertyItem root, String entryId)
        {
            bool result = false;
            CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, entryId);
            if (pi != null)
            {
                try
                {
                    result = bool.Parse(pi.EntryValue);
                }
                catch (Exception)
                {
                }
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
        protected static String ParseStringValue(CategoryPropertyItem root, String entryId, String defaultValue)
        {
            String result = defaultValue;
            CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, entryId);
            if (pi != null)
            {
                result = pi.EntryValue;
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
        protected static TEnum ParseEnumValue<TEnum>(CategoryPropertyItem root, String entryId) where TEnum : struct
        {
            TEnum result = default(TEnum);
            CategoryPropertyItem pi = ConfigurationAccessHelper.GetCategoryPropertyByPath(root.PropertyItems, entryId);
            if (pi != null)
            {
                try
                {
                    result = (TEnum)Enum.Parse(typeof(TEnum), pi.EntryValue, true);
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
