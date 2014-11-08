/* *********************************************************************
 * Date: 02 May 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Persistence.StorageProviders.HibernateStorageProvider
{

    /// <summary>
    /// Allocation table
    /// </summary>
    [Serializable]
    internal sealed class ItemTable
    {

        #region Field(s)

        private List<EntityId> mItemIds = new List<EntityId>();

        private long itemUid = 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTable"/> class.
        /// </summary>
        internal ItemTable()
        {
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Gets the item ids.
        /// </summary>
        [DebuggerHidden]
        internal List<EntityId> ItemIds
        {
            get { return mItemIds; }
        }

        /// <summary>
        /// Gets or sets the item uid.
        /// </summary>
        /// <value>
        /// The item uid.
        /// </value>
        [DebuggerHidden]
        internal long ItemUid
        {
            get { return itemUid; }
            set { itemUid = value; }
        }

        #endregion

    }

}
