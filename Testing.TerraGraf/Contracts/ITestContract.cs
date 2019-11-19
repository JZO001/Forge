using System;
using System.IO;
using Forge.Net.Remoting;

namespace Testing.TerraGraf.Contracts
{

    /// <summary>
    /// AllInOne test
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface ITestContract : ITestContractSimple, ITestContractStream, IRemoteContract, IDisposable
    {

        event EventHandler<EventArgs> EventTest1;

        event EventHandler EventTest2;

        string PropertyTest1 { get; }

        string PropertyTest2 { get; set; }

        string PropertyTest3 { set; }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(Direction = OperationDirectionEnum.ClientSide, IsOneWay = true)]
        void SendMessage(string message);

        /// <summary>
        /// Sends the non important message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void SendNonImportantMessage(string message);

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns></returns>
        [OperationContract(CallTimeout = 60000)]
        Stream GetImage();

        /// <summary>
        /// Sets the image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        [OperationContract]
        void SetImage(Stream stream);

        /// <summary>
        /// Does the nothing.
        /// </summary>
        void DoNothing();

    }

}
