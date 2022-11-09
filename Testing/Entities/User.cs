using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{

    [Class(EntityName = "TestUser", Name = "Forge.Testing.Entities.User, Forge.Testing")]
    [Serializable]
    public class User : EntityBase
    {

        [Property(NotNull = true)]
        private string name = String.Empty;

        public User() : base()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

    }

}
