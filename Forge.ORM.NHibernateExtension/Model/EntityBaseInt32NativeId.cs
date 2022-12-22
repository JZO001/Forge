/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Forge.Shared;
using NHibernate.Mapping.Attributes;

namespace Forge.ORM.NHibernateExtension.Model
{

    /// <summary>
    /// Represents an entity base with Int32 type identifier and native id generator
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("[{GetType()}, Id = '{Id}', Deleted = {Deleted}]")]
    public abstract class EntityBaseInt32NativeId : EntityBaseGenericId<int>
    {

        #region Field(s)

        /// <summary>
        /// The identifier
        /// </summary>
        [Id(0, Name = "id", Column = "id", TypeType = typeof(int), UnsavedValue = "-1")]
        [Generator(1, Class = "native")]
        private int id = -1;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBaseInt32NativeId"/> class.
        /// </summary>
        protected EntityBaseInt32NativeId()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        [DebuggerHidden]
        public override int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != -1 && IsSaved)
                {
                    ThrowHelper.ThrowArgumentException("Unable to replace identifier of an existing entity.", "value");
                }

                OnPropertyChanging("Id");
                id = value;
                IsSaved = false; // ha beállítok id-t, az azt jelenti, hogy most lesz először mentve
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is saved; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        public override bool IsSaved
        {
            get { return Id != -1 && base.IsSaved; }
            set { base.IsSaved = value; }
        }

        #endregion

    }

}
