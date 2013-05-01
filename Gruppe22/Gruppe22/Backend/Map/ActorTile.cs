using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        Actor _actor;
        public Actor actor
        {
            get { return _actor; }
        }
        public ActorType actorType
        {
            get
            {
                return _actor.actorType;
            }
        }
    }
}
