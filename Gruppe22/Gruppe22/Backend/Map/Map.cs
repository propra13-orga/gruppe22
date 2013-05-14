using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Gruppe22
{
    public class Map : IHandleEvent, IDisposable
    {
        #region Private Fields
        private object _parent = null;

        /// <summary>
        /// A two dimensional list of tiles
        /// </summary>
        private List<List<Tile>> _tiles = null;
        /// <summary>
        /// Internal current width
        /// </summary>
        private int _width = 10;

        /// <summary>
        /// Internal current height
        /// </summary>
        private int _height = 10;

        /// <summary>
        /// Blank tile returned when requesting tile outside map boundaries (i.e. negative values / values beyond width or height)
        /// </summary>
        private Tile _blankTile = null;

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
                foreach (Coords coords in _updateTiles)
                {
                    if (_tiles[coords.y][coords.x].hasEnemy || _tiles[coords.y][coords.x].hasPlayer)
                    {
                        if (_tiles[coords.y][coords.x].hasPlayer)
                        {
                            result.Insert(0, coords);
                        }
                        else
                        {
                            result.Add(coords);
                        };
                    }
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
        public Tile this[int x, int y]
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

        #region Private Methods
        /// <summary>
        /// Get the next tile based on included Connection info
        /// </summary>
        /// <param name="current">The start field</param>
        /// <returns>The next field or null if field would be beyond map</returns>
        private Path _Walk(Path current)
        {
            Path result = new Path(current.x, current.y, Connection.None);

            switch (current.dir)
            {
                case Connection.Up:
                    result.y -= 2;
                    break;
                case Connection.Right:
                    result.x += 2;
                    break;
                case Connection.Down:
                    result.y += 2;
                    break;
                case Connection.Left:
                    result.x -= 2;
                    break;
            };

            if ((result.x >= 1) && (result.x <= 2 * _width) && (result.y >= 0) && (result.y <= 2 * _height))
                return result;
            return null;
        }

        private void _NextTile(ref Path pos)
        {
            int count = 0;
            while ((_tiles[pos.y][pos.x].connected) && (count < _width * _height))
            {
                count += 1;
                pos.x += 2;
                if (pos.x > _width * 2)
                {
                    pos.x = 1;
                    pos.y += 2;
                };
                if (pos.y > _height * 2)
                {
                    pos.y = 1;
                    pos.x = 1;
                }
            }
            if (count >= _width * _height)
            {
                pos.x = -1;
                pos.y = -1;
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
            foreach (Coords c in _updateTiles)
            {
                _tiles[c.y][c.x].Update(gameTime);
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
            ((Tile)actor.tile.parent).Remove(actor.tile);
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

        public int firstActorID(int x,int y){
            int result = 0, i=0;
            foreach (Coords c in _updateTiles)
            {
                if ((_tiles[c.y][c.x].hasPlayer) && (result != 0)) result += 1;
                if ((_tiles[c.y][c.x].hasPlayer) || (_tiles[c.y][c.x].hasEnemy)) i += 1;
                if ((c.x == x) && (c.y == y))
                {
                    result = i;
                }
            }
            return result;
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
        /// Create a grid of walls
        /// </summary>
        public void ClearMaze()
        {
            foreach (List<Tile> row in _tiles)
            {
                row.Clear();
            }
            _tiles.Clear();
            for (int row = 0; row < _height * 2 + 1; row++)
            {
                _tiles.Add(new List<Tile>());
                for (int col = 0; col < _width * 2 + 1; col++)
                {
                    _tiles[row].Add(new Tile(this, new Coords(col, row),

                        (row % 2 == 1)
                        && (col % 2 == 1))
                        );
                }
            }
        }


        public void AddTraps(int amount = -1)
        {
            if (amount < 0) amount = 5;
            Random r = new Random();
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) && (!_tiles[pos.y][pos.x].canEnter))
                {
                    count += 1;
                    pos.x += 2;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 2;
                        pos.y += 2;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 2;
                        pos.x = 2;
                    }
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    TrapTile trapTile = new TrapTile(_tiles[pos.y][pos.x], 1);
                    _tiles[pos.y][pos.x].Add(trapTile);
                    _updateTiles.Add(pos);
                }
            }

        }

        public virtual void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {
            ((IHandleEvent)_parent).HandleEvent(sender, eventID, data);
        }

        public void AddPlayer(Coords pos)
        {
            if (pos == null)
            {
                pos = new Coords(1, 1);
            }
            Player player = new Player("Klaus", 100, 20, 20);
            ActorTile playerTile = new ActorTile(_tiles[pos.y][pos.x], player);
            player.tile = playerTile;
            _tiles[pos.y][pos.x].Add(playerTile);
            _updateTiles.Add(pos);
            _actors.Add(player);
        }

        public void AddEnemies(int amount = -1)
        {
            if (amount < 0) amount = 5;
            Random r = new Random();
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) &&
                    (_tiles[pos.y][pos.x].overlay.Count == 0))
                {
                    count += 1;
                    pos.x += 2;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 2;
                        pos.y += 2;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 2;
                        pos.x = 2;
                    }
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    Enemy enemy = new Enemy(50, 90, 20);
                    ActorTile enemyTile = new ActorTile(_tiles[pos.y][pos.x], enemy);
                    enemy.tile = enemyTile;
                    _tiles[pos.y][pos.x].Add(enemyTile);
                    _updateTiles.Add(pos);
                    _actors.Add(enemy);
                }
            }

        }


        public void AddDoors(int amount = -1)
        {
            if (amount < 0) amount = 5;
            if (amount < 0) amount = 5;
            Random r = new Random();
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) && (!_tiles[pos.y][pos.x].canEnter))
                {
                    count += 1;
                    pos.x += 2;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 2;
                        pos.y += 2;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 2;
                        pos.x = 2;
                    }
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    TeleportTile teleportTile = new TeleportTile(this, "Raum2", pos);
                    _tiles[pos.y][pos.x].Add(teleportTile);
                    _updateTiles.Add(pos);
                }
            }

        }


        public void AddItems(int amount = -1)
        {
            if (amount < 0) amount = 5;
            Random r = new Random();
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) &&
                    (_tiles[pos.y][pos.x].overlay.Count == 0))
                {
                    count += 1;
                    pos.x += 2;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 2;
                        pos.y += 2;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 2;
                        pos.x = 2;
                    }
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    Item item = new Item();
                    ItemTile itemTile = new ItemTile(_tiles[pos.y][pos.x], item);
                    item.tile = itemTile;
                    _tiles[pos.y][pos.x].Add(itemTile);
                    _items.Add(item);
                }
            }
        }

        public void ClearWalls(int number = -1)
        {
            if (number < 0)
            {
                number = ((width / 2) * (height / 2)) / 4;
            };
            Random r = new Random();

            for (int i = 0; i < number; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) && (_tiles[pos.y][pos.x].canEnter))
                {
                    count += 1;
                    pos.x += 2;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 2;
                        pos.y += 2;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 2;
                        pos.x = 2;
                    }
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
                _tiles[pos.y][pos.x].Remove(TileType.Wall);
            }
        }

        /// <summary>
        /// Create a new maze
        /// </summary>
        /// <param name="slow"></param>
        public void GenerateMaze()
        {
            ClearMaze();

            Random r = new Random();

            Path originPos = new Path(1 + r.Next(_width - 1) * 2, 1 + r.Next(_height - 1) * 2);
            Path currentPos = new Path(1 + r.Next(_width - 1) * 2, 1 + r.Next(_height - 1) * 2);

            _tiles[originPos.y][originPos.x].connected = true;
            _tiles[originPos.y][originPos.x].Remove(TileType.Wall);
            int remaining = _width * _height - 1;

            Path startPos = originPos;
            while ((currentPos.x > -1) && (remaining > 0))
            {
                _NextTile(ref startPos);
                currentPos = startPos;

                while ((currentPos.x > 0) && (!_tiles[currentPos.y][currentPos.x].connected))
                {
                    currentPos.dir = (Connection)r.Next(4) + 1;
                    Path next = _Walk(currentPos);
                    int dirCount = 0;
                    while ((next == null) && (dirCount < 4))
                    {
                        dirCount += 1;
                        currentPos.dir += 1;
                        if (currentPos.dir == Connection.Invalid)
                            currentPos.dir = Connection.Up;
                        next = _Walk(currentPos);
                    }
                    if (dirCount == 4) break;
                    _tiles[currentPos.y][currentPos.x].connection = currentPos.dir;
                    currentPos = next;
                }
                Path endPos = currentPos;
                currentPos = startPos;
                while ((currentPos.x != endPos.x) || (currentPos.y != endPos.y))
                {
                    _tiles[currentPos.y][currentPos.x].connected = true;
                    switch (_tiles[currentPos.y][currentPos.x].connection)
                    {
                        case Connection.Left:
                            _tiles[currentPos.y][currentPos.x - 1].Remove(TileType.Wall);
                            break;
                        case Connection.Right:
                            _tiles[currentPos.y][currentPos.x + 1].Remove(TileType.Wall);
                            break;
                        case Connection.Up:
                            _tiles[currentPos.y - 1][currentPos.x].Remove(TileType.Wall);
                            break;
                        case Connection.Down:
                            _tiles[currentPos.y + 1][currentPos.x].Remove(TileType.Wall);
                            break;
                    }
                    currentPos.dir = _tiles[currentPos.y][currentPos.x].connection;
                    _tiles[currentPos.y][currentPos.x].connection = Connection.Invalid;
                    currentPos = _Walk(currentPos);
                    startPos = new Path(1 + r.Next(_width - 1) * 2, 1 + r.Next(_height - 1) * 2);
                    --remaining;
                }
            }
            _width = _width * 2 + 1;
            _height = _height * 2 + 1;
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        /// <returns>true if read was successful</returns>
        public void Load(string filename, Coords player)
        {
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
                        _actors.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        _actors[i].tile = null;
                    }
                }
            }
            else
            {
                Player playerA = new Player("Klaus", 100, 20, 20);
                _actors.Add(playerA);
            }
            _tiles.Clear();
            _items.Clear();
            _updateTiles.Clear();
            XmlReader xmlr = XmlReader.Create(filename);
            xmlr.MoveToContent();//xml
            _width = int.Parse(xmlr.GetAttribute("width"));
            _height = int.Parse(xmlr.GetAttribute("height"));
            xmlr.ReadStartElement("GameMap");//GameMap

            for (int r = 0; r < _height; r++)
            { // Add Rows
                xmlr.Read();//row
                //TODO: dynamisches einlesen von overlay-Tiles
                _tiles.Add(new List<Tile>());
                for (int j = 0; j < _width; j++)
                { // Add Tiles and overlay-Tiles
                    switch (xmlr.Name)
                    {
                        case "Tile":
                            Tile tile = new Tile(this, new Coords(j, r), true);
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
                                        case "ActorTile":
                                            if ((j != 1) || (r != 1))
                                            {
                                                Enemy enemy = (new Enemy(40, 0, 20));
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
                            _tiles[r].Add(tile);
                            xmlr.Read();

                            break;
                        default:
                            j--;
                            xmlr.Read();
                            break;
                    }
                }
            }
            xmlr.Close();

            ActorTile actortile = new ActorTile(_tiles[player.y][player.x], actors[0]);
            _tiles[player.y][player.x].Add(actortile);
            actors[0].tile = actortile;
            actortile.parent = _tiles[player.y][player.x];
            _updateTiles.Add(player);
        }

        public void DebugMap()
        {
            string output = "";
            foreach (List<Tile> row in _tiles)
            {
                foreach (Tile tile in row)
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
        public void Save(string filename)
        {
            XmlWriter xmlw = XmlWriter.Create(filename);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            //_tiles[0][0].overlay.Add(new TeleportTile("map2.xml", new Microsoft.Xna.Framework.Vector2(0,0)));//test
            foreach (List<Tile> ltiles in _tiles)
            {
                xmlw.WriteStartElement("Row");
                foreach (Tile tile in ltiles)
                {
                    tile.Save(xmlw);
                }
                xmlw.WriteEndElement();
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
            _blankTile = new Tile();
            _tiles = new List<List<Tile>>();

        }

        /// <summary>
        /// Create an empty map
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="height">The height of the map</param>
        public Map(object parent = null, int width = 10, int height = 10, bool generate = false, Coords playerPos = null)
            : this()
        {
            _parent = parent;
            _width = width;
            _height = height;
            for (int y = 0; y < height; ++y)
            {
                _tiles.Add(new List<Tile>());
                for (int x = 0; x < width; ++x)
                {
                    _tiles[y].Add(new Tile(this, new Coords(x, y)));
                }
            }
            if (generate)
            {
                ClearMaze();
                GenerateMaze();
                ClearWalls();
                AddPlayer(playerPos);
                AddTraps();
                AddDoors();
                AddEnemies();
                AddItems();
            }
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename"></param>
        public Map(object parent = null, string filename = "", Coords playerPos = null)
            : this()
        {
            _parent = parent;
            Load(filename, playerPos);
        }



        public void Dispose()
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
    }
}
