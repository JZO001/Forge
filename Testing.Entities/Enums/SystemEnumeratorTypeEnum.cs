using System;

namespace Forge.Testing.Entities.Enums
{

    /// <summary>
    /// Represents the system manager database enumerators which are expandable runtime
    /// </summary>
    [Serializable]
    public enum SystemEnumeratorTypeEnum
    {

        /// <summary>
        /// System enumerator if not definied. Do not choose this item. This is just a default item.
        /// </summary>
        NotDefinied = 0,

        /// <summary>
        /// Fogyasztási típusok
        /// </summary>
        ConsumerTypeModes = 1,

        /// <summary>
        /// Mértékegységek
        /// </summary>
        Measures = 2,

        /// <summary>
        /// Fizetési módok
        /// </summary>
        PaymentModes = 3

    }

}
