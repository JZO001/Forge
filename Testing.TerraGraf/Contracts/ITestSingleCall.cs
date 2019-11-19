using System;
using System.IO;
using Forge.Net.Remoting;

namespace Testing.TerraGraf.Contracts
{

    /// <summary>
    /// SingleCall test
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.SingleCall)]
    public interface ITestSingleCall : IRemoteContract, IDisposable
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetName();

        /// <summary>
        /// Says the hello.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void SayHello();

        /// <summary>
        /// Sends the image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        [OperationContract]
        void SendImage(Stream stream);

    }

}
