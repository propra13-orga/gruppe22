using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class Generator : Map
    {
        private new List<List<GeneratorTile>> _tiles = null;
        private bool _connected = false;
        private Random r;
        public bool connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
            }
        }

        public List<int> connectedRooms
        {
            get
            {
                Regex re = new Regex(@"\d+");

                List<int> result = new List<int>();
                for (int x = 0; x < _width; ++x)
                {
                    if (_tiles[0][x].hasTeleport)
                    {
                        Match m = re.Match(_tiles[0][x].teleport.nextRoom);
                        result.Add(Convert.ToInt32(m.Value));
                    };
                    if (_tiles[_height - 1][x].hasTeleport)
                    {
                        Match m = re.Match(_tiles[_height - 1][x].teleport.nextRoom);
                        result.Add(Convert.ToInt32(m.Value));
                    };
                }
                for (int y = 0; y < _height; ++y)
                {
                    if (_tiles[y][0].hasTeleport)
                    {
                        Match m = re.Match(_tiles[y][0].teleport.nextRoom);
                        result.Add(Convert.ToInt32(m.Value));
                    };

                    if (_tiles[y][_width - 1].hasTeleport)
                    {
                        Match m = re.Match(_tiles[y][_width - 1].teleport.nextRoom);
                        result.Add(Convert.ToInt32(m.Value));
                    };

                }
                return result;
            }
        }
        public new Direction exits
        {
            get
            {
                Direction dirs = Direction.None;
                for (int x = 0; x < _width; ++x)
                {
                    if (_tiles[0][x].hasTeleport)
                        dirs = dirs | Direction.Up;
                    if (_tiles[_height - 1][x].hasTeleport)
                        dirs = dirs | Direction.Down;
                }
                for (int y = 0; y < _height; ++y)
                {
                    if (_tiles[y][0].hasTeleport)
                        dirs = dirs | Direction.Left;
                    if (_tiles[y][_width - 1].hasTeleport)
                        dirs = dirs | Direction.Right;
                }
                return dirs;
            }
        }

        public bool hasStairs
        {
            get
            {
                for (int x = 1; x < _width - 1; ++x)
                {
                    for (int y = 1; y < _height - 1; ++y)
                    {
                        if (((_tiles[y][x].hasTeleport) && (!_tiles[y][x].teleport.teleport)) || (_tiles[y][x].hasTarget))
                            return true;
                    }
                }
                return false;
            }
        }


        public Coords FindRoomForStairs
        {
            get
            {
                int x = -1;
                int y = -1;
                while (x == -1)
                {
                    x = 3 + r.Next(_width - 5);
                    y = 2 + r.Next(_height - 3);
                    for (int px = x - 3; px < x + 2; ++px)
                    {
                        for (int py = y - 2; py < y + 2; ++py)
                        {
                            if (_tiles[py][px].hasTeleport)
                            {
                                x = -1;
                                break;
                            }
                        }
                    }
                }
                return new Coords(x, y);
            }
        }

        public void AddStairs(Coords srcCoords, int targetRoom, Coords targetCoords, bool up)
        {

            for (int y = -1; y < 2; ++y)
            {
                for (int x = -2; x < 2; ++x)
                {
                    _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Clear();
                    if ((x < 2) && (y < 2) && (y > -2) && (x > -2))
                        _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Add(new WallTile(_tiles[srcCoords.y + y][srcCoords.x + x], r));
                }
            }

            _tiles[srcCoords.y][srcCoords.x].overlay.Clear();
            _tiles[srcCoords.y][srcCoords.x].overlay.Add(new TeleportTile(_tiles[srcCoords.y][srcCoords.x], "room" + targetRoom.ToString() + ".xml", targetCoords, false, false, true, up));


            if (!up)
            {
                                _tiles[srcCoords.y][srcCoords.x - 1].overlay.Clear();

                _tiles[srcCoords.y][srcCoords.x - 1].overlay.Add(new DoorTile(_tiles[srcCoords.y][srcCoords.x - 1], true, _level));
            }
            else
            {
                _tiles[srcCoords.y][srcCoords.x + 1].overlay.Clear();
            }

        }

        public Direction blocked
        {
            get
            {
                Direction dirs = Direction.Up | Direction.Left | Direction.Right | Direction.Down;
                for (int x = 0; x < _width; ++x)
                {
                    if (_tiles[0][x].hasTeleport)
                        dirs &= ~Direction.Up;
                    if (_tiles[_height - 1][x].hasTeleport)
                        dirs &= ~Direction.Down;
                }
                for (int y = 0; y < _height; ++y)
                {
                    if (_tiles[y][0].hasTeleport)
                        dirs &= ~Direction.Left;
                    if (_tiles[y][_width - 1].hasTeleport)
                        dirs &= ~Direction.Right;
                }
                return dirs;
            }
        }

        public void AddPlayer(Coords pos)
        {
            if (pos == null)
            {
                pos = new Coords(1, 1);
            }
            Player player = new Player(_content, 100, 0, 30);
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
            xmlw.WriteAttributeString("name", _name);
            xmlw.WriteAttributeString("level", _level.ToString());
            xmlw.WriteAttributeString("dungeon", _dungeonname);
            xmlw.WriteAttributeString("music", _music);
            xmlw.WriteAttributeString("floor", _floorFile);
            xmlw.WriteAttributeString("wall", _wallFile);
            xmlw.WriteAttributeString("light", _light.ToString());


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
                    Enemy enemy = new Enemy(_content, -1, -1, -1, -1, "", r, (_id == 1) ? 0 : _level);
                    ActorTile enemyTile = new ActorTile(_tiles[pos.y][pos.x], enemy);
                    enemy.tile = enemyTile;
                    _tiles[pos.y][pos.x].Add(enemyTile);
                    _actors.Add(enemy);
                }
            }

        }


        public void AddNPC(int amount = -1)
        {
            if (amount < 0) amount = 1;
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
                    NPC npc = new NPC(_content, -1, -1, -1, -1, "", r, _level);
                    npc.gold = 50000;
                    npc.hasShop = false;
                    npc.hasDialogue = true;
                    ActorTile NPCTile = new ActorTile(_tiles[pos.y][pos.x], npc);
                    npc.tile = NPCTile;
                    _tiles[pos.y][pos.x].Add(NPCTile);
                    _actors.Add(npc);
                }
            }

        }

        public void AddBoss()
        {
            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    if ((_tiles[y][x].hasEnemy) || (_tiles[y][x].hasNPC) || (_tiles[y][x].hasTreasure))
                        _tiles[y][x].overlay.Clear();

                }
            }

            Coords pos = new Coords(2 + r.Next(_width - 4), 2 + r.Next(_height - 4));
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    _tiles[pos.y + y][pos.x + x].overlay.Clear();

                }
            }

            Enemy boss = new Enemy(_content, -1, -1, -1, -1, "", r, 10 + _level);
            ActorTile BossTile = new ActorTile(_tiles[pos.y][pos.x], boss);
            boss.tile = BossTile;
            _tiles[pos.y][pos.x].Add(BossTile);
            _actors.Add(boss);
            if (_level == 1) _music = "boss1.wav"; else _music = "boss2.wav";
        }

        public void AddShop()
        {
            int x = -1;
            while (x == -1)
            {
                x = 3 + r.Next(_width - 5);
                if ((_tiles[0][x].hasTeleport) ||
                    (_tiles[0][x + 1].hasTeleport) ||
                    (_tiles[0][x - 1].hasTeleport))
                    x = -1;
            }
            _tiles[2][x + 1].overlay.Clear();
            _tiles[2][x].overlay.Clear();
            _tiles[2][x - 1].overlay.Clear();
            _tiles[1][x].overlay.Clear();
            _tiles[1][x - 1].overlay.Clear();
            _tiles[1][x + 1].overlay.Clear();

            NPC npc = new NPC(_content, -1, -1, -1, -1, "", r, _level, true);
            npc.gold = 50000;
            npc.hasShop = true;
            for (int count = 0; count < 25; ++count)
            {
                npc.inventory.Add(new Item(_content, r, 0, _level, false));
            }
            ActorTile NPCTile = new ActorTile(_tiles[2][x], npc);
            npc.tile = NPCTile;
            npc.stunned = -1;
            _tiles[2][x].Add(NPCTile);
            _actors.Add(npc);

            _tiles[2][x + 1].Add(new ReservedTile(this, ".\\Content\\shop.xml", 0));
            // 448, 192, 64, 64));
            //_tiles[2][pos.x - 1].Add(new ReservedTile(this, ".\\Content\\shop.xml", 1));
            // 354, 509, 64, 96)
            _tiles[2][x - 1].Add(new ReservedTile(this, ".\\Content\\shop.xml", 2));
            // 195, 256, 64, 64
            _tiles[1][x].Add(new ReservedTile(this, ".\\Content\\shop.xml", 3));
            // 0, 512, 96, 128


            _music = "shop.wav";


        }

        public void AddCheckpoint()
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

                CheckpointTile checkpoint = new CheckpointTile(_tiles[pos.y][pos.x], false, 0);
                _tiles[pos.y][pos.x].Add(checkpoint);
            }
        }

        public void AddTarget(Coords srcCoords)
        {


            for (int y = -1; y < 2; ++y)
            {
                for (int x = -2; x < 2; ++x)
                {
                    _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Clear();
                    if ((x < 2) && (y < 2) && (y > -2) && (x > -2))
                        _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Add(new WallTile(_tiles[srcCoords.y + y][srcCoords.x + x], r));
                }
            }

            _tiles[srcCoords.y][srcCoords.x].overlay.Clear();
            _tiles[srcCoords.y][srcCoords.x].overlay.Add(new TargetTile(_tiles[srcCoords.y][srcCoords.x]));

            _tiles[srcCoords.y][srcCoords.x - 1].overlay.Clear();
            _tiles[srcCoords.y][srcCoords.x - 1].overlay.Add(new DoorTile(_tiles[srcCoords.y][srcCoords.x - 1], true, _level));

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


        /// <summary>
        /// Find an appropriate place to pu an exit on the map
        /// </summary>
        /// <param name="dir">Wall on which exit should be placed</param>
        /// <returns>Coordinates of the exit</returns>
        public Coords SuggestExit(Direction dir)
        {
            switch (dir)
            {
                case Direction.UpLeft:
                    return new Coords(0, 0);

                case Direction.UpRight:
                    return new Coords(_width - 1, 0);

                case Direction.DownLeft:
                    return new Coords(0, _height - 1);

                case Direction.DownRight:
                    return new Coords(_width - 1, _height - 1);

                case Direction.Up:
                    {
                        Coords tmp = new Coords(1 + r.Next((_width - 1) / 2) * 2, 0);
                        _tiles[1][tmp.x].overlay.Clear();
                        return tmp;
                    }

                case Direction.Down:
                    {
                        Coords tmp = new Coords(1 + r.Next((_width - 1) / 2) * 2, _height - 1);
                        _tiles[tmp.y - 1][tmp.x].overlay.Clear();
                        return tmp;
                    }

                case Direction.Left:
                    {
                        Coords tmp = new Coords(0, 1 + r.Next((_height - 1) / 2) * 2);
                        _tiles[tmp.y][1].overlay.Clear();
                        return tmp;
                    }
                case Direction.Right:
                    {
                        Coords tmp = new Coords(_width - 1, 1 + r.Next((_height - 1) / 2) * 2);
                        _tiles[tmp.y][tmp.x - 1].overlay.Clear();
                        return tmp;
                    }
            }
            return Coords.Zero;
        }

        /// <summary>
        /// Add a doorway / stairway / teleporter to another room
        /// </summary>
        /// <param name="from">Coordinates in this room</param>
        /// <param name="Room">Uinque ID of target room</param>
        /// <param name="to">Coordinates in target room</param>
        public void ConnectTo(Coords from, int Room, Coords to, bool isTeleport = false)
        {
            // TODO: Umgebung "freisprengen", insb. in Diagonalen
            _tiles[from.y][from.x].overlay.Clear();
            _tiles[from.y][from.x].overlay.Add(new TeleportTile(this, "room" + (Room + 1).ToString() + ".xml", to, isTeleport));
        }

        public bool HasExit(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    for (int i = 0; i < _width; ++i)
                        if (_tiles[_height - 1][i].hasTeleport) return true;
                    return false;
                case Direction.Down:
                    for (int i = 0; i < _width; ++i)
                        if (_tiles[0][i].hasTeleport) return true;
                    return false;
                case Direction.Left:
                    for (int i = 0; i < _height; ++i)
                        if (_tiles[i][0].hasTeleport) return true;
                    return false;
                case Direction.Right:
                    for (int i = 0; i < _height; ++i)
                        if (_tiles[i][_width - 1].hasTeleport) return true;
                    return false;
                case Direction.UpLeft:
                    return _tiles[0][0].hasTeleport;
                case Direction.UpRight:
                    return _tiles[0][_width - 1].hasTeleport;
                case Direction.DownRight:
                    return _tiles[_height - 1][_width - 1].hasTeleport;
                case Direction.DownLeft:
                    return _tiles[_height - 1][0].hasTeleport;

            }
            return false;
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
                    Item item = new Item(_content, r, 0, _level);
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
                if (pos.x > -1)
                {
                    _tiles[pos.y][pos.x].Remove(TileType.Wall);
                }
                else break;
            }


        }

        public void CleanupRoom()
        {
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
                        && (col % 2 == 1), r)
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
                    TrapTile trapTile = new TrapTile(_tiles[pos.y][pos.x], r.Next(10 * _level) + 10);
                    /*if (r.Next(10) > 3)
                        trapTile.type = trapTile.type | TrapType.OnlyOnce;
                    if (r.Next(10) > 7)
                        trapTile.type = trapTile.type | TrapType.Hidden;
                    if (r.Next(10) > 4) */
                    trapTile.type = trapTile.type | TrapType.Changing;
                    trapTile.penetrate = r.Next(10);
                    trapTile.evade = r.Next(10);

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
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row), true, r));
                        col += 1;
                        break;
                    case '\n':
                        if (col > maxcol) { maxcol = col; };

                        col = 0;
                        row += 1;
                        _tiles.Add(new List<GeneratorTile>());
                        break;
                    case 'S':

                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row), false, r));

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
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row), false, r));

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
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row), false, r));

                        Enemy enemy = new Enemy(_content, -1, -1, -1, -1, "", r);
                        ActorTile enemyTile = new ActorTile(_tiles[row][col], enemy);
                        enemy.tile = enemyTile;
                        _tiles[row][col].Add(enemyTile);
                        _actors.Add(enemy);
                        col += 1;
                        break;
                    default:
                        _tiles[row].Add(new GeneratorTile(this, new Coords(col, row), false, r));
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
                    _tiles[i].Add(new GeneratorTile(this, new Coords(_tiles[i].Count + 1, i), false, r));
                }
            }
            return true;
        }

        public Generator(ContentManager content, object parent, string pattern, int roomNr = 1, int maxRoom = 3, List<Exit> exits = null, Random rnd = null)
            : base(content)
        {
            if (rnd == null) r = new Random(); else r = rnd;
            _tiles = new List<List<GeneratorTile>>();
            _id = roomNr;
            FromString(pattern, roomNr, maxRoom);
        }

        public void GenerateDungeon()
        {
            switch (r.Next(10))
            {
                case 0:
                    _dungeonname += "Temple ";
                    break;
                case 1:
                    _dungeonname += "Stronghold ";
                    break;
                case 2:
                    _dungeonname += "Castle ";
                    break;
                case 3:
                    _dungeonname += "Tunnel ";
                    break;
                case 4:
                    _dungeonname += "Tower ";
                    break;
                case 5:
                    _dungeonname += "Lair ";
                    break;
                case 6:
                    _dungeonname += "Caverns ";
                    break;
                case 7:
                    _dungeonname += "Home ";
                    break;
                case 8:
                    _dungeonname += "Asylum ";
                    break;
                case 9:
                    _dungeonname += "Prison ";
                    break;
            }
            switch (r.Next(11))
            {
                case 0:
                    _dungeonname += "of Evil ";
                    break;
                case 1:
                    _dungeonname += "of Deadly ";
                    break;
                case 2:
                    _dungeonname += "of Elemental ";
                    break;
                case 3:
                    _dungeonname += "of Strange ";
                    break;
                case 4:
                    _dungeonname += "of Mysterious ";
                    break;
                case 5:
                    _dungeonname += "of Unknown ";
                    break;
                case 6:
                    _dungeonname += "of Incredible ";
                    break;
                case 7:
                    _dungeonname += "of Legendary ";
                    break;
                case 8:
                    _dungeonname += "of Growing ";
                    break;
                case 9:
                    _dungeonname += "of Scary ";
                    break;
            }
            switch (r.Next(11))
            {
                case 0:
                    _dungeonname += "Power";
                    break;
                case 1:
                    _dungeonname += "Energy";
                    break;
                case 2:
                    _dungeonname += "Wealth";
                    break;
                case 3:
                    _dungeonname += "Strength";
                    break;
                case 4:
                    _dungeonname += "Spirit";
                    break;
                case 5:
                    _dungeonname += "Magic";
                    break;
                case 6:
                    _dungeonname += "Beasts";
                    break;
                case 7:
                    _dungeonname += "Monsters";
                    break;
                case 8:
                    _dungeonname += "Wizards";
                    break;
                case 9:
                    _dungeonname += "Enemies";
                    break;
            }
        }

        public void GenerateRoomName()
        {
            switch (r.Next(11))
            {
                case 0:
                    _name += "Dark ";
                    break;
                case 1:
                    _name += "Smelly ";
                    break;
                case 2:
                    _name += "Eery ";
                    break;
                case 3:
                    _name += "Frightening ";
                    break;
                case 4:
                    _name += "Scary ";
                    break;
                case 5:
                    _name += "Evil ";
                    break;
                case 6:
                    _name += "Haunting ";
                    break;
                case 7:
                    _name += "Mysterious ";
                    break;
                case 8:
                    _name += "Dangerous ";
                    break;
                case 9:
                    _name += "Musty ";
                    break;
            }
            switch (r.Next(10))
            {
                case 0:
                    _name += "Cave";
                    break;
                case 1:
                    _name += "Chamber";
                    break;
                case 2:
                    _name += "Hallway";
                    break;
                case 3:
                    _name += "Cavern";
                    break;
                case 4:
                    _name += "Room";
                    break;
                case 5:
                    _name += "Dungeon";
                    break;
                case 6:
                    _name += "Sewer";
                    break;
                case 7:
                    _name += "Hole";
                    break;
                case 8:
                    _name += "Antechamber";
                    break;
                case 9:
                    _name += "Barracks";
                    break;
            }
            switch (r.Next(10))
            {
                case 0:
                    _name += " of Evil ";
                    break;
                case 1:
                    _name += " of Mad ";
                    break;
                case 2:
                    _name += " of Dangerous ";
                    break;
                case 3:
                    _name += " of Forgotten ";
                    break;
                case 4:
                    _name += " of Predatory ";
                    break;
                case 5:
                    _name += " of Unknown ";
                    break;
                case 6:
                    _name += " of Timeless ";
                    break;
                case 7:
                    _name += " of Hidden ";
                    break;
                case 8:
                    _name += " of Treacherous ";
                    break;
                case 9:
                    _name += " of Lost ";
                    break;
            }
            switch (r.Next(10))
            {
                case 0:
                    _name += " Spirits";
                    break;
                case 1:
                    _name += " Shadows";
                    break;
                case 2:
                    _name += " Mysteries";
                    break;
                case 3:
                    _name += " Ghosts";
                    break;
                case 4:
                    _name += " Gods";
                    break;
                case 5:
                    _name += " Preachers";
                    break;
                case 6:
                    _name += " Rumors";
                    break;
                case 7:
                    _name += " Legends";
                    break;
                case 8:
                    _name += " History";
                    break;
                case 9:
                    _name += " Destruction";
                    break;
            }
        }

        public void DrawWalls()
        {
            for (int x = 1; x < _width - 1; ++x)
            {
                _tiles[0][x].Add(new WallTile(_tiles[0][x]));
                _tiles[_height - 1][x].Add(new WallTile(_tiles[_height - 1][x], r));
            }
            for (int y = 0; y < _height; ++y)
            {
                _tiles[y][0].Add(new WallTile(_tiles[y][0]));
                _tiles[y][_width - 1].Add(new WallTile(_tiles[y][_width - 1], r));
            }
        }

        /// <summary>
        /// Create an empty map
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="height">The height of the map</param>
        public Generator(ContentManager content, object parent = null, int width = 10, int height = 10, bool generate = false, Coords playerPos = null, int roomNr = 1, int maxRoom = 3, Random rnd = null, string dungeonname = "", int level = 0, bool hasShop = false, bool hasNPC = false, bool hasBoss = false)
            : base(content)
        {
            if (rnd == null) r = new Random(); else r = rnd;
            _tiles = new List<List<GeneratorTile>>();
            _width = width;
            _height = height;
            _level = level;
            _id = roomNr;
            for (int y = 0; y < height; ++y)
            {
                _tiles.Add(new List<GeneratorTile>());
                for (int x = 0; x < width; ++x)
                {
                    _tiles[y].Add(new GeneratorTile(this, new Coords(x, y), true, r));
                }
            }
            if (generate)
            {
                if (_level == 1) _music = "level1.wav"; else _music = "level2.wav";
                if (roomNr > 1)
                {
                    ClearMaze(); // set up grid
                    GenerateMaze();
                    ClearWalls(Math.Min((_level - 1) * (2 + r.Next(10)), ((_width - 1) * (_height - 1))));
                }

                else
                    DrawWalls();
                if (roomNr == 1)
                {
                    AddPlayer(playerPos);
                    _music = "shop.wav";
                }
                if (_id > 1)
                {
                    AddTraps();
                    AddItems();
                }
                AddEnemies(_level * (r.Next(5) + 3));
                if (hasShop)
                {
                    AddShop();
                }
                if (hasBoss)
                {
                    AddBoss();
                }
                if (hasNPC)
                {
                    AddNPC();
                }
                switch (_level)
                {
                    case 1:
                        _wallFile = "wall4";
                        _floorFile = "floor3";
                        _light = 12;
                        break;
                    case 2:
                        _wallFile = "wall2";
                        _floorFile = "floor2";
                        _light = 0;
                        break;
                    case 3:
                        _wallFile = "wall3";
                        _floorFile = "floor3";
                        _light = 20;
                        break;
                }
                if (_id == 1)
                {
                    _wallFile = "wall1";
                    _floorFile = "floor1";
                    _light = 200;
                }
                else
                    CleanupRoom(); // Remove "ugly" walls

                GenerateRoomName();
                if ((dungeonname != "") && (dungeonname != null))
                {
                    _dungeonname = dungeonname;
                }
                else
                {
                    GenerateDungeon();
                }

            }
        }
    }
}
