using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    public class Generator : Map
    {
        /// <summary>
        /// A list of GeneratorTiles which are needed to generate the map
        /// </summary>
        private new List<List<GeneratorTile>> _tiles = null;
        /// <summary>
        /// Hilfsfeld.
        /// </summary>
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

        /// <summary>
        /// True if there are stairs in this room to the next level.
        /// </summary>
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

        /// <summary>
        /// Possible coords for a stair to the next level.
        /// </summary>
        public Backend.Coords FindRoomForStairs
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
                return new Backend.Coords(x, y);
            }
        }

        /// <summary>
        /// Method to add the stairs to the next level to a room.
        /// </summary>
        /// <param name="srcCoords">The place where the stairs will get placed</param>
        /// <param name="targetRoom">The number of the room the stairs lead to</param>
        /// <param name="targetCoords">The coords of the stairs in the next room</param>
        /// <param name="up">Upstairs or Downstairs</param>
        public void AddStairs(Coords srcCoords, int targetRoom, Backend.Coords targetCoords, bool up)
        {

            for (int y = -1; y < 2; ++y)
            {
                for (int x = -2; x < 2; ++x)
                {
                    ClearTile(srcCoords.x + x, srcCoords.y + y);

                    if (!up && (x < 2) && (y < 2) && (y > -2) && (x > -2))
                        _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Add(new WallTile(_tiles[srcCoords.y + y][srcCoords.x + x], r));
                }
            }

            ClearTile(srcCoords.x, srcCoords.y);
            _tiles[srcCoords.y][srcCoords.x].overlay.Add(new TeleportTile(_tiles[srcCoords.y][srcCoords.x], "room" + targetRoom.ToString() + ".xml", targetCoords, false, false, true, up));


            if (!up)
            {
                ClearTile(srcCoords.x - 1, srcCoords.y);
                _tiles[srcCoords.y][srcCoords.x - 1].overlay.Add(new DoorTile(_tiles[srcCoords.y][srcCoords.x - 1], true, _level));
            }
            else
            {
                ClearTile(srcCoords.x - 1, srcCoords.y);

                ClearTile(srcCoords.x + 1, srcCoords.y);
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

        /// <summary>
        /// Method to add the player to a room.
        /// </summary>
        /// <param name="pos">The position at which the player will spawn, (1,1) by default</param>
        public void AddPlayer(Coords pos)
        {
            if (pos == null)
            {
                pos = new Backend.Coords(1, 1);
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
        public override void Save(string filename, string subdir = "save\\auto\\")
        {
            if (!System.IO.Directory.Exists("save"))
            {
                System.IO.Directory.CreateDirectory("save");
            }
            if (!System.IO.Directory.Exists(subdir))
            {
                System.IO.Directory.CreateDirectory(subdir);
            }

            int count = 0;
            for (int i = 0; i < _actors.Count; ++i)
            {
                if ((_actors[i].tile != null) && (_actors[i].tile.coords.x > -1))
                {
                    _actors[i].id = count;
                    ++count;

                }
            }

            XmlWriter xmlw = XmlWriter.Create(subdir + filename);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("GameMap");
            xmlw.WriteAttributeString("width", _width.ToString());
            xmlw.WriteAttributeString("height", _height.ToString());
            xmlw.WriteAttributeString("name", _name);
            xmlw.WriteAttributeString("level", _level.ToString());
            xmlw.WriteAttributeString("id", _id.ToString());
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

        /// <summary>
        /// Method to generate the enemys for a room.
        /// The method places enemys at free position and initializes them.
        /// </summary>
        /// <param name="amount">The number of enemys for a room, 5 by default</param>
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
                    Enemy enemy = new Enemy(-1, -1, -1, -1, "", r, (_id == 1) ? 0 : _level);
                    ActorTile enemyTile = new ActorTile(_tiles[pos.y][pos.x], enemy);
                    enemy.tile = enemyTile;
                    _tiles[pos.y][pos.x].Add(enemyTile);
                    _actors.Add(enemy);
                }
            }

        }

        /// <summary>
        /// Method to add NPCs in a room.
        /// By default the NPCs have some money and a dialogue.
        /// </summary>
        /// <param name="amount">1 by default</param>
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
                    NPC npc = new NPC(-1, -1, -1, -1, "", r, _level);
                    npc.gold = 50000;
                    npc.hasShop = false;
                    npc.hasDialog = true;
                    ActorTile NPCTile = new ActorTile(_tiles[pos.y][pos.x], npc);
                    npc.tile = NPCTile;
                    _tiles[pos.y][pos.x].Add(NPCTile);
                    _actors.Add(npc);
                }
            }

        }

        /// <summary>
        /// Remove all elements from a tile
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearTile(int x, int y)
        {
            foreach (Actor actor in _tiles[y][x].actors)
            {
                _actors.Remove(actor);
            }
            _tiles[y][x].overlay.Clear();

        }

        /// <summary>
        /// Method to add a boss enemy to a room.
        /// A boss is more powerful than a normal enemy
        /// and the room gets a special sound if there is a boss in it.
        /// </summary>
        public void AddBoss()
        {
            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    if ((_tiles[y][x].hasEnemy) || (_tiles[y][x].hasNPC) || (_tiles[y][x].hasTreasure))
                    {
                        ClearTile(x, y);
                    }

                }
            }

            Backend.Coords pos = new Backend.Coords(2 + r.Next(_width - 4), 2 + r.Next(_height - 4));
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    ClearTile(pos.x + x, pos.y + y);

                }
            }

            Enemy boss = new Enemy(-1, -1, -1, -1, "", r, 10 + _level);
            if (r.NextDouble() >= 0.5)
            {
                boss.fireDefense = 70;
                boss.fireDamage += r.Next(10);
            }
            else
            {
                boss.iceDefense = 70;
                boss.iceDamage += r.Next(10);
            }
            ActorTile BossTile = new ActorTile(_tiles[pos.y][pos.x], boss);
            boss.tile = BossTile;
            _tiles[pos.y][pos.x].Add(BossTile);
            _actors.Add(boss);
            if (_level == 1) _music = "boss1.wav"; else _music = "boss2.wav";
        }

        /// <summary>
        /// Method to add a shop to the map.
        /// The method adds a NPC with a shop and some custom tiles around him.
        /// </summary>
        public void AddShop()
        {
            int x = -1;
            while (x == -1)
            {
                x = 4 + r.Next(_width - 6);
                if ((_tiles[0][x].hasTeleport) ||
                    (_tiles[0][x + 1].hasTeleport) ||
                    (_tiles[0][x - 1].hasTeleport))
                    x = -1;
            }
            ClearTile(x - 1, 2);

            ClearTile(x + 1, 2);
            ClearTile(x - 1, 1);
            ClearTile(x, 1);
            ClearTile(x + 1, 1);

            NPC npc = new NPC(-1, -1, -1, -1, "", r, _level, true);
            npc.gold = 50000;
            npc.hasShop = true;
            for (int count = 0; count < 25; ++count)
            {
                npc.AddItem(new Item(r, 0, _level, false));
            }
            ActorTile NPCTile = new ActorTile(_tiles[2][x], npc);
            npc.tile = NPCTile;
            npc.stunned = -1;
            _tiles[2][x].Add(NPCTile);
            _actors.Add(npc);

            _tiles[2][x + 1].Add(new ReservedTile(_tiles[2][x + 1], ".\\Content\\shop.xml", 0));
            // 448, 192, 64, 64));
            //_tiles[2][pos.x - 1].Add(new ReservedTile(this, ".\\Content\\shop.xml", 1));
            // 354, 509, 64, 96)
            _tiles[2][x - 1].Add(new ReservedTile(_tiles[2][x - 1], ".\\Content\\shop.xml", 2));
            // 195, 256, 64, 64
            _tiles[1][x].Add(new ReservedTile(_tiles[1][x], ".\\Content\\shop.xml", 3));
            // 0, 512, 96, 128


            _music = "shop.wav";


        }

        /// <summary>
        /// Method to add the CheckpointTiles to a room.
        /// </summary>
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

        /// <summary>
        /// Method to add the TargetTile to a room (usually a room in the highest level).
        /// The Target is surrounded by walls and a door.
        /// </summary>
        /// <param name="srcCoords">The coordinates for the TargetTile</param>
        public void AddTarget(Coords srcCoords)
        {
            for (int y = -1; y < 2; ++y)
            {
                for (int x = -2; x < 2; ++x)
                {
                    ClearTile(srcCoords.x + x, srcCoords.y + y);
                    if ((x < 2) && (y < 2) && (y > -2) && (x > -2))
                        _tiles[srcCoords.y + y][srcCoords.x + x].overlay.Add(new WallTile(_tiles[srcCoords.y + y][srcCoords.x + x], r));
                }
            }
            ClearTile(srcCoords.x, srcCoords.y);
            _tiles[srcCoords.y][srcCoords.x].overlay.Add(new TargetTile(_tiles[srcCoords.y][srcCoords.x]));

            ClearTile(srcCoords.x - 1, srcCoords.y);
            _tiles[srcCoords.y][srcCoords.x - 1].overlay.Add(new DoorTile(_tiles[srcCoords.y][srcCoords.x - 1], true, _level));

        }

        /// <summary>
        /// Method to add a door to the next level to a room.
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="maxRoom"></param>
        /// <param name="doors"></param>
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
                        _tiles[_height - 1][x].Add(new TeleportTile(_tiles[_height - 1][x], "room" + (roomID + 1).ToString() + ".xml", new Backend.Coords(x, 0)));
                        _exits.Add(new Exit(new Backend.Coords(x, _height - 1), "room" + (roomID).ToString() + ".xml", new Backend.Coords(x, 0), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Up:
                        _tiles[0][x].Remove(TileType.Wall);
                        _tiles[0][x].Add(new TeleportTile(_tiles[0][x], "room" + (roomID + 1).ToString() + ".xml", new Backend.Coords(x, y)));
                        _exits.Add(new Exit(new Backend.Coords(x, 0), "room" + (roomID).ToString() + ".xml", new Backend.Coords(x, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Right:
                        _tiles[y][_width - 1].Remove(TileType.Wall);
                        _tiles[y][_width - 1].Add(new TeleportTile(_tiles[y][_width - 1], "room" + (roomID + 1).ToString() + ".xml", new Backend.Coords(0, y)));
                        _exits.Add(new Exit(new Backend.Coords(_width - 1, y), "room" + (roomID).ToString() + ".xml", new Backend.Coords(0, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                    case Direction.Left:
                        _tiles[y][0].Remove(TileType.Wall);
                        _tiles[y][0].Add(new TeleportTile(_tiles[y][0], (roomID + 1).ToString() + ".xml", new Backend.Coords(x, y)));
                        _exits.Add(new Exit(new Backend.Coords(0, y), (roomID).ToString() + ".xml", new Backend.Coords(x, y), "room" + (roomID + 1).ToString() + ".xml"));
                        break;
                }
            }
        }


        /// <summary>
        /// Find an appropriate place to put an exit on the map
        /// </summary>
        /// <param name="dir">Wall on which exit should be placed</param>
        /// <returns>Coordinates of the exit</returns>
        public Backend.Coords SuggestExit(Direction dir)
        {
            switch (dir)
            {
                case Direction.UpLeft:
                    return new Backend.Coords(0, 0);

                case Direction.UpRight:
                    return new Backend.Coords(_width - 1, 0);

                case Direction.DownLeft:
                    return new Backend.Coords(0, _height - 1);

                case Direction.DownRight:
                    return new Backend.Coords(_width - 1, _height - 1);

                case Direction.Up:
                    {
                        Backend.Coords tmp = new Backend.Coords(1 + r.Next((_width - 1) / 2) * 2, 0);
                        ClearTile(tmp.x, 1);
                        return tmp;
                    }

                case Direction.Down:
                    {
                        Backend.Coords tmp = new Backend.Coords(1 + r.Next((_width - 1) / 2) * 2, _height - 1);
                        ClearTile(tmp.x, tmp.y - 1);
                        return tmp;
                    }

                case Direction.Left:
                    {
                        Backend.Coords tmp = new Backend.Coords(0, 1 + r.Next((_height - 1) / 2) * 2);
                        ClearTile(1, tmp.y);
                        return tmp;
                    }
                case Direction.Right:
                    {
                        Backend.Coords tmp = new Backend.Coords(_width - 1, 1 + r.Next((_height - 1) / 2) * 2);
                        ClearTile(tmp.x - 1, tmp.y);
                        return tmp;
                    }
            }
            return Backend.Coords.Zero;
        }

        /// <summary>
        /// Add a doorway / stairway / teleporter to another room
        /// </summary>
        /// <param name="from">Coordinates in this room</param>
        /// <param name="Room">Uinque ID of target room</param>
        /// <param name="to">Coordinates in target room</param>
        public void ConnectTo(Coords from, int Room, Backend.Coords to, bool isTeleport = false)
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                    if ((x > 1) && (y < 1) && (x < _width - 1) && (y < _height - 1))
                    {
                        ClearTile(from.x + x, from.y + y);
                    }
            }
            ClearTile(from.x, from.y);
            _tiles[from.y][from.x].overlay.Add(new TeleportTile(_tiles[from.y][from.x], "room" + (Room + 1).ToString() + ".xml", to, isTeleport));
        }

        /// <summary>
        /// Method to check whether there is a connection to another room.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>True if there is a exit in one outer wall.</returns>
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

        /// <summary>
        /// Method to place some random items in a room.
        /// </summary>
        /// <param name="amount">5 by default.</param>
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
                    Item item = new Item(r, 0, _level);
                    ItemTile itemTile = new ItemTile(_tiles[pos.y][pos.x], item);
                    item.tile = itemTile;
                    _tiles[pos.y][pos.x].Add(itemTile);
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
                    _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row),

                        (row % 2 == 1)
                        && (col % 2 == 1), r)
                        );
                }
            }
        }

        /// <summary>
        /// Method to add some TrapTiles to a room.
        /// </summary>
        /// <param name="amount">5 by default.</param>
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
        public void GenerateMaze()
        {
            Path originPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 2) / 2) * 2);
            Path currentPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 3) / 2) * 2);

            _tiles[originPos.y][originPos.x].connected = true;
            _tiles[originPos.y][originPos.x].Remove(TileType.Wall);
            int remaining = _width / 2 * _height / 2 + 1;

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
                    if (!_tiles[currentPos.y][currentPos.x].connected)
                    {
                        _tiles[currentPos.y][currentPos.x].connected = true;
                        remaining--;
                    }
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
                }
                startPos = new Path(1 + r.Next((_width - 3) / 2) * 2, 1 + r.Next((_height - 2) / 2) * 2);
            }
        }

        /// <summary>
        /// Method to generate a room from a string (e.g. a .txt)
        /// </summary>
        /// <param name="input">The string from which the room will be constructed</param>
        /// <param name="roomID">The number of the room.</param>
        /// <param name="MaxRoom">The max number of rooms</param>
        /// <returns>True.</returns>
        public bool FromString(string input, int roomID, int MaxRoom)
        {
            int col = 0, row = 0, maxcol = 0;
            _tiles.Add(new List<GeneratorTile>());
            foreach (char c in input)
            {
                switch (c)
                {
                    case '#':
                        _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row), true, r));
                        col += 1;
                        break;
                    case '\n':
                        if (col > maxcol) { maxcol = col; };

                        col = 0;
                        row += 1;
                        _tiles.Add(new List<GeneratorTile>());
                        break;
                    case 'S':

                        _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row), false, r));

                        if (roomID != 1)
                        {
                            _tiles[row][col].Add(new TeleportTile(_tiles[row][col], "room" + (roomID - 1).ToString() + ".xml", new Backend.Coords(col, row)));
                            _exits.Add(new Exit(new Backend.Coords(col, row), "room" + (roomID).ToString() + ".xml", new Backend.Coords(col, row), "room" + (roomID - 1).ToString() + ".xml"));
                        }
                        else
                        {
                            AddPlayer(new Backend.Coords(col, row));
                        }
                        col += 1;
                        break;
                    case 'G':
                        _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row), false, r));

                        if (roomID == MaxRoom)
                        {
                            TargetTile target = new TargetTile(_tiles[row][col]);
                            _tiles[row][col].Add(target);
                        }
                        else
                        {
                            _tiles[row][col].Add(new TeleportTile(_tiles[row][col], "room" + (roomID + 1).ToString() + ".xml", new Backend.Coords(col, row)));
                            _exits.Add(new Exit(new Backend.Coords(col, row), "room" + (roomID).ToString() + ".xml", new Backend.Coords(col, row), "room" + (roomID + 1).ToString() + ".xml"));
                        }
                        col += 1;
                        break;
                    case 'F':
                        _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row), false, r));

                        Enemy enemy = new Enemy(-1, -1, -1, -1, "", r);
                        ActorTile enemyTile = new ActorTile(_tiles[row][col], enemy);
                        enemy.tile = enemyTile;
                        _tiles[row][col].Add(enemyTile);
                        _actors.Add(enemy);
                        col += 1;
                        break;
                    default:
                        _tiles[row].Add(new GeneratorTile(this, new Backend.Coords(col, row), false, r));
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
                    _tiles[i].Add(new GeneratorTile(this, new Backend.Coords(_tiles[i].Count + 1, i), false, r));
                }
            }
            return true;
        }

        /// <summary>
        /// The constructor for the Generator.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pattern"></param>
        /// <param name="roomNr"></param>
        /// <param name="maxRoom"></param>
        /// <param name="exits"></param>
        /// <param name="rnd"></param>
        public Generator(object parent, string pattern, int roomNr = 1, int maxRoom = 3, List<Exit> exits = null, Random rnd = null)
            : base()
        {
            if (rnd == null) r = new Random(); else r = rnd;
            _tiles = new List<List<GeneratorTile>>();
            _id = roomNr;
            FromString(pattern, roomNr, maxRoom);
        }

        /// <summary>
        /// Method to choose a name for the dungeon.
        /// </summary>
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

        /// <summary>
        /// Method to choose a name for a room.
        /// </summary>
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

        /// <summary>
        /// Method to add the outer walls to a room.
        /// </summary>
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
        public Generator(object parent = null, int width = 10, int height = 10, bool generate = false, Backend.Coords playerPos = null, int roomNr = 1, int maxRoom = 3, Random rnd = null, string dungeonname = "", int level = 0, bool hasShop = false, bool hasNPC = false, bool hasBoss = false)
            : base()
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
                    _tiles[y].Add(new GeneratorTile(this, new Backend.Coords(x, y), true, r));
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
