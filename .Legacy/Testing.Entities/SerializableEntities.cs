using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class(EntityName = "TestSerializableEntities", Name = "Forge.Testing.Entities.SerializableEntities, Forge.Testing.Entities")]
    public class SerializableEntities : EntityBase
    {

        #region Fields

        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private string name = String.Empty;

        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private uint uintNumber = 0;

        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private double doubleNumber = 0.0;

        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private ulong ulongNumber = 0;


        #endregion

        #region Constructor

        public SerializableEntities(): base()
        {

        }

        #endregion

        #region Properties

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual uint UintNumber
        {
            get { return uintNumber; }
            set { uintNumber = value; }
        }

        public virtual double DoubleNumber
        {
            get { return doubleNumber; }
            set { doubleNumber = value; }
        }

        public virtual ulong UlongNumber
        {
            get { return ulongNumber; }
            set { ulongNumber = value; }
        }


        #endregion

    }
}
