using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content;
using System.Linq;

namespace Gruppe22.Backend
{
    /// <summary>
    /// Backend object representing a room in the game
    /// </summary>
    public class Map : IHandleEvent, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Level of current map (used to determine difficulty etc.)
        /// </summary>
        protected int _level;
        /// <summary>
        /// Descriptive name of current map
        /// </summary>
        protected string _name;
        /// <summary>
        /// Descriptive name of current dungeon
        /// </summary>
        protected string _dungeonname;
        /// <summary>
        /// XML-File containing tileset used in dungeon
        /// </summary>
        protected string _wallFile = "wall1";
        /// <summary>
        /// XML-File containing floor used in dungeon
        /// </summary>
        protected string _floorFile = "floor1";
        /// <summary>
        /// Ambient light in dungeon (higher value means increased radius)
        /// </summary>
        protected int _light;
        /// <summary>
        /// Reference to music used in room
        /// </summary>
        protected string _music = "level1";
        /// <summary>
        /// A unique ID-number for current room
        /// </summary>
        protected int _id;

        /// <summary>
        /// An event handler to pass events to (usually a logic object)
        /// </summary>
        private IHandleEvent _parent = null;

        /// <summary>
        /// A two dimensional list of tiles
        /// </summary>
        protected List<List<FloorTile>> _tiles = null;

        /// <summary>
        /// Internal current width
        /// </summary>
        protected int _width = 10;

        /// <summary>
        /// A list of all exits from current room
        /// </summary>
        protected List<Exit> _exits;

        /// <summary>
        /// Internal current height
        /// </summary>
        protected int _height = 10;

        /// <summary>
        /// Blank tile returned when requesting tile outside map boundaries (i.e. negative values / values beyond width or height)
        /// </summary>
        private FloorTile _blankTile = null;

        /// <summary>
        /// A list of Actors in the current room
        /// </summary>
        protected List<Actor> _actors = null;
        /// <summary>
        /// A list of tiles to update each cycle (save ressources)
        /// </summary>
        protected HashSet<Coords> _updateTiles = null;
        private bool _updating = false;
        #endregion

        #region Public Fields

        /// <summary>
        /// Current level (read/write)
        /// </summary>
        public int level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }


        /// <summary>
        /// Current XML-file used for floor-tileset (read/write)
        /// </summary>
        public string floorFile
        {
            get { return _floorFile; }
            set { _floorFile = value; }
        }

        /// <summary>
        /// Music-file to play in current room (read/write)
        /// </summary>
        public string music
        {
            get { return _music; }
            set { _music = value; }
        }

        /// <summary>
        /// Tileset to use for walls in current room (read/write)
        /// </summary>
        public string wallFile
        {
            get { return _wallFile; }
            set { _wallFile = value; }
        }

        /// <summary>
        /// Ambient light in current room (read/write)
        /// </summary>
        public int light
        {
            get
            {
                return _light;
            }
            set
            {
                _light = value;
            }
        }

        /// <summary>
        /// Descriptive name of current room (read/write)
        /// </summary>
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Descriptive name of current dungeon (read/write)
        /// </summary>
        public string dungeonname
        {
            get
            {
                return _dungeonname;
            }
            set
            {
                _dungeonname = value;
            }
        }

        /// <summary>
        /// List of tiles to update (read/write)
        /// </summary>
        public HashSet<Coords> updateTiles
        {
            get
            {
                return _updateTiles;
            }
        }

        /// <summary>
        /// Unique ID-number of current room (read/write)
        /// </summary>
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// List of actors in current room (read/write)
        /// </summary>
        public List<Actor> actors
        {
            get
            {
                return _actors;
            }
            set
            {
                _actors = value;
            }
        }

        /// <summary>
        /// List of exits to other rooms (read/write)
        /// </summary>
        public List<Exit> exits
        {
            get
            {
                return _exits;
            }
            set
            {
                _exits = value;
            }
        }


        /// <summary>
        /// Current Width of the maze
        /// </summary>
        public int width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Current Height of the maze
        /// </summary>
        public int height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        /// <summary>
        /// A list of the current position of every actor
        /// </summary>
        public List<Coords> actorPositions
        {
            get
            {
                List<Coords> result = new List<Coords>();
                foreach (Actor actor in _actors)
                {
                    result.Add(actor.tile.coords);
                }
                return result;
            }
        }



        /// <summary>
        /// Get the tile at coordinates specified by a coords-object
        /// </summary>
        /// <param name="coords">Coordinate object specifying the tile</param>
        /// <returns>The floortile at the specified coordinates</returns>
        public FloorTile this[Coords coords]
        {
            get
            {
                return this[coords.x, coords.y];
            }
            set
            {
                this[coords.x, coords.y] = value;
            }
        }

        /// <summary>
        /// Get the tile at coordinates x and y
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>The tile at the specified coordinates</returns>
        public FloorTile this[int x, int y]
        {
            get
            {
                if ((y < _tiles.Count) && (y > -1) && (x < _tiles[y].Count) && (x > -1))
                {
                    return _tiles[y][x];
                }
                else
                {
                    return _blankTile;
                }
            }
            set
            {
                if ((y < _tiles.Count) && (y > -1) && (x < _tiles[y].Count) && (x > -1))
                {
                    _tiles[y][x] = value;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Refresh tiles which do something (traps, enemies, NPCs)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (!_updating)
            {
                _updating = true;
                Coords[] temp = _updateTiles.ToArray();
                for (int i = 0; i < temp.Length; ++i)
                {
                    _tiles[temp[i].y][temp[i].x].Update(gameTime);
                }
                _updating = false;
            }
        }

        /// <summary>
        /// Make a square-shaped area of a specified radius visible on minimap
        /// </summary>
        /// <param name="coords">Center point</param>
        /// <param name="radius">steps to move up/left/right/down</param>
        public void Uncover(Coords coords, int radius = 4)
        {
            for (int i = 0; i < radius; ++i)
            {
                for (int x = Math.Max(0, coords.x - radius); x <= Math.Min(coords.x + radius, _width); ++x)
                {
                    this[x, coords.y - i].visible = true;
                    this[x, coords.y + i].visible = true;
                    // System.Diagnostics.Debug.WriteLine(new Backend.Coords(x, coords.y - radius).ToString() + " - " + new Backend.Coords(x, coords.y + radius).ToString());
                }
                for (int y = Math.Max(0, coords.y - radius); y <= Math.Min(coords.y + radius, _height); ++y)
                {
                    this[coords.x - i, y].visible = true;
                    this[coords.x + i, y].visible = true;
                    //   System.Diagnostics.Debug.WriteLine(new Backend.Coords(coords.x - radius, y).ToString() + " - " + new Backend.Coords(coords.x + radius, y).ToString());
                }
            }

        }

        /// <summary>
        /// Move an actor on the map in a specified direction (does not check for walls - use CanMove)
        /// </summary>
        /// <param name="actor">The actor object to move</param>
        /// <param name="dir">Direction to move to</param>
        public void MoveActor(Actor actor, Direction dir)
        {
            Backend.Coords source = actor.tile.coords;
            Backend.Coords target = DirectionTile(actor.tile.coords, dir);
            if (this[target].coords.x > -1)
            {
                // Remove ActorTile from current tile
                ((FloorTile)actor.tile.parent).Remove(actor.tile);
                // Add ActorTile to new Tile
                _tiles[target.y][target.x].Add(actor.tile);
                actor.tile.parent = _tiles[target.y][target.x];
                // Remove old tile from updatelist (if no other actor or trap)
                if (!((_tiles[source.y][source.x].hasEnemy)
                    || (_tiles[source.y][source.x].hasPlayer)
                    || (_tiles[source.y][source.x].hasTrap)))
                    _updateTiles.Remove(source);
                // Add new tile to updatelist
                _updateTiles.Add(target);
            }
        }


        /// <summary>
        /// Move an actor on the map in a specified direction (does not check for walls - use CanMove)
        /// </summary>
        /// <param name="actor">The actor object to move</param>
        /// <param name="dir">Direction to move to</param>
        public void PositionActor(Actor actor, Coords coords)
        {
            Backend.Coords source = actor.tile.coords;
            Backend.Coords target = coords;
            if (this[target].coords.x > -1)
            {
                // Remove ActorTile from current tile
                ((FloorTile)actor.tile.parent).Remove(actor.tile);
                // Add ActorTile to new Tile
                _tiles[target.y][target.x].Add(actor.tile);
                actor.tile.parent = _tiles[target.y][target.x];
                // Remove old tile from updatelist (if no other actor or trap)
                if (!((_tiles[source.y][source.x].hasEnemy)
                    || (_tiles[source.y][source.x].hasPlayer)
                    || (_tiles[source.y][source.x].hasTrap)))
                    _updateTiles.Remove(source);
                // Add new tile to updatelist
                _updateTiles.Add(target);
            }
        }

        /// <summary>
        /// Get coordinates for closest enemy within a specified radius
        /// </summary>
        /// <param name="coords">Center point to start checking from</param>
        /// <param name="radius">Number of squares to move up/left/right/down</param>
        /// <param name="includePlayer">true if player should be an "enemy"</param>
        /// <param name="includeNPC">true if NPCs should be "enemies"</param>
        /// <param name="includeEnemy">true if monsters should be "enemies"</param>
        /// <returns>Coordinates of first hostile target found</returns>
        public Backend.Coords ClosestEnemy(Coords coords, int radius = 4, bool includePlayer = true, bool includeNPC = true, bool includeEnemy = true)
        {
            for (int distance = 0; distance <= radius; ++distance)
            {
                for (int x = Math.Max(coords.x - distance, 0); x <= Math.Min(coords.x + distance, _width); ++x)
                {
                    if ((((includePlayer && this[x, coords.y - distance].hasPlayer))
                        //                    ||((includeNPC&&this[coords.x-distance,y].hasNPC))
                    || ((includeEnemy && this[x, coords.y - distance].hasEnemy)))
                        && ((x != coords.x) || (distance != 0)))
                    {
                        //System.Diagnostics.Debug.WriteLine(coords.x + "/" + coords.y + "->" + x + "/" + (coords.y - distance).ToString() + " = " + (this[x, coords.y - distance].hasPlayer ? "Player" : "Enemy"));
                        return new Backend.Coords(x, coords.y - distance);
                    }
                    if ((((includePlayer && this[x, coords.y + distance].hasPlayer))
                        //                    ||((includeNPC&&this[coords.x-distance,y].hasNPC))
                    || ((includeEnemy && this[x, coords.y + distance].hasEnemy)))
                                            && ((x != coords.x) || (distance != 0)))
                    {
                        // System.Diagnostics.Debug.WriteLine(coords.x + "/" + coords.y + "->" + x + "/" + (coords.y + distance).ToString() + " = " + (this[x, coords.y + distance].hasPlayer ? "Player" : "Enemy"));

                        return new Backend.Coords(x, coords.y + distance);
                    }
                }
                for (int y = Math.Max(coords.y - distance, 0); y <= Math.Min(coords.y + distance, _height); ++y)
                {
                    if ((((includePlayer && this[coords.x - distance, y].hasPlayer))
                        //                    ||((includeNPC&&this[coords.x-distance,y].hasNPC))
                    || ((includeEnemy && this[coords.x - distance, y].hasEnemy)))
                        && ((y != coords.y) || (distance != 0)))
                    {
                        //   System.Diagnostics.Debug.WriteLine(coords.x + "/" + coords.y + "->" + (coords.x - distance) + "/" + y.ToString() + " = " + (this[coords.x - distance, y].hasPlayer ? "Player" : "Enemy"));

                        return new Backend.Coords(coords.x - distance, y);
                    }
                    if ((((includePlayer && this[coords.x + distance, y].hasPlayer))
                        //                    ||((includeNPC&&this[coords.x-distance,y].hasNPC))
                    || ((includeEnemy && this[coords.x + distance, y].hasEnemy)))
                        && ((y != coords.y) || (distance != 0)))
                    {
                        //  System.Diagnostics.Debug.WriteLine(coords.x + "/" + coords.y + "->" + (coords.x + distance) + "/" + y.ToString() + " = " + (this[coords.x + distance, y].hasPlayer ? "Player" : "Enemy"));

                        return new Backend.Coords(coords.x + distance, y);
                    }
                }
            }
            return new Backend.Coords(-1, -1);
        }

        /// <summary>
        /// Find a path between two tiles (not necessarily only or shortest route!)
        /// </summary>
        /// <param name="from">Tile to start from</param>
        /// <param name="to">Tile to move to </param>
        /// <param name="result">List to put result path into</param>
        /// <param name="visited">TIles visited on current path (avoid circles)</param>
        /// <param name="maxlength">Maximum length of path</param>
        public void PathTo(Coords from, Backend.Coords to, out List<Coords> result, ref HashSet<Coords> visited, int maxlength = 20)
        {
            result = null;
            if (visited == null)
            {
                visited = new HashSet<Coords>();
            }
            visited.Add(from);

            if ((
                (from.x >= 0) &&
                (to.x >= 0)) &&
                (from.y >= 0) &&
                (to.y >= 0) &&
                (to.x < _width) &&
                (from.x < _width) &&
                (to.y < _height) &&
                (from.y < _height)
                && (maxlength > 0))
            {
                if (from.Equals(to))
                {
                    result = new List<Coords>();
                    result.Add(to);
                    visited.Remove(from);
                    return;
                }


                Direction dir = WhichWayIs(to, from, true); // start by direct route
                int count = 0;
                while (count < 4)
                {
                    Backend.Coords tmp = Map.DirectionTile(from, dir);


                    if ((this[tmp.x, tmp.y].canEnter) && (!visited.Contains(tmp)))
                    {
                        // System.Diagnostics.Debug.WriteLine(indent + "Looking " + dir + " of " + from + " to " + tmp);

                        PathTo(tmp, to, out result, ref visited, maxlength - 1);
                        if (result != null)
                        {
                            //   System.Diagnostics.Debug.WriteLine(indent + " - " + from.x + "/" + from.y);
                            if (result.Count > 1)
                            {
                                if ((Math.Abs(result[1].x - from.x) < 2) && (Math.Abs(result[1].y - from.y) < 2) && (CanMove(result[1], WhichWayIs(result[1], from))))
                                    result[0] = from; // Diagonals preferred

                                else result.Insert(0, from);
                            }
                            else
                            {

                                result.Insert(0, from); // Only starting point in list                                 
                            }
                            visited.Remove(from);

                            // System.Diagnostics.Debug.WriteLine(indent + "*" + from.x + "/" + from.y + " leads over " + dir.ToString() + " to " + to.x + " / " + to.y);
                            return;

                        }
                        /*    else
                            {
                                System.Diagnostics.Debug.WriteLine(indent + from.x + "/" + from.y + " no exit " + dir.ToString() + " to " + to.x + " / " + to.y);
                            } */
                    }
                    /*    else
                        {
                            if (!visited.Contains(tmp))
                            {
                                System.Diagnostics.Debug.WriteLine(indent + dir.ToString() + " of " + from.x + "/" + from.y + " is blocked");
                            }
                        } */
                    dir = NextDirection(dir, true);
                    count += 1;
                }

            }
            //PathTo(from, to, maxlength - 1);
            visited.Remove(from);

            return;
        }

        /// <summary>
        /// Get coordinates of checkpoint on map (if any)
        /// </summary>
        /// <returns>null if no checkpoint exists, coords object otherwise</returns>
        public Backend.Coords GetCheckpointCoords()
        {
            foreach (List<FloorTile> ltiles in _tiles)
                foreach (FloorTile tile in ltiles)
                {
                    if (tile.hasCheckpoint)
                        return tile.coords;
                }
            return null;
        }

        /// <summary>
        /// Get tile specified by a coords-object (deprecated, use direct access by map[])
        /// </summary>
        /// <param name="coords">Coordinates of tile</param>
        /// <returns>Tile object at specified coordinates</returns>
        public FloorTile TileByCoords(Coords coords)
        {
            return this[coords.x, coords.y];
        }

        /// <summary>
        /// Get ID of first actor on a specified tile
        /// </summary>
        /// <param name="x">x-coordinates of tile</param>
        /// <param name="y">y-coordinates of tile</param>
        /// <returns></returns>
        public int firstActorID(int x, int y)
        {
            if (_tiles[y][x].firstActor != null)
                return _tiles[y][x].firstActor.id;
            else
                return -1;
        }

        /// <summary>
        /// Check whether it is possible to move from a certain place on a map in a certain direction
        /// </summary>
        /// <param name="currentPos">Coordinates on current map</param>
        /// <param name="dir">Direction to move to</param>
        /// <returns>true if move is allowed</returns>
        public bool CanMove(Coords currentPos, Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    if ((currentPos.x > 0) && (this[currentPos.x - 1, currentPos.y].canEnter))
                        return true;
                    break;
                case Direction.Right:
                    if ((currentPos.x < this.width - 1) && (this[currentPos.x + 1, currentPos.y].canEnter))
                        return true;
                    break;
                case Direction.Down:
                    if ((currentPos.y < this.height - 1) && (this[currentPos.x, currentPos.y + 1].canEnter))
                        return true;
                    break;
                case Direction.Up:
                    if ((currentPos.y > 0) && (this[currentPos.x, currentPos.y - 1].canEnter))
                        return true;
                    break;

                // Diagonal movement
                case Direction.DownLeft:
                    if ((currentPos.x > 0) && (currentPos.y < this.height - 1)
                        && (this[currentPos.x - 1, currentPos.y + 1].canEnter)
                        && ((this[currentPos.x, currentPos.y + 1].canEnter)
                        || (this[currentPos.x - 1, currentPos.y].canEnter)
                        )
                        )
                        return true;
                    break;
                case Direction.UpRight:
                    if ((currentPos.x < this.width - 1) && (currentPos.y > 0)
                        && ((this[currentPos.x, currentPos.y - 1].canEnter)
                        || (this[currentPos.x + 1, currentPos.y].canEnter))
                        && (this[currentPos.x + 1, currentPos.y - 1].canEnter)
                        )
                        return true;
                    break;
                case Direction.DownRight:
                    if ((currentPos.y < this.height - 1) && (currentPos.x < this.width - 1)
                        && ((this[currentPos.x, currentPos.y + 1].canEnter)
                        || (this[currentPos.x + 1, currentPos.y].canEnter))
                        && (this[currentPos.x + 1, currentPos.y + 1].canEnter))
                        return true; break;
                case Direction.UpLeft:
                    if ((currentPos.y > 0)
                        && (currentPos.x > 0)
                        && (this[currentPos.x - 1, currentPos.y - 1].canEnter)
                        && ((this[currentPos.x - 1, currentPos.y].canEnter)
                        || (this[currentPos.x, currentPos.y - 1].canEnter)))
                        return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Event Handling (implementation of IHandleEvent)
        /// </summary>
        /// <param name="DownStream">true if message is meant for children; false if it is meant for parents</param>
        /// <param name="eventID">Unique ID of the event</param>
        /// <param name="data">Any data associated with the event</param>
        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (!DownStream)
            {
                switch (eventID)
                {
                    default:
                        _parent.HandleEvent(false, eventID, data);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="targetCoords"></param>
        /// <param name="resetPlayer"></param>
        public void Load(string filename, Backend.Coords targetCoords = null, bool resetPlayer = false)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader xmlr = XmlReader.Create(filename, settings);
            ReadXML(xmlr, targetCoords, resetPlayer);
            xmlr.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetCoords"></param>
        /// <param name="resetPlayer"></param>
        public void FromXML(string input, Backend.Coords targetCoords = null, bool resetPlayer = false)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader xmlr = XmlReader.Create(new System.IO.StringReader(input));
            ReadXML(xmlr, targetCoords, resetPlayer);
            xmlr.Close();
        }
        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        /// <param name="player">The starting position on the loaded map</param>
        public void ReadXML(XmlReader xmlr, Backend.Coords targetCoords = null, bool resetPlayer = false)
        {

            List<Player> players = new List<Player>();

            // Move all players to the new map
            if (_actors.Count > 0)
            {
                for (int i = 0; i < _actors.Count; ++i)
                {
                    if (_actors[i] is Player)
                    {
                        players.Add(_actors[i] as Player);
                        players[players.Count - 1].tile = null;
                        break;
                    }
                }
                _actors.Clear();
            }


            _tiles.Clear();
            _actors.Clear();
            _updateTiles.Clear();
            xmlr.MoveToContent();//xml
            _width = int.Parse(xmlr.GetAttribute("width"));
            _height = int.Parse(xmlr.GetAttribute("height"));
            if (xmlr.GetAttribute("name") != null) _name = xmlr.GetAttribute("name");
            if (xmlr.GetAttribute("level") != null) _level = int.Parse(xmlr.GetAttribute("level"));
            if (xmlr.GetAttribute("dungeon") != null) _dungeonname = xmlr.GetAttribute("dungeon");
            if (xmlr.GetAttribute("floor") != null) _floorFile = xmlr.GetAttribute("floor");
            if (xmlr.GetAttribute("wall") != null) _wallFile = xmlr.GetAttribute("wall");
            if (xmlr.GetAttribute("id") != null) _id = Int32.Parse(xmlr.GetAttribute("id"));

            if (xmlr.GetAttribute("light") != null) _light = int.Parse(xmlr.GetAttribute("light"));
            if (xmlr.GetAttribute("music") != null) _music = xmlr.GetAttribute("music");

            xmlr.ReadStartElement("GameMap");//GameMap

            for (int row = 0; row < _height; ++row)
            {
                _tiles.Add(new List<FloorTile>());
                for (int col = 0; col < _width; ++col)
                {
                    _tiles[row].Add(new FloorTile(this, new Backend.Coords(col, row)));
                }
            }

            while ((xmlr.NodeType != XmlNodeType.EndElement) && (xmlr.NodeType != XmlNodeType.None))
            { // Add Tiles and overlay-Tiles
                switch (xmlr.Name)
                {
                    case "Tile":
                        FloorTile tile = _tiles[Int32.Parse(xmlr.GetAttribute("CoordY"))][Int32.Parse(xmlr.GetAttribute("CoordX"))];
                        if (xmlr.GetAttribute("visited") != null)
                        {
                            tile.visible = Boolean.Parse(xmlr.GetAttribute("visited"));
                        }
                        if (!xmlr.IsEmptyElement)
                        {
                            xmlr.Read();

                            while ((xmlr.NodeType != XmlNodeType.EndElement))
                            {
                                switch (xmlr.Name)
                                {
                                    case "WallTile":
                                        WallTile wall = new WallTile(this);
                                        if (xmlr.GetAttribute("Enabled") != null) wall.enabled = Boolean.Parse(xmlr.GetAttribute("Enabled"));
                                        if (xmlr.GetAttribute("Health") != null) wall.health = Int32.Parse(xmlr.GetAttribute("Health"));
                                        if (xmlr.GetAttribute("Type") != null) wall.type = (WallType)Enum.Parse(typeof(WallType), xmlr.GetAttribute("Type").ToString());

                                        if (xmlr.GetAttribute("Illusion") != null) wall.illusion = Boolean.Parse(xmlr.GetAttribute("Illusion"));
                                        if (xmlr.GetAttribute("Illusionvisible") != null) wall.illusionVisible = Boolean.Parse(xmlr.GetAttribute("Illusionvisible"));
                                        tile.Add(wall);
                                        break;
                                    case "ProjectileTile":
                                        {
                                            uint id = 0;
                                            if (xmlr.GetAttribute("id", _id.ToString()) != null) id = UInt32.Parse(xmlr.GetAttribute("id"));
                                            Direction dir = Direction.Up;
                                            if (xmlr.GetAttribute("direction", _id.ToString()) != null) dir = (Direction)Enum.Parse(typeof(Direction), xmlr.GetAttribute("direction").ToString());
                                            ProjectileTile projectile = new ProjectileTile(tile, dir, id);
                                        }
                                        break;
                                    case "ItemTile":
                                        Item item = new Item();
                                        xmlr.Read();
                                        item.Load(xmlr);
                                        ItemTile itemTile = new ItemTile(tile, item);
                                        item.tile = itemTile;
                                        tile.Add(itemTile);
                                        xmlr.Read(); // End Item
                                        break;

                                    case "TargetTile":
                                        tile.Add(new TargetTile(tile));
                                        break;

                                    case "TrapTile":
                                        TrapTile trap = new TrapTile(this, Int32.Parse(xmlr.GetAttribute("damage")));
                                        if (xmlr.GetAttribute("penetrate") != null) trap.penetrate = Int32.Parse(xmlr.GetAttribute("penetrate"));

                                        if (xmlr.GetAttribute("evade") != null) trap.evade = Int32.Parse(xmlr.GetAttribute("evade"));

                                        if (xmlr.GetAttribute("changing") != null)
                                        {
                                            trap.type = trap.type | TrapType.Changing;
                                        }
                                        if (xmlr.GetAttribute("hidden") != null)
                                        {
                                            trap.type = trap.type | TrapType.Hidden;
                                        }
                                        if (xmlr.GetAttribute("onlyonce") != null)
                                        {
                                            trap.type = trap.type | TrapType.OnlyOnce;
                                        }
                                        if (xmlr.GetAttribute("broken") != null)
                                        {
                                            trap.status = TrapState.Destroyed;
                                        }
                                        if (xmlr.GetAttribute("disabled") != null)
                                        {
                                            trap.status = TrapState.Disabled;
                                        }
                                        if (xmlr.GetAttribute("invisible") != null)
                                        {
                                            trap.status = TrapState.NoDisplay;
                                        }
                                        tile.Add(trap);
                                        _updateTiles.Add(tile.coords);
                                        break;
                                    case "ReservedTile":
                                        ReservedTile reserved = new ReservedTile(tile);

                                        if (xmlr.GetAttribute("Enabled") != null)
                                        { reserved.enabled = Boolean.Parse(xmlr.GetAttribute("Enabled")); }

                                        if (xmlr.GetAttribute("CanEnter") != null)
                                        { reserved.canEnter = Boolean.Parse(xmlr.GetAttribute("CanEnter")); }
                                        if (xmlr.GetAttribute("Filename") != null)
                                        { reserved.filename = xmlr.GetAttribute("Filename"); }
                                        if (xmlr.GetAttribute("Index") != null)
                                        { reserved.index = Int32.Parse(xmlr.GetAttribute("Index")); }

                                        tile.Add(reserved);

                                        break;

                                    case "DoorTile":
                                        DoorTile door = new DoorTile(tile);
                                        if (xmlr.GetAttribute("open") != null)
                                        { door.open = Boolean.Parse(xmlr.GetAttribute("open")); }

                                        if (xmlr.GetAttribute("key") != null)
                                        { door.key = int.Parse(xmlr.GetAttribute("key")); }
                                        tile.Add(door);
                                        break;

                                    case "TeleportTile":


                                        TeleportTile transporter = new TeleportTile(tile, xmlr.GetAttribute("nextRoom"), new Backend.Coords(Int32.Parse(xmlr.GetAttribute("nextX")), Int32.Parse(xmlr.GetAttribute("nextY"))));
                                        if (xmlr.GetAttribute("hidden") != null)
                                        { transporter.hidden = Boolean.Parse(xmlr.GetAttribute("hidden")); }
                                        if (xmlr.GetAttribute("enabled") != null)
                                        { transporter.enabled = Boolean.Parse(xmlr.GetAttribute("enabled")); }
                                        if (xmlr.GetAttribute("teleport") != null)
                                        { transporter.teleport = Boolean.Parse(xmlr.GetAttribute("teleport")); }
                                        if (xmlr.GetAttribute("down") != null)
                                        { transporter.down = Boolean.Parse(xmlr.GetAttribute("down")); }

                                        tile.Add(transporter);
                                        break;

                                    case "TriggerTile":
                                        Backend.Coords target = new Backend.Coords(-1, -1);
                                        if (xmlr.GetAttribute("affectX") != null) target.x = Int32.Parse(xmlr.GetAttribute("affectX"));

                                        if (xmlr.GetAttribute("affectY") != null) target.y = Int32.Parse(xmlr.GetAttribute("affectY"));

                                        TriggerTile trigger = new TriggerTile(tile, target);

                                        if (xmlr.GetAttribute("explanation") != null) { trigger.explanationEnabled = xmlr.GetAttribute("explanation"); };
                                        if (xmlr.GetAttribute("explanationdisabled") != null) { trigger.explanationDisabled = xmlr.GetAttribute("explanationdisabled"); };
                                        if (xmlr.GetAttribute("enabled") != null) { trigger.enabled = Boolean.Parse(xmlr.GetAttribute("enabled")); };
                                        if (xmlr.GetAttribute("repeat") != null) { trigger.repeat = Int32.Parse(xmlr.GetAttribute("repeat")); };

                                        if (xmlr.GetAttribute("alwaysShowDisabled") != null) { trigger.alwaysShowDisabled = Boolean.Parse(xmlr.GetAttribute("alwaysShowDisabled")); };
                                        if (xmlr.GetAttribute("alwaysShowEnabled") != null) { trigger.alwaysShowEnabled = Boolean.Parse(xmlr.GetAttribute("alwaysShowEnabled")); };


                                        if (xmlr.GetAttribute("tileimage") != null) { trigger.tileimage = xmlr.GetAttribute("tileimage"); };
                                        if (xmlr.GetAttribute("reactToEnemies") != null) { trigger.reactToEnemies = Boolean.Parse(xmlr.GetAttribute("reactToEnemies")); };
                                        if (xmlr.GetAttribute("reactToObjects") != null) { trigger.reactToObjects = Boolean.Parse(xmlr.GetAttribute("reactToObjects")); };
                                        if (xmlr.GetAttribute("reactToItem") != null)
                                        {
                                            trigger.reactToItem = Int32.Parse(xmlr.GetAttribute("reactToItem"));
                                        }
                                        tile.Add(trigger);
                                        break;
                                    case "CheckpointTile":
                                        int bonusLifes = 0;
                                        bool visited = false;

                                        if (xmlr.GetAttribute("bonuslife") != null)
                                            bonusLifes = Convert.ToInt32(xmlr.GetAttribute("bonuslife"));
                                        if (xmlr.GetAttribute("visited") != null)
                                            visited = Convert.ToBoolean(xmlr.GetAttribute("visited"));
                                        tile.Add(new CheckpointTile(tile, visited, bonusLifes));
                                        break;

                                    case "ActorTile":
                                        Actor actor;
                                        xmlr.Read();
                                        switch (xmlr.Name)
                                        {
                                            case "Enemy":
                                                actor = new Enemy();
                                                actor.Load(xmlr);
                                                break;
                                            case "Player":
                                                actor = new Player();
                                                actor.Load(xmlr);
                                                break;
                                            default:
                                                actor = new NPC();
                                                actor.Load(xmlr);
                                                break;
                                        }

                                        if (!(actor is Player))
                                        {
                                            ActorTile tile2 = new ActorTile(tile, actor);
                                            actor.tile = tile2;
                                            tile2.enabled = (actor.health > 0);
                                            tile.Add(tile2);
                                            _actors.Add(actor);
                                            _updateTiles.Add(tile.coords);
                                        }
                                        else
                                        {
                                            if (players.Count > 0)
                                            {
                                                if (!resetPlayer)
                                                    actor.copyFrom(players[0]);
                                                players.RemoveAt(0);

                                            }

                                            if (!resetPlayer)
                                            {
                                                if (targetCoords == null)
                                                {
                                                    targetCoords = tile.coords;
                                                }
                                                ActorTile actortile = new ActorTile(this[targetCoords], actor);
                                                actor.tile = actortile;
                                                actortile.enabled = (actor.health > 0);
                                                actortile.parent = this[targetCoords];
                                                this[targetCoords].Add(actortile);

                                                _actors.Add(actor);

                                                _updateTiles.Add(targetCoords);
                                            }
                                            else
                                            {
                                                if (targetCoords == null)
                                                {
                                                    targetCoords = tile.coords;
                                                }
                                                ActorTile actortile = new ActorTile(tile, actor);
                                                actor.tile = actortile;
                                                actortile.enabled = (actor.health > 0);
                                                actortile.parent = tile;
                                                tile.Add(actortile);
                                                _actors.Add(actor);
                                                _updateTiles.Add(tile.coords);
                                            }

                                        }

                                        break;
                                }
                                xmlr.Read();
                            }
                        }
                        xmlr.ReadEndElement();
                        break;
                }
            }

            if (targetCoords == null)
            {
                targetCoords = new Coords(1, 1);
            }
            while (players.Count > 0)
            {

                ActorTile actortile = new ActorTile(this[targetCoords], players[0]);
                players[0].tile = actortile;
                actortile.enabled = (players[0].health > 0);
                actortile.parent = this[targetCoords];
                this[targetCoords].Add(actortile);

                _actors.Add(players[0]);

                _updateTiles.Add(targetCoords);
                players.RemoveAt(0);
            }

            for (int i = 0; i < _actors.Count; ++i)
            {
                _actors[i].id = i;
                if (_actors[i] is Player) Uncover(actors[i].tile.coords, actors[i].viewRange);

            }
        }

        /// <summary>
        /// Display map & walls in text form
        /// </summary>
        public override string ToString()
        {
            string output = "";
            foreach (List<FloorTile> row in _tiles)
            {
                foreach (FloorTile tile in row)
                {
                    if (tile.hasWall)
                    {
                        output += "#";
                    }
                    else
                    {
                        if (tile.hasPlayer)
                        {
                            output += "@";
                        }
                        else if (tile.hasEnemy)
                        {
                            output += "X";
                        }
                        else if (tile.hasTrap)
                        {
                            output += "!";
                        }
                        else if (tile.hasNPC)
                        {
                            output += "0";
                        }
                        else if (tile.hasTeleport)
                        {
                            output += ">";
                        }
                        else if (tile.hasTreasure)
                        {
                            output += "*";
                        }
                        else output += " ";
                    }
                }
                output += Environment.NewLine;
                // System.Diagnostics.Debug.WriteLine(output);
            }
            return output;
        }

        /// <summary>
        /// Method to add the player character in the current map(room)
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public virtual int AssignPlayer(string GUID = "")
        {
            for (int i = 0; i < _actors.Count; ++i)
            {
                if ((_actors[i] is Player) && ((_actors[i].GUID == "") || (_actors[i].GUID == "")))
                {
                    _actors[i].GUID = GUID;
                    _actors[i].online = true;
                    return i;
                }
            }
            Player temp = new Player();
            _actors.Add(temp);
            int newID = _actors.Count;
            temp.online = true;
            temp.tile = new ActorTile(this[1, 1]);
            this[1, 1].Add(temp.tile);
            return newID;
            // Create New Player with specified GUID
        }

        /// <summary>
        /// Get the current Map as an XML-String
        /// </summary>
        public virtual string ToXML()
        {
            StringBuilder output = new StringBuilder("");
            XmlWriter xmlw = XmlWriter.Create(output);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            xmlw.WriteAttributeString("name", _name);
            xmlw.WriteAttributeString("level", _level.ToString());
            xmlw.WriteAttributeString("dungeon", _dungeonname);
            xmlw.WriteAttributeString("light", _light.ToString());
            xmlw.WriteAttributeString("floor", _floorFile);
            xmlw.WriteAttributeString("music", _music);
            xmlw.WriteAttributeString("wall", _wallFile);
            xmlw.WriteAttributeString("id", _id.ToString());


            foreach (List<FloorTile> ltiles in _tiles)
            {
                foreach (FloorTile tile in ltiles)
                {
                    if (tile.overlay.Count > 0)
                        tile.Save(xmlw);
                }
            }
            xmlw.WriteEndElement();
            xmlw.WriteEndDocument();
            xmlw.Close();
            string result = output.ToString();
            result = result.Trim();
            result = result.Replace("  ", " ");
            result = result.Replace(Environment.NewLine, " ");
            result = result.Replace('\0', ' ');
            return result;
        }

        /// <summary>
        /// Write the current map to a file
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        public virtual void Save(string filename)
        {
            XmlWriter xmlw = XmlWriter.Create(filename);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            xmlw.WriteAttributeString("name", _name);
            xmlw.WriteAttributeString("level", _level.ToString());
            xmlw.WriteAttributeString("dungeon", _dungeonname);
            xmlw.WriteAttributeString("light", _light.ToString());
            xmlw.WriteAttributeString("floor", _floorFile);
            xmlw.WriteAttributeString("music", _music);
            xmlw.WriteAttributeString("wall", _wallFile);
            xmlw.WriteAttributeString("id", _id.ToString());


            foreach (List<FloorTile> ltiles in _tiles)
            {
                foreach (FloorTile tile in ltiles)
                {
                    if (tile.overlay.Count > 0)
                        tile.Save(xmlw);
                }
            }
            xmlw.WriteEndElement();
            xmlw.WriteEndDocument();
            xmlw.Close();
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Basic constructor (creating an empty map)
        /// </summary>

        public Map()
        {
            _updateTiles = new HashSet<Coords>();
            _actors = new List<Actor>();
            _blankTile = new FloorTile(this);
            _blankTile.coords = new Backend.Coords(-1, -1);
            _blankTile.Add(new GapTile(_blankTile));
            _tiles = new List<List<FloorTile>>();
            _exits = new List<Exit>();

        }

        /// <summary>
        /// Constructor for a loading a map from a file
        /// </summary>
        /// <param name="parent">An event handler to pass events to</param>
        /// <param name="filename">Filename of XML-file containing map data</param>
        /// <param name="playerPos">Coordinates of player on map</param>
        public Map(IHandleEvent parent, string filename = "", Backend.Coords playerPos = null)
            : this()
        {
            _parent = parent;
            Load(filename, playerPos, true);
        }

        /// <summary>
        /// Clean up: Remove all List objects manually (avoid garbage collection)
        /// </summary>
        public virtual void Dispose()
        {
            while (_tiles.Count > 0)
            {
                while (_tiles[0].Count > 0)
                {
                    _tiles[0][0].Dispose();
                    _tiles[0].RemoveAt(0);
                }
                _tiles[0].Clear();
                _tiles.RemoveAt(0);
            }
            _tiles.Clear();
            _updateTiles.Clear();
            _actors.Clear();
        }
        #endregion

        #region Static Helpers

        /// <summary>
        /// Determine which way one square is from another
        /// </summary>
        /// <param name="from">Source square</param>
        /// <param name="to">Target Square</param>
        /// <param name="DirectOnly">false (default) if diagonals are allowed</param>
        /// <returns>Direction to look</returns>
        public static Direction WhichWayIs(Coords from, Backend.Coords to, bool DirectOnly = false)
        {
            if (from.x < to.x)
            {
                if (from.y < to.y)
                {
                    if (!DirectOnly)
                        return Direction.UpLeft;
                    else
                        return Direction.Up;
                }
                else
                {
                    if (from.y > to.y)
                    {
                        if (!DirectOnly)
                            return Direction.DownLeft;
                        else
                            return Direction.Left;
                    }
                    else
                    {
                        return Direction.Left;
                    }
                }
            }
            else
            {
                if (from.x > to.x)
                {

                    if (from.y < to.y)
                    {
                        if (!DirectOnly)
                            return Direction.UpRight;
                        else
                            return Direction.Up;
                    }
                    else
                    {
                        if (from.y > to.y)
                        {
                            if (!DirectOnly)
                                return Direction.DownRight;
                            else
                                return Direction.Right;
                        }
                        else
                        {
                            return Direction.Right;
                        }
                    }
                }
                else
                {
                    if (from.y < to.y)
                    {
                        return Direction.Up;
                    }
                    else
                    {
                        if (from.y > to.y)
                        {
                            return Direction.Down;
                        }
                    }
                }
            }
            return Direction.None;
        }

        /// <summary>
        /// Find the exact opposite facing of a direction
        /// </summary>
        /// <param name="dir">Direction to start</param>
        /// <returns>Direction which is the other way</returns>
        public static Direction OppositeDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.UpLeft:
                    return Direction.DownRight;
                case Direction.DownRight:
                    return Direction.UpLeft;
                case Direction.UpRight:
                    return Direction.DownLeft;
                case Direction.DownLeft:
                    return Direction.UpRight;
            }
            return Direction.None;
        }

        /// <summary>
        /// Get coordinates for next tile in a certain direction
        /// </summary>
        /// <param name="start">Current tile</param>
        /// <param name="dir">Direction to look at</param>
        /// <returns>Coordinates of next tile in specified direction</returns>
        public static Backend.Coords DirectionTile(Coords start, Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return new Backend.Coords(start.x - 1, start.y);

                case Direction.Right:
                    return new Backend.Coords(start.x + 1, start.y);

                case Direction.Down:
                    return new Backend.Coords(start.x, start.y + 1);

                case Direction.Up:
                    return new Backend.Coords(start.x, start.y - 1);

                case Direction.DownLeft:
                    return new Backend.Coords(start.x - 1, start.y + 1);

                case Direction.UpRight:
                    return new Backend.Coords(start.x + 1, start.y - 1);

                case Direction.DownRight:
                    return new Backend.Coords(start.x + 1, start.y + 1);

                case Direction.UpLeft:
                    return new Backend.Coords(start.x - 1, start.y - 1);
            }
            return start;
        }

        /// <summary>
        /// Turns around clockwise (i.e. Up->Right->Down->Left->Up)
        /// </summary>
        /// <param name="dir">Start direction</param>
        /// <param name="directOnly">true if diagonals should not be allowed</param>
        /// <returns>Next direction (clockwise)</returns>
        public static Direction NextDirection(Direction dir, bool directOnly = false)
        {
            switch (dir)
            {
                case Direction.Up:
                    if (!directOnly)
                        return Direction.UpRight;
                    else
                        return Direction.Right;
                case Direction.UpRight:
                    return Direction.Right;
                case Direction.Right:
                    if (!directOnly)
                        return Direction.DownRight;
                    else
                        return Direction.Down;
                case Direction.DownRight:
                    return Direction.Down;
                case Direction.Down:
                    if (!directOnly)
                        return Direction.DownLeft;
                    else
                        return Direction.Left;
                case Direction.DownLeft:
                    return Direction.Left;
                case Direction.Left:
                    if (!directOnly)
                        return Direction.UpLeft;
                    else
                        return Direction.Up;
                case Direction.UpLeft:
                    return Direction.Up;
            }
            return Direction.None;
        }

        /// <summary>
        /// Switch Entrance and Exit in a list of exit-objects (for corresponding rooms)
        /// </summary>
        /// <param name="ToRoom">Which room to switch</param>
        /// <param name="exits">List of exits</param>
        /// <returns>List of entrances</returns>
        public static List<Exit> ExitToEntry(int ToRoom, List<Exit> exits)
        {
            List<Exit> result = new List<Exit>();
            foreach (Exit exit in exits)
            {
                if (exit.toRoom.Contains("om" + ToRoom.ToString() + ".xml"))
                {
                    result.Add(new Exit(exit.to, exit.toRoom, exit.from, exit.fromRoom));
                }
            }
            return result;
        }
        #endregion
    }
}
