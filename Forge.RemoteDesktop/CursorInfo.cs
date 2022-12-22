/* *********************************************************************
 * Date: 07 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Windows.Forms;

namespace Forge.RemoteDesktop
{

    /// <summary>
    /// Represents a cursor information
    /// </summary>
    [Serializable]
    public class CursorInfo
    {

        #region Constructor(s)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CursorInfo"/> class.
        /// </summary>
        /// <param name="cursorId">The cursor id.</param>
        /// <param name="cursor">The cursor.</param>
        public CursorInfo(string cursorId, Cursor cursor)
        {
            if (string.IsNullOrEmpty("cursorId"))
            {
                ThrowHelper.ThrowArgumentNullException("cursorId");
            }
            if (cursor == null)
            {
                ThrowHelper.ThrowArgumentNullException("cursor");
            }

            CursorId = cursorId;
            Cursor = cursor;
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Gets the cursor id.
        /// </summary>
        /// <value>
        /// The cursor id.
        /// </value>
        public string CursorId { get; private set; }

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <value>
        /// The cursor.
        /// </value>
        public Cursor Cursor { get; private set; }

        #endregion

    }

}
