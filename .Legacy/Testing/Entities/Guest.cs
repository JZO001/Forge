using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{

    [Serializable]
    [Class]
    public class Guest : EntityBase
    {

        [Property]
        private string name = String.Empty;

        public Guest() : base()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

    }

}
