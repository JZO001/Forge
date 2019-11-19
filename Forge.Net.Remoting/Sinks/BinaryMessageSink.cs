/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using Forge.Configuration.Shared;
using Forge.Net.Remoting.Messaging;
using Forge.Persistence.Formatters;

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Binary message sink with compression feature
    /// </summary>
    public sealed class BinaryMessageSink : MessageSinkBase
    {

        #region Field(s)

        private static readonly string BINARY_MESSAGE_SINK_ID = "15218C48-9D94-490E-973A-B9AE73A0EB93";

        private readonly BinaryFormatter<IMessage> mFormatter = new BinaryFormatter<IMessage>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMessageSink"/> class.
        /// </summary>
        public BinaryMessageSink()
        {
            this.mMessageSinkId = BINARY_MESSAGE_SINK_ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMessageSink"/> class.
        /// </summary>
        /// <param name="compressData">if set to <c>true</c> [compress data].</param>
        /// <param name="compressDataOverSize">Size of the compress data over.</param>
        public BinaryMessageSink(bool compressData, int compressDataOverSize)
            : base(BINARY_MESSAGE_SINK_ID, compressData, compressDataOverSize)
        {
            this.mInitialized = true;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initialize message sink from configuration
        /// </summary>
        /// <param name="pi">The pi.</param>
        public override void Initialize(CategoryPropertyItem pi)
        {
            if (!this.mInitialized)
            {
                base.Initialize(pi);
                this.mInitialized = true;
            }
        }

        /// <summary>
        /// Serialize data and save it into the return object. Return object optionaly stores information to deserialization.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Message Sink parameters</returns>
        /// <exception cref="System.FormatException"></exception>
        public override MessageSinkParameters Serialize(IMessage message)
        {
            if (message == null)
            {
                ThrowHelper.ThrowArgumentNullException("message");
            }

            MessageSinkParameters result = null;

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    mFormatter.Write(ms, message);
                    byte[] finalData = null;
                    bool comp = false;
                    if (mCompressData)
                    {
                        if (ms.Length > this.mCompressDataOverSize && this.mCompressDataOverSize >= 0)
                        {
                            comp = true;
                            finalData = Compress(ms.ToArray());
                        }
                        else
                        {
                            finalData = ms.ToArray();
                        }
                    }
                    else
                    {
                        finalData = ms.ToArray();
                    }
                    result = new MessageSinkParameters(new MessageSinkConfiguration(mMessageSinkId, comp), finalData);
                }
                catch (Exception ex)
                {
                    throw new FormatException(ex.Message, ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Deserialize data from the parameters with the provided configuration.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Deserialized message</returns>
        /// <exception cref="System.FormatException">
        /// </exception>
        public override IMessage Deserialize(MessageSinkParameters parameters)
        {
            if (parameters == null)
            {
                ThrowHelper.ThrowArgumentNullException("parameters");
            }
            if ((parameters.ConfigurationToDeserialize as MessageSinkConfiguration) == null)
            {
                throw new FormatException(string.Format("Configuration class type is not {0}", typeof(MessageSinkConfiguration).FullName));
            }

            IMessage result = null;

            try
            {
                MessageSinkConfiguration config = (MessageSinkConfiguration)parameters.ConfigurationToDeserialize;
                using (MemoryStream ms = new MemoryStream(config.DecompressData ? Decompress(parameters.SerializedData) : parameters.SerializedData))
                {
                    ms.Position = 0;
                    result = mFormatter.Read(ms);
                }
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }

            return result;
        }

        #endregion

    }

}
