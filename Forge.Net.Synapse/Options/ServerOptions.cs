namespace Forge.Net.Synapse.Options
{

#if NETCOREAPP3_1_OR_GREATER
#nullable enable
#endif

    /// <summary>Represents a TCP server startup configuration</summary>
    public class ServerOptions
    {

        /// <summary>Gets or sets the host IP which is the IP of the interface where the TCP server will listen. Leave it empty or null to listen on all available interfaces.</summary>
        /// <value>The host ip.</value>
        public string
#if NETCOREAPP3_1_OR_GREATER
            ? 
#endif
            HostIP { get; set; }

        /// <summary>Gets or sets the port where the TCP server will listen. The range is the allowed port range 0-65535.</summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>Gets or sets the backlog.
        /// This value controls the maximum number of pending incoming connections in the queue. Leave it null or empty to use the default value (int.MaxValue).</summary>
        /// <value>The backlog.</value>
        public int? Backlog { get; set; }

    }

#if NETCOREAPP3_1_OR_GREATER
#nullable disable
#endif

}
