using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class(EntityName = "TestCustomerEntity", Name = "Forge.Testing.Entities.CustomerEntity, Forge.Testing.Entities")]
    public class CustomerEntity : EntityBase
    {
        #region Fields

        
        [Property(NotNull = true, Type = "Serializable", Length = 10000000)]
        private string name = String.Empty;

        [Property(NotNull = true)]
        private byte data = byte.MinValue;

        [Property(NotNull = false)]
        private byte[] dataArray = null;

        [Property(NotNull = false)]
        private int number = 0;

        //Ennek Flatot hoz létre
        //[Property(NotNull = false)]
        //private double doubleNumber = 0.0;

        [Property(NotNull = false)]
        private float floatNumber = 0;

        [Property(NotNull = false)]
        private decimal decimalNumber = new decimal();

        [Property(NotNull = false)]
        private long longNumber = 0;


        #endregion


        #region Constructor

        public CustomerEntity()
            : base()
        {

        }

        #endregion

        #region Properties

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual byte Data
        {
            get { return data; }
            set { data = value; }
        }

        public virtual byte[] DataArray
        {
            get { return dataArray; }
            set { dataArray = value; }
        }

        public virtual int Number
        {
            get { return number; }
            set { number = value; }
        }

        //public virtual double DoubleNumber
        //{
        //    get { return doubleNumber; }
        //    set { doubleNumber = value; }
        //}

        public virtual float FloatNumber
        {
            get { return floatNumber; }
            set { floatNumber = value; }
        }

        public virtual decimal DecimalNumber
        {
            get { return decimalNumber; }
            set { decimalNumber = value; }
        }

        public virtual long LongNumber
        {
            get { return longNumber; }
            set { longNumber = value; }
        }

        #endregion

    }
}
