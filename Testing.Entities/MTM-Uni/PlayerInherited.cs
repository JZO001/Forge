using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities.MTM_Uni
{

    [Serializable]
    [UnionSubclass(Table = "PlayerInheriteds", ExtendsType = typeof(Player))]
    public class PlayerInherited : Player
    {

        public PlayerInherited()
        {
        }

    }

}
