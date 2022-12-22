using Forge.Net.Remoting;

namespace Testing.Net.Remoting.Generator.Contracts
{

    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface ITestNullable : IRemoteContract
    {

        [OperationContract]
        bool GetValue(bool? isCheck);

        [OperationContract(Direction = OperationDirectionEnum.ClientSide)]
        bool SendValue(bool? isCheck);

    }

}
