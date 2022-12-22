using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.OTO
{

    [Serializable]
    [Class]
    public class Boy : EntityBase
    {

        [Property]
        private string name;

        [OneToOne(Name = "girl", ClassType = typeof(Girl), Cascade = "none", Constrained = false, PropertyRef = "boy")]
        private Girl girl = null;

        public Boy()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual Girl Girl
        {
            get { return girl; }
            set { girl = value; }
        }

    }

}
