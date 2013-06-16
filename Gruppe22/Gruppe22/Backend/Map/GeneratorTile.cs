using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    class GeneratorTile : FloorTile
    {
        #region Private Fields
        /// <summary>
        /// Used by map generator (determines whether tile can be reached from at least one other tile
        /// </summary>
        protected bool _connected = false;
        /// <summary>
        /// Direction of connection
        /// </summary>
        protected Connection _connection = Connection.Invalid;
        #endregion

        #region Public Fields

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

        public GeneratorTile(object parent)
            : base(parent)
        {

        }


        /// <summary>
        /// An empty constructor (setting default values)
        /// </summary>
        public GeneratorTile(object parent, Coords coords, bool canEnter, Random r)
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
