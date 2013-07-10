using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile on the map used to represent an actor.
    /// </summary>
    public class ActorTile : Tile
    {
        #region Private Fields
        private Actor _actor;
        /// <summary>
        /// Die verstrichene Zeit.
        /// </summary>
        private int _elapsed = 0;
        /// <summary>
        /// The number of clocks an actor waits before he can do the next action.
        /// </summary>
        private int _timeToThink = 50;
        /// <summary>
        /// Determines if the actor is disabled.
        /// </summary>
        private bool _disabled = false;
        /// <summary>
        /// The last direction the actor faced.
        /// </summary>
        private Direction _lastDir = Direction.None;
        private Random _random = null;
        /// <summary>
        /// Funktionsfähigkeit.
        /// </summary>
        private bool _working = false;
        #endregion

        #region Public Fields
        /// <summary>
        /// The actor connected with the actortile.
        /// </summary>
        public Actor actor
        {
            get 
            { 
                return _actor; 
            }
        }

        /// <summary>
        /// The type of the to the tile connected actor.
        /// </summary>
        public ActorType actorType
        {
            get
            {
                return _actor.actorType;
            }
        }

        /// <summary>
        /// Determines if the actor is activ and so do something.
        /// </summary>
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

        /// <summary>
        /// Method to save the actortile.
        /// Calls the save-method of the connected actor.
        /// </summary>
        /// <param name="xmlw">The used XMLwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            if (actor != null)
            {
                xmlw.WriteStartElement("ActorTile");
                actor.Save(xmlw);
                xmlw.WriteEndElement();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for the ActorTile
        /// Calls the constructor of Tile.
        /// </summary>
        /// <param name="parent">Just the parent object</param>
        /// <param name="actor">The to the tile connected actor.</param>
        public ActorTile(object parent, Actor actor)
            : this(parent)
        {
            _actor = actor;
        }

        /// <summary>
        /// Method to drop the full inventory of the connected player to the tile he stands on.
        /// </summary>
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

        /// <summary>
        /// Method to determine the next moves of a computer controlled actor.
        /// </summary>
        /// <returns>Ergebnis von asynchonen Task.</returns>
        public async Task WorkoutMoves()
        {
            bool canAttack = false;
            bool finished = false;
            Map map = (Map)((FloorTile)_parent).parent;

            if (_actor.stunned > 0) _actor.stunned -= 1;
            if (_actor.charmed > 0)
            {
                _actor.charmed -= 1;
                if (_actor.charmed == 0) _actor.friendly = false;
            }
            if (_actor.stunned == 0)
            {

                Direction dir = Direction.None;
                Backend.Coords closestEnemy = map.ClosestEnemy(coords, actor.viewRange, !(actor is NPC) && (actor.aggro) && (!actor.friendly), !(actor is NPC) && !(actor.aggro) && (!actor.friendly), (actor.friendly) || actor.crazy);

                if (closestEnemy.x > -1) // There is an enemy close by
                {

                    if ((Math.Abs(closestEnemy.x - coords.x) < 2) &&
                        (Math.Abs(closestEnemy.y - coords.y) < 2) &&
                        (map.CanMove(coords, Map.WhichWayIs(closestEnemy, coords))))
                    {
                        dir = Map.WhichWayIs(closestEnemy, coords);
                        //System.Diagnostics.Debug.WriteLine("Attack -> " + coords.x + "/" + coords.y + "->" + closestEnemy.x + "/" + closestEnemy.y + "=>" + dir);
                        canAttack = true;
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

                    if ((actor.health < actor.maxHealth / 4) || (actor.scared != 0))
                    {
                        // Low health => try to flee
                        //System.Diagnostics.Debug.WriteLine("=> Flee!");

                        if ((!canAttack) || ((actor.health > 10) && (_random.Next(100) > 50)))
                        {
                            dir = Map.OppositeDirection(dir);
                            // TODO: Versuche Pfade zu finden, die ABstand vergrößern (insb. wenn geblockt)
                            finished = true;
                        }
                    }
                    int count = 1;
                    if (!finished)
                    {
                        while ((!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter) && (count < 9))
                        {
                            //System.Diagnostics.Debug.WriteLine("Rotate");
                            dir = Map.NextDirection(dir);
                            count += 1;
                        }
                        if (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter) dir = Direction.None;
                    }
                }
                else

                    if (dir == Direction.None)
                    {
                        // Nobody close by, just wander aimlessly
                        // TODO: Try to grab nearby items
                        if (_random.Next(10) > 5)
                        {
                            dir = (Direction)_random.Next(4);
                            int count = 1;
                            while (((dir == Map.OppositeDirection(_lastDir)) || (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter)) && (count < 9))
                            {
                                dir = Map.NextDirection(dir);
                                count += 1;
                            }
                            if ((dir == Map.OppositeDirection(_lastDir)) || (!map.TileByCoords(Map.DirectionTile(coords, dir)).canEnter))
                            {
                                dir = Direction.None;
                                _lastDir = Direction.None;
                            }

                        }
                    }

                // Do not attack friendly units!
                if ((map.TileByCoords(Map.DirectionTile(coords, dir)).hasEnemy) && (!map.TileByCoords(Map.DirectionTile(coords, dir)).firstActor.isDead)
                    && (!actor.crazy) && (!actor.friendly))
                    dir = Direction.None;

                if ((map.TileByCoords(Map.DirectionTile(coords, dir)).hasNPC) && (!map.TileByCoords(Map.DirectionTile(coords, dir)).firstActor.isDead))
                    dir = Direction.None;


                if ((map.TileByCoords(Map.DirectionTile(coords, dir)).hasPlayer) && (!map.TileByCoords(Map.DirectionTile(coords, dir)).firstActor.isDead)
                    && ((!actor.aggro) || (actor.friendly)))
                    dir = Direction.None;

                if ((dir != Direction.None) && ((!map.TileByCoords(Map.DirectionTile(coords, dir)).hasTeleport) || (map.TileByCoords(Map.DirectionTile(coords, dir)).hasPlayer)))
                {
                    ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.MoveActor, actor.id, dir);
                    //System.Diagnostics.Debug.WriteLine("#####" + dir + "######");
                }
            }

        }
        /// <summary>
        /// Die in erwägung gezogene Bewegung.
        /// </summary>
        /// <returns>Ergenbnis.</returns>
        public async Task ConsiderMoves()
        {
            _working = true;
            await WorkoutMoves();
            _working = false;
        }

        /// <summary>
        /// the update-routine for the ActorTile.
        /// </summary>
        /// <param name="gameTime">The Gametime</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!actor.isDead)
            {
                _elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (_elapsed > _timeToThink)
                {
                    actor.Regen();
                    _elapsed -= _timeToThink;
                    if (enabled
                        && !(actor is Player)
                        && (!_working))
                    {
                        ConsiderMoves();
                    }
                }
            }
        }
        /// <summary>
        /// The constructor for the ActorTile.
        /// Calls the parent constructor.
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="r">A random needed for some methods</param>
        public ActorTile(object parent, Random r = null)
            : base(parent)
        {
            if (_random == null)
                _random = new Random();

        }
        #endregion

    }
}
