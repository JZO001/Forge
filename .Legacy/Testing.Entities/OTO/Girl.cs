using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.OTO
{

    [Serializable]
    [Class]
    public class Girl : EntityBase
    {

        [Property]
        private string name;

        [ManyToOne(0, Name = "boy", ClassType = typeof(Boy), Cascade = "none", Unique = true)]
        [Column(1, Name = "boy_restId")]
        [Column(2, Name = "boy_deviceId")]
        [Column(3, Name = "boy_id")]
        private Boy boy = null;

        public Girl()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual Boy Boy
        {
            get { return boy; }
            set { boy = value; }
        }

    }

}
