namespace Forge.Net.Remoting
{

    /// <summary>Defines the states in which an System.ServiceModel.ICommunicationObject can exist.</summary>
    public enum CommunicationStateEnum
    {
        /// <summary>Indicates that the communication object has been instantiated and is configurable, but not yet open or ready for use.</summary>
        Created,

        /// <summary>
        /// Indicates that the communication object is being transitioned from the Forge.Net.Remoting.CommunicationStateEnum.Created state to the Forge.Net.Remoting.CommunicationStateEnum.Opened state.
        /// </summary>
        Opening,

        /// <summary>Indicates that the communication object is now open and ready to be used.</summary>
        Opened,

        /// <summary>Indicates that the communication object is transitioning to the Forge.Net.Remoting.CommunicationStateEnum.Closed state.</summary>
        Closing,

        /// <summary>Indicates that the communication object has been closed and is no longer usable.</summary>
        Closed,

        /// <summary>Indicates that the communication object has encountered an error or fault from which it cannot recover and from which it is no longer usable.</summary>
        Faulted
    }
}
