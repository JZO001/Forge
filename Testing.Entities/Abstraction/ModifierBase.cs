using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.Abstraction
{

    [Serializable]
    public abstract class ModifierBase : EntityBase
    {

        [Property(NotNull = false)]
        private string name = String.Empty;

        [Property(NotNull = true)]
        private ModifierEnum modifierType = ModifierEnum.Undefinied;

        [Property]
        private short shortValueTestInModifierBase = 0;

        [Property]
        private int intValueTestInModifierBase = 0;

        [Property]
        private long longValueTestInModifierBase = 0;

        [Property]
        private float floatValueTestInModiferBase = 0;

        [Property]
        private double doubleValueTestInModifierBase = 0;

        [Property]
        private decimal decimalValueTestInModifierBase = 0;

        protected ModifierBase() : base()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual ModifierEnum ModifierType
        {
            get { return modifierType; }
            set { modifierType = value; }
        }

        public virtual short ShortValueTestInModifierBase
        {
            get { return shortValueTestInModifierBase; }
            set { shortValueTestInModifierBase = value; }
        }

        public virtual int IntValueTestInModifierBase
        {
            get { return intValueTestInModifierBase; }
            set { intValueTestInModifierBase = value; }
        }

        public virtual long LongValueTestInModifierBase
        {
            get { return longValueTestInModifierBase; }
            set { longValueTestInModifierBase = value; }
        }

        public virtual float FloatValueTestInModiferBase
        {
            get { return floatValueTestInModiferBase; }
            set { floatValueTestInModiferBase = value; }
        }

        public virtual double DoubleValueTestInModifierBase
        {
            get { return doubleValueTestInModifierBase; }
            set { doubleValueTestInModifierBase = value; }
        }

        public virtual decimal DecimalValueTestInModifierBase
        {
            get { return decimalValueTestInModifierBase; }
            set { decimalValueTestInModifierBase = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}, Name: {1}", base.ToString(), name);
        }

    }

}
