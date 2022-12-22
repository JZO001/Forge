/* *********************************************************************
 * Date: 08 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;

namespace Forge.Net.Remoting.Sinks
{

    /// <summary>
    /// Represents the the message sink which help to restore a message
    /// </summary>
    [Serializable]
    public class MessageSinkConfiguration : IMessageSinkConfiguration
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSinkConfiguration"/> class.
        /// </summary>
        /// <param name="messageSinkId">The message sink id.</param>
        /// <param name="decompressData">if set to <c>true</c> [decompress data].</param>
        public MessageSinkConfiguration(string messageSinkId, bool decompressData)
        {
            if (string.IsNullOrEmpty(messageSinkId))
            {
                ThrowHelper.ThrowArgumentNullException("messageSinkId");
            }
            MessageSinkId = messageSinkId;
            DecompressData = decompressData;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the message sink id.
        /// </summary>
        /// <value>
        /// The message sink id.
        /// </value>
        public string MessageSinkId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [decompress data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [decompress data]; otherwise, <c>false</c>.
        /// </value>
        public bool DecompressData { get; private set; }

        #endregion

        #region Public method(s)

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

            MessageSinkConfiguration other = (MessageSinkConfiguration)obj;
            return other.MessageSinkId == MessageSinkId && other.DecompressData.Equals(DecompressData);
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

        #endregion

    }

}
