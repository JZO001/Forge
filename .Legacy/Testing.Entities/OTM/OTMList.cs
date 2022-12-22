using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.OTM
{

    [Serializable]
    [Class]
    public class OTMList : EntityBase
    {

        [Set(0, Name = "otmItems", Generic = true, Lazy = CollectionLazy.True, Cascade = "none", Inverse = true)]
        [Key(1)]
        [Column(2, Name = "otmList_restId")]
        [Column(3, Name = "otmList_deviceId")]
        [Column(4, Name = "otmList_id")]
        //[Index(2)]
        //[Column(6, Name = "restId")]
        //[Column(7, Name = "deviceId")]
        //[Column(8, Name = "id")]
        [OneToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(OTMItem))]
        private ISet<OTMItem> otmItems = new HashSet<OTMItem>();

        public OTMList()
        {
        }

        public virtual ISet<OTMItem> OTMItems
        {
            get { return otmItems; }
            set { otmItems = value; }
        }

    }

}
