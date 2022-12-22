/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents a mouse button event argument
    /// </summary>
    [Serializable]
    public class MouseButtonEventArgs
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseButtonEventArgs" /> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="button">The button.</param>
        /// <param name="point">The point.</param>
        public MouseButtonEventArgs(MouseButtonEventTypeEnum eventType, MouseButtons button, Point point)
        {
            EventType = eventType;
            Button = button;
            Point = point;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public MouseButtonEventTypeEnum EventType { get; private set; }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>
        /// The point.
        /// </value>
        public Point Point { get; private set;}

        #endregion

    }

}
