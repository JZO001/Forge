/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;

namespace Forge.RemoteDesktop
{

    /// <summary>
    /// Represents an area
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType()}, StartX = {StartX}, EndX = {EndX}, StartY = {StartY}, EndY = {EndY}]")]
    public sealed class Area
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Area"/> struct.
        /// </summary>
        /// <param name="startX">The start X.</param>
        /// <param name="startY">The start Y.</param>
        /// <param name="endX">The end X.</param>
        /// <param name="endY">The end Y.</param>
        public Area(int startX, int startY, int endX, int endY)
        {
            if (startX < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("startX");
            }
            if (startY < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("startY");
            }
            if (endX <= startX)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("endX");
            }
            if (endY <= startY)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("endY");
            }

            this.StartX = startX;
            this.StartY = startY;
            this.EndX = endX;
            this.EndY = endY;
        } 

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets or sets the start X.
        /// </summary>
        /// <value>
        /// The start X.
        /// </value>
        public int StartX { get; private set; }

        /// <summary>
        /// Gets or sets the start Y.
        /// </summary>
        /// <value>
        /// The start Y.
        /// </value>
        public int StartY { get; private set; }

        /// <summary>
        /// Gets or sets the end X.
        /// </summary>
        /// <value>
        /// The end X.
        /// </value>
        public int EndX { get; private set; }

        /// <summary>
        /// Gets or sets the end Y.
        /// </summary>
        /// <value>
        /// The end Y.
        /// </value>
        public int EndY { get; private set; }

        #endregion

    }

}
