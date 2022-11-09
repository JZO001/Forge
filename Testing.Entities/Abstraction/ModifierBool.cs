using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities.Abstraction
{

    [Class]
    [Serializable]
    public class ModifierBool : ModifierBase
    {

        [Property]
        private bool enabled = false;

        public ModifierBool() : base()
        {
        }

        public virtual bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

    }

}
