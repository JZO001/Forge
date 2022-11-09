using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    
    [Class]//(EntityName = "CustomEntities2", Name = "Forge.Testing.Everlight.Entities.CustomEntities2, Forge.Testing.Everlight.Entities")]
    [Serializable]
    public class CustomEntities2 : EntityBase
    {

        #region Fields

        [Property(NotNull = true)]
        private string name = String.Empty;


        [Set(0, Name = "customEntitiesSet", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "CustomEntities_CustomEntities2_Switch")]
        [Key(1)]
        [Column(2, Name = "customEntities2_restId")]
        [Column(3, Name = "customEntities2_deviceId")]
        [Column(4, Name = "customEntities2_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(CustomEntities))]
        [Column(6, Name = "customEntities_restId")]
        [Column(7, Name = "customEntities_deviceId")]
        [Column(8, Name = "customEntities_id")]
        private ISet<CustomEntities> customEntitiesSet = new HashSet<CustomEntities>();

        [OneToOne(Name = "customEntities", ClassType = typeof(CustomEntities), Cascade = "none", Constrained = false, PropertyRef = "customEntities2")]
        private CustomEntities customEntities = null;

        #endregion
        
        #region Constructor

        public CustomEntities2()
        {

        }

        #endregion
        
        #region Properties

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual ISet<CustomEntities> CustomEntitiesSet
        {
            get { return customEntitiesSet; }
            set { customEntitiesSet = value; }
        }


        public virtual CustomEntities CustomEntities
        {
            get { return customEntities; }
            set { customEntities = value; }
        }

        #endregion

    }
}
