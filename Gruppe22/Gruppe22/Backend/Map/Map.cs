using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Gruppe22
{
    public class Map : IHandleEvent, IDisposable
    {
        #region Private Fields
        private IHandleEvent _parent;

        /// <summary>
        /// A two dimensional list of tiles
        /// </summary>
        private List<List<Tile>> _tiles;
        /// <summary>
        /// Internal current width
        /// </summary>
        private int _width;

        /// <summary>
        /// Internal current height
        /// </summary>
        private int _height;

        /// <summary>
        /// Blank tile returned when requesting tile outside map boundaries (i.e. negative values / values beyond width or height)
        /// </summary>
        private Tile _blankTile;

        #endregion

        #region Public Fields

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
                if ((x < width) && (x > -1) && (y < height) && (y > -1))
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
                if ((x < width) && (x > -1) && (y < height) && (y > -1))
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

        public void HandleEvent(UIElement sender, Events eventID, int data)
        {
            _parent.HandleEvent(sender, eventID, data);
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
                    _tiles[row].Add(new Tile(
                        (row % 2 == 1)
                        && (col % 2 == 1))
                        );
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
                _tiles[pos.y][pos.x].canEnter = true;
                System.Diagnostics.Debug.WriteLine("Remove Wall at " + pos.x + "/" + pos.y);
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
            _tiles[originPos.y][originPos.x].canEnter = true;
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
                            _tiles[currentPos.y][currentPos.x - 1].canEnter = true;
                            break;
                        case Connection.Right:
                            _tiles[currentPos.y][currentPos.x + 1].canEnter = true;
                            break;
                        case Connection.Up:
                            _tiles[currentPos.y - 1][currentPos.x].canEnter = true;
                            break;
                        case Connection.Down:
                            _tiles[currentPos.y + 1][currentPos.x].canEnter = true;
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
        public bool Load(string filename)
        {
            bool result = true;
            XmlTextReader source = new XmlTextReader(filename);

            try
            {

            }
            finally
            {
                source.Close();
            }
            return result;
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

        public bool FromString(string input)
        {
            int col = 0, row = 0;
            foreach (char c in input)
            {
                switch (c)
                {
                    case '#':
                        if ((col < _width) && (row < height))
                            _tiles[row][col].canEnter = false;
                        col += 1;
                        break;
                    case '\n':
                        col = 0;
                        row += 1;
                        break;
                    default:
                        if ((col < _width) && (row < height))
                            _tiles[row][col].canEnter = true;
                        col += 1;

                        break;
                }

            }
            return true;
        }

        /// <summary>
        /// Write the current map to a file
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        /// <returns>true if writing was successful</returns>
        public bool Save(string filename)
        {
            bool result = true;
            XmlTextWriter target = new XmlTextWriter(filename, Encoding.UTF8);
            try
            {
                target.WriteStartDocument();
                target.WriteDocType("GameMap", null, null, null);
                foreach (List<Tile> row in _tiles)
                {
                    target.WriteStartElement("row");
                    foreach (Tile tile in row)
                    {
                        XmlSerializer x=new XmlSerializer(typeof(Map));
                        result = tile.Save(target,x);
                        if (result == false) break;
                    }
                    target.WriteEndElement();
                    if (result == false) break;
                };
                target.WriteEndDocument();
            }
            finally
            {
                target.Close();
            }
            return result;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an empty map
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="height">The height of the map</param>
        public Map(int width = 10, int height = 10)
        {
            _width = width;
            _height = height;
            _tiles = new List<List<Tile>>();
            for (int y = 0; y < height; ++y)
            {
                _tiles.Add(new List<Tile>());
                for (int x = 0; x < width; ++x)
                {
                    _tiles[y].Add(new Tile());
                }
            }
            _blankTile = new Tile();
            //_blankTile.canEnter = false;
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename"></param>
        public Map(string filename = "")
        {
            Load(filename);
        }

        public void Dispose()
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < height; ++x)
                {
                    _tiles[0][0].Dispose();
                }
                _tiles[0].Clear();
            }
            _tiles.Clear();
        }
        #endregion
    }
}
