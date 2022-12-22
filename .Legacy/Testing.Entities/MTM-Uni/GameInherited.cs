using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities.MTM_Uni
{

    [Serializable]
    [UnionSubclass(Table = "GameInheriteds", ExtendsType = typeof(Game))]
    public class GameInherited : Game
    {

        public GameInherited()
        {
        }

    }

}
