using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{

    public class Map : IHandleEvent, IDisposable
    {
        #region Private Fields
        protected ContentManager _content;
        protected int _level;
        protected string _name;
        protected string _dungeonname;
        protected string _wallFile = "wall1";
        protected string _floorFile = "floor1";
        protected int _light;
        protected string _music = "level1";
        protected int _id;

        private object _parent = null;

        /// <summary>
        /// A two dimensional list of tiles
        /// </summary>
        protected List<List<FloorTile>> _tiles = null;

        /// <summary>
        /// Internal current width
        /// </summary>
        protected int _width = 10;

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
        protected List<Item> _items = null;
        protected List<Coords> _updateTiles = null;
        #endregion

        #region Public Fields

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



        public string floorFile
        {
            get { return _floorFile; }
            set { _floorFile = value; }
        }

        public string music
        {
            get { return _music; }
            set { _music = value; }
        }

        public string wallFile
        {
            get { return _wallFile; }
            set { _wallFile = value; }
        }

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
        public List<Coords> updateTiles
        {
            get
            {
                return _updateTiles;
            }
        }
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

        public List<Item> items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
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
        /// Get the tile at coordinates x and y
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>The tile at the specified coordinates</returns>
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
            for (int i = 0; i < _updateTiles.Count; ++i)
            {
                _tiles[_updateTiles[i].y][_updateTiles[i].x].Update(gameTime);
            }
        }

        /// <summary>
        /// Add an actor to a tile
        /// </summary>
        /// <param name="item"></param>
        /// <param name="from"></param>
        public void AddActor(Item item, Backend.Coords from)
        {

        }

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
        /// <param name="actor"></param>
        /// <param name="from"></param>
        /// <param name="dir"></param>
        public void MoveActor(Actor actor, Direction dir)
        {
            Backend.Coords source = actor.tile.coords;
            Backend.Coords target = DirectionTile(actor.tile.coords, dir);

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

        /// <summary>
        /// Add an item to a tile
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="to"></param>
        public void AddItem(Item item, Backend.Coords to)
        {
            // Create ItemTile (if non existant)
            // or: Remove ItemTile from current tile
            // Add ItemTile to new Tile
        }

        /// <summary>
        /// Move an item on the map (not carried by an actor) in a specified direction 
        /// </summary>
        /// <param name="item">item to move</param>
        /// <param name="from">square from which to move item</param>
        /// <param name="dir">Direction to move to</param>
        public void MoveItem(Item item, Backend.Coords from, Direction dir)
        {
            // or: Remove ItemTile from current tile specified by coords
            // Add ItemTile to new Tile
        }


        /// <summary>
        /// Get coordinates for closest enemy within a specified radius
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="radius"></param>
        /// <param name="includePlayer"></param>
        /// <param name="includeNPC"></param>
        /// <param name="includeEnemy"></param>
        /// <returns></returns>
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

        public void PathTo(Coords from, Backend.Coords to, out List<Coords> result, ref SortedSet<Coords> visited, int maxlength = 20, string indent = "")
        {
            result = null;
            if (visited == null)
            {
                visited = new SortedSet<Coords>();
            }
            visited.Add(from);

            if (maxlength > 0)
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

                        PathTo(tmp, to, out result, ref visited, maxlength - 1, indent + " ");
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

        public FloorTile TileByCoords(Coords coords)
        {
            return this[coords.x, coords.y];
        }
        public void RemoveActor(int x, int y)
        {
            _tiles[y][x].Remove(TileType.Enemy);
        }

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

        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (!DownStream)
            {
                switch (eventID)
                {
                    default:
                        ((Backend.IHandleEvent)_parent).HandleEvent(false, eventID, data);
                        break;
                }
            }
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        /// <param name="player">The starting position on the loaded map</param>
        public void Load(string filename, Backend.Coords player = null)
        {
            Regex re = new Regex(@"\d+");
            Match m = re.Match(filename);
            _id = Convert.ToInt32(m.Value);
            bool isReady = false;
            Player playerA = null;

            if (player == null)
            {
                player = new Backend.Coords(1, 1);
            }
            if (_actors.Count > 0)
            {
                for (int i = 0; i < _actors.Count; ++i)
                {
                    if (_actors[i] is Player)
                    {
                        playerA = (Player)_actors[i];
                        playerA.tile = null;
                        isReady = true;
                        break;
                    }
                }
                _actors.Clear();
            }
            else
            {
                playerA = new Player(_content, 100, 0, 30);
            }
            _actors.Add(playerA);

            _tiles.Clear();
            _items.Clear();
            _updateTiles.Clear();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader xmlr = XmlReader.Create(filename, settings);
            xmlr.MoveToContent();//xml
            _width = int.Parse(xmlr.GetAttribute("width"));
            _height = int.Parse(xmlr.GetAttribute("height"));
            if (xmlr.GetAttribute("name") != null) _name = xmlr.GetAttribute("name");
            if (xmlr.GetAttribute("level") != null) _level = int.Parse(xmlr.GetAttribute("level"));
            if (xmlr.GetAttribute("dungeon") != null) _dungeonname = xmlr.GetAttribute("dungeon");
            if (xmlr.GetAttribute("floor") != null) _floorFile = xmlr.GetAttribute("floor");
            if (xmlr.GetAttribute("wall") != null) _wallFile = xmlr.GetAttribute("wall");
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
                                            Direction dir = Direction.None;
                                            if (xmlr.GetAttribute("direction", _id.ToString()) != null) dir = (Direction)Enum.Parse(typeof(Direction), xmlr.GetAttribute("direction").ToString());
                                            ProjectileTile projectile = new ProjectileTile(tile, dir, id);
                                        }
                                        break;
                                    case "ItemTile":
                                        Item item = new Item(_content);
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
                                        if (xmlr.GetAttribute("locked") != null)
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
                                                actor = new Enemy(_content);
                                                actor.Load(xmlr);
                                                break;
                                            case "Player":
                                                actor = new Player(_content);
                                                actor.Load(xmlr);
                                                break;
                                            default:
                                                actor = new NPC(_content);
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
                                            if (!isReady)
                                            {
                                                _actors[0].copyFrom(actor);
                                                player.x = tile.coords.x;
                                                player.y = tile.coords.y;
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

            xmlr.Close();

            ActorTile actortile = new ActorTile(_tiles[player.y][player.x], actors[0]);
            _tiles[player.y][player.x].Add(actortile);
            actors[0].tile = actortile;
            actortile.parent = _tiles[player.y][player.x];
            _updateTiles.Add(player);
            for (int i = 0; i < _actors.Count; ++i)
            {
                _actors[i].id = i;
            }
            Uncover(actors[0].tile.coords, actors[0].viewRange);
        }

        public void DebugMap()
        {
            string output = "";
            foreach (List<FloorTile> row in _tiles)
            {
                foreach (FloorTile tile in row)
                {
                    if (tile.canEnter)
                    {
                        output += "#";
                    }
                    else
                    {
                        output += " ";
                    }
                }
                // System.Diagnostics.Debug.WriteLine(output);
                output = "";

            }
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

        public Map(ContentManager content)
        {
            _content = content;
            _updateTiles = new List<Coords>();
            _actors = new List<Actor>();
            _items = new List<Item>();
            _blankTile = new FloorTile(this);
            _blankTile.coords = new Backend.Coords(-1, -1);
            _blankTile.Add(new GapTile(_blankTile));
            _tiles = new List<List<FloorTile>>();
            _exits = new List<Exit>();

        }

        /// <summary>
        /// Constructor for using a previously saved map
        /// </summary>
        /// <param name="filename"></param>
        public Map(ContentManager content, object parent, string filename = "", Backend.Coords playerPos = null)
            : this(content)
        {
            _parent = parent;
            Load(filename, playerPos);
        }


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
            _items.Clear();
        }
        #endregion

        #region Static Helpers
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
