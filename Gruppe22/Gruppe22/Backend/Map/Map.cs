using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace Gruppe22
{
    public class Map
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
                _height= value;
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
            get{
                return _tiles[x][y];
            }
            set
            {
                _tiles[x][y] = value;
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
        public Map(int width=10, int height=10)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Load a map from a file
        /// </summary>
        /// <param name="filename"></param>
        public Map(string filename="")
        {
            Load(filename);
        }
        #endregion
    }
}
