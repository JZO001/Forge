/* *********************************************************************
 * Date: 11 Jun 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.Persistence.Serialization
{

    /// <summary>
    /// Behavior of the binary serializer
    /// </summary>
    [Serializable]
    public enum BinarySerializerBehaviorEnum
    {

        /// <summary>
        /// The throw exception on missing field
        /// </summary>
        ThrowExceptionOnMissingField = 0,

        /// <summary>
        /// The do not throw exception on missing field
        /// </summary>
        DoNotThrowExceptionOnMissingField = 1

    }

}
