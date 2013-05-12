using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public enum ActorType
    {
        Player = 0,
        NPC = 1,
        Enemy = 2
    }

    public class Actor
    {
        private ActorTile _tile;
        protected ActorType _actorType;

        public ActorTile tile
        {
            get { return _tile; }
            set { _tile = value; }
        }
        public ActorType actorType
        {
            get
            {
                return _actorType;
            }
        }
    }
}
