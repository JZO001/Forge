/* *********************************************************************
 * Date: 11 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents the keys
    /// </summary>
    [Serializable]
    public class SubscribedKeyPressEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribedKeyPressEventArgs" /> class.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        public SubscribedKeyPressEventArgs(KeysSubscription subscription)
        {
            if (subscription == null)
            {
                ThrowHelper.ThrowArgumentNullException("subscription");
            }
            this.Subscription = subscription;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public KeysSubscription Subscription { get; private set; }

    }

}
