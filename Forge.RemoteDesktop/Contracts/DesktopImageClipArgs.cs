/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Drawing;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents an image clip data and its position on the screen
    /// </summary>
    [Serializable]
    public class DesktopImageClipArgs
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopImageClipArgs"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="imageData">The image data.</param>
        public DesktopImageClipArgs(Point location, byte[] imageData)
        {
            this.Location = location;
            this.ImageData = imageData;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets the image data.
        /// </summary>
        /// <value>
        /// The image data.
        /// </value>
        public byte[] ImageData { get; private set; }

        #endregion

    }

}
