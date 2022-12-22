using System;
using Forge.Net.Remoting;

namespace Testing.Net.Remoting.Generator.Contracts
{

    /// <summary>
    /// Simple test
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface ITestContractSimple : IRemoteContract, ICloneable, IEquatable<ITestContractSimple>
    {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetName();

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="name">The name.</param>
        [OperationContract]
        void SetName(string name);

        /// <summary>
        /// Gets the age.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int GetAge();

        /// <summary>
        /// Sets the age.
        /// </summary>
        /// <param name="age">The age.</param>
        [OperationContract(IsOneWay = true)]
        void SetAge(int age);

    }

}
