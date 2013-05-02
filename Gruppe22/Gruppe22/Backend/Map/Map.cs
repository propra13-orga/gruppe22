using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace Gruppe22
{
    public class Map : IDisposable
    {
        #region Private Fields
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

        #region Public Methods
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

            DebugMap();
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
                        result = tile.Save(target);
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
