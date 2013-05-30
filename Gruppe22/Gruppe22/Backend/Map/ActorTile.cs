using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        #region Private Fields
        private Actor _actor;
        private int _elapsed = 0;
        private int _timeToThink = 0;
        private bool _disabled = false;
        private Direction _lastDir = Direction.None;
        private Random _random = null;
        private bool _working = false;
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

        public bool enabled
        {
            get
            {
                return !_disabled;
            }
            set
            {
                _disabled = !value;
            }
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ActorTile");
            actor.Save(xmlw);
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor
        public ActorTile(object parent, Actor actor)
            : this(parent)
        {
            _actor = actor;
        }

        public void DropItems()
        {
            while (actor.inventory.Count > 0)
            {
                ItemTile temp = new ItemTile(_parent, actor.inventory[0]);
                actor.inventory[0].tile = temp;
                ((FloorTile)_parent).Add(temp);
                actor.inventory.RemoveAt(0);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (enabled && !(actor is Player) && (!_working))
            {
                _working = true;
                _elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (_elapsed > _timeToThink)
                {
                    _elapsed -= _timeToThink;
                    Map map = (Map)((FloorTile)_parent).parent;
                    if (!actor.isDead)
                    {

                        Direction dir = Direction.None;
                        Coords closestEnemy = map.ClosestEnemy(coords);
                        if (closestEnemy.x > -1) // There is an enemy close by
                        {
                            if (actor.health < actor.maxHealth / 4)
                            {
                                // Low health => try to flee

                                dir = Map.OppositeDirection(Map.WhichWayIs(coords, map.PathTo(coords, closestEnemy, 5)[0]));
                                int count = 1;

                                while ((!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter) && (count < 9))
                                {
                                    dir = Map.NextDirection(dir);
                                    count += 1;
                                }
                            }
                            else
                            {
                                // High health => Follow opponent or attack
                                dir = Map.WhichWayIs(coords, map.PathTo(coords, closestEnemy, 5)[0]);
                            }
                            if (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter) dir = Direction.None;

                        }
                        else
                        {
                            // Nobody close by, just wander aimlessly
                            // TODO: Try to grab nearby items
                            dir = (Direction)_random.Next(4);
                            int count = 1;
                            while (((dir == Map.OppositeDirection(_lastDir)) || (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter)) && (count < 9))
                            {
                                dir = Map.NextDirection(dir);
                                count += 1;
                            }
                            if ((dir == Map.OppositeDirection(_lastDir)) || (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter)) dir = Direction.None;
                        }
                        if (dir != Direction.None)
                            ((IHandleEvent)parent).HandleEvent(null, Events.MoveActor, actor.id, dir);
                        _working = false;
                    }
                    else
                    {
                        enabled = false;
                    }
                }


            }
        }

        public ActorTile(object parent, Random r = null)
            : base(parent)
        {
            if (_random == null)
                _random = new Random();

        }
        #endregion

    }
}
