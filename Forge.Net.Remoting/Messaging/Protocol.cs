/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.IO;
using System.Text;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Synapse;
using Forge.Persistence.Formatters;
using Forge.Persistence.Serialization;
using Forge.Reflection;

namespace Forge.Net.Remoting.Messaging
{

    /// <summary>
    /// Transmission formatter
    /// </summary>
    public sealed class Protocol
    {

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(Protocol));

        private BinarySerializerFormatter<MessageHeader> mFormatter = new BinarySerializerFormatter<MessageHeader>(BinarySerializerBehaviorEnum.DoNotThrowExceptionOnMissingField, TypeLookupModeEnum.AllowAll, true);

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        public Protocol()
        {
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="maxMessageSize">Size of the max message.</param>
        /// <returns>Message header with body</returns>
        /// <exception cref="ProtocolViolationException">
        /// Invalid header length.
        /// or
        /// Invalid message length number format.
        /// or
        /// Invalid header length.
        /// </exception>
        /// <exception cref="MessageSecurityException">
        /// Message header length is larger then the maximum allowed.
        /// or
        /// Message header length is larger then the maximum allowed.
        /// or
        /// Message length is larger then the maximum allowed.
        /// </exception>
        public MessageHeaderWithBody Read(NetworkStream stream, int maxMessageSize)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("PROTOCOL, begin read message header with body from stream ({0})", stream.Id.ToString()));

            MessageHeaderWithBody result = null;
            using (MemoryStream ms = new MemoryStream())
            {
                int b = stream.ReadByte();
                if (b == 0)
                {
                    throw new ProtocolViolationException("Invalid header length.");
                }
                string len = maxMessageSize.ToString();
                while (b > 0)
                {
                    ms.WriteByte((byte)b);
                    if (maxMessageSize > 0 && ms.Position > len.Length)
                    {
                        throw new MessageSecurityException("Message header length is larger then the maximum allowed.");
                    }
                    b = stream.ReadByte();
                }

                // header hosszának kinyerése
                string headerLenString = Encoding.UTF8.GetString(ms.ToArray());
                int headerLength = 0;
                try
                {
                    headerLength = int.Parse(headerLenString);
                    if (maxMessageSize > 0 && headerLength > maxMessageSize)
                    {
                        throw new MessageSecurityException("Message header length is larger then the maximum allowed.");
                    }
                }
                catch (Exception ex)
                {
                    throw new ProtocolViolationException("Invalid message length number format.", ex);
                }
                if (headerLength == 0)
                {
                    throw new ProtocolViolationException("Invalid header length.");
                }

                // header kiolvasása a stream-ből
                byte[] data = new byte[headerLength]; // méretezés
                ReadStream(stream, data); // olvasás
                MessageHeader header = null;
                using (MemoryStream inputStream = new MemoryStream(data))
                {
                    header = mFormatter.Read(inputStream);
                }

                // hasznos adat kinyerése a stream-ből a header infok alapján
                if (header.MessageLength <= 0)
                {
                    result = new MessageHeaderWithBody(header, new byte[0]); // null esete
                }
                else
                {
                    if (maxMessageSize > 0 && header.MessageLength > maxMessageSize)
                    {
                        throw new MessageSecurityException("Message length is larger then the maximum allowed.");
                    }
                    data = new byte[header.MessageLength]; // hasznos adat hossza, méretezés
                    ReadStream(stream, data); // olvasás
                    result = new MessageHeaderWithBody(header, data);
                }
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("PROTOCOL, end read message header with body from stream ({0}), result hash: {1}", stream.Id.ToString(), result.GetHashCode().ToString()));

            return result;
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        /// <param name="messageSinkId">The message sink id.</param>
        /// <param name="messageSinkConfiguration">The message sink configuration.</param>
        /// <param name="maxMessageSize">Size of the max message.</param>
        /// <exception cref="MessageSecurityException">
        /// Provided data size is larger than the maximum allowed message size.
        /// or
        /// Serialized message size is larger then the maximum allowed size.
        /// </exception>
        public void Write(NetworkStream stream, byte[] data, String messageSinkId, IMessageSinkConfiguration messageSinkConfiguration, int maxMessageSize)
        {
            if (stream == null)
            {
                ThrowHelper.ThrowArgumentNullException("stream");
            }
            if (data == null)
            {
                ThrowHelper.ThrowArgumentNullException("data");
            }

            if (maxMessageSize > 0 && data.Length > maxMessageSize)
            {
                throw new MessageSecurityException(string.Format("Provided data size ({0}) is larger than the maximum allowed message size ({1}).", data.Length.ToString(), maxMessageSize.ToString()));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                MessageHeader header = new MessageHeader(messageSinkId, data.Length, messageSinkConfiguration);
                using (MemoryStream outputStream = new MemoryStream())
                {
                    mFormatter.Write(outputStream, header);

                    if (maxMessageSize > 0 && outputStream.Length > maxMessageSize)
                    {
                        throw new MessageSecurityException("Serialized message size is larger then the maximum allowed size.");
                    }
                    byte[] streamData = outputStream.ToArray();
                    byte[] lenBytes = Encoding.UTF8.GetBytes(streamData.Length.ToString());
                    ms.Write(lenBytes, 0, lenBytes.Length); // fejléc hossza
                    ms.WriteByte((byte)0); // lezáró karakter
                    ms.Write(streamData, 0, streamData.Length); // header info
                    ms.Write(data, 0, data.Length); // data
                    ms.WriteTo(stream);
                }
            }
        }

        private static void ReadStream(Stream stream, byte[] data)
        {
            int readBytes = 0;
            while (readBytes < data.Length)
            {
                int bytes = stream.Read(data, readBytes, data.Length - readBytes);
                if (bytes == 0)
                {
                    throw new IOException("End of stream detected. Connection closed.");
                }
                readBytes += bytes;
            }
        }

    }

}
