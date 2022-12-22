/* *********************************************************************
 * Date: 12 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;

namespace Forge.RemoteDesktop.Client
{

    /// <summary>
    /// Represents a key subscription
    /// </summary>
    [Serializable]
    public sealed class KeysSubscription
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeysSubscription"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public KeysSubscription(Keys key)
        {
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeysSubscription"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="alt">if set to <c>true</c> [alt].</param>
        /// <param name="control">if set to <c>true</c> [control].</param>
        /// <param name="shift">if set to <c>true</c> [shift].</param>
        public KeysSubscription(Keys key, bool alt, bool control, bool shift)
            : this(key)
        {
            Alt = alt;
            Control = control;
            Shift = shift;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Keys Key { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [alt].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [alt]; otherwise, <c>false</c>.
        /// </value>
        public bool Alt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [control].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [control]; otherwise, <c>false</c>.
        /// </value>
        public bool Control { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [shift].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [shift]; otherwise, <c>false</c>.
        /// </value>
        public bool Shift { get; private set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!GetType().Equals(obj.GetType())) return false;

            KeysSubscription s = (KeysSubscription)obj;

            return Key.Equals(s.Key) && Alt.Equals(s.Alt) && Control.Equals(s.Control) && Shift.Equals(s.Shift);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 3;
            hash = 43 * hash + Key.ToString().GetHashCode();
            hash = 40 ^ hash + Alt.GetHashCode();
            hash = 41 ^ hash + Control.GetHashCode();
            hash = 42 ^ hash + Shift.GetHashCode();
            return hash;
        }

        #endregion

    }

}
