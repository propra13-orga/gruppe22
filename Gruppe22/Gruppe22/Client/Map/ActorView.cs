using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{


    public class ActorView : TileSet
    {
        #region Private Fields
        /// <summary>
        /// Current position of the actor (in pixels)
        /// </summary>
        private Backend.Coords _position = Backend.Coords.Zero;
        /// <summary>
        /// Target the actor is supposed to move to
        /// </summary>
        private Backend.Coords _target = null;
        private bool _nomove = true;
        private Backend.Coords _cacheTarget = null;
        private Camera _camera = null;

        /// <summary>
        /// Current animation played
        /// </summary>
        private Backend.Activity _activity = Backend.Activity.Walk;
        /// <summary>
        /// Direction the actor is facing
        /// </summary>
        private Backend.Actor _actor = null;
        /// <summary>
        /// Movement speed (nur relevant to animation, see _animationTime below)
        /// </summary>
        private int _speed = 3;
        /// <summary>
        /// Hide actor (when dead or hidden assassin)
        /// </summary>
        private bool _invisible = false;
        /// <summary>
        /// Aura or other effect on sprite
        /// </summary>
        private MapEffect _effect = null;
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
        private Backend.IHandleEvent _parent = null;
        /// <summary>
        /// Caching for an animation to play once the current transition is finished
        /// </summary>
        private Backend.Activity _playAfterMove = Backend.Activity.Walk;
        /// <summary>
        /// Whether the actor is locked during an animation (i.e. playing other animations is not allowed
        /// </summary>
        private bool _lock = false;
        private Backend.Direction _prevDir = Backend.Direction.Up;
        private bool _blockUpdates = false;
        /// <summary>
        /// Whether the actor was killed (i.e. should neither move nor animate)
        /// </summary>
        private bool _dead = false;
        private int _xpertick = 0;
        private int _ypertick = 0;
        private bool _focussed = false;
        #endregion

        #region Public fields

        public bool focussed
        {
            get
            {
                return _focussed;
            }
            set
            {
                _focussed = value;
            }
        }

        public bool invisible
        {
            get
            {
                return _invisible;
            }
            set
            {
                _invisible = value;
            }
        }

        public MapEffect effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
            }
        }
        public Backend.Coords position
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
        public Backend.Coords target
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
                // _nomove = true;
                if (value != _position)
                {
                    _target = value;
                    _cacheTarget = value;
                    _xpertick = 4;
                    _ypertick = 3;
                    // System.Diagnostics.Debug.WriteLine(Math.Abs(_position.x - _target.x) + " vs " + Math.Abs(_position.y - _target.y));

                    /*    if ((_position.x != _target.x) && (_position.y != _target.y))
                        {
                            if (Math.Abs(_position.x - _target.x) < Math.Abs(_position.y - _target.y))
                            {
                                _xpertick = (speed * Math.Abs(_position.x - _target.x)) / Math.Abs(_position.y - _target.y);
                                // System.Diagnostics.Debug.WriteLine("x:" + _xpertick);
                            }
                            else
                            {
                                _ypertick = (speed * Math.Abs(_position.y - _target.y)) / Math.Abs(_position.x - _target.x);
                                //System.Diagnostics.Debug.WriteLine("y:" + _ypertick);

                            }
                 
                        }*/
                    //_textures[(int)_activity * 8 + (int)Math.Log((double)_direction,2)].ResetAnimation();
                }

                _blockUpdates = false;
            }
        }

        public Backend.Coords cacheTarget
        {
            set
            {
                if (_target != _position)
                    _cacheTarget = value;
                else
                    target = value;
            }
        }


        public bool isMoving
        {
            get
            {
                return ((_target.x != (int)position.x) || (target.y != (int)position.y) || _lock);
            }
        }



        public Backend.Activity activity
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
                    if (_textures[(int)value * 8 + (int)Math.Log((double)direction, 2)].animationTexture != null)
                    {
                        _activity = value;
                        _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].ResetAnimation();
                    }
                    else
                        if (value == Backend.Activity.Die)
                        {
                            _invisible = true;
                        }
                }
            }
        }

        public Backend.Direction direction
        {
            get
            {
                if ((_actor.direction != Backend.Direction.None) && ((int)_actor.direction != -1))
                    return _actor.direction;
                else return _prevDir;
            }
        }

        public Texture2D animationTexture
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].animationTexture;
            }
        }

        public Rectangle animationRect
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].animationRect;
            }
        }

        public int offsetY
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].offsetY;
            }

        }

        public int offsetX
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].offsetX;
            }
            set { }

        }

        public int cropX
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].cropX;
            }
            set { }

        }
        public int cropY
        {
            get
            {
                return _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].cropY;
            }
            set { }

        }
        #endregion

        #region Public Methods

        public void PlayNowOrAfterMove(Backend.Activity activity, bool locked = false)
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

        public void EndMoveAndPlay(Backend.Activity activity, bool locked = false)
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
            this.activity = Backend.Activity.Die;
            _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].FinalAnimation();
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
                Backend.Activity acti = (Backend.Activity)Enum.Parse(typeof(Backend.Activity), reader.GetAttribute("Activity").ToString());
                Backend.Direction dir = (Backend.Direction)Enum.Parse(typeof(Backend.Direction), reader.GetAttribute("Direction").ToString());

                _textures[(int)acti * 8 + (int)dir].ReadXml(reader);
                _textures[(int)acti * 8 + (int)dir].loop = ((acti == Backend.Activity.Walk));
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
                writer.WriteAttributeString("Activity", ((Backend.Activity)(i / 8)).ToString());
                writer.WriteAttributeString("Direction", ((Backend.Direction)(i % 8)).ToString());
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
        public void Add(Backend.Activity activity, Backend.Direction direction, string filename, Backend.Coords startPos, int cols = 1, int rows = 1, Backend.Coords offset = null, Backend.Coords crop = null, bool vertical = false)
        {
            int x = (int)Math.Log((double)direction, 2);
            _textures[(int)activity * 8 + (int)Math.Log((double)direction, 2)].AddAnimation(filename, startPos, cols, rows, offset, crop, vertical);
        }



        public void Animate(bool hasMoved)
        {

            if (_activity != Backend.Activity.Walk)
            {
                if (_textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].NextAnimation())
                {
                    if (_activity != Backend.Activity.Die)
                    {
                        _parent.HandleEvent(false, Backend.Events.FinishedAnimation, _id, _activity);
                        this.activity = _playAfterMove;
                        if (_playAfterMove != Backend.Activity.Walk) _playAfterMove = Backend.Activity.Walk;
                        _lock = false;
                        _dead = false;
                    }
                    else
                    {
                        if (!_dead)
                        {
                            _parent.HandleEvent(false, Backend.Events.FinishedAnimation, _id, _activity);
                            _dead = true;
                            _lock = false;

                        }
                    }
                }
            }
            else
            {
                if (hasMoved)
                {
                    _textures[(int)_activity * 8 + (int)Math.Log((double)direction, 2)].NextAnimation();
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
                Backend.Direction tmp = _actor.direction;
                if ((tmp != Backend.Direction.None) && ((int)tmp > -1) && (tmp != _prevDir)) _prevDir = tmp;
                if (_effect != null)
                {
                    if (_effect.finished)
                    {
                        _effect = null;
                    }
                    else
                    {
                        _effect.Update(gametime);
                    }

                }
                _elapsed += gametime.ElapsedGameTime.Milliseconds;

                if (_target != _position)
                {
                    if (_elapsed > _animationTime / _moveAccel)
                    {

                        if (!_nomove)
                        {
                            _nomove = true;
                            Animate(true);
                        }
                        else
                        {
                            _nomove = false;
                        }
                        //  System.Diagnostics.Debug.WriteLine("ENDED AT " + _textures[(int)_activity * 8 + (int)Math.Log((double)_direction,2)].animationID);

                        // System.Diagnostics.Debug.WriteLine(_textures[(int)_activity * 8 + (int)Math.Log((double)_direction,2)].animationID);

                        _elapsed -= _animationTime / _moveAccel;

                        if (_target.x > _position.x)
                        {
                            //   if (_id == 0) System.Diagnostics.Debug.Write(_xpertick.ToString());

                            _position.x += _xpertick;

                        }
                        else
                        {
                            if (_target.x < _position.x)
                            {
                                //     if (_id == 0) System.Diagnostics.Debug.Write(-_xpertick);

                                _position.x -= Math.Min(_xpertick, Math.Abs(_target.x - _position.x));

                            }
                            //else
                            //     if (_id == 0) System.Diagnostics.Debug.Write("0");
                        }



                        if (_target.y > _position.y)
                        {
                            //   if (_id == 0) System.Diagnostics.Debug.WriteLine("/" + _ypertick.ToString());
                            _position.y += _ypertick;
                        }
                        else
                            if (_target.y < _position.y)
                            {
                                // if (_id == 0) System.Diagnostics.Debug.WriteLine("/-" + _ypertick.ToString());

                                _position.y -= _ypertick;
                            }
                            else
                            {
                                //  if (_id == 0) System.Diagnostics.Debug.WriteLine("/0");
                            }
                        if (_focussed)
                            _camera.position = new Vector2(-38 - position.x, -30 - position.y);

                        if (_target == _position)
                        {
                            //                            _position = _target;
                            //                          _target = _cacheTarget;
                            _parent.HandleEvent(false, Backend.Events.TileEntered, _id, direction);



                        }
                    }
                }
                else
                {

                    if (_elapsed > _animationTime)
                    {
                        _elapsed -= _animationTime;
                        if (_playAfterMove != Backend.Activity.Walk)
                        {
                            this.activity = _playAfterMove;
                            _playAfterMove = Backend.Activity.Walk;
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
        public ActorView(Camera camera, Backend.IHandleEvent parent, int id, ContentManager content, Backend.Coords position, Backend.Actor actor, string filename = "", int speed = 5, bool alive = true, int width = 96, int height = 96)
            : base(content, width, height, "")
        {
            _camera = camera;
            _position = position;
            _actor = actor;
            _id = id;
            _speed = speed;
            _target = new Backend.Coords((int)position.x, (int)position.y);
            for (int i = 0; i < (Enum.GetValues(typeof(Backend.Activity)).Length) * 8; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }
            if (filename != "")
            {
                Load(filename);
            }
            _parent = parent;
            if (!alive)
            {
                Kill();
            }
        }
        #endregion

    }
}
