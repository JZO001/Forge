/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Diagnostics;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>
    /// Represents the arguments of the channel registration event
    /// </summary>
    [Serializable]
    public class ChannelRegistrationEventArgs : EventArgs
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Channel mChannel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelRegistrationEventArgs"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public ChannelRegistrationEventArgs(Channel channel)
        {
            if (channel == null)
            {
                ThrowHelper.ThrowArgumentNullException("channel");
            }
            mChannel = channel;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        [DebuggerHidden]
        public Channel Channel
        {
            get { return mChannel; }
        }

    }

}
