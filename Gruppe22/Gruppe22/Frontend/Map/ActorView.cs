using System;
using System.Collections.Generic;
using System.Text;
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
        Die
    }

    public class ActorView : TileSet
    {
        #region Private Fields
        private Coords _position = null;
        private Coords _target = null;
        private Activity _activity = Activity.Walk;
        private Direction _direction = Direction.Down;
        private SpriteBatch _spriteBatch = null;
        private int _speed = 5;
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
                return ((_target.x != position.x) || (target.y != position.y));
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
                _activity = activity;
            }
        }

        public Texture2D animationTexture
        {
            get
            {
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename = "")
        {

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
            if ((_target.x != _position.x) || (_target.y != _position.y))
            {
                if (_target.x > _position.x)
                {
                    _position.x += Math.Min(_speed, Math.Abs(_target.x - position.x));

                    if (target.y > position.y)
                    {
                        if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y))
                            _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y));
                        _direction = Direction.DownRight;
                    }
                    else
                    {
                        if (target.y < position.y)
                        {
                            if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y))
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
                            if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y))
                                _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y));

                            _direction = Direction.UpLeft;
                        }
                        else
                        {
                            if (target.y < position.y)
                            {
                                if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y))
                                    _position.y -= Math.Min(_speed, Math.Abs(_target.y - position.y));
                                _direction = Direction.DownLeft;
                            }
                            else
                            {
                                _direction = Direction.Left;
                            }
                        }
                    }
                    else
                    {
                        if (_target.y > _position.y) {
                            _position.y += Math.Min(_speed, Math.Abs(_target.y - position.y)); 
                            _direction = Direction.Down; }
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

        }
        #endregion


        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="name"></param>
        /// <param name="controllable"></param>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        public ActorView(ContentManager content, Coords position)
            : base(content, 96, 96, "")
        {
            _position = position;
            _target = position;
            for (int i = 0; i < (Enum.GetValues(typeof(Activity)).Length) * 8; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }
        }
        #endregion

    }
}
