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
        private Coords _position = null;
        /// <summary>
        /// Target the actor is supposed to move to
        /// </summary>
        private Coords _target = null;
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
        private int _speed = 5;
        /// <summary>
        /// The unique ID of the actor
        /// </summary>
        private int _id = 0;
        /// <summary>
        /// Number of milliseconds to wait until displaying next frame of animation
        /// </summary>
        private int _animationTime = 20;
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
        /// <summary>
        /// Whether the actor was killed (i.e. should neither move nor animate)
        /// </summary>
        private bool _dead = false;
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

        public int speed
        {
            set
            {
                _speed = value;
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
                _target = value;
            }
        }
        public bool isMoving
        {
            get
            {
                return ((_target.x != position.x) || (target.y != position.y) || _lock);
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
                _elapsed = 0;
                _activity = value;
                _textures[(int)_activity * 8 + (int)_direction].ResetAnimation();
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

                _elapsed = 0;
                if (value != Direction.None)
                    _direction = value;
                _textures[(int)_activity * 8 + (int)_direction].ResetAnimation();
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
            _activity = activity;
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
            _activity = Activity.Die;
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
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="difference"></param>
        public void Move(Coords difference)
        {
            if (_target.x == -1)
            {
                _target = _position;
            }
            _target.x += difference.x;
            _target.y += difference.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (_elapsed > _animationTime)
            {
                if ((_target.x != _position.x) || (_target.y != _position.y))
                {
                    if (_target.x > _position.x)
                    {
                        _position.x += Math.Min(_speed, Math.Abs(_target.x - position.x));

                        if (target.y > position.y)
                        {
                            if (Math.Abs(_target.x - _position.x) <= Math.Abs(_target.y - _position.y))
                                _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y));

                            _direction = Direction.DownRight;
                        }
                        else
                        {
                            if (target.y < position.y)
                            {
                                if (Math.Abs(_target.x - _position.x) <= Math.Abs(_target.y - _position.y))
                                    _position.y -= Math.Min(_speed, Math.Abs(_target.y - position.y));

                                _direction = Direction.UpRight;
                            }
                            else
                            {
                                _direction = Direction.Right;
                            }
                        }
                    }
                    else
                    {
                        if (_target.x < _position.x)
                        {
                            _position.x -= Math.Min(_speed, Math.Abs(_target.x - position.x));

                            if (target.y > position.y)
                            {
                                if (Math.Abs(_target.x - _position.x) <= Math.Abs(_target.y - _position.y))
                                    _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y));

                                _direction = Direction.DownLeft;
                            }
                            else
                            {
                                if (target.y < position.y)
                                {
                                    if (Math.Abs(_target.x - _position.x) <= Math.Abs(_target.y - _position.y))
                                        _position.y -= Math.Min(_speed, Math.Abs(_target.y - position.y));
                                    _direction = Direction.UpLeft;
                                }
                                else
                                {
                                    _direction = Direction.Left;
                                }
                            }
                        }
                        else
                        {
                            if (_target.y > _position.y)
                            {
                                _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y));
                                _direction = Direction.Down;
                            }
                            else
                                if (_target.y < _position.y)
                                {
                                    _position.y -= Math.Min(_speed, Math.Abs(_target.y - position.y));
                                    _direction = Direction.Up;
                                }
                        }
                    }

                    _textures[(int)_activity * 8 + (int)_direction].NextAnimation();

                }
                else
                {
                    if (_playAfterMove != Activity.Walk)
                    {
                        _activity = _playAfterMove;
                        _playAfterMove = Activity.Walk;
                    }
                }
                if ((_activity != Activity.Walk) && (_activity != Activity.Run))
                {
                    if (_elapsed > _animationTime)
                    {
                        _elapsed -= _animationTime;
                        if (_textures[(int)_activity * 8 + (int)_direction].NextAnimation())
                        {
                            if (_activity != Activity.Die)
                            {
                                _parent.HandleEvent(null, Events.FinishedAnimation, _id, _activity);
                                _activity = _playAfterMove;
                                if (_playAfterMove != Activity.Walk) _playAfterMove = Activity.Walk;
                                _lock = false;
                            }
                            else
                            {
                                if (!_dead)
                                {
                                    _dead = true;
                                    _parent.HandleEvent(null, Events.FinishedAnimation, _id, _activity);

                                }
                            }
                        }
                    }
                }
            }
            _elapsed += (gametime.TotalGameTime.Milliseconds / 100);

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
        public ActorView(IHandleEvent parent, int id, ContentManager content, Coords position, string filename = "", int speed = 5, bool alive=true)
            : base(content, 96, 96, "")
        {
            _position = position;
            _id = id;
            _target = position;
            for (int i = 0; i < (Enum.GetValues(typeof(Activity)).Length) * 8; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }
            if (filename != "")
            {
                Load(filename);
            }
            _parent = parent;
            _speed = speed;
            if (!alive)
            {
                Kill();
            }
        }
        #endregion

    }
}
