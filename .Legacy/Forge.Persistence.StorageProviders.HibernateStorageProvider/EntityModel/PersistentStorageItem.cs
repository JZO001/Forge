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
    /// Store serialized item data
    /// </summary>
    [Serializable]
    [Class(Table = "PersistentStorageItems")]
    public class PersistentStorageItem : EntityBase
    {

        #region Field(s)

        [EntityFieldDescription("This field stores an object in serialized blob format which size is maximum 10.000.000 bytes.")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Property(Type = "BinaryBlob", Length = 10000000, NotNull = false)]
        //[Property(Type = "Serializable", Length = 1073741823, NotNull = false)]
        private byte[] entryData = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentStorageItem"/> class.
        /// </summary>
        public PersistentStorageItem()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the entry data.
        /// </summary>
        /// <value>
        /// The entity data.
        /// </value>
        public virtual byte[] EntryData
        {
            get { return entryData; }
            set
            {
                OnPropertyChanging("EntryData");
                entryData = value;
                OnPropertyChanged("EntryData");
            }
        }

        #endregion

    }

}
