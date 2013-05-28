using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    class Generator : Map
    {
        private new List<List<GeneratorTile>> _tiles = null;
        private Random r;

        public void AddPlayer(Coords pos)
        {
            if (pos == null)
            {
                pos = new Coords(1, 1);
            }
            Player player = new Player(100, 0, 30);
            ActorTile playerTile = new ActorTile(_tiles[pos.y][pos.x], player);
            player.tile = playerTile;
            _tiles[pos.y][pos.x].Add(playerTile);
            _actors.Add(player);
        }

        /// <summary>
        /// Write the current map to a file
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        /// <returns>true if writing was successful</returns>
        public override void Save(string filename)
        {
            XmlWriter xmlw = XmlWriter.Create(filename);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            //_tiles[0][0].overlay.Add(new TeleportTile("map2.xml", new Microsoft.Xna.Framework.Vector2(0,0)));//test
            foreach (List<GeneratorTile> ltiles in _tiles)
            {
                foreach (GeneratorTile tile in ltiles)
                {
                    if (tile.overlay.Count > 0)
                        tile.Save(xmlw);
                }
            }
            xmlw.WriteEndElement();
            xmlw.WriteEndDocument();
            xmlw.Close();
        }

        public void AddEnemies(int amount = -1)
        {
            if (amount < 0) amount = 5;
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(1 + r.Next(_width - 2), 1 + r.Next(_height - 2));
                while ((count < _width * height) && (pos.x > 0)
         && (_tiles[pos.y][pos.x].overlay.Count != 0)
         )
                {
                    count += 1;
                    pos.x += 1;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 1;
                        pos.y += 1;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 1;
                        pos.x = 1;
                    }
                    if (count >= _width * _height)
                    {
                        pos.x = -1;
                        pos.y = -1;
                    }
                }


                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    Enemy enemy = new Enemy(-1, -1, -1, -1, "", r);
                    ActorTile enemyTile = new ActorTile(_tiles[pos.y][pos.x], enemy);
                    enemy.tile = enemyTile;
                    _tiles[pos.y][pos.x].Add(enemyTile);
                    _actors.Add(enemy);
                }
            }

        }

        public void AddTarget()
        {

            int count = 0;
            Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
            while ((pos.x > 0) &&
                (_tiles[pos.y][pos.x].overlay.Count != 0))
            {
                count += 1;
                pos.x += 1;
                if (pos.x > _width - 2)
                {
                    pos.x = 1;
                    pos.y += 1;
                };
                if (pos.y > _height - 2)
                {
                    pos.y = 1;
                    pos.x = 1;
                }

                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                }
            }

            if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
            {

                TargetTile target = new TargetTile(_tiles[pos.y][pos.x]);
                _tiles[pos.y][pos.x].Add(target);
            }
        }

        public void AddDoors(int roomID = 1, int maxRoom = 3, List<Exit> doors = null)
        {
            if (doors != null)
            {
                foreach (Exit door in doors)
                {
                    _exits.Add(door);
                    _tiles[door.from.y][door.from.x].Remove(TileType.Wall);
                    _tiles[door.from.y][door.from.x].Add(new TeleportTile(_tiles[door.from.y][door.from.x], door.toRoom, door.to));
                }
            }
            if (roomID != maxRoom)
            {

                Direction targetWall = Direction.Right;


                if (r.Next(2) == 0)
                {
                    targetWall = Direction.Down;
                }
                else
                {
                    targetWall = Direction.Right;
                }

                int y = 1 + r.Next(_height / 2 - 2) * 2;
                int x = 1 + r.Next(_width / 2 - 2) * 2;


                if (targetWall == Direction.Up) // This determines the height (!) of the next map, indicated by negative numbers
                {
                    y = -(10 + r.Next(20));
                }

                if (targetWall == Direction.Left)  // This determines the width (!) of the next map, indicated by negative numbers
                {
                    x = -(10 + r.Next(20));
                }

                switch (targetWall)
                {
                    case Direction.Down:
                        _tiles[_height - 1][x].Remove(TileType.Wall);
                        _tiles[_height - 1][x].Add(new TeleportTile(_tiles[_height - 1][x], "room" + (roomID + 1).ToString() + ".xml", new Coords(x, 0)));
                        _exits.Add(new Exit(new Coords(x, _height - 1), "room" + (roomID).ToString() + ".xml", new Coords(x, 0), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Up:
                        _tiles[0][x].Remove(TileType.Wall);
                        _tiles[0][x].Add(new TeleportTile(_tiles[0][x], "room" + (roomID + 1).ToString() + ".xml", new Coords(x, y)));
                        _exits.Add(new Exit(new Coords(x, 0), "room" + (roomID).ToString() + ".xml", new Coords(x, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Right:
                        _tiles[y][_width - 1].Remove(TileType.Wall);
                        _tiles[y][_width - 1].Add(new TeleportTile(_tiles[y][_width - 1], "room" + (roomID + 1).ToString() + ".xml", new Coords(0, y)));
                        _exits.Add(new Exit(new Coords(_width - 1, y), "room" + (roomID).ToString() + ".xml", new Coords(0, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Left:
                        _tiles[y][0].Remove(TileType.Wall);
                        _tiles[y][0].Add(new TeleportTile(_tiles[y][0], (roomID + 1).ToString() + ".xml", new Coords(x, y)));
                        _exits.Add(new Exit(new Coords(0, y), (roomID).ToString() + ".xml", new Coords(x, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                }
            }
        }


        public void AddItems(int amount = -1)
        {
            if (amount < 0) amount = 5;
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(2 + r.Next(_width / 2 - 2) * 2, 2 + r.Next(_height / 2 - 2) * 2);
                while ((pos.x > 0) &&
                    (_tiles[pos.y][pos.x].overlay.Count != 0))
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

                    if (count >= _width * _height)
                    {
                        pos.x = -1;
                        pos.y = -1;
                    }
                }

                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    Item item = new Item(r);
                    ItemTile itemTile = new ItemTile(_tiles[pos.y][pos.x], item);
                    item.tile = itemTile;
                    _tiles[pos.y][pos.x].Add(itemTile);
                    _items.Add(item);
                }
            }
        }

        /// <summary>
        /// Remove walls at random for more space
        /// </summary>
        /// <param name="amount">Amount of walls to remove</param>
        public void ClearWalls(int amount = -1)
        {
            if (amount < 0)
            {
                amount = (int)(((((float)width - 1) / 2) * (((float)height - 1) / 2)) / 4); // every 4th wall!
            };

            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 2) / 2) * 2);
                while ((pos.x > 0) && (_tiles[pos.y][pos.x].canEnter))
                {
                    count += 1;
                    pos.x += 1;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 1;
                        pos.y += 1;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 1;
                        pos.x = 1;
                    }
                    if (count >= _width * _height)
                    {
                        pos.x = -1;
                        pos.y = -1;
                    }
                }
                _tiles[pos.y][pos.x].Remove(TileType.Wall);
            }

            // Remove bad looking places: Walls surrounded by free space become free space, free space surrounded by walls becomes free space surrounded by free space
            for (int y = 1; y < _height - 1; ++y)
            {
                for (int x = 1; x < _width - 1; ++x)
                {
                    int _wallsaround = 0;
                    int _freearound = 0;
                    if (_tiles[y - 1][x].canEnter)
                    {
                        _freearound += 1;
                    }
                    else
                    {
                        _wallsaround += 1;
                    };
                    if (_tiles[y][x - 1].canEnter)
                    {
                        _freearound += 1;
                    }
                    else
                    {
                        _wallsaround += 1;
                    };
                    if (_tiles[y][x + 1].canEnter)
                    {
                        _freearound += 1;
                    }
                    else
                    {
                        _wallsaround += 1;
                    };
                    if (_tiles[y + 1][x].canEnter)
                    {
                        _freearound += 1;
                    }
                    else
                    {
                        _wallsaround += 1;
                    };
                    if (_freearound == 4)
                        _tiles[y][x].Remove(TileType.Wall);
                    if (_wallsaround == 4)
                    {
                        if (y > 1)
                            _tiles[y - 1][x].Remove(TileType.Wall);
                        if (x > 1)
                            _tiles[y][x - 1].Remove(TileType.Wall);
                        if (y < _height - 2)
                            _tiles[y + 1][x].Remove(TileType.Wall);
                        if (x < _width - 2)
                            _tiles[y][x + 1].Remove(TileType.Wall);

                        y = y - ((y > 1) ? 2 : 1); // restart at wall above
                        x = x - ((x > 1) ? 2 : 1); // restart at wall left

                        if (y < 1) y = 1;
                        if (x < 1) x = 1;

                    }
                }
            }
        }



        /// <summary>
        /// Create a grid of walls
        /// </summary>
        public void ClearMaze()
        {
            foreach (List<GeneratorTile> row in _tiles)
            {
                row.Clear();
            }
            _tiles.Clear();
            if (_height % 2 == 0) _height += 1;
            if (_width % 2 == 0) _width += 1;
            for (int row = 0; row < _height; row++)
            {
                _tiles.Add(new List<GeneratorTile>());
                for (int col = 0; col < _width; col++)
                {
                    _tiles[row].Add(new GeneratorTile(this, new Coords(col, row),

                        (row % 2 == 1)
                        && (col % 2 == 1))
                        );
                }
            }
        }


        public void AddTraps(int amount = -1)
        {
            if (amount < 0) amount = 5;
            for (int i = 0; i < amount; ++i)
            {
                int count = 0;
                Path pos = new Path(1 + r.Next(_width - 2), 1 + r.Next(_height - 2));

                while ((count < _width * height) && (pos.x > 0)
                    && (_tiles[pos.y][pos.x].overlay.Count != 0)
                    )
                {
                    count += 1;
                    pos.x += 1;
                    if (pos.x > _width - 2)
                    {
                        pos.x = 1;
                        pos.y += 1;
                    };
                    if (pos.y > _height - 2)
                    {
                        pos.y = 1;
                        pos.x = 1;
                    }
                    if (count >= _width * _height)
                    {
                        pos.x = -1;
                        pos.y = -1;
                    }
                }


                if ((pos.x >= 0) && (pos.x < _width) && (pos.y < _height) && (pos.y >= 0))
                {
                    TrapTile trapTile = new TrapTile(_tiles[pos.y][pos.x], r.Next(10) + 5);
                    _tiles[pos.y][pos.x].Add(trapTile);
                }
            }

        }
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

            if ((result.x > 0) && (result.x < _width - 1)
                && (result.y > 0) && (result.y < _height - 1))
                return result;
            return null;
        }

        private void _NextTile(ref Path pos)
        {
            int count = 0;
            while (_tiles[pos.y][pos.x].connected)
            {
                count += 1;
                pos.x += 2;
                if (pos.x > _width - 2)
                {
                    pos.x = 1;
                    pos.y += 2;
                };
                if (pos.y > _height - 2)
                {
                    pos.y = 1;
                    pos.x = 1;
                }
                if (count >= _width * _height)
                {
                    pos.x = -1;
                    pos.y = -1;
                    return;
                }
            }
        }
        /// <summary>
        /// Create a new maze
        /// </summary>
        /// <param name="slow"></param>
        public void GenerateMaze()
        {



            Path originPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 2) / 2) * 2);
            Path currentPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 3) / 2) * 2);

            _tiles[originPos.y][originPos.x].connected = true;
            _tiles[originPos.y][originPos.x].Remove(TileType.Wall);
            int remaining = _width / 2 * _height / 2 - 2;

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
                    --remaining;
                }
                startPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 2) / 2) * 2);
            }
            ClearWalls(); // Remove random walls
        }

        public bool FromString(string input, int roomID, int MaxRoom)
        {
            int col = 0, row = 0, maxcol = 0;
            _tiles.Add(new List<GeneratorTile>());
            foreach (char c in input)
            {
                switch (c)
                {
                    case '#':
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row)));
                        _tiles[row][col].Add(TileType.Wall);
                        col += 1;
                        break;
                    case '\n':
                                                if (col > maxcol) { maxcol = col; };

                        col = 0;
                        row += 1;
                        _tiles.Add(new List<GeneratorTile>());
                        break;
                    case 'S':
                        
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row)));
                        
                        if (roomID != 1)
                        {
                            _tiles[row][col].Add(new TeleportTile(_tiles[row][col], "room" + (roomID - 1).ToString() + ".xml", new Coords(col, row)));
                            _exits.Add(new Exit(new Coords(col, row), "room" + (roomID).ToString() + ".xml", new Coords(col, row), "room" + (roomID - 1).ToString() + ".xml"));
                        }
                        else
                        {
                            AddPlayer(new Coords(col, row));
                        }
                        col += 1;
                        break;
                    case 'G':
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row)));
                        
                        if (roomID == MaxRoom)
                        {
                            TargetTile target = new TargetTile(_tiles[row][col]);
                            _tiles[row][col].Add(target);
                        }
                        else
                        {
                            _tiles[row][col].Add(new TeleportTile(_tiles[row][col], "room" + (roomID + 1).ToString() + ".xml", new Coords(col, row)));
                            _exits.Add(new Exit(new Coords(col, row), "room" + (roomID).ToString() + ".xml", new Coords(col, row), "room" + (roomID + 1).ToString() + ".xml"));
                        }
                        col += 1;
                        break;
                    case 'F':
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row)));
                        
                        Enemy enemy = new Enemy(-1, -1, -1, -1, "", r);
                        ActorTile enemyTile = new ActorTile(_tiles[row][col], enemy);
                        enemy.tile = enemyTile;
                        _tiles[row][col].Add(enemyTile);
                        _actors.Add(enemy);
                        col += 1;
                        break;
                    default:
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row)));
                        col += 1;
                        break;
                }

            }
            if (col > maxcol) { maxcol = col; };

            _width = maxcol;
            _height = row + 1;
            // Fill all rows to maximum width
            for (int i = 0; i < _height; ++i)
            {
                while (_tiles[i].Count < maxcol)
                {
                    _tiles[i].Add(new GeneratorTile(this, new Coords(_tiles[i].Count + 1, i)));
                }
            }
            return true;
        }

        public Generator(object parent, string pattern, Coords playerPos = null, int roomNr = 1, int maxRoom = 3, List<Exit> exits = null, Random rnd = null)
            : base()
        {
            if (rnd == null) r = new Random(); else r = rnd;
            _tiles = new List<List<GeneratorTile>>();
            FromString(pattern, roomNr, maxRoom);
        }

        /// <summary>
        /// Create an empty map
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="height">The height of the map</param>
        public Generator(object parent = null, int width = 10, int height = 10, bool generate = false, Coords playerPos = null, int roomNr = 1, int maxRoom = 3, List<Exit> exits = null, Random rnd = null)
            : base()
        {
            if (rnd == null) r = new Random(); else r = rnd;
            _tiles = new List<List<GeneratorTile>>();
            _width = width;
            _height = height;
            for (int y = 0; y < height; ++y)
            {
                _tiles.Add(new List<GeneratorTile>());
                for (int x = 0; x < width; ++x)
                {
                    _tiles[y].Add(new GeneratorTile(this, new Coords(x, y)));
                }
            }
            if (generate)
            {
                ClearMaze(); // set up grid
                GenerateMaze();
                if (roomNr == 1)
                {
                    AddPlayer(playerPos);
                }
                AddTraps();
                AddDoors(roomNr, maxRoom, exits);
                AddEnemies();
                AddItems();
                if (roomNr == 3)
                {
                    AddTarget();
                }
            }
        }
    }
}
