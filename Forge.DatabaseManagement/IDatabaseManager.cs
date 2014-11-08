/* *********************************************************************
 * Date: 05 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using Forge.Configuration;

namespace Forge.DatabaseManagement
{

    /// <summary>
    /// Represents a database manager which ensure the underlying database integrity is valid.
    /// For example a file based database just like SQL Server Compact or SQLite file is not corrupted.
    /// If invalid database file detected, try to repair it or create a new one.
    /// </summary>
    public interface IDatabaseManager : IInitializable, IDisposable
    {

        /// <summary>
        /// Ensures the database integrity.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="mode">The mode.</param>
        void EnsureDatabaseIntegrity(long systemId, Dictionary<string, string> descriptor, SchemaFactoryModeEnum mode);

    }
}
