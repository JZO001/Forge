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
    /// Represents a cursor move event argument on the service side
    /// </summary>
    [Serializable]
    public class MouseMoveServiceEventArgs : MouseMoveEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMoveServiceEventArgs"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="cursorId">The cursor id.</param>
        public MouseMoveServiceEventArgs(Point position, string cursorId)
            : base(position)
        {
            if (cursorId == null)
            {
                ThrowHelper.ThrowArgumentNullException("cursorId");
            }
            this.CursorId = cursorId;
        }

        /// <summary>
        /// Gets the cursor id.
        /// </summary>
        /// <value>
        /// The cursor id.
        /// </value>
        public string CursorId { get; private set; }

    }

}
