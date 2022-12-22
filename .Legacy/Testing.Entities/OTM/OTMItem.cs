using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.OTM
{

    [Serializable]
    [Class]
    public class OTMItem : EntityBase
    {

        [ManyToOne(0, Name = "otmList", Cascade = "none")]
        [Column(1, Name = "otmList_restId")]
        [Column(2, Name = "otmList_deviceId")]
        [Column(3, Name = "otmList_id")]
        private OTMList otmList = null;

        [Property]
        private string name = string.Empty;

        public OTMItem()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual OTMList OTMList
        {
            get { return otmList; }
            set { otmList = value; }
        }

    }

}
