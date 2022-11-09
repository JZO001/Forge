using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{

    [Class]
    [Serializable]
    public class Product : EntityBase
    {

        [Property(NotNull = true)]
        private string name = String.Empty;

        [Property(NotNull = true)]
        private string category = String.Empty;

        [Property(NotNull = true)]
        private bool discountinued = false;

        [Property(NotNull = true, Precision = 19, Scale = 10)]
        private decimal price = 0;

        [Property(NotNull = false)]
        private string testField = String.Empty;

        public Product() : base()
        {
        }

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

        public virtual string Category
        {
            get { return category; }
            set { category = value; }
        }

        public virtual bool Discontinued
        {
            get { return discountinued; }
            set { discountinued = value; }
        }

        public virtual decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public virtual String TestField
        {
            get { return testField; }
            set { testField = value; }
        }

    }

}
