using System;
using Forge.ORM.NHibernateExtension.Model;
using NHibernate.Mapping.Attributes;

namespace Forge.Test.EntitiesIntNative
{

    [Serializable]
    [Class]
    public class Product : EntityBaseInt32NativeId
    {

        [Property]
        private string name = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product()
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
                OnPropertyChanging("Name");
                name = value;
                OnPropertyChanged("Name");
            }
        }

    }

}
