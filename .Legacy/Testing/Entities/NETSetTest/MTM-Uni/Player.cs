using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.NETSetTest.MTM_Uni
{

    // ManyToMany, unidirectional. Only one entity contains an IList to hold the list of the other type of entities.
    [Serializable]
    [Class]
    public class Player : EntityBase
    {

        [Property]
        private string model = string.Empty;

        public Player()
        {
        }

        public virtual string Model
        {
            get { return model; }
            set { model = value; }
        }

    }

}
