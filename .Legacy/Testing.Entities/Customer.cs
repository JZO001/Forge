using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Class(EntityName = "TestCustomer", Name = "Forge.Testing.Entities.Customer, Forge.Testing.Entities")]
    [Serializable]
    public class Customer : EntityBase
    {

        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private string name = String.Empty;

        public Customer()
            : base()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
