using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        #region Private Fields
        Actor _actor;
        #endregion

        #region Public Fields
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
        #endregion

        #region Constructor
        public ActorTile(object parent, Actor actor)
            : this(parent)
        {
            _actor = actor;
        }

        public ActorTile()
            : this(null)
        {
        }

        public ActorTile(object parent)
            : base(parent, null, true)
        {
            _parent = parent;
        }
        #endregion

    }
}
