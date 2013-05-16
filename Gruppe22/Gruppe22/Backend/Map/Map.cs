﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Gruppe22
{
    public class Exit
    {
        private Coords _from;
        private Coords _to;
        private string _fromRoom;
        private string _toRoom;

        public Coords from
        {
            get { return _from; }
        }

        public Coords to
        {
            get { return _to; }
        }
        public string fromRoom { get { return _fromRoom; } }

        public string toRoom { get { return _toRoom; } }

        public Exit(Coords from, string fromRoom, Coords to = null, string toRoom = "")
        {
            _from = from;
            _fromRoom = fromRoom;
            _toRoom = toRoom;
            _to = to;
        }
    }
    public class Map : IHandleEvent, IDisposable
    {
        #region Private Fields
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

        public List<Actor> _actors = null;
        public List<Item> _items = null;
        private List<Coords> _updateTiles = null;
        #endregion

        #region Public Fields


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
        /// CUrrent Width of the maze
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
        public void AddActor(Item item, Coords from)
        {

        }

        /// <summary>
        /// Move an actor on the map in a specified direction (does not check for walls - use CanMove)
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="from"></param>
        /// <param name="dir"></param>
        public void MoveActor(Actor actor, Direction dir)
        {
            Coords source = actor.tile.coords;
            Coords target = DirectionTile(actor.tile.coords, dir);

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
        public void AddItem(Item item, Coords to)
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
        public void MoveItem(Item item, Coords from, Direction dir)
        {
            // or: Remove ItemTile from current tile specified by coords
            // Add ItemTile to new Tile
        }

        public Coords DirectionTile(Coords start, Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return new Coords(start.x - 1, start.y);

                case Direction.Right:
                    return new Coords(start.x + 1, start.y);

                case Direction.Down:
                    return new Coords(start.x, start.y + 1);

                case Direction.Up:
                    return new Coords(start.x, start.y - 1);

                case Direction.DownLeft:
                    return new Coords(start.x - 1, start.y + 1);

                case Direction.UpRight:
                    return new Coords(start.x + 1, start.y - 1);

                case Direction.DownRight:
                    return new Coords(start.x + 1, start.y + 1);

                case Direction.UpLeft:
                    return new Coords(start.x - 1, start.y - 1);
            }
            return start;
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

        public virtual void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {
            ((IHandleEvent)_parent).HandleEvent(sender, eventID, data);
        }


        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        /// <returns>true if read was successful</returns>
        public void Load(string filename, Coords player)
        {
            Player playerA = null;
            if (player == null)
            {
                player = new Coords(1, 1);
            }
            if (_actors.Count > 0)
            {
                for (int i = 0; i < _actors.Count; ++i)
                {
                    if (_actors[i] is Player)
                    {
                        playerA = (Player)_actors[i];
                        playerA.tile = null;
                        break;
                    }
                }
                _actors.Clear();
            }
            else
            {
                playerA = new Player(100, 0, 30);
            }
            _actors.Add(playerA);

            _tiles.Clear();
            _items.Clear();
            _updateTiles.Clear();
            XmlReader xmlr = XmlReader.Create(filename);
            xmlr.MoveToContent();//xml
            _width = int.Parse(xmlr.GetAttribute("width"));
            _height = int.Parse(xmlr.GetAttribute("height"));
            xmlr.ReadStartElement("GameMap");//GameMap

            for (int row = 0; row < _height; ++row)
            {
                _tiles.Add(new List<FloorTile>());
                for (int col = 0; col < _width; ++col)
                {
                    _tiles[row].Add(new FloorTile(this, new Coords(col, row)));
                }
            }

            while ((xmlr.NodeType != XmlNodeType.EndElement) && (xmlr.NodeType != XmlNodeType.None))
            { // Add Tiles and overlay-Tiles
                switch (xmlr.Name)
                {
                    case "Tile":
                        FloorTile tile = _tiles[Int32.Parse(xmlr.GetAttribute("CoordY"))][Int32.Parse(xmlr.GetAttribute("CoordX"))];
                        if (!xmlr.IsEmptyElement)
                        {
                            xmlr.Read();

                            while ((xmlr.NodeType != XmlNodeType.EndElement))
                            {
                                switch (xmlr.Name)
                                {
                                    case "WallTile":
                                        tile.Add(TileType.Wall);
                                        break;
                                    case "ItemTile":

                                        ItemType type = ItemType.Armor;
                                        string name = "";
                                        int strength = 0;


                                        if (xmlr.GetAttribute("type") != null)
                                        {
                                            type = (ItemType)Enum.Parse(typeof(ItemType), xmlr.GetAttribute("type").ToString());
                                        }

                                        if (xmlr.GetAttribute("name") != null)
                                        {
                                            name = xmlr.GetAttribute("name").ToString();
                                        }

                                        if (xmlr.GetAttribute("strength") != null)
                                        {
                                            strength = Int32.Parse(xmlr.GetAttribute("strength").ToString());
                                        }
                                        Item item = new Item(type, name, strength);

                                        ItemTile itemTile = new ItemTile(tile, item);
                                        item.tile = itemTile;
                                        tile.Add(itemTile);
                                        break;
                                    case "TargetTile":
                                        tile.Add(new TargetTile(tile));
                                        break;
                                    case "TrapTile":
                                        tile.Add(new TrapTile(this, Int32.Parse(xmlr.GetAttribute("damage"))));
                                        _updateTiles.Add(tile.coords);
                                        break;
                                    case "TeleportTile":
                                        tile.Add(new TeleportTile(tile, xmlr.GetAttribute("nextRoom"), new Coords(Int32.Parse(xmlr.GetAttribute("nextX")), Int32.Parse(xmlr.GetAttribute("nextY")))));
                                        break;
                                    case "ActorTile":
                                        string actorname = "";
                                        int actorhealth = 40;
                                        int maxHealth = -1;
                                        int armour = 0;
                                        int damage = 20;
                                        ActorType atype = ActorType.Enemy;

                                        if (xmlr.GetAttribute("name") != null)
                                        {
                                            actorname = xmlr.GetAttribute("name").ToString();
                                        }

                                        if (xmlr.GetAttribute("health") != null)
                                        {
                                            actorhealth = Int32.Parse(xmlr.GetAttribute("health").ToString());
                                        }

                                        if (xmlr.GetAttribute("maxhealth") != null)
                                        {
                                            maxHealth = Int32.Parse(xmlr.GetAttribute("maxhealth").ToString());
                                        }

                                        if (xmlr.GetAttribute("armor") != null)
                                        {
                                            armour = Int32.Parse(xmlr.GetAttribute("armor").ToString());
                                        }
                                        if (xmlr.GetAttribute("damage") != null)
                                        {
                                            damage = Int32.Parse(xmlr.GetAttribute("damage").ToString());
                                        }

                                        if (xmlr.GetAttribute("type") != null)
                                        {
                                            atype = (ActorType)Enum.Parse(typeof(ActorType), xmlr.GetAttribute("type").ToString());
                                        }

                                        if (xmlr.GetAttribute("player") != null)
                                        {
                                            atype = ActorType.Player;
                                        }
                                        if (atype!=ActorType.Player)
                                        {
                                            Enemy enemy = (new Enemy(actorhealth, armour, damage, maxHealth, actorname));
                                            ActorTile tile2 = new ActorTile(tile, enemy);
                                            enemy.tile = tile2;
                                            tile.Add(tile2);
                                            _actors.Add(enemy);
                                            _updateTiles.Add(tile.coords);
                                        }
                                        break;
                                }
                                xmlr.Read();
                            }
                        }
                        xmlr.Read();
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
                System.Diagnostics.Debug.WriteLine(output);
                output = "";

            }
        }

        /// <summary>
        /// Write the current map to a file
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        /// <returns>true if writing was successful</returns>
        public virtual void Save(string filename)
        {
            XmlWriter xmlw = XmlWriter.Create(filename);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            //_tiles[0][0].overlay.Add(new TeleportTile("map2.xml", new Microsoft.Xna.Framework.Vector2(0,0)));//test
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

        public Map()
        {
            _updateTiles = new List<Coords>();
            _actors = new List<Actor>();
            _items = new List<Item>();
            _blankTile = new FloorTile(this);
            _blankTile.coords = new Coords(0, 0);
            _tiles = new List<List<FloorTile>>();
            _exits = new List<Exit>();

        }


        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename"></param>
        public Map(object parent, string filename = "", Coords playerPos = null)
            : this()
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
        public static Direction WhichWayIs(Coords from, Coords to)
        {
            if (from.x < to.x)
            {
                if (from.y < to.y)
                {

                    return Direction.UpLeft;
                }
                else
                {
                    if (from.y > to.y)
                    {
                        return Direction.DownLeft;
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
                        return Direction.UpRight;
                    }
                    else
                    {
                        if (from.y > to.y)
                        {
                            return Direction.DownRight;
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
