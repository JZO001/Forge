/* *********************************************************************
 * Date: 10 Sep 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Forge.Native.Structures
{

    /// <summary>
    /// Represents the cursor information structure
    /// http://www.pinvoke.net/default.aspx/user32.getcursorinfo
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {

        /// <summary>
        /// Specifies the size, in bytes, of the structure.
        /// </summary>
        public Int32 cbSize;


        /// <summary>
        /// The caller must set this to Marshal.SizeOf(typeof(CursorInfo)).
        /// 
        /// Specifies the cursor state. This parameter can be one of the following values:
        /// 0 - The cursor is hidden.
        /// 1 - The cursor is showing.
        /// </summary>
        public Int32 flags;

        /// <summary>
        /// Handle to the cursor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr hCursor;

        /// <summary>
        /// A Point structure that receives the screen coordinates of the cursor.
        /// </summary>
        public Point ptScreenPos;

    }

}
