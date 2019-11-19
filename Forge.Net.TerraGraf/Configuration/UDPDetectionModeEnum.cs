using System;

namespace Forge.Net.TerraGraf.Configuration
{

    /// <summary>
    /// UDP Detection mode
    /// </summary>
    [Serializable]
    public enum UDPDetectionModeEnum
    {
        /// <summary>
        /// IPv4 broadcast
        /// </summary>
        Broadcast = 0,

        /// <summary>
        /// IPv4/IPv6 multicast
        /// </summary>
        Multicast
    }

}
