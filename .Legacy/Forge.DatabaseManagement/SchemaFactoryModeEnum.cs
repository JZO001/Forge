/* *********************************************************************
 * Date: 19 Jul 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.DatabaseManagement
{

    /// <summary>
    /// Factory mode for a schema
    /// </summary>
    [Serializable]
    public enum SchemaFactoryModeEnum : int
    {

        /// <summary>
        /// Validate schema, but do not modify it
        /// </summary>
        Validate,

        /// <summary>
        /// Validate and update schema if necessary
        /// </summary>
        Update,

        /// <summary>
        /// Create new schema
        /// </summary>
        Create,

        /// <summary>
        /// Create new schema and drop it after the Session Factory closed
        /// </summary>
        Create_And_Drop

    }

}
