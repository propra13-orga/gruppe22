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
            Direction selected = Direction.None;

            if (_actor.stunned > 0) _actor.stunned -= 1;
            if (_actor.charmed > 0)
            {
                _actor.charmed -= 1;
                if (_actor.charmed == 0) _actor.friendly = false;
            }

            if (_actor.stunned == 0)
            {
                HashSet<Direction> attackDir = new HashSet<Direction>();
                HashSet<Direction> validDir = new HashSet<Direction>();
                Map map = (Map)((FloorTile)_parent).parent;

                foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
                {
                    if (map.CanMove(coords, direction))
                    {
                        FloorTile tile = map[Map.DirectionTile(coords, direction)];
                        bool allow = true;
                        if (!(tile.hasTeleport))
                        {
                            if ((actor.friendly) || (!actor.aggro) || (actor.actorType == ActorType.NPC))
                            {
                                allow = allow && !tile.hasNPC && !tile.hasPlayer;
                            }
                            if ((!actor.crazy) && (!actor.friendly))
                            {
                                allow = allow && !tile.hasEnemy;
                            }
                        }
                        if ((actor.crazy) || (actor.friendly))
                        {
                            if (tile.hasEnemy) attackDir.Add(direction);
                        }
                        if ((actor.actorType == ActorType.Enemy)
                            && (!actor.friendly)
                            && (actor.aggro)
                            && (tile.hasPlayer))
                            attackDir.Add(direction);
                        if (allow) validDir.Add(direction);
                    }
                }
                if (validDir.Count > 0)
                {

                    if ((actor.health < actor.maxHealth / 4) || (actor.scared != 0))
                    {

                        if (attackDir.Count > 0)
                        {
                            // Flee if attacked
                            if (validDir.Contains(Map.OppositeDirection(attackDir.ToArray()[0])))
                            {
                                selected = Map.OppositeDirection(attackDir.ToArray()[0]);
                                // Easy way out: Move in exact opposite direction
                            }
                            else
                            {
                                validDir.ExceptWith(attackDir);
                                if (validDir.Count > 0)
                                {
                                    selected = validDir.ToArray()[_random.Next(validDir.Count)];
                                    // Move in any way leading away from attacker
                                }
                                else
                                {
                                    if (attackDir.Contains(_lastDir))
                                        selected = _lastDir;
                                    else selected = attackDir.ToArray()[_random.Next(attackDir.Count)];
                                    // Attack if cornered
                                }
                            }
                        }
                        else
                        {
                            // if no attacker in sight: Move around randomly
                            // TODO: Implement walking away from enemies

                            if ((validDir.Count > 1) && (_random.Next(100) < 90))
                            {
                                validDir.Remove(_lastDir);
                            }
                            selected = validDir.ToArray()[_random.Next(validDir.Count)];
                        }
                    }
                    else
                    {
                        // Normal mode: Attack if possible

                        if (attackDir.Count > 0)
                        {
                            // Pick any target to attack

                            if (attackDir.Contains(_lastDir))
                                selected = _lastDir;
                            else
                                selected = attackDir.ToArray()[_random.Next(attackDir.Count)];
                        }
                        else
                        {
                            // Find closest enemy
                            Backend.Coords closestEnemy = map.ClosestEnemy(coords, actor.viewRange, !(actor is NPC) && (actor.aggro) && (!actor.friendly), !(actor is NPC) && !(actor.aggro) && (!actor.friendly), (actor.friendly) || actor.crazy);
                            if (closestEnemy.x > -1) // There is an enemy close by
                            {
                                List<Coords> shortestPath = null;
                                foreach (Direction dir in validDir)
                                {
                                    List<Coords> path = null;
                                    SortedSet<Coords> temp = null;

                                    map.PathTo(Map.DirectionTile(coords, dir), closestEnemy, out path, ref temp, 10);
                                    if ((shortestPath == null) || (shortestPath.Count > path.Count))
                                    {
                                        selected = dir;
                                        shortestPath = path;
                                    };
                                }
                            }
                            else
                            {
                                // Pick any direction at random
                                if ((validDir.Count > 1) && (_random.Next(100) < 90))
                                {
                                    validDir.Remove(_lastDir);
                                }
                                selected = validDir.ToArray()[_random.Next(validDir.Count)];
                            }
                        }
                    }
                    _working = false;
                    if (selected != Direction.None)
                    {
                        _lastDir = selected;

                        ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.MoveActor, actor.id, selected);
                    }
                }
            }

        }



        /// <summary>
        /// Die in erwägung gezogene Bewegung.
        /// </summary>
        /// <returns>Ergenbnis.</returns>
        public async Task ConsiderMoves()
        {
            if (!_working)
            {
                _working = true;
                await WorkoutMoves();
            }
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
                        && (!actor.locked)
                        && !(actor is Player)
                        && (!_working))
                    {
                        ConsiderMoves();
                    }
                }
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
