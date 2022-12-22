/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Drawing;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the description of the service
    /// </summary>
    [Serializable]
    public class DescriptionResponseArgs
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionResponseArgs" /> class.
        /// </summary>
        /// <param name="desktopSize">Size of the desktop.</param>
        /// <param name="clipSize">Size of the clip.</param>
        /// <param name="cursors">The cursors.</param>
        /// <param name="acceptKeyboardAndMouseInputs">if set to <c>true</c> [accept keyboard and mouse inputs].</param>
        public DescriptionResponseArgs(Size desktopSize, Size clipSize, CursorInfo[] cursors, bool acceptKeyboardAndMouseInputs)
        {
            if (desktopSize == null)
            {
                ThrowHelper.ThrowArgumentNullException("desktopSize");
            }
            if (clipSize == null)
            {
                ThrowHelper.ThrowArgumentNullException("clipSize");
            }
            if (cursors == null)
            {
                ThrowHelper.ThrowArgumentNullException("cursors");
            }

            DesktopSize = desktopSize;
            ClipSize = clipSize;
            Cursors = cursors;
            AcceptKeyboardAndMouseInputs = acceptKeyboardAndMouseInputs;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the size of the desktop.
        /// </summary>
        /// <value>
        /// The total size of the desktop.
        /// </value>
        public Size DesktopSize { get; private set; }

        /// <summary>
        /// Gets the size of the clip.
        /// </summary>
        /// <value>
        /// The size of the clip.
        /// </value>
        public Size ClipSize { get; private set; }

        /// <summary>
        /// Gets the cursors.
        /// </summary>
        /// <value>
        /// The cursors.
        /// </value>
        public CursorInfo[] Cursors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [accept keyboard and mouse inputs].
        /// </summary>
        /// <value>
        /// <c>true</c> if [accept keyboard and mouse inputs]; otherwise, <c>false</c>.
        /// </value>
        public bool AcceptKeyboardAndMouseInputs { get; private set; }

        #endregion

    }

}
