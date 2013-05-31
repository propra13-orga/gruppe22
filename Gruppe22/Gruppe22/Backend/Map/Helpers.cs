using System;
using System.Collections;
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
    public enum Events
    {
        ContinueGame = 0,
        EndGame,
        ToggleButton,
        HideNotification,
        MoveActor,
        ChangeMap,
        NewMap,
        ResetGame,
        About,
        AnimateActor,
        FinishedAnimation,
        ShowMessage,
        Player1,
        Player2,
        Network,
        Settings,
        Local,
        LAN,
        FetchFile,
        TileEntered,
        Attack
    }

    public enum GameStatus
    {
        Running,
        NoRedraw,
        Paused,
        FetchingData
    }

    /// <summary>
    /// A two dimensional coordinate
    /// </summary>
    public class Coords : IEquatable<Coords>, IComparable<Coords>, IComparable<Int32>
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
        public override int GetHashCode()
        {
            return Math.Abs(_x)+Math.Abs(y);
        }
        public override bool Equals(object obj)
        {
            if (obj is Coords)
            {
                return ((((Coords)obj).x == _x) && (((Coords)obj).y == _y));
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return _x + "/" + _y;
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

        public bool Equals(Coords other)
        {
            return ((other.x == _x) && (other.y == _y));
        }


        public static Coords operator +(Coords c1, Coords c2)
        {
            return new Coords(c1.x + c2.x, c1.y + c2.y);
        }

        public static bool operator !=(Coords c1, Coords c2)
        {
            if ((object)c1 == null) return ((object)c2 != null);
            if ((object)c2 == null) return true;
            return ((c1.x != c2.x) || (c1.y != c2.y));
        }

        public static bool operator ==(Coords c1, Coords c2)
        {
            if ((object)c1 == null) return ((object)c2 == null);
            if ((object)c2 == null) return false;
            return ((c1.x==c2.x)&&(c1.y==c2.y));
        }

        public static Coords Zero
        {
            get
            {
                return new Coords(0, 0);
            }
        }

        public int CompareTo(Coords other)
        {
            if ((other.x == _x) && (other.y == _y))
            {
                return 0;
            }
            if ((other.x >= _x) && (other.y >= _y))
            {
                return 1;
            }
            if ((other.x <= _x) && (other.y <= _y))
            {
                return -1;
            }

            if (Math.Abs(_x - other.x) > Math.Abs(_y - other.y))
            {
                return _x - other.x;
            }
            else
            {
                return _y - other.y;
            }
        }

        public int CompareTo(int other)
        {
            return (Math.Abs(_x) + Math.Abs(_y)).CompareTo(other);
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
