/* *********************************************************************
 * Date: 21 Nov 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Forge.Native.Structures
{

    /// <summary>
    /// System time structure to get or set system date
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms724950(v=vs.85).aspx
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {

        /// <summary>
        /// The year
        /// </summary>
        public ushort Year;

        /// <summary>
        /// The month
        /// </summary>
        public ushort Month;

        /// <summary>
        /// The day of week
        /// </summary>
        public ushort DayOfWeek;

        /// <summary>
        /// The day
        /// </summary>
        public ushort Day;

        /// <summary>
        /// The hour
        /// </summary>
        public ushort Hour;

        /// <summary>
        /// The minute
        /// </summary>
        public ushort Minute;

        /// <summary>
        /// The second
        /// </summary>
        public ushort Second;

        /// <summary>
        /// The milliseconds
        /// </summary>
        public ushort Milliseconds;

    }

}
