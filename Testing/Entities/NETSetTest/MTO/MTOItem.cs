using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.NETSetTest.MTO
{

    // ManyToOne: several items associated to the list
    [Serializable]
    [Class]
    public class MTOItem : EntityBase
    {

        [Property]
        private string name;

        [ManyToOne(0, Name = "mtoList", ClassType = typeof(MTOList), Cascade = "none")]
        [Column(1, Name = "mtoList_restId")]
        [Column(2, Name = "mtoList_deviceId")]
        [Column(3, Name = "mtoList_id")]
        private MTOList mtoList = null;

        public MTOItem()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual MTOList MTOList
        {
            get { return mtoList; }
            set { mtoList = value; }
        }

    }

}
