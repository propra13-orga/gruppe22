using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gruppe22.Backend
{

    public enum Activity
    {
        Walk = 0,
        Talk,
        Attack,
        Hit,
        Die,
        Special
    }

    public enum PacketType
    {
        Pause,
        Connect,
        Disconnect,
        Move,
        UpdateMap,
        AddItem,
        RemoveItem,
        ToggleItem,
        ChangeStats,
        UpdateClients,
        Chat,
        FinishedAnim,
        FinishedMove,
        KillActor,
        DamageActor,
        PlaySound,
        Animate,
        ActorText,
        Dialog,
        Shop,
        GameOver
    }
    public class ImageData
    {
        private Rectangle _rect;
        private Coords _crop;
        private Coords _offset;
        private string _name;

        public Rectangle rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
            }
        }


        public Coords crop
        {
            get
            {
                return _crop;
            }
            set
            {

                if (value == null) _crop = Coords.Zero;
                else _crop = value;
            }
        }

        public Coords offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (value == null) _offset = Coords.Zero;
                else _offset = value;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public ImageData(string src, Rectangle rect, Coords crop = null, Coords offset = null)
        {
            _rect = rect;
            _name = src;
            if (crop == null) _crop = Coords.Zero;
            else _crop = crop;
            if (offset == null) _offset = Coords.Zero;
            else _offset = offset;
        }
    }

    public interface IHandleEvent
    {
        void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data);
    }

    /// <summary>
    /// Enumeration of eight ways of movement
    /// </summary>
    [Flags]
    public enum Direction
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
        DownLeft = 16,
        UpRight = 32,
        DownRight = 64,
        UpLeft = 128,
        None = 0
    }
    public enum WallType
    {
        Normal = 0,
        OpenDoor = 1,
        ClosedDoor = 2,
        Deco1 = 3,
        Deco2 = 4,
        Deco3 = 5
    }

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
        // Central Game Events
        ContinueGame = 0,
        Disconnect,
        Initialize,
        EndGame,
        ResetGame,
        ChangeTrapState,
        RejectMove,
        LoadFromCheckPoint,
        UpdateToolbar,
        About,
        Network,
        FetchFile,
        RequestFocus,

        // Events returned by UI elements
        ButtonPressed,
        AddDragItem,

        // Events used to display UI
        ShowCharacter,
        ShowInventory,
        ShowMenu,
        ShowAbilities,
        Dialog,
        Pause,
        Chat,
        Settings,

        // Events presenting feedback
        FloatText,
        PlaySound,
        Magic,
        ShowMessage,
        ShowTextBox,

        // Communication between game logic and interface / clients
        TileEntered,
        Attack,
        ActorText,
        SetItemTiles,
        MoveActor,
        ChangeMap,
        NewMap,
        ExplodeProjectile,
        MoveProjectile,
        ActivateAbility,
        DamageActor,
        KillActor,
        AddProjectile,
        TrapActivate,
        AnimateActor,
        FinishedAnimation,
        Checkpoint,
        FinishedProjectileMove,
        FireProjectile,
        Shop,
        GameOver,
        ChangeStats
    }

    public enum SoundFX
    {
        ChangeMap = 0,
        Pickup = 1,
        Checkpoint = 2,
        Damage = 4,
        Trap = 5
    }
    public enum Buttons
    {
        Close,
        TwoPlayers,
        SinglePlayer,
        Settings,
        Restart,
        Reset,
        NewMap,
        Local,
        Load,
        LAN,
        Credits,
        Quit,
        StartServer,
        Connect,
        Cancel
    }
    public enum GameStatus
    {
        Running,
        NoRedraw,
        Paused,
        FetchingData,
        GameOver,
        Loading
    }

    public class DialogLine
    {
        private string _response;
        private int _nextLineID;
        public string response
        {
            get { return _response; }
        }
        public int nextLineID { get { return _nextLineID; } }
        public DialogLine(string response, int nextLineID)
        {
            _nextLineID = nextLineID;
            _response = response;
        }
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

        public int DistanceFrom(Coords coords)
        {
            return DistanceFrom(coords.x, coords.y);
        }

        public int DistanceFrom(int x, int y)
        {
            return (Math.Abs(x - _x) + Math.Abs(y - _y));
        }

        public override int GetHashCode()
        {
            return Math.Abs(_x) + Math.Abs(y);
        }
        public override bool Equals(object obj)
        {
            if (obj is Backend.Coords)
            {
                return ((((Coords)obj).x == _x) && (((Coords)obj).y == _y));
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return _x + "/" + _y;
        }
        public Vector2 vector
        {
            get
            {
                return new Vector2(_x, _y);
            }
            set
            {
                _x = (int)value.X;
                _y = (int)value.Y;
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

        public Coords(Vector2 vector)
        {
            _x = (int)vector.X;
            _y = (int)vector.Y;
        }

        public bool Equals(Coords other)
        {
            return ((other.x == _x) && (other.y == _y));
        }


        public static Coords operator +(Coords c1, Backend.Coords c2)
        {
            return new Backend.Coords(c1.x + c2.x, c1.y + c2.y);
        }

        public static bool operator !=(Coords c1, Backend.Coords c2)
        {
            if ((object)c1 == null) return ((object)c2 != null);
            if ((object)c2 == null) return true;
            return ((c1.x != c2.x) || (c1.y != c2.y));
        }

        public static bool operator ==(Coords c1, Backend.Coords c2)
        {
            if ((object)c1 == null) return ((object)c2 == null);
            if ((object)c2 == null) return false;
            return ((c1.x == c2.x) && (c1.y == c2.y));
        }

        public static Coords Zero
        {
            get
            {
                return new Backend.Coords(0, 0);
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
    public class Path : Backend.Coords
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
