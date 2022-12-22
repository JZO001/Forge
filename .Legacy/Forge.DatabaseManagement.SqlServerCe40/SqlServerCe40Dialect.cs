/* *********************************************************************
 * Date: 06 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Data;
using NHibernate.Dialect;

namespace Forge.DatabaseManagement.SqlServerCe40
{

    /// <summary>
    /// Represents a specialized NHibernate SQL Server CE 4.0 dialect
    /// </summary>
    public class SqlServerCe40Dialect : MsSqlCe40Dialect
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCe40Dialect"/> class.
        /// </summary>
        public SqlServerCe40Dialect()
            : base()
        {
            RegisterColumnType(DbType.Binary, 10000000, "IMAGE");
            RegisterColumnType(DbType.AnsiString, "NVARCHAR(4000)");
            RegisterColumnType(DbType.String, "NVARCHAR(4000)");
        }

    }

}
