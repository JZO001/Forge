using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities.MTM_Uni
{

    // ManyToMany, unidirectional. Only one entity contains an IList to hold the list of the other type of entities.
    [Serializable]
    [UnionSubclass(Table = "Game", ExtendsType = typeof(GameBase))]
    public class Game : GameBase
    {

        [Property]
        private string name = string.Empty;

        public Game() : base()
        {
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

    }

}
