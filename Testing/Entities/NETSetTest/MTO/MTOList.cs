using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.NETSetTest.MTO
{

    // OneToMany: several items associated to this instance, but there is no IList field exist
    [Serializable]
    [Class]
    public class MTOList : EntityBase
    {

        [Property]
        private string name = String.Empty;

        public MTOList()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

    }

}
