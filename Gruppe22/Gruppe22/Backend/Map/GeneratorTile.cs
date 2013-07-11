using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile used to generate the rooms.
    /// </summary>
    class GeneratorTile : FloorTile
    {
        #region Private Fields
        
        protected bool _connected = false;
        protected Connection _connection = Connection.Invalid;
        #endregion

        #region Public Fields
        /// <summary>
        /// Used by map generator (determines whether tile can be reached from at least one other tile
        /// </summary>
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
        /// <summary>
        /// Direction of connection
        /// </summary>
        public Connection connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Alternative constructor.
        /// </summary>
        /// <param name="parent"></param>
        public GeneratorTile(object parent)
            : base(parent)
        {
        }

        /// <summary>
        /// An empty constructor (setting default values)
        /// </summary>
        public GeneratorTile(object parent, Backend.Coords coords, bool canEnter, Random r)
            : base(parent)
        {
            if (coords != null)
            {
                _coords = coords;
            }
            if (!canEnter)
            {
                Add(new WallTile(this, r));
            }
        }
        #endregion
    }
}
