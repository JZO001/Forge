/* *********************************************************************
 * Date: 08 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the text information of the clipboard
    /// </summary>
    [Serializable]
    public class ClipboardChangedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardChangedEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ClipboardChangedEventArgs(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }

    }

}
