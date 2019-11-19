/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using Forge.Configuration.Shared;
using Forge.Net.Remoting.Messaging;
using Forge.Persistence.Formatters;

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Base of the message sinks
    /// </summary>
    public abstract class MessageSinkBase : MBRBase, IMessageSink
    {

        #region Field(s)

        private static GZipFormatter mGZipFormatter = new GZipFormatter();

        /// <summary>
        /// Message sink identifier
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected String mMessageSinkId = Guid.NewGuid().ToString();

        /// <summary>
        /// Compress data
        /// </summary>
        protected bool mCompressData = false;

        /// <summary>
        /// Compression over limit
        /// </summary>
        protected int mCompressDataOverSize = 1024;

        /// <summary>
        /// Represents the initialization state of the sink
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool mInitialized = false;

        private bool mDisposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSinkBase"/> class.
        /// </summary>
        protected MessageSinkBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSinkBase"/> class.
        /// </summary>
        /// <param name="messageSinkId">The message sink id.</param>
        /// <param name="compressData">if set to <c>true</c> [compress data].</param>
        /// <param name="compressDataOverSize">Size of the compress data over.</param>
        protected MessageSinkBase(string messageSinkId, bool compressData, int compressDataOverSize)
        {
            if (string.IsNullOrEmpty(messageSinkId))
            {
                ThrowHelper.ThrowArgumentNullException("messageSinkId");
            }
            if (compressDataOverSize < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("compressDataOverSize");
            }
            this.mMessageSinkId = messageSinkId;
            this.mCompressData = compressData;
            this.mCompressDataOverSize = compressDataOverSize;
        } 

        #endregion
        
        #region Public properties

        /// <summary>
        /// Get the unique identifier of the sink.
        /// </summary>
        /// <value>
        /// The message sink id.
        /// </value>
        public string MessageSinkId
        {
            get { return mMessageSinkId; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initialize message sink from configuration
        /// </summary>
        /// <param name="pi">The pi.</param>
        public virtual void Initialize(CategoryPropertyItem pi)
        {
            if (pi == null)
            {
                ThrowHelper.ThrowArgumentNullException("pi");
            }

            if (!this.mInitialized)
            {
                CategoryPropertyItem piCompressData = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "CompressData");
                if (piCompressData != null)
                {
                    bool.TryParse(piCompressData.EntryValue, out mCompressData);
                    CategoryPropertyItem piCompressDataOverSize = ConfigurationAccessHelper.GetCategoryPropertyByPath(pi.PropertyItems, "CompressDataOverSize");
                    if (piCompressDataOverSize != null)
                    {
                        int result = 0;
                        if (int.TryParse(piCompressDataOverSize.EntryValue, out result))
                        {
                            mCompressDataOverSize = result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serialize data and save it into the return object. Return object optionaly stores information to deserialization.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Serialized message content
        /// </returns>
        public abstract MessageSinkParameters Serialize(IMessage message);

        /// <summary>
        /// Deserialize data from the parameters with the provided configuration.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// Deserialized message content
        /// </returns>
        public abstract IMessage Deserialize(MessageSinkParameters parameters);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!obj.GetType().Equals(GetType())) return false;

            MessageSinkBase other = (MessageSinkBase)obj;
            return other.mMessageSinkId == mMessageSinkId && other.mInitialized == mInitialized;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                mGZipFormatter.Write(ms, data);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected byte[] Decompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                return mGZipFormatter.Read(ms);
            }
        }

        /// <summary>
        /// Does the dispose check.
        /// </summary>
        protected void DoDisposeCheck()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            mDisposed = true;
        }

        #endregion

    }

}
