using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        #region Private Fields
        private Actor _actor;
        private int _elapsed = 0;
        private int _timeToThink = 50;
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

        public async Task WorkoutMoves()
        {
            if (_elapsed > _timeToThink)
            {
                _elapsed -= _timeToThink;
                Map map = (Map)((FloorTile)_parent).parent;


                Direction dir = Direction.None;
                Coords closestEnemy = map.ClosestEnemy(coords, 6);

                if (closestEnemy.x > -1) // There is an enemy close by
                {

                    if ((Math.Abs(closestEnemy.x - coords.x) < 2) &&
                        (Math.Abs(closestEnemy.y - coords.y) < 2) &&
                        (map.CanMove(coords, Map.WhichWayIs(closestEnemy, coords))))
                    {
                        dir = Map.WhichWayIs(closestEnemy, coords);
                        //System.Diagnostics.Debug.WriteLine("Attack -> " + coords.x + "/" + coords.y + "->" + closestEnemy.x + "/" + closestEnemy.y + "=>" + dir);

                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("==============================");
                        //System.Diagnostics.Debug.WriteLine(actor.ToString() + " (" + coords.ToString() + ") tries to catch " + map[closestEnemy].firstActor.ToString() + " (" + closestEnemy.ToString() + ")");
                        //System.Diagnostics.Debug.WriteLine("------------------------------");

                        List<Coords> path = null;
                        SortedSet<Coords> temp = null;

                        map.PathTo(coords, closestEnemy, out path, ref temp, 10);
                        if ((path != null) && (path.Count > 0))
                        {
                            dir = Map.WhichWayIs(path[1], coords);
                            // System.Diagnostics.Debug.WriteLine("=> Path found:" + path[1] + " (" + dir + ")");
                        }
                    }

                    if (actor.health < actor.maxHealth / 4)
                    {
                        // Low health => try to flee
                        //System.Diagnostics.Debug.WriteLine("=> Flee!");

                        dir = Map.OppositeDirection(dir);
                    }
                    int count = 1;

                    while ((!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter) && (count < 9))
                    {
                        //System.Diagnostics.Debug.WriteLine("Rotate");
                        dir = Map.NextDirection(dir);
                        count += 1;
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

                if (dir == Direction.None)
                {
                    dir = (Direction)_random.Next(4);
                }


                if (dir != Direction.None)
                {
                    ((IHandleEvent)parent).HandleEvent(false, Events.MoveActor, actor.id, dir);
                    //System.Diagnostics.Debug.WriteLine("#####" + dir + "######");
                }
            }
        }

        public async Task ConsiderMoves()
        {
            _working = true;
            await WorkoutMoves();
            _working = false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _elapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (enabled 
                && !(actor is Player) 
                && (!_working))
            {
                if (actor.isDead)
                {
                    enabled = false;
                }
                else
                {
                    ConsiderMoves();

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
