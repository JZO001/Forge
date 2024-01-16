using System.Collections.Generic;

namespace Forge.Net.Synapse.Options
{

    /// <summary>Options for Synapse</summary>
    public class SynapseOptions
    {

        /// <summary>Gets the server stream factory options.</summary>
        /// <value>The server stream factory options.</value>
        public StreamFactoryOptions ServerStreamFactoryOptions { get; private set; } = new StreamFactoryOptions();

        /// <summary>Gets the client stream factory options.</summary>
        /// <value>The client stream factory options.</value>
        public StreamFactoryOptions ClientStreamFactoryOptions { get; private set; } = new StreamFactoryOptions();

        /// <summary>Gets the SSL server stream factory options.</summary>
        /// <value>The SSL server stream factory options.</value>
        public SslServerStreamFactoryOptions SslServerStreamFactoryOptions { get; private set; } = new SslServerStreamFactoryOptions();

        /// <summary>Gets the SSL client stream factory options.</summary>
        /// <value>The SSL client stream factory options.</value>
        public SslClientStreamFactoryOptions SslClientStreamFactoryOptions { get; private set; } = new SslClientStreamFactoryOptions();

        /// <summary>Gets the servers' configurations which will be started.</summary>
        /// <value>The servers.</value>
        public List<ServerOptions> Servers { get; private set; } = new List<ServerOptions>();

    }

}
