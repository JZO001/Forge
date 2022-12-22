/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using Forge.Shared;
using NHibernate.Criterion;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Represents an association. This class used by the infrastructure.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{GetType()}, Key = {Key}, Association = {Association}, Alias = {Alias}]")]
    public class AssociationEntry
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationEntry" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="association">The association.</param>
        /// <param name="alias">The alias.</param>
        public AssociationEntry(string key, string association, string alias)
        {
            if (string.IsNullOrEmpty(key))
            {
                ThrowHelper.ThrowArgumentNullException("key");
            }
            if (string.IsNullOrEmpty(association))
            {
                ThrowHelper.ThrowArgumentNullException("association");
            }
            if (string.IsNullOrEmpty(alias))
            {
                ThrowHelper.ThrowArgumentNullException("alias");
            }

            Key = key;
            Association = association;
            Alias = alias;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the key. For example: enumeratorType.guest
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the association. For example: t.guest
        /// </summary>
        /// <value>
        /// The association.
        /// </value>
        public string Association { get; private set; }

        /// <summary>
        /// Gets the alias. For example: g
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets or sets the criteria.
        /// </summary>
        /// <value>
        /// The criteria.
        /// </value>
        public DetachedCriteria Criteria { get; set; }

        #endregion

    }

}
