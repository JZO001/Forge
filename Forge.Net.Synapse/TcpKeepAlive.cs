/* *********************************************************************
 * Date: 07 Jun 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if IS_WINDOWS

using System;

namespace Forge.Net.Synapse
{

    /// <summary>
    /// Represents the struct which helps to set keep alive timers on a socket
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    struct TcpKeepAlive
    {

        [System.Runtime.InteropServices.FieldOffset(0)]
        private unsafe fixed byte Bytes[12];

        [System.Runtime.InteropServices.FieldOffset(0)]
        public uint State;

        [System.Runtime.InteropServices.FieldOffset(4)]
        public uint KeepAliveTime;

        [System.Runtime.InteropServices.FieldOffset(8)]
        public uint KeepAliveInterval;

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        internal byte[] ToArray()
        {
            unsafe
            {
                fixed (byte* ptr = Bytes)
                {
                    IntPtr p = new IntPtr(ptr);
                    byte[] BytesArray = new byte[12];

                    System.Runtime.InteropServices.Marshal.Copy(p, BytesArray, 0, BytesArray.Length);
                    return BytesArray;
                }
            }
        }

    }

}

#endif
