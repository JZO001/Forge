using System.IO;
using Forge.Net.Remoting;

namespace Testing.Net.Remoting.Generator.Contracts
{

    /// <summary>
    /// Stream test with ClientSide effect
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface ITestContractStream : IRemoteContract
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
        [OperationContract(Direction = OperationDirectionEnum.ClientSide)]
        void SendImage(Stream stream);

    }

}
