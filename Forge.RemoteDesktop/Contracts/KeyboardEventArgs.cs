/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Windows.Forms;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the key event arguments
    /// </summary>
    [Serializable]
    public class KeyboardEventArgs
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardEventArgs"/> class.
        /// </summary>
        /// <param name="keyEventType">Type of the key event.</param>
        /// <param name="key">The key.</param>
        public KeyboardEventArgs(KeyboardEventTypeEnum keyEventType, Keys key)
        {
            KeyEventType = keyEventType;
            Key = key;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the type of the key event.
        /// </summary>
        /// <value>
        /// The type of the key event.
        /// </value>
        public KeyboardEventTypeEnum KeyEventType { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Keys Key { get; private set; }

        #endregion

    }

}
