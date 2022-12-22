/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.ORM.NHibernateExtension.Model;
using Forge.ORM.NHibernateExtension.Model.Distributed;
using NHibernate.Mapping.Attributes;

namespace Forge.Persistence.StorageProviders.HibernateStorageProvider.EntityModel
{

    /// <summary>
    /// Store allocation table serialized data
    /// </summary>
    [Serializable]
    [Class(Table = "PersistentStorageAllocationTables")]
    public class PersistentStorageAllocationTable : EntityBase
    {

        #region Field(s)

        [EntityFieldDescription("Stores the allocation table in serialized format which size is maximum 10.000.000 bytes.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(Type = "BinaryBlob", Length = 10000000, NotNull = true)]
        //[Property(Type = "Serializable", Length = 1073741823, NotNull = true)]
        private byte[] itemAllocationTableData = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStorageAllocationTable"/> class.
        /// </summary>
        public PersistentStorageAllocationTable()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the item allocation table data.
        /// </summary>
        /// <value>
        /// The item allocation table data.
        /// </value>
        public virtual byte[] ItemAllocationTableData
        {
            get { return itemAllocationTableData; }
            set
            {
                OnPropertyChanging("ItemAllocationTableData");
                itemAllocationTableData = value;
                OnPropertyChanged("ItemAllocationTableData");
            }
        }

        #endregion

    }

}
