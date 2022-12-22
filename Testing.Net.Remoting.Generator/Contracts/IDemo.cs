using Forge.Net.Remoting;

namespace Testing.Net.Remoting.Generator.Contracts
{

    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IDemo : IRemoteContract
    {

        [OperationContract(Direction = OperationDirectionEnum.ClientSide)]
        void SendMessageToClient(string message);

        [OperationContract]
        void SendMessageToService(string message);

    }

}
