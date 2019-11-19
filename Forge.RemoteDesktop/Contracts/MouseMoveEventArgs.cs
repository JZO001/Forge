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
    /// Represents a mouse move event on the client side
    /// </summary>
    [Serializable]
    public class MouseMoveEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMoveEventArgs"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public MouseMoveEventArgs(Point position)
        {
            if (position == null)
            {
                ThrowHelper.ThrowArgumentNullException("position");
            }
            this.Position = position;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Point Position { get; private set; }

    }

}
