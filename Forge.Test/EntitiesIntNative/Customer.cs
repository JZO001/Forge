using System;
using Forge.ORM.NHibernateExtension.Model;
using NHibernate.Mapping.Attributes;

namespace Forge.Test.EntitiesIntNative
{

    /// <summary>
    /// Customer
    /// </summary>
    [Serializable]
    [Class]
    public class Customer : EntityBaseInt32NativeId
    {

        [Property]
        private string name = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        public Customer()
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name
        {
            get { return name; }
            set
            {
                OnPropertyChanging("name");
                name = value;
                OnPropertyChanged("name");
            }
        }

    }

}
