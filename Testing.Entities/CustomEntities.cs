using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    
    [Class]//(EntityName = "CustomEntities", Name = "Forge.Testing.Everlight.Entities.CustomEntities, Forge.Testing.Everlight.Entities")]
    [Serializable]
    public class CustomEntities : EntityBase
    {

        #region Fields


        [Property(NotNull = false)]
        private string name = null;

        [Property(NotNull = false)]
        private int age = 0;

        [Property(NotNull = false)]
        private byte byteData = 0;

        [Property(NotNull = false)]
        private byte[] binariData = null;

        [Property(NotNull = false)]
        private DateTime date = DateTime.Now;

        [Property(NotNull = false)]
        private float floatNumber = 0;

        [Property(NotNull = false, Type = "Serializable", Length = 10000000)]
        private float floatNumber2 = 0;

        [Set(0, Name = "customEntities2Set", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "CustomEntities_CustomEntities2_Switch", Inverse = true)]
        [Key(1)]
        [Column(2, Name = "customEntities_restId")]
        [Column(3, Name = "customEntities_deviceId")]
        [Column(4, Name = "customEntities_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(CustomEntities2))]
        [Column(6, Name = "customEntities2_restId")]
        [Column(7, Name = "customEntities2_deviceId")]
        [Column(8, Name = "customEntities2_id")]
        private ISet<CustomEntities2> customEntities2Set = new HashSet<CustomEntities2>();

        [ManyToOne(0, Name = "customEntities2", ClassType = typeof(CustomEntities2), Cascade = "none", Unique = true)]
        [Column(1, Name = "customEntities2_restId")]
        [Column(2, Name = "customEntities2_deviceId")]
        [Column(3, Name = "customEntities2_id")]
        private CustomEntities2 customEntities2 = null;

        #endregion

        #region Constructor

        public CustomEntities()
        {
        }

        #endregion

        #region Properties
        

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual int Age
        {
            get { return age; }
            set
            { age = value; }
        }

        public virtual byte ByteData
        {
            get { return byteData; }
            set { byteData = value; }
        }

        public virtual byte[] BinariData
        {
            get { return binariData; }
            set { binariData = value; }
        }

        public virtual DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public virtual float FloatNumber
        {
            get { return floatNumber;}
            set { floatNumber = value; }
        }

        public virtual float FloatNumber2
        {
            get { return floatNumber2; }
            set { floatNumber2 = value; }
        }

        public virtual ISet<CustomEntities2> CustomEntities2Set
        {
            get { return customEntities2Set; }
            set { customEntities2Set = value; }
        }

        public virtual CustomEntities2 CustomEntities2
        {
            get { return customEntities2; }
            set { customEntities2 = value; }
        }

        #endregion

    }
}
