using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    /// <summary>
    /// A direction to which the random generator may move
    /// </summary>
    public enum Connection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Right = 3,
        Left = 4,
        Invalid = 5
    }

    /// <summary>
    /// A two dimensional coordinate
    /// </summary>
    public class Coords
    {
        private int _x = -1;
        private int _y = -1;

        /// <summary>
        /// Current x coordinate
        /// </summary>
        public int x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Current y coordinate
        /// </summary>
        public int y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        /// <summary>
        /// Creates a new two dimensional point
        /// </summary>
        /// <param name="x">X-coordinate (default:-1)</param>
        /// <param name="y">Y-coordinate (default:-1)</param>
        public Coords(int x = -1, int y = -1)
        {
            _x = x;
            _y = y;
        }
    }

    /// <summary>
    /// A path through the maze
    /// </summary>
    public class Path : Coords
    {
        #region Private Fields
        /// <summary>
        /// Connection to move to
        /// </summary>
        private Connection _dir = Connection.Down;
        #endregion

        #region Public Fields
        /// <summary>
        /// Connection to move to from current field
        /// </summary>
        public Connection dir
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new field object
        /// </summary>
        /// <param name="x">X-coordinate (default 0)</param>
        /// <param name="y">Y-coordinate (default 0)</param>
        /// <param name="dir">Connection to move to (default:up)</param>
        public Path(int x = 0, int y = 0, Connection dir = Connection.Up)
            : base(x, y)
        {

            _dir = dir;
        }
        #endregion

    }
}
