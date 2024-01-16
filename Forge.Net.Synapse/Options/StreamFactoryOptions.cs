using System.Diagnostics;
using System.Threading;

namespace Forge.Net.Synapse.Options
{

    /// <summary>Basic options for stream factory</summary>
    public class StreamFactoryOptions
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mReceiveBufferSize = 8192;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSendBufferSize = 8192;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mReceiveTimeout = Timeout.Infinite;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mSendTimeout = 120000;

        /// <summary>Initializes a new instance of the <see cref="StreamFactoryOptions" /> class.</summary>
        public StreamFactoryOptions()
        {
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

    }

}
