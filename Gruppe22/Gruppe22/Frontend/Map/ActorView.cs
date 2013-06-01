using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public enum Activity
    {
        Walk = 0,
        Talk,
        Attack,
        Hit,
        Die,
        Run
    }

    public class ActorView : TileSet
    {
        #region Private Fields
        /// <summary>
        /// Current position of the actor (in pixels)
        /// </summary>
        private Coords _position = Coords.Zero;
        /// <summary>
        /// Target the actor is supposed to move to
        /// </summary>
        private Coords _target = null;

        private Coords _cacheTarget = null;
        private Direction _cacheDir = Direction.None;
        /// <summary>
        /// Current animation played
        /// </summary>
        private Activity _activity = Activity.Walk;
        /// <summary>
        /// Direction the actor is facing
        /// </summary>
        private Direction _direction = Direction.Down;
        /// <summary>
        /// Movement speed (nur relevant to animation, see _animationTime below)
        /// </summary>
        private int _speed = 2;
        /// <summary>
        /// The unique ID of the actor
        /// </summary>
        private int _id = 0;
        /// <summary>
        /// Number of milliseconds to wait until displaying next frame of animation
        /// </summary>
        private int _animationTime = 80;
        private int _moveAccel = 4;
        /// <summary>
        /// Seconds elapsed since last redraw
        /// </summary>
        private int _elapsed = 0;
        /// <summary>
        /// Map the actor is displayed on
        /// </summary>
        private IHandleEvent _parent = null;
        /// <summary>
        /// Caching for an animation to play once the current transition is finished
        /// </summary>
        private Activity _playAfterMove = Activity.Walk;
        /// <summary>
        /// Whether the actor is locked during an animation (i.e. playing other animations is not allowed
        /// </summary>
        private bool _lock = false;
        private bool _blockUpdates = false;
        /// <summary>
        /// Whether the actor was killed (i.e. should neither move nor animate)
        /// </summary>
        private bool _dead = false;
        int _xpertick = 0;
        int _ypertick = 0;
        #endregion

        #region Public fields
        public Coords position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public int id
        {
            get
            {
                return _id;
            }
        }
        public int speed
        {
            set
            {
                _speed = value * 16;
            }
            get
            {
                return _speed;
            }
        }
        public Coords target
        {
            get
            {
                return _target;
            }
            set
            {
                // if (_id == 0)
                //    System.Diagnostics.Debug.WriteLine("Move from " + _position.X.ToString() + "/" + _position.Y.ToString() + " to " + _target.x.ToString() + "/" + value.y.ToString());
                _blockUpdates = true;
                if (value != _position)
                {
                    _target = value;
                    _cacheTarget = value;
                    _xpertick = speed;
                    _ypertick = speed;
                    // System.Diagnostics.Debug.WriteLine(Math.Abs(_position.x - _target.x) + " vs " + Math.Abs(_position.y - _target.y));

                    if ((_position.x != _target.x) && (_position.y != _target.y))
                    {
                        if (Math.Abs(_position.x - _target.x) < Math.Abs(_position.y - _target.y))
                        {
                            _xpertick = (speed * Math.Abs(_position.x - _target.x)) / Math.Abs(_position.y - _target.y);
                            // System.Diagnostics.Debug.WriteLine("x:" + _xpertick);
                        }
                        else
                        {
                            _ypertick = (speed * Math.Abs(_position.y - _target.y)) / Math.Abs(_position.x - _target.x);
                            System.Diagnostics.Debug.WriteLine("y:" + _ypertick);

                        }
                    }
                }
                _blockUpdates = false;
            }
        }

        public Coords cacheTarget
        {
            set
            {
                if (_target != _position)
                    _cacheTarget = value;
                else
                    target = value;
            }
        }

        public Direction cacheDir
        {
            set
            {
                _cacheDir = value;
            }
            get
            {
                return _cacheDir;
            }
        }

        public bool isMoving
        {
            get
            {
                return ((_target.x != (int)position.x) || (target.y != (int)position.y) || _lock);
            }
        }



        public Activity activity
        {
            get
            {
                return _activity;
            }
            set
            {
                if (_activity != value)
                {
                    _elapsed = 0;
                    _activity = value;
                    _textures[(int)_activity * 8 + (int)_direction].ResetAnimation();
                }
            }
        }

        public Direction direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (_direction != value)
                {
                    _elapsed = 0;
                    if (value != Direction.None)
                        _direction = value;
                    _textures[(int)_activity * 8 + (int)_direction].ResetAnimation();
                }
            }
        }

        public Texture2D animationTexture
        {
            get
            {
                if (_direction == Direction.None)
                {
                    return _textures[(int)_activity].animationTexture;
                }
                return _textures[(int)_activity * 8 + (int)_direction].animationTexture;
            }
        }

        public Rectangle animationRect
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)_direction].animationRect;
            }
        }

        public int offsetY
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)_direction].offsetY;
            }

        }

        public int offsetX
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)_direction].offsetX;
            }
            set { }

        }

        public int cropX
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)_direction].cropX;
            }
            set { }

        }
        public int cropY
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)_direction].cropY;
            }
            set { }

        }
        #endregion

        #region Public Methods

        public void PlayNowOrAfterMove(Activity activity, bool locked = false)
        {
            if (this.isMoving)
            {
                _playAfterMove = activity;
            }
            if (locked)
            {
                _lock = true;
            }
        }

        public void EndMoveAndPlay(Activity activity, bool locked = false)
        {
            _position.x = _target.x;
            _position.y = _target.y;
            this.activity = activity;
            if (locked)
            {
                _lock = true;
            }
        }

        /// <summary>
        /// Sets an actor to "dead" state and displays final frame of animation
        /// </summary>
        public void Kill()
        {
            _lock = true;
            this.activity = Activity.Die;
            _textures[(int)_activity * 8 + (int)_direction].FinalAnimation();
        }

        /// <summary>
        /// Load object from XML-file
        /// </summary>
        /// <param name="reader">Load object from XML-stream</param>
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            _width = Int32.Parse(reader.GetAttribute("width"));
            _height = Int32.Parse(reader.GetAttribute("height"));
            Boolean isEmptyElement = reader.IsEmptyElement;

            if (isEmptyElement)
                return;

            reader.ReadStartElement("ActorView");
            reader.Read();
            while ((reader.NodeType != System.Xml.XmlNodeType.EndElement) && (reader.NodeType != System.Xml.XmlNodeType.None))
            {
                TileObject temp = new TileObject(_content, _width, _height);
                Activity acti = (Activity)Enum.Parse(typeof(Activity), reader.GetAttribute("Activity").ToString());
                Direction dir = (Direction)Enum.Parse(typeof(Direction), reader.GetAttribute("Direction").ToString());

                _textures[(int)acti * 8 + (int)dir].ReadXml(reader);
                _textures[(int)acti * 8 + (int)dir].loop = ((acti == Activity.Walk) || (acti == Activity.Run));
            }
            reader.ReadEndElement();


            while ((reader.NodeType != System.Xml.XmlNodeType.EndElement) && (reader.NodeType != System.Xml.XmlNodeType.None))
                reader.Read();
            if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                reader.ReadEndElement();
        }

        /// <summary>
        /// Save (merely a shortcut to the serializer
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename = "bla.xml")
        {
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, Encoding.Unicode);
            XmlSerializer serializer = new XmlSerializer(typeof(ActorView));
            serializer.Serialize(writer, this);
            writer.Close();
        }

        /// <summary>
        /// Load (cannot use serializer at this level!)
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename = "bla.xml")
        {
            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(filename, settings);

            ReadXml(reader);
            reader.Close();
        }

        /// <summary>
        /// Dump object to XML-file
        /// </summary>
        /// <param name="writer">Write object to XML-stream</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("width", _width.ToString());
            writer.WriteAttributeString("height", _height.ToString());
            writer.WriteStartElement("activities");
            for (int i = 0; i < _textures.Count; ++i)
            {
                writer.WriteStartElement("activity");
                writer.WriteAttributeString("Activity", ((Activity)(i / 8)).ToString());
                writer.WriteAttributeString("Direction", ((Direction)(i % 8)).ToString());
                _textures[i].WriteXml(writer);
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Add animation for certain activity from a file
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="direction"></param>
        /// <param name="filename"></param>
        /// <param name="startPos"></param>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <param name="vertical"></param>
        public void Add(Activity activity, Direction direction, string filename, Coords startPos, int cols = 1, int rows = 1, bool vertical = false)
        {
            _textures[(int)activity * 8 + (int)direction].AddAnimation(filename, startPos, cols, rows, vertical);
        }



        public void Animate(bool hasMoved)
        {

            if ((_activity != Activity.Walk) && (_activity != Activity.Run))
            {
                if (_textures[(int)_activity * 8 + (int)_direction].NextAnimation())
                {
                    if (_activity != Activity.Die)
                    {
                        _parent.HandleEvent(false, Events.FinishedAnimation, _id, _activity);
                        this.activity = _playAfterMove;
                        if (_playAfterMove != Activity.Walk) _playAfterMove = Activity.Walk;
                        _lock = false;
                    }
                    else
                    {
                        if (!_dead)
                        {
                            _dead = true;
                            _parent.HandleEvent(false, Events.FinishedAnimation, _id, _activity);

                        }
                    }
                }
            }
            else
            {
                if (hasMoved)
                {
                    _textures[(int)_activity * 8 + (int)_direction].NextAnimation();
                    _textures[(int)_activity * 8 + (int)_direction].NextAnimation();

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (!_blockUpdates)
            {
                _elapsed += gametime.ElapsedGameTime.Milliseconds;

                if ((_target.x != (int)_position.x) || (_target.y != (int)_position.y))
                {
                    if (_elapsed > _animationTime / _moveAccel)
                    {
                        _elapsed -= _animationTime / _moveAccel;
                        bool EndMove = false;

                        if (_target.x > _position.x)
                        {
                            //   if (_id == 0) System.Diagnostics.Debug.Write(_xpertick.ToString());

                            _position.x += _xpertick;
                            if (_target.x < _position.x) { EndMove = true; }

                        }
                        else
                        {
                            if (_target.x < _position.x)
                            {
                                //     if (_id == 0) System.Diagnostics.Debug.Write(-_xpertick);

                                _position.x -= Math.Min(_xpertick, Math.Abs(_target.x - _position.x));
                                if (_target.x > _position.x) { EndMove = true; }

                            }
                            //else
                            //     if (_id == 0) System.Diagnostics.Debug.Write("0");
                        }



                        if (_target.y > _position.y)
                        {
                            //   if (_id == 0) System.Diagnostics.Debug.WriteLine("/" + _ypertick.ToString());
                            _position.y += _ypertick;
                            if (_target.y < _position.y) { EndMove = true; }
                        }
                        else
                            if (_target.y < _position.y)
                            {
                                // if (_id == 0) System.Diagnostics.Debug.WriteLine("/-" + _ypertick.ToString());

                                _position.y -= _ypertick;
                                if (_target.y > _position.y) { EndMove = true; }
                            }
                            else
                            {
                                //  if (_id == 0) System.Diagnostics.Debug.WriteLine("/0");
                            }
                        Animate(true);
                        if (EndMove || ((Math.Abs(_target.x - _position.x) < 0.1) && (Math.Abs(_target.y - _position.y) < 0.1)))
                        {
                            //                            _position = _target;
                            //                          _target = _cacheTarget;
                            _parent.HandleEvent(false, Events.TileEntered, _id, _direction);
                            if (_cacheDir != Direction.None)
                            {
                                _parent.HandleEvent(false, Events.MoveActor, _id, _cacheDir, true);
                                _cacheDir = Direction.None;

                            }

                        }
                    }
                }
                else
                {

                    if (_elapsed > _animationTime)
                    {
                        _elapsed -= _animationTime;
                        if (_playAfterMove != Activity.Walk)
                        {
                            this.activity = _playAfterMove;
                            _playAfterMove = Activity.Walk;
                        }
                        Animate(false);
                    }
                }
            }
        }
        #endregion

        #region Constructor

        public ActorView()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="name"></param>
        /// <param name="controllable"></param>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        public ActorView(IHandleEvent parent, int id, ContentManager content, Coords position, string filename = "", int speed = 5, bool alive = true)
            : base(content, 96, 96, "")
        {
            _position = position;
            _id = id;
            _target = new Coords((int)position.x, (int)position.y);
            for (int i = 0; i < (Enum.GetValues(typeof(Activity)).Length) * 8; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }
            if (filename != "")
            {
                Load(filename);
            }
            _parent = parent;
            _speed = 4;
            if (!alive)
            {
                Kill();
            }
        }
        #endregion

    }
}
