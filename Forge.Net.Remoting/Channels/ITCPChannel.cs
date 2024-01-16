namespace Forge.Net.Remoting.Channels
{

    /// <summary>Members of a TCP channel</summary>
    public interface ITCPChannel : IChannel
    {

        /// <summary>Gets or sets the temporary stream storage folder.</summary>
        /// <value>The temporary stream storage folder.</value>
        string TempStreamStorageFolder { get; set; }

        /// <summary>Gets or sets the maximum size of the send message.</summary>
        /// <value>The maximum size of the send message.</value>
        int MaxSendMessageSize { get; set; }

        /// <summary>Gets or sets the maximum size of the send stream.</summary>
        /// <value>The maximum size of the send stream.</value>
        long MaxSendStreamSize { get; set; }

        /// <summary>Gets or sets the maximum size of the receive message.</summary>
        /// <value>The maximum size of the receive message.</value>
        int MaxReceiveMessageSize { get; set; }

        /// <summary>Gets or sets the maximum size of the receive stream.</summary>
        /// <value>The maximum size of the receive stream.</value>
        long MaxReceiveStreamSize { get; set; }

    }

}
