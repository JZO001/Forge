using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.MTM_Uni
{

    [Serializable]
    [Class(Abstract = true)]
    public abstract class GameBase : EntityBase
    {

        [Set(0, Name = "players", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "Game_Player_Switch")]
        [Key(1)]
        [Column(2, Name = "game_restId")]
        [Column(3, Name = "game_deviceId")]
        [Column(4, Name = "game_id")]
        //[Index(5)]
        //[Column(6, Name = "cars_restId")]
        //[Column(7, Name = "cars_deviceId")]
        //[Column(8, Name = "cars_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(Player))]
        [Column(6, Name = "player_restId")]
        [Column(7, Name = "player_deviceId")]
        [Column(8, Name = "player_id")]
        private ISet<Player> players = new HashSet<Player>();

        protected GameBase() : base()
        {
        }

        public virtual ISet<Player> Players
        {
            get { return players; }
            set { players = value; }
        }

    }

}
