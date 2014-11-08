/* *********************************************************************
 * Date: 17 Aug 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Data;

namespace Forge.DatabaseManagement.SqlServer2008
{

    /// <summary>
    /// Represents a custom MS SQL Server 2008 NHibernate dialect, which fixes the double bug
    /// </summary>
    public class MsSql2008Dialect : NHibernate.Dialect.MsSql2008Dialect
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSql2008Dialect"/> class.
        /// </summary>
        public MsSql2008Dialect()
            : base()
        {
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Registers the character type mappings.
        /// </summary>
        protected override void RegisterCharacterTypeMappings()
        {
            base.RegisterCharacterTypeMappings();
            base.RegisterColumnType(DbType.String, "NVARCHAR(MAX)");
            base.RegisterColumnType(DbType.AnsiString, "VARCHAR(MAX)");
        }

        /// <summary>
        /// Registers the numeric type mappings.
        /// </summary>
        protected override void RegisterNumericTypeMappings()
        {
            base.RegisterNumericTypeMappings();
            base.RegisterColumnType(DbType.Double, "FLOAT"); // DOUBLE PRECISION changed to FLOAT
        }

        #endregion

    }

}
