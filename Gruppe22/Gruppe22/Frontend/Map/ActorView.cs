using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{

    /*

                player.AddAnimation("Stand", new Vector2(0, 0), -1, 1, 1);


            player.AddAnimation("Walk", new Vector2(0, 192), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 576), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 480), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 288), -1, 8, 1);


            player.AddAnimation("Walk", new Vector2(0, 0), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 96), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 384), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 672), -1, 8, 1);


*/
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
                return (!_position.Equals(_target));
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
        /// 
        /// </summary>
        /// <param name="difference"></param>
        public void Move(Vector2 difference)
        {
            if (_target.x == -1)
            {
                _target = _position;
            }
            _target.x += (int)difference.X;
            _target.y += (int)difference.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (!_target.Equals(_position))
            {
                if ((_target.x != _position.x) || (_target.y != _position.x))
                {
                    if (_target.x > _position.x)
                    {

                        if (target.y > position.y)
                        {
                            _position.x += 1;
                            if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y)) _position.y += 1;
                            _direction = Direction.DownRight;
                        }
                        else
                        {
                            if (target.y < position.y)
                            {
                                _position.x += 1;
                                if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y)) _position.y -= 1;
                                _direction = Direction.UpRight;
                            }
                            else
                            {
                                _position.x += 1;
                                _direction = Direction.Right;
                            }
                        }
                    }
                    else
                    {
                        if (_target.x < _position.x)
                        {
                            if (target.y > position.y)
                            {
                                _position.x -= 1;
                                if (Math.Abs(_target.x - _position.x) < Math.Abs(_target.y - _position.y)) _position.y += 1;

                                _direction = Direction.UpLeft;
                            }
                            else
                            {
                                if (target.y < position.y)
                                {
                                    _position.x -= 1;
                                    if (Math.Abs(_target.y - _position.y) < Math.Abs(_target.y - _position.y)) _position.y -= 1;
                                    _direction = Direction.DownLeft;
                                }
                                else
                                {
                                    _position.x -= 1;
                                    _direction = Direction.Left;
                                }
                            }
                        }
                        else
                        {
                            if (_target.y > _position.y) { _position.y += 1; _direction = Direction.Down; }
                            else
                                if (_target.y < _position.y) { _position.y -= 1; _direction = Direction.Up; }
                        }
                    }
                    _textures[(int)_activity * 8 + (int)_direction].NextAnimation();
                }
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
        public ActorView(ContentManager content, Vector2 position)
            : base(content, 92, 92, "")
        {
            _position = new Coords((int)position.X, (int)position.Y);
            _target = _position;
        }
        #endregion

    }
}
